#!/usr/bin/env python
from __future__ import print_function

import argparse
import json
import os
import sys
import time

from mpyq import MPQArchive
from s2protocol.versions import build, latest


KEEPALIVE = None


def read_contents(archive, name):
    contents = archive.read_file(name)
    if not contents:
        raise RuntimeError("Archive missing {0}".format(name))
    return contents


def count_attribute_events(attributes):
    total = 0
    for scope in attributes.get("scopes", {}).values():
        for values in scope.values():
            total += len(values)
    return total


def count_retained_message_events(message_events):
    retained_names = set(["NNet.Game.SChatMessage", "NNet.Game.SPingMessage"])
    return sum(1 for event in message_events if event.get("_event") in retained_names)


def decode_replay(path):
    archive = MPQArchive(path)

    header_content = archive.header["user_data_header"]["content"]
    header = latest().decode_replay_header(header_content)
    base_build = header["m_version"]["m_baseBuild"]

    try:
        protocol = build(base_build)
    except Exception as exc:
        raise RuntimeError("Unsupported base build: {0} ({1!s})".format(base_build, exc))

    metadata = json.loads(read_contents(archive, "replay.gamemetadata.json"))
    details = protocol.decode_replay_details(read_contents(archive, "replay.details"))
    details_backup = protocol.decode_replay_details(read_contents(archive, "replay.details.backup"))
    initdata = protocol.decode_replay_initdata(read_contents(archive, "replay.initData"))
    game_events = list(protocol.decode_replay_game_events(read_contents(archive, "replay.game.events")))
    message_events = list(protocol.decode_replay_message_events(read_contents(archive, "replay.message.events")))

    if hasattr(protocol, "decode_replay_tracker_events"):
        tracker_events = list(protocol.decode_replay_tracker_events(read_contents(archive, "replay.tracker.events")))
    else:
        tracker_events = []

    attributes = protocol.decode_replay_attributes_events(read_contents(archive, "replay.attributes.events"))

    return {
        "header": header,
        "metadata": metadata,
        "details": details,
        "details_backup": details_backup,
        "initdata": initdata,
        "game_events": game_events,
        "message_events": message_events,
        "tracker_events": tracker_events,
        "attributes": attributes,
    }


def main(argv):
    parser = argparse.ArgumentParser(description="Decode one replay with Blizzard's Python s2protocol package.")
    parser.add_argument("replay_file", help=".SC2Replay file to decode")
    args = parser.parse_args(argv)

    if not os.path.exists(args.replay_file):
        print("Replay not found: {0}".format(args.replay_file), file=sys.stderr)
        return 1

    started = time.time()
    try:
        decoded = decode_replay(args.replay_file)
    except RuntimeError as exc:
        print(str(exc), file=sys.stderr)
        return 2
    elapsed_ms = (time.time() - started) * 1000.0

    global KEEPALIVE
    KEEPALIVE = decoded

    details = decoded["details"]
    header = decoded["header"]

    print("file={0}".format(os.path.basename(args.replay_file)))
    print("baseBuild={0}".format(header["m_version"]["m_baseBuild"]))
    print("elapsedMs={0:.2f}".format(elapsed_ms))
    print("players={0}".format(len(details.get("m_playerList", []))))
    print("gameEvents={0}".format(len(decoded["game_events"])))
    print("messageEvents={0}".format(count_retained_message_events(decoded["message_events"])))
    print("trackerEvents={0}".format(len(decoded["tracker_events"])))
    print("attributeEvents={0}".format(count_attribute_events(decoded["attributes"])))

    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))

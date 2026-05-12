using System.Diagnostics;
using System.Globalization;
using s2protocol.NET;
using s2protocol.NET.Models;

string replayPath = args.Length > 0
    ? args[0]
    : @"C:\Users\pax77\source\repos\s2protocol.NET\src\s2protocol.NET.Benchmarks\bin\Release\net10.0\replays\test6.SC2Replay";

Stopwatch stopwatch = Stopwatch.StartNew();
using ReplayDecoder replayDecoder = new();
Sc2Replay sc2Replay = await replayDecoder.DecodeAsync(replayPath).ConfigureAwait(false)
    ?? throw new InvalidOperationException($"Could not decode replay '{replayPath}'.");
stopwatch.Stop();

Console.WriteLine($"file={Path.GetFileName(replayPath)}");
Console.WriteLine($"baseBuild={sc2Replay.Header.BaseBuild}");
Console.WriteLine($"elapsedMs={stopwatch.Elapsed.TotalMilliseconds.ToString("F2", CultureInfo.InvariantCulture)}");
Console.WriteLine($"players={sc2Replay.Details?.Players.Count ?? 0}");
Console.WriteLine($"gameEvents={sc2Replay.GameEvents?.BaseGameEvents.Count ?? 0}");
Console.WriteLine($"messageEvents={(sc2Replay.ChatMessages?.Count ?? 0) + (sc2Replay.PingMessages?.Count ?? 0)}");
Console.WriteLine($"trackerEvents={CountTrackerEvents(sc2Replay.TrackerEvents)}");
Console.WriteLine($"attributeEvents={sc2Replay.AttributeEvents?.Scopes.Count ?? 0}");

GC.KeepAlive(sc2Replay);

static int CountTrackerEvents(TrackerEvents? trackerEvents)
{
    if (trackerEvents is null)
    {
        return 0;
    }

    return trackerEvents.SPlayerSetupEvents.Count
        + trackerEvents.SPlayerStatsEvents.Count
        + trackerEvents.SUnitBornEvents.Count
        + trackerEvents.SUnitDiedEvents.Count
        + trackerEvents.SUnitOwnerChangeEvents.Count
        + trackerEvents.SUnitPositionsEvents.Count
        + trackerEvents.SUnitTypeChangeEvents.Count
        + trackerEvents.SUpgradeEvents.Count
        + trackerEvents.SUnitInitEvents.Count
        + trackerEvents.SUnitDoneEvents.Count;
}

[![.NET](https://github.com/ipax77/s2protocol.NET/actions/workflows/dotnet.yml/badge.svg)](https://github.com/ipax77/s2protocol.NET/actions/workflows/dotnet.yml)

# Introduction

dotnet/C# implementation of [Blizzards s2protocol](https://github.com/Blizzard/s2protocol) for decoding/parsing StarCraft II replays (*.SC2Replay)

# Getting started

## Installation

```bash
dotnet add package s2protocol.NET
```

## Usage
Decode from file path
```csharp
ReplayDecoder decoder = new();
Sc2Replay replay = await decoder.DecodeAsync(pathToSC2Replay);
Console.WriteLine(replay.Header.BaseBuild);
```

Decode from stream
```csharp
ReplayDecoder decoder = new();

await using FileStream stream = File.OpenRead(pathToSC2Replay);

Sc2Replay replay = await decoder.DecodeAsync(stream);

Console.WriteLine(replay.Header.BaseBuild);
```

`DecodeAsync(Stream)` leaves the caller-owned stream open. The caller remains responsible for disposing it.

Optional options:
```csharp
ReplayDecoder decoder = new();

ReplayDecoderOptions options = new ReplayDecoderOptions()
{
    Initdata = true,
    Details = true,
    Metadata = true,
    GameEvents = false,
    MessageEvents = false,
    TrackerEvents = true,
    AttributeEvents = false
};

CancellationTokenSource cts = new();

Sc2Replay replay = await decoder.DecodeAsync(pathToSC2Replay, options, cts.Token);
Console.WriteLine(replay.TrackerEvents?.SUnitBornEvents.FirstOrDefault());
```

`ReplayDecoder` owns no persistent resources and is not disposable. For bounded parallel work, use caller-owned
`Parallel.ForEachAsync` and choose the degree of parallelism appropriate for the available memory and CPU.

## Ordered events and canonical JSON

Game, message, and tracker events retain their encoded order in `BaseGameEvents`, `BaseMessageEvents`, and
`BaseTrackerEvents`. Tracker category properties such as `SUnitBornEvents` are synchronized, mutable, lazily filtered
views over that one ordered store; their `Count` values are cached. Unsupported replay protocol names are retained as
`UnknownGameEvent`, `UnknownMessageEvent`, or `UnknownTrackerEvent` with their stream metadata.

The complete replay graph supports canonical JSON round-tripping with stable class-name `$type` discriminators:

```csharp
using System.Text.Json;

string json = JsonSerializer.Serialize(replay, S2ProtocolJsonSerializerContext.Default.Sc2Replay);
Sc2Replay copy = JsonSerializer.Deserialize(json, S2ProtocolJsonSerializerContext.Default.Sc2Replay)
    ?? throw new JsonException("Replay JSON was null.");
```

Only the ordered base event collections are serialized. Compatibility category/message views and derived unit
relationships are omitted; tracker unit indexes and relationships are rebuilt after deserialization. Unknown JSON
discriminators fail with `JsonException`. `S2ProtocolJsonSerializerContext` is public and source-generated, so the same
contract is safe for trimming and Native AOT.

Version 1.x targets `net10.0` only. Pin a supported .NET 10 SDK when building or publishing Native AOT applications.
The repository sample can be published with `dotnet publish -r linux-x64 --self-contained /p:PublishAot=true` and
performs both replay decoding and the canonical JSON round-trip.

# Known Limitations / ToDo

## GameEvents
All 105 game-event names present in the bundled Blizzard protocol schemas decode to public strongly typed models. `UnknownGameEvent` is reserved for unsupported future protocol names.

## MessageEvents
All five message-event names present in the bundled Blizzard protocol schemas decode to public strongly typed models in stream order. Use `replay.MessageEvents.BaseMessageEvents` for complete coverage; `ChatMessages` and `PingMessages` are synchronized compatibility views excluded from JSON. `UnknownMessageEvent` preserves metadata for unsupported future protocol names.

## TrackerEvents
All ten bundled tracker-event names decode into `BaseTrackerEvents` in encoded order. Categorized collections remain synchronized mutable views, and unknown protocol event names are retained instead of discarded.

## AttributeEvents
All records in `replay.attributes.events` decode in stream order with the same generic wire semantics used by Blizzard's 95 bundled protocol definitions. Use `replay.AttributeEvents.Attributes`, `MapNamespace`, `AttributeId`, and `Scope`.

`MPQArchive.ExtractFilesToDisk(...)` is the correctly named selective extraction API. Extraction rejects rooted and traversal paths.

STriggerSoundLengthSyncEvent => no data
SControlGroupUpdateEvent => no mask

# s2cli

A .NET global tool that emulates Blizzard's `s2_cli.exe`, powered by [s2protocol.NET](https://www.nuget.org/packages/s2protocol.NET).  
It decodes `.SC2Replay` files and prints structured JSON output.

> ⚙️ Built with .NET 10 and System.CommandLine.  
> 🔍 Output is always JSON or NDJSON.  
---

## Installation

```bash
dotnet tool install -g s2cli
```

## Usage
```bash
s2cli --replay path/to/game.SC2Replay [options]
```
| Option           | Description                              |
| ---------------- | ---------------------------------------- |
| `-r`, `--replay` | Path to `.SC2Replay` file (not required with `--versions`) |

| Option                     | Description                               |
| -------------------------- | ----------------------------------------- |
| `--header`                 | Print protocol header                     |
| `-md`, `--metadata`        | Print game metadata                       |
| `-d`, `--details`          | Print protocol details                    |
| `-db`, `--details_backup`  | Print anonymized details                  |
| `-id`, `--initdata`        | Print protocol initdata                   |
| `-ge`, `--gameevents`      | Print game events                         |
| `-me`, `--messageevents`   | Print message events                      |
| `-te`, `--trackerevents`   | Print tracker events                      |
| `-at`, `--attributeevents` | Print attribute events                    |
| `-a`, `--all`              | Print all available data                  |
| `-nd`, `--ndjson`          | Output as NDJSON (newline-delimited JSON) |
| `--versions`               | Show supported protocol versions          |

## Benchmark
Decode-only process comparison using `test6.SC2Replay` (base build `88500`). Each target decodes the same replay once per process, materializes the decoded object graph, prints only a compact summary, and keeps the decoded data alive until exit. Results are 5 process launches on Windows 11 / .NET 10.

| Decoder | Mean Time | Min Time | Max Time | Peak Working Set | Peak Private Bytes |
| ------- | --------: | -------: | -------: | ---------------: | -----------------: |
| `s2protocol.NET` v0.9.4 sample exe | 435.90 ms | 379.41 ms | 467.22 ms | 38.27 MB | 23.30 MB |
| Blizzard Python `s2protocol` 5.0.15.95299.0 helper | 1,472.93 ms | 1,432.50 ms | 1,558.93 ms | 47.41 MB | 41.38 MB |

The pre-release workflow benchmarks full, tracker-only, message-only, attribute-only, and caller-owned bounded parallel decoding. The checked-in baseline allows up to 2% more allocated bytes and 20% more elapsed time; both budgets are enforced. Raw BenchmarkDotNet reports are uploaded with every workflow run.

# ChangeLog

<details open="open"><summary>v1.0.0</summary>

>- Freeze the public API with a reviewed analyzer baseline and package validation against v0.9.8
>- Preserve tracker events in one ordered store with lazy mutable category views and retained unknown events
>- Add canonical, polymorphic, AOT-safe JSON round-tripping for the complete replay graph and use it from `s2cli`
>- Remove the obsolete decoder-owned parallel APIs, decoder disposal, misspelled attribute/extraction APIs, and `EngineException`
>- Correct `SCmdUpdateTargetUnitEvent`, make successful asynchronous decode results non-nullable, and target `net10.0`
>- Add the build 97563 / game 5.0.16 replay and broaden ordered-event, JSON, archive, stream, and cancellation tests
>- Pin SDK 10.0.302 and add coverage, package, Native AOT, vulnerability, performance, and 30-replay stress gates
>- Produce deterministic `1.0.0` packages and `.snupkg` symbols; publish only from a validated `v1.0.0` tag

</details>

<details><summary>v0.9.8</summary>

>- Decode selection delta and command manager state game events into their strongly typed models
>- Add strongly typed coverage for all 105 game-event names in the bundled Blizzard protocols
>- Preserve historical game-event layouts and expose dialog choices, camera/mouse coordinates, and millisecond decrement data
>- Verify strongly typed coverage for all 10 tracker-event names across the bundled Blizzard protocols
>- Preserve the optional tracker player setup slot ID instead of converting a missing value to zero
>- Add synthetic versioned tracker-event coverage for every canonical protocol resource
>- Reduce tracker-only benchmark allocations from 6.96 MB to 6.76 MB per decode without a time regression
>- Add strongly typed coverage for all five message-event names in the bundled Blizzard protocols
>- Preserve ordered message streams while retaining mutable chat and ping compatibility views
>- Add a message-events-only allocation benchmark
>- Preserve every AttributeEvents record, including repeated scope and attribute IDs, with Blizzard-compatible value decoding
>- Add corrected AttributeEvents API aliases while preserving the existing source and JSON contracts
>- Reduce transient AttributeEvents allocations and add an attribute-events-only benchmark

</details>

<details><summary>v0.9.6</summary>

>- Add missing GameEvent types
>- Add new s2protocol versions with deduplication

</details>

<details><summary>v0.9.4</summary>

>- Improved performance/momory usage
>- MPQArchive.ReadFile(Async) now returns ReadOnlyMemory<byte> instead of byte[]

</details>

<details><summary>v0.9.3</summary>

>- replaced runtime `protocol*.py` parsing with generated compact JSON protocol resources
>- moved Python protocol files to the `S2ProtocolJsonGenerator` tool
>- reduced protocol resource payload and optimized version fallback lookup
>- marked built-in parallel decoding helpers obsolete; prefer `DecodeAsync` with caller-owned parallelism

</details>

<details><summary>v0.9.2</summary>

>- improved protocol decoding performance by replacing reflection-based dispatch with prebuilt decoder metadata
>- reduced MPQ decompression allocations by passing expected output lengths
>- optimized tracker event parsing and unit connection mapping
>- replay metadata deserialization now uses the generated JSON serializer context
>- refactored public replay and event models from records to sealed classes
>- `ReplayDecoder.Dispose()` no longer forces `GC.Collect()`
>- added BenchmarkDotNet benchmarks and optional Direct Strike stress replay tests

</details>

<details><summary>v0.9.1.1</summary>

>- update to dotnet 10
>- add decoding from stream

</details>

<details><summary>v0.9.0</summary>

>- **Breaking Changes**
>- removed requirement for IronPython

</details>

<details><summary>v0.8.4</summary>

>- s2protocol v5.0.14.93333.0

</details>

<details><summary>v0.8.3</summary>

>- s2protocol v5.0.14.93272.0

</details>

<details><summary>v0.8.2</summary>

>- s2protocol v5.0.13.92440.0

</details>

<details><summary>v0.8.0</summary>

**Breaking Changes**
>- dotnet 8
>- SC2 Patch 5.0.13 - s2protocol 92028
>- PingMessageEvents

</details>

<details><summary>v0.8.0-rc1.0</summary>

**Breaking Changes**
>- dotnet 8
>- removed logging
>- improved error handling

</details>

<details><summary>v0.6.12</summary>

>- Protocol 91115

</details>

<details><summary>v0.6.11</summary>

>- Protocol 90136

</details>

<details><summary>v0.6.10</summary>

>- Protocol 89720

</details>

<details><summary>v0.6.9</summary>

>- Protocol 89634
>- Fix Gametime to UTC

</details>

<details><summary>v0.6.8</summary>

>- Catch UnitIndex BigInteger
>- New parallel decoding with ErrorReport: decoder.DecodeParallelWithErrorReport
>- Parallel decoding tests

</details>

<details><summary>v0.6.7</summary>

>- Catch Currupted Trackerevents
>- Protocoll 88500 fix

</details>

<details><summary>v0.6.6</summary>

>- Call GC.Collect() in dispose to release file locks
>- Disabled default console-logging
>- Added Test for protocol 88500 (5.0.10)

</details>

<details><summary>v0.6.5</summary>

>- Save full path in FileName

</details>

<details><summary>v0.6.4</summary>

>- Patch 5.0.9 - Protocol 87702

</details>

<details><summary>v0.6.3</summary>

>- Python.StdLib to version 2.7.12
>- JsonIgnore on UnitBorn <-> UnitDied cycles

</details>

<details><summary>v0.6.2</summary>

>- GameEvents
>- AttributeEvents
>- Tracker-Unit-Events mapping (Born -> Died ...)
>- Tracker-Unit-Events UnitIndex from ```protocol.unit_tag(index, recycle)```

</details>

<details><summary>v0.6.1</summary>

>- Fixed some types (nullable/BigInteger/long)
>- Initdata is now available
>- Json de-/serialization

</details>

<details><summary>v0.6.0</summary>

>- Init

</details>

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
Sc2Replay? replay = await decoder.DecodeAsync(pathToSC2Replay);
Console.WriteLine(replay?.Header.BaseBuild);
```

Decode from stream
```csharp
ReplayDecoder decoder = new();

await using FileStream stream = File.OpenRead(pathToSC2Replay);

Sc2Replay? replay = await decoder.DecodeAsync(stream);

Console.WriteLine(replay?.Header.BaseBuild);
```

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

Sc2Replay? replay = await decoder.DecodeAsync(pathToSC2Replay, options, cts.Token);
Console.WriteLine(replay.TrackerEvents.SUnitBornEvents.FirstOrDefault());
```

# Known Limitations / ToDo

## GameEvents
STriggerSoundLengthSyncEvent => no data
SControlGroupUpdateEvent => no mask
No BigInteger support

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
| `-r`, `--replay` | **(Required)** Path to `.SC2Replay` file |

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
Performance comparison between s2cli (.NET 10, C#) and the original s2_cli.exe (Python):

| Metric       | C# / .NET | Python        | Winner               |
| ------------ | --------- | ------------- | -------------------- |
| Avg Time     | 1,598 ms  | 10,766 ms     | ✅ C# (\~6.7× faster) |
| Min Time     | 333 ms    | 970 ms        | ✅ C#                 |
| Max Time     | 3,911 ms  | 31,603 ms (!) | ✅ C#                 |
| Success Rate | ✅ 23/23   | ✅ 23/23       | 🎉 Tie               |

⏱ Benchmarked on 23 real StarCraft II replays across multiple match types with option '--all'.

Run the BenchmarkDotNet replay benchmarks:

```bash
dotnet run -c Release --project src/s2protocol.NET.Benchmarks
```

Set `S2PROTOCOL_BENCHMARK_REPLAY_DIR` to benchmark a custom replay corpus. Optional Direct Strike stress replay tests can be enabled with `S2PROTOCOL_RUN_STRESS_TESTS=1`; they use `S2PROTOCOL_BENCHMARK_REPLAY_DIR` when set.

# ChangeLog

<details open="open"><summary>v0.9.2</summary>

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

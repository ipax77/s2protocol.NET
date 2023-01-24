[![.NET](https://github.com/ipax77/s2protocol.NET/actions/workflows/dotnet.yml/badge.svg)](https://github.com/ipax77/s2protocol.NET/actions/workflows/dotnet.yml)

# Introduction

dotnet 6 wrapper for [Blizzards s2protocol](https://github.com/Blizzard/s2protocol) for decoding/parsing StarCraft II replays (*.SC2Replay)
using IronPython (2.7)

# Getting started
## Prerequisites
dotnet 6
## Installation
```
dotnet add package IronPython.StdLib --version 2.7.12
dotnet add package s2protocol.NET
```
## Usage

IronPyhton has a known memory leak [Issue](https://github.com/IronLanguages/ironpython2/issues/322) - try initializing the ReplayDecoder just once and reusing it.

```csharp
public static readonly string? assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
```

```csharp
ReplayDecoder decoder = new(assemblyPath);
Sc2Replay? replay = await decoder.DecodeAsync(pathToSC2Replay);
Console.WriteLine(replay.Header.BaseBuild);
```

Optional options:
```csharp
ReplayDecoder decoder = new(assemblyPath);

ReplayDecoderOptions options = new ReplayDecoderOptions()
{
    Details = false,
    Metadata = false,
    MessageEvents = false,
    TrackerEvents = true,
    GameEvents = false,
    AttributeEvents = false
};

CancellationTokenSource cts = new();

Sc2Replay? replay = await decoder.DecodeAsync(pathToSC2Replay, options, cts.Token);
Console.WriteLine(replay.TrackerEvents.SUnitBornEvents.FirstOrDefault());
```

Multiple replays:
```csharp
ReplayDecoder decoder = new(assemblyPath);
var folder = "path_to_replay_folder";
List<string> replays = Directory.GetFiles(folder, "*.SC2Replay").ToList();
ReplayDecoderOptions options = new ReplayDecoderOptions() { TrackerEvents = false };
int threads = 8;
CancellationTokenSource cts = new();

int decoded = 0;
int errors = 0;
await foreach (DecodeParallelResult decodeResult in decoder.DecodeParallelWithErrorReport(replays, 2, options, cts.Token))
{
    if (decodeResult.Sc2Replay == null)
    {
        Console.WriteLine($"failed decoding replay {decodeResult.ReplayPath}: {decodeResult.Exception}");
        errors++;
    }
    else
    {
        Console.WriteLine($"{decoded} {decodeResult.Sc2Replay.Details?.DateTimeUTC}");
        decoded++;
    }
}
```

# Known Limitations / ToDo

## GameEvents
STriggerSoundLengthSyncEvent => no data
SControlGroupUpdateEvent => no mask

# ChangeLog

<details open="open"><summary>v0.6.9</summary>

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

# Introduction

dotnet 6 wrapper for [Blizzards s2protocol](https://github.com/Blizzard/s2protocol)
using IronPython (2.7)

# Getting started
## Prerequisites
dotnet 6
## Installation
```csharp
dotnet add package s2protocol.NET
```
## Usage

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
    TrackerEvents = true
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

int i = 0;
await foreach (var sc2rep in decoder.DecodeParallel(replays, threads, options, cts.Token))
{
    i++;
    Console.WriteLine($"{i} {sc2rep.Details.DateTimeUTC}");
}
```

# Known Limitations
* No GameEvents
* No AttirbuteEvents
* No InitData

# ChangeLog
<details open="open"><summary>v0.5.0</summary>

>- Init

</details>

<details><summary>v0.5.0</summary>

>- Init
</details>
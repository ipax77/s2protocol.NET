using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using s2protocol.NET;
using s2protocol.NET.Models;

string replayPath = args.Length > 0
    ? args[0]
    : @"C:\Users\pax77\source\repos\s2protocol.NET\src\s2protocol.NET.Benchmarks\bin\Release\net10.0\replays\test6.SC2Replay";

Stopwatch stopwatch = Stopwatch.StartNew();
ReplayDecoder replayDecoder = new();
Sc2Replay sc2Replay = await replayDecoder.DecodeAsync(replayPath).ConfigureAwait(false);
stopwatch.Stop();

Console.WriteLine($"file={Path.GetFileName(replayPath)}");
Console.WriteLine($"baseBuild={sc2Replay.Header.BaseBuild}");
Console.WriteLine($"elapsedMs={stopwatch.Elapsed.TotalMilliseconds.ToString("F2", CultureInfo.InvariantCulture)}");
Console.WriteLine($"players={sc2Replay.Details?.Players.Count ?? 0}");
Console.WriteLine($"gameEvents={sc2Replay.GameEvents?.BaseGameEvents.Count ?? 0}");
Console.WriteLine($"messageEvents={sc2Replay.MessageEvents?.BaseMessageEvents.Count ?? 0}");
Console.WriteLine($"trackerEvents={CountTrackerEvents(sc2Replay.TrackerEvents)}");
Console.WriteLine($"attributeEvents={sc2Replay.AttributeEvents?.Attributes.Count ?? 0}");

GC.KeepAlive(sc2Replay);

string json = JsonSerializer.Serialize(sc2Replay, S2ProtocolJsonSerializerContext.Default.Sc2Replay);
Sc2Replay roundTripped = JsonSerializer.Deserialize(json, S2ProtocolJsonSerializerContext.Default.Sc2Replay)
    ?? throw new InvalidOperationException("The canonical replay JSON could not be deserialized.");
Console.WriteLine($"jsonRoundTripTrackerEvents={roundTripped.TrackerEvents?.BaseTrackerEvents.Count ?? 0}");

static int CountTrackerEvents(TrackerEvents? trackerEvents)
{
    if (trackerEvents is null)
    {
        return 0;
    }

    return trackerEvents.BaseTrackerEvents.Count;
}

using s2protocol.NET;

var replayPath = @"C:\Users\pax77\source\repos\s2protocol.NET\s2protocol.NET.tests\replays\test7.SC2Replay";

Console.WriteLine("Warmup...");
{
    using var warmupDecoder = new ReplayDecoder();
    var warmupReplay = await warmupDecoder.DecodeAsync(replayPath);
    ArgumentNullException.ThrowIfNull(warmupReplay);
}

GC.Collect();
GC.WaitForPendingFinalizers();
GC.Collect();

Console.WriteLine("Attach/start allocation recording now.");
Console.WriteLine("Press Enter to decode exactly once...");
Console.ReadLine();

long before = GC.GetTotalAllocatedBytes(precise: true);

using var replayDecoder = new ReplayDecoder();

var sc2Replay = await replayDecoder.DecodeAsync(replayPath);

long after = GC.GetTotalAllocatedBytes(precise: true);

ArgumentNullException.ThrowIfNull(sc2Replay);
ArgumentNullException.ThrowIfNull(sc2Replay.Details);

Console.WriteLine(sc2Replay.Details.DateTimeUTC.ToShortDateString());
Console.WriteLine($"Single decode allocated: {(after - before) / 1024.0 / 1024.0:N2} MB");

GC.KeepAlive(sc2Replay);

Console.WriteLine("Stop allocation recording now.");
Console.WriteLine("Press Enter to exit...");
Console.ReadLine();
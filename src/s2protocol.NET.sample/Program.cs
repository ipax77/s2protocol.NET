// See https://aka.ms/new-console-template for more information
using s2protocol.NET;
using s2protocol.NET.Models;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

Console.WriteLine("Hello, World!");

string? assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);


if (assemblyPath == null)
{
    Console.WriteLine($"assemblyPath was null");
}
else
{
    string replayPath = Path.Combine(assemblyPath, @"..\..\..\..\..\s2protocol.NET.tests\replays");
    string replayFilePath = Path.Combine(replayPath, "test3.SC2Replay");

    replayPath = @"C:\Users\pax77\Documents\StarCraft II\Accounts\107095918\2-S2-1-226401\Replays\Multiplayer";
    replayFilePath = Path.Combine(replayPath, "Direct Strike (2823).SC2Replay");

    List<string> replayFilePaths = Directory.GetFiles(replayPath, "*.SC2Replay").ToList();

    Console.WriteLine($"Found {replayFilePaths.Count} replays");

    ReplayDecoder decoder = new(assemblyPath, Microsoft.Extensions.Logging.LogLevel.Debug);

    ReplayDecoderOptions options = new ReplayDecoderOptions()
    {
        Initdata = false,
        Details = true,
        Metadata = false,
        MessageEvents = false,
        TrackerEvents = false,
        GameEvents = true
    };

    Sc2Replay? replay = await decoder.DecodeAsync(replayFilePath, options);

    if (replay != null && replay.Details != null)
    {
        Console.WriteLine($"replay dateTime: {replay.Details.DateTimeUTC}");

        Console.WriteLine($"replay gameevent SSelectionDeltaEvent: {replay.GameEvents.GetGameEvents<SBankFileEvent>().First()}");
    }

    //Stopwatch sw = new Stopwatch();
    //sw.Start();

    //int i = 0;
    //await foreach (var rep in decoder.DecodeParallel(replayFilePaths, 16, options))
    //{
    //    i++;
    //    if (rep != null && rep.Details != null)
    //    {
    //        Console.WriteLine($"replay {rep.Details.DateTimeUTC} {rep.FileName}");
    //    }
    //}

    //sw.Stop();
    //Console.WriteLine($"done decoding {i} replays in {sw.ElapsedMilliseconds}ms");
    Console.ReadLine();

    decoder.Dispose();
}
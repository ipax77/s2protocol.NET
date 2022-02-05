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

    List<string> replayFilePaths = Directory.GetFiles(replayPath).Take(1000).ToList();


    ReplayDecoder decoder = new(assemblyPath);

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
        var json = JsonSerializer.Serialize(replay, new JsonSerializerOptions() { WriteIndented = true });
        Console.WriteLine(json);

    }

    //Stopwatch sw = new Stopwatch();
    //sw.Start();

    //int i = 0;
    //await foreach (var rep in decoder.DecodeParallel(replayFilePaths, 16, options))
    //{
    //   i++;
    //   if (rep != null && rep.Details != null)
    //   {
    //    Console.WriteLine($"replay {rep.Details.DateTimeUTC}");
    //   }
    //}

    //sw.Stop();
    //Console.WriteLine($"done decoding {i} replays in {sw.ElapsedMilliseconds}ms");
    //Console.ReadLine();
    
    decoder.Dispose();
}
// See https://aka.ms/new-console-template for more information
using s2protocol.NET;
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
    string replayFilePath = Path.Combine(replayPath, "test1.SC2Replay");
    List<string> replayFilePaths = Directory.GetFiles(replayPath).ToList();


    ReplayDecoder decoder = new(assemblyPath);

    ReplayDecoderOptions options = new ReplayDecoderOptions()
    {
        Initdata = false,
        Details = false,
        Metadata = false,
        MessageEvents = false,
        TrackerEvents = false
    };

    // Sc2Replay? replay = await decoder.DecodeAsync(replayPath, options);
    // Sc2Replay? replay = await decoder.DecodeAsync(replayPath);

    await foreach (var replay in decoder.DecodeParallel(replayFilePaths, 8))
    {
        Console.WriteLine($"replay {replay.Details.DateTimeUTC}");
    }
}
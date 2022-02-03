// See https://aka.ms/new-console-template for more information
using s2protocol.NET;
using System.Reflection;

Console.WriteLine("Hello, World!");

string? assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);


if (assemblyPath == null)
{
    Console.WriteLine($"assemblyPath was null");
} 
else
{
    string replayPath = Path.Combine(assemblyPath, @"..\..\..\..\..\s2protocol.NET.tests\replays\test1.SC2Replay");

    ReplayDecoder decoder = new(assemblyPath);

    ReplayDecoderOptions options = new ReplayDecoderOptions()
    {
        Initdata = true,
        Details = false,
        Metadata = false,
        MessageEvents = false,
        TrackerEvents = false
    };

    Sc2Replay? replay = await decoder.DecodeAsync(replayPath, options);

    if (replay != null && replay.Initdata != null)
    {
        Console.WriteLine("indahouse");
    }
}
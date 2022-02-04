﻿// See https://aka.ms/new-console-template for more information
using s2protocol.NET;
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
    string replayFilePath = Path.Combine(replayPath, "test1.SC2Replay");

    replayPath = @"C:\Users\pax77\Documents\StarCraft II\Accounts\107095918\2-S2-1-226401\Replays\Multiplayer";

    List<string> replayFilePaths = Directory.GetFiles(replayPath).Take(1000).ToList();


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

    Stopwatch sw = new Stopwatch();
    sw.Start();

    int i = 0;
    await foreach (var replay in decoder.DecodeParallel(replayFilePaths, 16))
    {
        i++;
        Console.WriteLine($"replay {replay.Details.DateTimeUTC}");
    }

    sw.Stop();
    Console.WriteLine($"done decoding {i} replays in {sw.ElapsedMilliseconds}ms");
    Console.ReadLine();
}
// See https://aka.ms/new-console-template for more information
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
    string replayFilePath = Path.Combine(replayPath, "test2.SC2Replay");

    List<string> replayFilePaths = Directory.GetFiles(replayPath, "*.SC2Replay").Take(1000).ToList();

    Console.WriteLine($"Found {replayFilePaths.Count} replays");

    using ReplayDecoder decoder = new(assemblyPath);

    ReplayDecoderOptions options = new ReplayDecoderOptions()
    {
        // Initdata = false,
        // Details = true,
        // Metadata = false,
        // MessageEvents = false,
        // TrackerEvents = false,
        // GameEvents = false,
        // AttributeEvents = true
    };
    Stopwatch sw = new Stopwatch();
    sw.Start();
    Sc2Replay? replay = await decoder.DecodeAsync(replayFilePath, options);

    if (replay != null && replay.Details != null && replay.TrackerEvents != null)
    {
        var json = JsonSerializer.Serialize(replay);
        System.IO.File.WriteAllText("/data/ds/test/bab.json", json);

        Console.WriteLine($"replay dateTime: {replay.Details.DateTimeUTC}");

        foreach (var player in replay.Details.Players)
        {
            Console.WriteLine($"player {player.Name}({player.ClanName}) pos {player.WorkingSetSlotId} race {player.Race}");
        }

        foreach (var unit in replay.TrackerEvents.SUnitBornEvents)
        {
            if (unit.Gameloop == 0) continue;
            if (unit.ControlPlayerId != 1) continue;
            Console.WriteLine($"{unit.Gameloop} unit born: {unit.UnitTypeName} {unit.X}|{unit.Y}  control {unit.ControlPlayerId}");
            if (unit.SUnitDiedEvent != null)
            {
                Console.WriteLine($"\t{unit.SUnitDiedEvent.Gameloop} died at {unit.SUnitDiedEvent.X}|{unit.SUnitDiedEvent.Y} by {unit.SUnitDiedEvent.KillerPlayerId}");
                if (unit.SUnitDiedEvent.KillerUnitBornEvent != null)
                {
                    Console.WriteLine($"\t\t{unit.SUnitDiedEvent.KillerUnitBornEvent.Gameloop} born killer: {unit.SUnitDiedEvent.KillerUnitBornEvent.UnitTypeName} control {unit.SUnitDiedEvent.KillerUnitBornEvent.ControlPlayerId}");
                }
                if (unit.SUnitDiedEvent.KillerUnitInitEvent != null)
                {
                    Console.WriteLine($"\t\t{unit.SUnitDiedEvent.KillerUnitInitEvent.Gameloop} init killer: {unit.SUnitDiedEvent.KillerUnitInitEvent.UnitTypeName} control {unit.SUnitDiedEvent.KillerUnitInitEvent.ControlPlayerId}");
                }
            }
        }


    }

    int i = 0;

    // await foreach (var rep in decoder.DecodeParallel(replayFilePaths, 16, options))
    // {
    //     i++;
    //     if (rep != null && rep.Details != null)
    //     {
    //         Console.WriteLine($"replay {rep.Details.DateTimeUTC} {rep.FileName}");
    //     }
    // }

    sw.Stop();
    Console.WriteLine($"done decoding {i} replays in {sw.ElapsedMilliseconds}ms");
    Console.ReadLine();

    decoder.Dispose();
}
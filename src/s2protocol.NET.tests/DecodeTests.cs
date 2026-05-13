using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace s2protocol.NET.tests;

[TestClass]
public class DecodeTests
{
    public static readonly string? assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    [TestMethod]
    [DataRow("test1.SC2Replay")]
    [DataRow("test2.SC2Replay")]
    [DataRow("test3.SC2Replay")]
    [DataRow("test4.SC2Replay")]
    [DataRow("test5.SC2Replay")]
    [DataRow("test6.SC2Replay")]
    [DataRow("test7.SC2Replay")]
    public async Task HeaderTestAsync(string replayFile)
    {
        Assert.IsTrue(assemblyPath != null, "Could not get ExecutionAssembly path");
        if (assemblyPath == null)
        {
            return;
        }
        using ReplayDecoder decoder = new();
        ReplayDecoderOptions options = new ReplayDecoderOptions()
        {
            Initdata = false,
            Details = false,
            Metadata = false,
            MessageEvents = false,
            TrackerEvents = false,
            GameEvents = false,
            AttributeEvents = false,
        };
        var replay = await decoder.DecodeAsync(Path.Combine(assemblyPath, "replays", replayFile), options);
        Assert.IsTrue(replay != null, "Sc2Replay was null");
        if (replay == null)
        {
            decoder.Dispose();
            return;
        }
        Assert.IsTrue(replay.Header.BaseBuild > 0, "Could not get replay.Header BaseBuild");
        decoder.Dispose();
    }

    [TestMethod]
    [DataRow("test1.SC2Replay")]
    [DataRow("test2.SC2Replay")]
    [DataRow("test3.SC2Replay")]
    [DataRow("test4.SC2Replay")]
    [DataRow("test5.SC2Replay")]
    [DataRow("test6.SC2Replay")]
    [DataRow("test7.SC2Replay")]
    public async Task DetailsTestAsync(string replayFile)
    {
        Assert.IsTrue(assemblyPath != null, "Could not get ExecutionAssembly path");
        if (assemblyPath == null)
        {
            return;
        }
        using ReplayDecoder decoder = new();
        ReplayDecoderOptions options = new ReplayDecoderOptions()
        {
            Initdata = false,
            Details = true,
            Metadata = false,
            MessageEvents = false,
            TrackerEvents = false,
            GameEvents = false,
            AttributeEvents = false,
        };
        var replay = await decoder.DecodeAsync(Path.Combine(assemblyPath, "replays", replayFile), options);
        Assert.IsTrue(replay != null, "Sc2Replay was null");
        if (replay == null)
        {
            decoder.Dispose();
            return;
        }
        Assert.IsTrue(replay.Details != null, "Could not get replay.Details");
        if (replay.Details == null)
        {
            decoder.Dispose();
            return;
        }
        Assert.IsTrue(replay.Details.DateTimeUTC != DateTime.MinValue, "Could not get replay.Details DateTimeUTC");

        decoder.Dispose();
    }

    [TestMethod]
    [DataRow("test1.SC2Replay")]
    [DataRow("test2.SC2Replay")]
    [DataRow("test3.SC2Replay")]
    [DataRow("test4.SC2Replay")]
    [DataRow("test5.SC2Replay")]
    [DataRow("test6.SC2Replay")]
    [DataRow("test7.SC2Replay")]
    public async Task MetadataTestAsync(string replayFile)
    {
        Assert.IsTrue(assemblyPath != null, "Could not get ExecutionAssembly path");
        if (assemblyPath == null)
        {
            return;
        }
        using ReplayDecoder decoder = new();
        ReplayDecoderOptions options = new ReplayDecoderOptions()
        {
            Initdata = false,
            Details = false,
            Metadata = true,
            MessageEvents = false,
            TrackerEvents = false,
            GameEvents = false,
            AttributeEvents = false,
        };
        var replay = await decoder.DecodeAsync(Path.Combine(assemblyPath, "replays", replayFile), options);
        Assert.IsTrue(replay != null, "Sc2Replay was null");
        if (replay == null)
        {
            decoder.Dispose();
            return;
        }
        Assert.IsTrue(replay.Metadata != null, "Could not get replay.Metadata");
        if (replay.Metadata == null)
        {
            decoder.Dispose();
            return;
        }
        Assert.IsTrue(replay.Metadata.BaseBuild.Length > 0, "Could not get replay.Metadata BaseBuild");

        decoder.Dispose();
    }

    [TestMethod]
    [DataRow("test2.SC2Replay")]
    public async Task MessageEventsTestAsync(string replayFile)
    {
        Assert.IsTrue(assemblyPath != null, "Could not get ExecutionAssembly path");
        if (assemblyPath == null)
        {
            return;
        }
        using ReplayDecoder decoder = new();
        ReplayDecoderOptions options = new ReplayDecoderOptions()
        {
            Initdata = false,
            Details = false,
            Metadata = false,
            MessageEvents = true,
            TrackerEvents = false,
            GameEvents = false,
            AttributeEvents = false,
        };
        var replay = await decoder.DecodeAsync(Path.Combine(assemblyPath, "replays", replayFile), options);
        Assert.IsTrue(replay != null, "Sc2Replay was null");
        if (replay == null)
        {
            decoder.Dispose();
            return;
        }
        Assert.IsTrue(replay.ChatMessages != null, "Could not get replay.ChatMessages");
        if (replay.ChatMessages == null)
        {
            decoder.Dispose();
            return;
        }
        Assert.IsTrue(replay.ChatMessages.Count > 0, "Could not get replay.ChatMessages");

        decoder.Dispose();
    }

    [TestMethod]
    [DataRow("test1.SC2Replay")]
    [DataRow("test2.SC2Replay")]
    [DataRow("test3.SC2Replay")]
    [DataRow("test4.SC2Replay")]
    [DataRow("test5.SC2Replay")]
    [DataRow("test6.SC2Replay")]
    [DataRow("test7.SC2Replay")]
    [DataRow("test8.SC2Replay")]
    public async Task TrackerTestAsync(string replayFile)
    {
        Assert.IsTrue(assemblyPath != null, "Could not get ExecutionAssembly path");
        if (assemblyPath == null)
        {
            return;
        }
        using ReplayDecoder decoder = new();
        ReplayDecoderOptions options = new ReplayDecoderOptions()
        {
            Initdata = false,
            Details = false,
            Metadata = false,
            MessageEvents = false,
            TrackerEvents = true,
            GameEvents = false,
            AttributeEvents = false,
        };
        var replay = await decoder.DecodeAsync(Path.Combine(assemblyPath, "replays", replayFile), options);
        Assert.IsTrue(replay != null, "Sc2Replay was null");
        if (replay == null)
        {
            decoder.Dispose();
            return;
        }
        Assert.IsTrue(replay.TrackerEvents != null, "Could not get replay.TrackerEvents");
        if (replay.TrackerEvents == null)
        {
            decoder.Dispose();
            return;
        }
        Assert.IsTrue(replay.TrackerEvents.SUnitBornEvents.Count > 0, "Could not get replay.TrackerEvents SUnitBornEvents");
        Assert.IsTrue(replay.TrackerEvents.SPlayerStatsEvents.Count > 0, "Could not get replay.TrackerEvents SPlayerStatsEvents");
        Assert.IsTrue(replay.TrackerEvents.SUnitPositionsEvents.Count > 0, "Could not get replay.TrackerEvents SUnitPositionsEvents");

        var bornEvent = replay.TrackerEvents.SUnitBornEvents.First();
        Assert.IsFalse(string.IsNullOrEmpty(bornEvent.UnitTypeName));
        Assert.IsTrue(bornEvent.UnitTagIndex >= 0);

        var statsEvent = replay.TrackerEvents.SPlayerStatsEvents.First();
        Assert.IsTrue(statsEvent.FoodUsed >= 0);
        Assert.IsTrue(statsEvent.MineralsCurrent >= 0);

        var positionsEvent = replay.TrackerEvents.SUnitPositionsEvents.First();
        Assert.IsTrue(positionsEvent.FirstUnitIndex >= 0);
        Assert.IsTrue(positionsEvent.UnitPositions.Count > 0);

        decoder.Dispose();
    }

    [TestMethod]
    [DataRow("test1.SC2Replay")]
    [DataRow("test2.SC2Replay")]
    [DataRow("test3.SC2Replay")]
    [DataRow("test4.SC2Replay")]
    [DataRow("test5.SC2Replay")]
    [DataRow("test6.SC2Replay")]
    [DataRow("test7.SC2Replay")]
    [DataRow("test8.SC2Replay")]
    public async Task InitdataTestAsync(string replayFile)
    {
        Assert.IsTrue(assemblyPath != null, "Could not get ExecutionAssembly path");
        if (assemblyPath == null)
        {
            return;
        }
        using ReplayDecoder decoder = new();
        ReplayDecoderOptions options = new ReplayDecoderOptions()
        {
            Initdata = true,
            Details = false,
            Metadata = false,
            MessageEvents = false,
            TrackerEvents = false,
            GameEvents = false,
            AttributeEvents = false,
        };
        var replay = await decoder.DecodeAsync(Path.Combine(assemblyPath, "replays", replayFile), options);
        Assert.IsTrue(replay != null, "Sc2Replay was null");
        if (replay == null)
        {
            decoder.Dispose();
            return;
        }
        Assert.IsTrue(replay.Initdata != null, "Could not get replay.TrackerEvents");
        if (replay.Initdata == null)
        {
            decoder.Dispose();
            return;
        }
        decoder.Dispose();
    }

    [TestMethod]
    [DataRow("test1.SC2Replay")]
    [DataRow("test2.SC2Replay")]
    [DataRow("test3.SC2Replay")]
    [DataRow("test4.SC2Replay")]
    [DataRow("test5.SC2Replay")]
    [DataRow("test6.SC2Replay")]
    [DataRow("test7.SC2Replay")]
    [DataRow("test8.SC2Replay")]
    public async Task GameventsTestAsync(string replayFile)
    {
        Assert.IsTrue(assemblyPath != null, "Could not get ExecutionAssembly path");
        if (assemblyPath == null)
        {
            return;
        }
        using ReplayDecoder decoder = new();
        ReplayDecoderOptions options = new ReplayDecoderOptions()
        {
            Initdata = false,
            Details = false,
            Metadata = false,
            MessageEvents = false,
            TrackerEvents = false,
            GameEvents = true,
            AttributeEvents = false,
        };
        var replay = await decoder.DecodeAsync(Path.Combine(assemblyPath, "replays", replayFile), options);
        Assert.IsTrue(replay != null, "Sc2Replay was null");
        if (replay == null)
        {
            decoder.Dispose();
            return;
        }
        Assert.IsTrue(replay.GameEvents != null, "Could not get replay.GameEvents");
        if (replay.GameEvents == null)
        {
            decoder.Dispose();
            return;
        }
        decoder.Dispose();
    }

    [TestMethod]
    [DataRow("test1.SC2Replay")]
    [DataRow("test2.SC2Replay")]
    [DataRow("test3.SC2Replay")]
    [DataRow("test4.SC2Replay")]
    [DataRow("test5.SC2Replay")]
    [DataRow("test6.SC2Replay")]
    [DataRow("test7.SC2Replay")]
    [DataRow("test8.SC2Replay")]
    public async Task AttributeEventsTestAsync(string replayFile)
    {
        Assert.IsTrue(assemblyPath != null, "Could not get ExecutionAssembly path");
        if (assemblyPath == null)
        {
            return;
        }
        using ReplayDecoder decoder = new();
        ReplayDecoderOptions options = new ReplayDecoderOptions()
        {
            Initdata = false,
            Details = false,
            Metadata = false,
            MessageEvents = false,
            TrackerEvents = false,
            GameEvents = false,
            AttributeEvents = true,
        };
        var replay = await decoder.DecodeAsync(Path.Combine(assemblyPath, "replays", replayFile), options);
        Assert.IsTrue(replay != null, "Sc2Replay was null");
        if (replay == null)
        {
            decoder.Dispose();
            return;
        }
        Assert.IsTrue(replay.AttributeEvents != null, "Could not get replay.AttributeEvents");
        if (replay.AttributeEvents == null)
        {
            decoder.Dispose();
            return;
        }
        decoder.Dispose();
    }
}

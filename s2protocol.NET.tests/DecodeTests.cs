using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace s2protocol.NET.tests;

public class DecodeTests
{
    public static readonly string? assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    [Theory]
    [InlineData("test1.SC2Replay")]
    [InlineData("test2.SC2Replay")]
    [InlineData("test3.SC2Replay")]
    [InlineData("test4.SC2Replay")]
    [InlineData("test5.SC2Replay")]
    [InlineData("test6.SC2Replay")]
    [InlineData("test7.SC2Replay")]
    public async Task HeaderTestAsync(string replayFile)
    {
        Assert.True(assemblyPath != null, "Could not get ExecutionAssembly path");
        if (assemblyPath == null)
        {
            return;
        }
        using ReplayDecoder decoder = new(assemblyPath);
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
        Assert.True(replay != null, "Sc2Replay was null");
        if (replay == null)
        {
            decoder.Dispose();
            return;
        }
        Assert.True(replay.Header.BaseBuild > 0, "Could not get replay.Header BaseBuild");
        decoder.Dispose();
    }

    [Theory]
    [InlineData("test1.SC2Replay")]
    [InlineData("test2.SC2Replay")]
    [InlineData("test3.SC2Replay")]
    [InlineData("test4.SC2Replay")]
    [InlineData("test5.SC2Replay")]
    [InlineData("test6.SC2Replay")]
    [InlineData("test7.SC2Replay")]
    public async Task DetailsTestAsync(string replayFile)
    {
        Assert.True(assemblyPath != null, "Could not get ExecutionAssembly path");
        if (assemblyPath == null)
        {
            return;
        }
        using ReplayDecoder decoder = new(assemblyPath);
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
        Assert.True(replay != null, "Sc2Replay was null");
        if (replay == null)
        {
            decoder.Dispose();
            return;
        }
        Assert.True(replay.Details != null, "Could not get replay.Details");
        if (replay.Details == null)
        {
            decoder.Dispose();
            return;
        }
        Assert.True(replay.Details.DateTimeUTC != DateTime.MinValue, "Could not get replay.Details DateTimeUTC");

        decoder.Dispose();
    }

    [Theory]
    [InlineData("test1.SC2Replay")]
    [InlineData("test2.SC2Replay")]
    [InlineData("test3.SC2Replay")]
    [InlineData("test4.SC2Replay")]
    [InlineData("test5.SC2Replay")]
    [InlineData("test6.SC2Replay")]
    [InlineData("test7.SC2Replay")]
    public async Task MetadataTestAsync(string replayFile)
    {
        Assert.True(assemblyPath != null, "Could not get ExecutionAssembly path");
        if (assemblyPath == null)
        {
            return;
        }
        using ReplayDecoder decoder = new(assemblyPath);
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
        Assert.True(replay != null, "Sc2Replay was null");
        if (replay == null)
        {
            decoder.Dispose();
            return;
        }
        Assert.True(replay.Metadata != null, "Could not get replay.Metadata");
        if (replay.Metadata == null)
        {
            decoder.Dispose();
            return;
        }
        Assert.True(replay.Metadata.BaseBuild.Length > 0, "Could not get replay.Metadata BaseBuild");

        decoder.Dispose();
    }

    [Theory]
    [InlineData("test2.SC2Replay")]
    public async Task MessageEventsTestAsync(string replayFile)
    {
        Assert.True(assemblyPath != null, "Could not get ExecutionAssembly path");
        if (assemblyPath == null)
        {
            return;
        }
        using ReplayDecoder decoder = new(assemblyPath);
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
        Assert.True(replay != null, "Sc2Replay was null");
        if (replay == null)
        {
            decoder.Dispose();
            return;
        }
        Assert.True(replay.ChatMessages != null, "Could not get replay.ChatMessages");
        if (replay.ChatMessages == null)
        {
            decoder.Dispose();
            return;
        }
        Assert.True(replay.ChatMessages.Count > 0, "Could not get replay.ChatMessages");

        decoder.Dispose();
    }

    [Theory]
    [InlineData("test1.SC2Replay")]
    [InlineData("test2.SC2Replay")]
    [InlineData("test3.SC2Replay")]
    [InlineData("test4.SC2Replay")]
    [InlineData("test5.SC2Replay")]
    [InlineData("test6.SC2Replay")]
    [InlineData("test7.SC2Replay")]
    [InlineData("test8.SC2Replay")]
    public async Task TrackerTestAsync(string replayFile)
    {
        Assert.True(assemblyPath != null, "Could not get ExecutionAssembly path");
        if (assemblyPath == null)
        {
            return;
        }
        using ReplayDecoder decoder = new(assemblyPath);
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
        Assert.True(replay != null, "Sc2Replay was null");
        if (replay == null)
        {
            decoder.Dispose();
            return;
        }
        Assert.True(replay.TrackerEvents != null, "Could not get replay.TrackerEvents");
        if (replay.TrackerEvents == null)
        {
            decoder.Dispose();
            return;
        }
        Assert.True(replay.TrackerEvents.SUnitBornEvents.Count > 0, "Could not get replay.TrackerEvents SUnitBornEvents");

        decoder.Dispose();
    }

    [Theory]
    [InlineData("test1.SC2Replay")]
    [InlineData("test2.SC2Replay")]
    [InlineData("test3.SC2Replay")]
    [InlineData("test4.SC2Replay")]
    [InlineData("test5.SC2Replay")]
    [InlineData("test6.SC2Replay")]
    [InlineData("test7.SC2Replay")]
    [InlineData("test8.SC2Replay")]
    public async Task InitdataTestAsync(string replayFile)
    {
        Assert.True(assemblyPath != null, "Could not get ExecutionAssembly path");
        if (assemblyPath == null)
        {
            return;
        }
        using ReplayDecoder decoder = new(assemblyPath);
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
        Assert.True(replay != null, "Sc2Replay was null");
        if (replay == null)
        {
            decoder.Dispose();
            return;
        }
        Assert.True(replay.Initdata != null, "Could not get replay.TrackerEvents");
        if (replay.Initdata == null)
        {
            decoder.Dispose();
            return;
        }
        decoder.Dispose();
    }

    [Theory]
    [InlineData("test1.SC2Replay")]
    [InlineData("test2.SC2Replay")]
    [InlineData("test3.SC2Replay")]
    [InlineData("test4.SC2Replay")]
    [InlineData("test5.SC2Replay")]
    [InlineData("test6.SC2Replay")]
    [InlineData("test7.SC2Replay")]
    [InlineData("test8.SC2Replay")]
    public async Task GameventsTestAsync(string replayFile)
    {
        Assert.True(assemblyPath != null, "Could not get ExecutionAssembly path");
        if (assemblyPath == null)
        {
            return;
        }
        using ReplayDecoder decoder = new(assemblyPath);
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
        Assert.True(replay != null, "Sc2Replay was null");
        if (replay == null)
        {
            decoder.Dispose();
            return;
        }
        Assert.True(replay.GameEvents != null, "Could not get replay.GameEvents");
        if (replay.GameEvents == null)
        {
            decoder.Dispose();
            return;
        }
        decoder.Dispose();
    }

    [Theory]
    [InlineData("test1.SC2Replay")]
    [InlineData("test2.SC2Replay")]
    [InlineData("test3.SC2Replay")]
    [InlineData("test4.SC2Replay")]
    [InlineData("test5.SC2Replay")]
    [InlineData("test6.SC2Replay")]
    [InlineData("test7.SC2Replay")]
    [InlineData("test8.SC2Replay")]
    public async Task AttributeEventsTestAsync(string replayFile)
    {
        Assert.True(assemblyPath != null, "Could not get ExecutionAssembly path");
        if (assemblyPath == null)
        {
            return;
        }
        using ReplayDecoder decoder = new(assemblyPath);
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
        Assert.True(replay != null, "Sc2Replay was null");
        if (replay == null)
        {
            decoder.Dispose();
            return;
        }
        Assert.True(replay.AttributeEvents != null, "Could not get replay.AttributeEvents");
        if (replay.AttributeEvents == null)
        {
            decoder.Dispose();
            return;
        }
        decoder.Dispose();
    }
}
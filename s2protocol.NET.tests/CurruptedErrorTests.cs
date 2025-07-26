using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace s2protocol.NET.tests;

public class CurruptedErrorTests
{
    public static readonly string? assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    [Theory]
    [InlineData("testError.SC2Replay")]
    public async Task CurruptedTrackereventsTestsAsync(string replayFile)
    {
        Assert.True(assemblyPath != null, "Could not get ExecutionAssembly path");
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

        try
        {
            var replay = await decoder.DecodeAsync(Path.Combine(assemblyPath, "replays", replayFile), options);
        }
        catch (DecodeException ex)
        {
            Assert.Equal("Exception has been thrown by the target of an invocation.", ex.Message);
        }

        decoder.Dispose();
    }

    [Theory]
    [InlineData("testError2.SC2Replay")]
    public async Task CurruptedTestsAsync(string replayFile)
    {
        Assert.True(assemblyPath != null, "Could not get ExecutionAssembly path");
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

        try
        {
            var replay = await decoder.DecodeAsync(Path.Combine(assemblyPath, "replays", replayFile), options);
        }
        catch (DecodeException ex)
        {
            Assert.Equal("Value cannot be null. (Parameter 'trackerEvents')", ex.Message);
        }

        decoder.Dispose();
    }  
}

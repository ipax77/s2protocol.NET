using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
        ReplayDecoder decoder = new(assemblyPath);
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

        bool exception = false;
        try
        {
            var replay = await decoder.DecodeAsync(Path.Combine(assemblyPath, "replays", replayFile), options);
        }
        catch (Exception ex)
        {
            exception = ex.Message == "CorruptedError";
        }
        Assert.True(exception);

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
        ReplayDecoder decoder = new(assemblyPath);
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

        bool exception = false;
        try
        {
            var replay = await decoder.DecodeAsync(Path.Combine(assemblyPath, "replays", replayFile), options);
        }
        catch (Exception ex)
        {
            exception = ex.Message == "Could not generate MPQ archive";
        }
        Assert.True(exception);

        decoder.Dispose();
    }  
}

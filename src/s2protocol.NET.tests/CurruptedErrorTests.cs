using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace s2protocol.NET.tests;

[TestClass]
public class CurruptedErrorTests
{
    public static readonly string? assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    [TestMethod]
    [DataRow("testError.SC2Replay")]
    public async Task CurruptedTrackereventsTestsAsync(string replayFile)
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

        try
        {
            _ = await decoder.DecodeAsync(Path.Combine(assemblyPath, "replays", replayFile), options);
            Assert.Fail("Expected a decode exception.");
        }
        catch (DecodeException ex)
        {
            Assert.AreEqual("VersionedTypedDecoder", ex.Message);
        }

        decoder.Dispose();
    }

    [TestMethod]
    [DataRow("testError2.SC2Replay")]
    public async Task CurruptedTestsAsync(string replayFile)
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

        try
        {
            _ = await decoder.DecodeAsync(Path.Combine(assemblyPath, "replays", replayFile), options);
            Assert.Fail("Expected a decode exception.");
        }
        catch (DecodeException ex)
        {
            Assert.AreEqual("Value cannot be null. (Parameter 'trackerEvents')", ex.Message);
        }

        decoder.Dispose();
    }  
}

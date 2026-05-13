using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace s2protocol.NET.tests;

[TestClass]
public class PingMessagesTests
{
    public static readonly string? assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    [TestMethod]
    [DataRow("test1.SC2Replay")]
    [DataRow("test2.SC2Replay")]
    [DataRow("test7.SC2Replay")]
    public async Task PingMessagesTest(string replayFile)
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
        Assert.IsTrue(replay.PingMessages?.Count > 0);

        if (replay.PingMessages == null)
        {
            return;
        }
        Assert.IsTrue(replay.PingMessages.All(a => a.Gameloop > 0), "Some gameloops were 0.");
        Assert.IsTrue(replay.PingMessages.All(a => a.X > 0 && a.Y > 0), "Some point coords were 0");
        decoder.Dispose();
    }
}

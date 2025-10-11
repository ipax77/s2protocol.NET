using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace s2protocol.NET.tests;

public class LatestVersionTests
{
    public static readonly string? assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    [Theory]
    [InlineData("test9.SC2Replay")]
    public async Task HeaderTestAsync(string replayFile)
    {
        Assert.True(assemblyPath != null, "Could not get ExecutionAssembly path");
        if (assemblyPath == null)
        {
            return;
        }
        using ReplayDecoder decoder = new();
        ReplayDecoderOptions options = new ReplayDecoderOptions()
        {
            Initdata = true,
            Details = true,
            Metadata = true,
            MessageEvents = false,
            TrackerEvents = false,
            GameEvents = false,
            AttributeEvents = false,
        };
        var replay = await decoder.DecodeAsync(Path.Combine(assemblyPath, "replays", replayFile), options);
        Assert.NotNull(replay);
        if (replay == null)
        {
            decoder.Dispose();
            return;
        }

        int latestVersion = 95299;
        string latestVersionString = "5.0.15.95299";
        Assert.True(replay.Header.BaseBuild > 0, "Could not get replay.Header BaseBuild");
        Assert.Equal(latestVersion, replay.Header.BaseBuild);
        Assert.Equal(latestVersionString, replay.Metadata?.GameVersion.ToString()); 
        decoder.Dispose();
    }
}
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;


namespace s2protocol.NET.tests;

public class DecodeTests
{
    [Fact]
    public async Task HeaderTest()
    {
        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        Assert.True(path != null, "Could not get ExecutionAssembly path");
        if (path != null)
        {
            ReplayDecoder decoder = new(path);
            var replay = await decoder.DecodeAsync(@"C:\Users\pax77\Documents\StarCraft II\Accounts\107095918\2-S2-1-226401\Replays\Multiplayer\Direct Strike (4468).SC2Replay").ConfigureAwait(false);
            Assert.True(replay != null, "Sc2Replay was null");
            if (replay != null)
            {
                Assert.True(replay.HeaderPyDic != null, "Could not get replay.Header");
            }
            decoder.Dispose();
        }
    }
}
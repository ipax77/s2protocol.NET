using s2protocol.NET.Mpq;
using s2protocol.NET.S2Protocol;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xunit;

namespace s2protocol.NET.tests;

public class MPQArchiveTests
{
    public static readonly string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";

    [Fact]
    public void MPQArchiveHeaderTest()
    {
        var replayPath = Path.Combine(assemblyPath, "replays", "test.SC2Replay");
        using var mpqArchive = new MPQArchive(replayPath);
        var headerContent = mpqArchive.GetUserDataHeaderContent();
        Assert.NotNull(headerContent);
        var s2protocol = TypeInfoLoader.GetLatestVersion();
        var header = s2protocol.DecodeReplayHeader(headerContent);
        Assert.NotNull(header);
        long expectedVersion = 15405;
        if (header is Dictionary<string, object> headerDict)
        {
            Assert.True(headerDict.ContainsKey("m_version"));
            Assert.True(headerDict["m_version"] is Dictionary<string, object>);
            var versionDict = (Dictionary<string, object>)headerDict["m_version"];
            Assert.True(versionDict.ContainsKey("m_baseBuild"));
            Assert.IsType<long>(versionDict["m_baseBuild"]);
            long actualVersion = (long)versionDict["m_baseBuild"];
            Assert.Equal(expectedVersion, actualVersion);
        }
        else
        {
            Assert.Fail("Header is not a dictionary.");
        }
    }

    [Fact]
    public void FilesTest()
    {
        var replayPath = Path.Combine(assemblyPath, "replays", "test.SC2Replay");
        using var mpqArchive = new MPQArchive(replayPath);
        var files = mpqArchive.PrintFiles();
        Assert.NotNull(files);
        var expectedFiles = @"replay.attributes.events
replay.details
replay.game.events
replay.initData
replay.load.info
replay.message.events
replay.smartcam.events
replay.sync.events
";
        Assert.Equal(expectedFiles, files);
    }

    [Fact]
    public void HashTableTest()
    {
        var replayPath = Path.Combine(assemblyPath, "replays", "test.SC2Replay");
        using var mpqArchive = new MPQArchive(replayPath);
        var hashTable = mpqArchive.PrintHashTable();
        Assert.NotNull(hashTable);
        var expectedHashString = @"MPQ archive hash table
----------------------
 Hash A   Hash B  Locl Plat BlockIdx
D38437CB 07DFEAEC 0000 0000 00000009
AAC2A54B F4762B95 0000 0000 00000002
FFFFFFFF FFFFFFFF FFFF FFFF FFFFFFFF
FFFFFFFF FFFFFFFF FFFF FFFF FFFFFFFF
FFFFFFFF FFFFFFFF FFFF FFFF FFFFFFFF
C9E5B770 3B18F6B6 0000 0000 00000005
343C087B 278E3682 0000 0000 00000004
3B2B1EA0 B72EF057 0000 0000 00000006
5A7E8BDC FF253F5C 0000 0000 00000001
FD657910 4E9B98A7 0000 0000 00000008
D383C29C EF402E92 0000 0000 00000000
FFFFFFFF FFFFFFFF FFFF FFFF FFFFFFFF
FFFFFFFF FFFFFFFF FFFF FFFF FFFFFFFF
FFFFFFFF FFFFFFFF FFFF FFFF FFFFFFFF
1DA8B0CF A2CEFF28 0000 0000 00000007
31952289 6A5FFAA3 0000 0000 00000003

";
        Assert.Equal(expectedHashString, hashTable);
    }

    [Fact]
    public void BlockTableTest()
    {
        var replayPath = Path.Combine(assemblyPath, "replays", "test.SC2Replay");
        using var mpqArchive = new MPQArchive(replayPath);
        var hashTable = mpqArchive.PrintBlockTable();
        Assert.NotNull(hashTable);
        var expectedHashString = @"MPQ archive block table
----------------------
FileOffset ArchSize RealSize Flags
0000002C 727 890 81000200
00000303 801 1257 81000200
00000624 194096 479869 81000200
0002FC54 226 334 81000200
0002FD36 97 97 81000200
0002FD97 1323 1970 81000200
000302C2 6407 12431 81000200
00031BC9 533 2400 81000200
00031DDE 120 164 81000200
00031E56 254 288 81000200

";
        Assert.Equal(expectedHashString, hashTable);
    }
}

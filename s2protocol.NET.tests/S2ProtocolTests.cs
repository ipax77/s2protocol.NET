using s2protocol.NET.S2Protocol;
using Xunit;

namespace s2protocol.NET.tests;

public class S2ProtocolTests
{
    [Fact]
    public void VersionTest()
    {
        int requestVersion = 15405;
        var protocol = TypeInfoLoader.LoadTypeInfos(requestVersion);
        Assert.NotNull(protocol);
        Assert.Equal(requestVersion, protocol.Version);
    }

    [Fact]
    public void VersionFallbackTest()
    {
        int requestVersion = 15406;
        int minVersion = 15405;
        var protocol = TypeInfoLoader.LoadTypeInfos(requestVersion);
        Assert.NotNull(protocol);
        Assert.Equal(minVersion, protocol.Version);
    }

    [Fact]
    public void TypeIdTest()
    {
        var protocol = TypeInfoLoader.GetLatestVersion();
        Assert.NotNull(protocol.GameEventIdTypeId);
        Assert.NotNull(protocol.MessageEventIdTypeId);
        Assert.NotNull(protocol.TrackerEventIdTypeId);
        Assert.NotNull(protocol.SVarUint32TypeId);
        Assert.NotNull(protocol.ReplayUserIdTypeId);
        Assert.NotNull(protocol.ReplayHeaderTypeId);
        Assert.NotNull(protocol.GameDetailsTypeId);
        Assert.NotNull(protocol.ReplayInitDataTypeId);
    }
}

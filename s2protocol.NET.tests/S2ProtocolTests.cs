using s2protocol.NET.S2Protocol;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
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

    [Fact]
    public void ProtocolResourcesAreEmbeddedAsJsonOnly()
    {
        var resourceNames = typeof(TypeInfoLoader).Assembly.GetManifestResourceNames();

        var jsonResources = resourceNames
            .Where(name => name.StartsWith("s2protocol.NET.Resources.versions.protocol", StringComparison.Ordinal)
                && name.EndsWith(".json", StringComparison.Ordinal))
            .ToArray();
        var pythonResources = resourceNames
            .Where(name => name.StartsWith("s2protocol.NET.Resources.versions.protocol", StringComparison.Ordinal)
                && name.EndsWith(".py", StringComparison.Ordinal))
            .ToArray();

        Assert.Equal(91, jsonResources.Length);
        Assert.Empty(pythonResources);
    }

    [Fact]
    public void LatestVersionUsesHighestEmbeddedJsonProtocol()
    {
        var resourceNames = typeof(TypeInfoLoader).Assembly.GetManifestResourceNames();
        var latestResourceVersion = resourceNames
            .Select(name => Regex.Match(name, @"protocol(\d+)\.json"))
            .Where(match => match.Success)
            .Select(match => int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture))
            .Max();

        var protocol = TypeInfoLoader.GetLatestVersion();

        Assert.Equal(latestResourceVersion, protocol.Version);
    }

    [Fact]
    public void GeneratedJsonMatchesProtocol15405Fixture()
    {
        var protocol = TypeInfoLoader.LoadTypeInfos(15405);

        Assert.Equal(126, protocol.TypeInfos.Count);
        Assert.Equal(73, protocol.GameEvents.Count);
        Assert.Equal(3, protocol.MessageEvents.Count);
        Assert.Empty(protocol.TrackerEvents);
        Assert.Equal(11, protocol.ReplayHeaderTypeId);
        Assert.Equal(29, protocol.GameDetailsTypeId);
        Assert.Equal(54, protocol.ReplayInitDataTypeId);

        var choiceType = protocol.TypeInfos[6];
        Assert.Equal("_choice", choiceType.TypeName);
        var choiceBounds = Assert.IsType<S2TypeInfoTypeElement>(choiceType.Elements[0]);
        Assert.Equal(0, choiceBounds.Bounds.Min);
        Assert.Equal(2, choiceBounds.Bounds.Max);
        var choices = Assert.IsType<DsTypeInfoChoiceElemet>(choiceType.Elements[1]);
        Assert.Equal("m_uint22", choices.Elements[2].TypeName);
        Assert.Equal(4, choices.Elements[2].Number);

        var headerStruct = Assert.IsType<S2TypeInfoMElement>(protocol.TypeInfos[11].Elements[0]);
        Assert.Contains(headerStruct.Elements, field =>
            field.TypeName == "m_elapsedGameLoops"
            && field.Bounds.Min == 5
            && field.Bounds.Max == 3);

        Assert.Equal(61, protocol.GameEvents[5].TypeId);
        Assert.Equal("NNet.Game.SUserFinishedLoadingSyncEvent", protocol.GameEvents[5].Name);
        Assert.Equal(123, protocol.MessageEvents[0].TypeId);
        Assert.Equal("NNet.Game.SChatMessage", protocol.MessageEvents[0].Name);
    }
}

using s2protocol.NET.S2Protocol;
using System;
using System.Linq;
using System.Text.Json;
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

        Assert.Equal(24, jsonResources.Length);
        Assert.Contains(
            "s2protocol.NET.Resources.versions.protocol-versions.json",
            jsonResources);
        Assert.DoesNotContain(
            "s2protocol.NET.Resources.versions.protocol97563.json",
            jsonResources);
        Assert.Equal(
            23,
            jsonResources.Count(name => Regex.IsMatch(name, @"protocol\d+\.json$")));
        Assert.Empty(pythonResources);
    }

    [Fact]
    public void ProtocolManifestContainsAllSupportedVersions()
    {
        var manifest = LoadManifest();

        Assert.Equal(95, manifest.Versions.Count);
        Assert.Equal(80949, manifest.Versions[96883]);
        Assert.Equal(80949, manifest.Versions[97364]);
        Assert.Equal(80949, manifest.Versions[97425]);
        Assert.Equal(80949, manifest.Versions[97563]);
    }

    [Fact]
    public void LatestVersionUsesHighestManifestProtocol()
    {
        var latestManifestVersion = LoadManifest().Versions.Keys.Max();

        var protocol = TypeInfoLoader.GetLatestVersion();

        Assert.Equal(97563, latestManifestVersion);
        Assert.Equal(latestManifestVersion, protocol.Version);
    }

    [Fact]
    public void AliasedVersionsShareCanonicalDecoderTables()
    {
        var canonical = TypeInfoLoader.LoadTypeInfos(80949);
        var alias = TypeInfoLoader.LoadTypeInfos(97563);

        Assert.Equal(80949, canonical.Version);
        Assert.Equal(97563, alias.Version);
        Assert.Same(canonical.TypeInfos, alias.TypeInfos);
        Assert.Same(canonical.GameEvents, alias.GameEvents);
        Assert.Same(canonical.MessageEvents, alias.MessageEvents);
        Assert.Same(canonical.TrackerEvents, alias.TrackerEvents);
    }

    [Fact]
    public void ManifestVersionFallbackUsesNearestLowerBuild()
    {
        var protocol = TypeInfoLoader.LoadTypeInfos(97564);

        Assert.Equal(97563, protocol.Version);
    }

    [Fact]
    public void VersionBeforeEarliestManifestBuildThrows()
    {
        Assert.Throws<DecodeException>(() => TypeInfoLoader.LoadTypeInfos(15404));
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

    private static ProtocolVersionManifest LoadManifest()
    {
        var assembly = typeof(TypeInfoLoader).Assembly;
        using var stream = assembly.GetManifestResourceStream(
            "s2protocol.NET.Resources.versions.protocol-versions.json");
        Assert.NotNull(stream);

        var manifest = JsonSerializer.Deserialize(
            stream,
            S2ProtocolTypeInfoJsonSerializerContext.Default.ProtocolVersionManifest);
        return Assert.IsType<ProtocolVersionManifest>(manifest);
    }
}

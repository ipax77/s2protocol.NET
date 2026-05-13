using s2protocol.NET.S2Protocol;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace s2protocol.NET.tests;

[TestClass]
public class S2ProtocolTests
{
    [TestMethod]
    public void VersionTest()
    {
        int requestVersion = 15405;
        var protocol = TypeInfoLoader.LoadTypeInfos(requestVersion);
        Assert.IsNotNull(protocol);
        Assert.AreEqual(requestVersion, protocol.Version);
    }

    [TestMethod]
    public void VersionFallbackTest()
    {
        int requestVersion = 15406;
        int minVersion = 15405;
        var protocol = TypeInfoLoader.LoadTypeInfos(requestVersion);
        Assert.IsNotNull(protocol);
        Assert.AreEqual(minVersion, protocol.Version);
    }

    [TestMethod]
    public void TypeIdTest()
    {
        var protocol = TypeInfoLoader.GetLatestVersion();
        Assert.IsNotNull(protocol.GameEventIdTypeId);
        Assert.IsNotNull(protocol.MessageEventIdTypeId);
        Assert.IsNotNull(protocol.TrackerEventIdTypeId);
        Assert.IsNotNull(protocol.SVarUint32TypeId);
        Assert.IsNotNull(protocol.ReplayUserIdTypeId);
        Assert.IsNotNull(protocol.ReplayHeaderTypeId);
        Assert.IsNotNull(protocol.GameDetailsTypeId);
        Assert.IsNotNull(protocol.ReplayInitDataTypeId);
    }

    [TestMethod]
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

        Assert.AreEqual(91, jsonResources.Length);
        Assert.AreEqual(0, pythonResources.Length);
    }

    [TestMethod]
    public void LatestVersionUsesHighestEmbeddedJsonProtocol()
    {
        var resourceNames = typeof(TypeInfoLoader).Assembly.GetManifestResourceNames();
        var latestResourceVersion = resourceNames
            .Select(name => Regex.Match(name, @"protocol(\d+)\.json"))
            .Where(match => match.Success)
            .Select(match => int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture))
            .Max();

        var protocol = TypeInfoLoader.GetLatestVersion();

        Assert.AreEqual(latestResourceVersion, protocol.Version);
    }

    [TestMethod]
    public void GeneratedJsonMatchesProtocol15405Fixture()
    {
        var protocol = TypeInfoLoader.LoadTypeInfos(15405);

        Assert.AreEqual(126, protocol.TypeInfos.Count);
        Assert.AreEqual(73, protocol.GameEvents.Count);
        Assert.AreEqual(3, protocol.MessageEvents.Count);
        Assert.AreEqual(0, protocol.TrackerEvents.Count);
        Assert.AreEqual(11, protocol.ReplayHeaderTypeId);
        Assert.AreEqual(29, protocol.GameDetailsTypeId);
        Assert.AreEqual(54, protocol.ReplayInitDataTypeId);

        var choiceType = protocol.TypeInfos[6];
        Assert.AreEqual("_choice", choiceType.TypeName);
        Assert.IsInstanceOfType(choiceType.Elements[0], typeof(S2TypeInfoTypeElement));
        var choiceBounds = (S2TypeInfoTypeElement)choiceType.Elements[0];
        Assert.AreEqual(0, choiceBounds.Bounds.Min);
        Assert.AreEqual(2, choiceBounds.Bounds.Max);
        Assert.IsInstanceOfType(choiceType.Elements[1], typeof(DsTypeInfoChoiceElemet));
        var choices = (DsTypeInfoChoiceElemet)choiceType.Elements[1];
        Assert.AreEqual("m_uint22", choices.Elements[2].TypeName);
        Assert.AreEqual(4, choices.Elements[2].Number);

        Assert.IsInstanceOfType(protocol.TypeInfos[11].Elements[0], typeof(S2TypeInfoMElement));
        var headerStruct = (S2TypeInfoMElement)protocol.TypeInfos[11].Elements[0];
        Assert.IsTrue(headerStruct.Elements.Any(field =>
            field.TypeName == "m_elapsedGameLoops"
            && field.Bounds.Min == 5
            && field.Bounds.Max == 3));

        Assert.AreEqual(61, protocol.GameEvents[5].TypeId);
        Assert.AreEqual("NNet.Game.SUserFinishedLoadingSyncEvent", protocol.GameEvents[5].Name);
        Assert.AreEqual(123, protocol.MessageEvents[0].TypeId);
        Assert.AreEqual("NNet.Game.SChatMessage", protocol.MessageEvents[0].Name);
    }
}

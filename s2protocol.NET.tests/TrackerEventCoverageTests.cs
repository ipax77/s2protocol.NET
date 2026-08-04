using s2protocol.NET.Models;
using s2protocol.NET.S2Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace s2protocol.NET.tests;

public partial class TrackerEventCoverageTests
{
    [Fact]
    public void EveryBundledTrackerEventDecodesToItsPublicConcreteModel()
    {
        HashSet<string> union = new(StringComparer.Ordinal);
        int[] versions = CanonicalProtocolVersions();
        Assert.Equal(23, versions.Length);
        Assert.Equal(14, versions.Count(version => TypeInfoLoader.LoadTypeInfos(version).TrackerEvents.Count > 0));

        foreach (int version in versions)
        {
            S2ProtocolVersion protocol = TypeInfoLoader.LoadTypeInfos(version);
            foreach ((int eventId, S2EventType eventType) in protocol.TrackerEvents)
            {
                string eventName = ShortName(eventType.Name);
                union.Add(eventName);

                byte[] encoded = VersionedTestEncoder.EncodeTrackerEvent(protocol, eventId);
                TrackerEvent decoded = Assert.Single(AllEvents(protocol.DecodeReplayTrackerEvents(encoded)));

                Assert.Equal(eventName, decoded.GetType().Name);
                Assert.Equal(eventId, decoded.EventId);
                Assert.NotEqual(TrackerEventType.None, decoded.EventType);
            }
        }

        Assert.Equal(10, union.Count);
    }

    [Fact]
    public void EveryBundledNameHasASealedPublicModelAndEnumValue()
    {
        HashSet<string> union = CanonicalProtocolVersions()
            .Select(TypeInfoLoader.LoadTypeInfos)
            .SelectMany(protocol => protocol.TrackerEvents.Values)
            .Select(eventType => ShortName(eventType.Name))
            .ToHashSet(StringComparer.Ordinal);

        Assert.Equal(10, union.Count);
        foreach (string eventName in union)
        {
            Type? model = typeof(TrackerEvent).Assembly.GetType($"s2protocol.NET.Models.{eventName}");
            Assert.NotNull(model);
            Assert.True(model.IsPublic, eventName);
            Assert.True(model.IsSealed, eventName);
            Assert.True(typeof(TrackerEvent).IsAssignableFrom(model), eventName);
            Assert.True(Enum.TryParse(eventName, out TrackerEventType parsed), eventName);
            Assert.NotEqual(TrackerEventType.None, parsed);
        }
    }

    [Fact]
    public void PlayerSetupPreservesPresentAndMissingSlotId()
    {
        SPlayerSetupEvent missing = Decode<SPlayerSetupEvent>(80949);
        SPlayerSetupEvent present = Decode<SPlayerSetupEvent>(80949,
            new Dictionary<string, object?> { ["m_slotId"] = 7L });

        Assert.Null(missing.SlotId);
        Assert.Equal(7, present.SlotId);
    }

    [Fact]
    public void HistoricalNullableUnitReferencesRemainDistinct()
    {
        SUnitBornEvent beforeCreatorFields = Decode<SUnitBornEvent>(54518);
        Assert.Null(beforeCreatorFields.CreatorUnitTagIndex);
        Assert.Null(beforeCreatorFields.CreatorUnitTagRecycle);
        Assert.Null(beforeCreatorFields.CreatorAbilityName);

        SUnitBornEvent withCreatorFields = Decode<SUnitBornEvent>(55505,
            new Dictionary<string, object?>
            {
                ["m_creatorUnitTagIndex"] = 11L,
                ["m_creatorUnitTagRecycle"] = 12L,
                ["m_creatorAbilityName"] = "Build",
            });
        Assert.Equal(11, withCreatorFields.CreatorUnitTagIndex);
        Assert.Equal(12, withCreatorFields.CreatorUnitTagRecycle);
        Assert.Equal("Build", withCreatorFields.CreatorAbilityName);

        SUnitDiedEvent beforeKillerTagFields = Decode<SUnitDiedEvent>(26490);
        Assert.Null(beforeKillerTagFields.KillerUnitTagIndex);
        Assert.Null(beforeKillerTagFields.KillerUnitTagRecycle);

        SUnitDiedEvent withKillerTagFields = Decode<SUnitDiedEvent>(27950,
            new Dictionary<string, object?>
            {
                ["m_killerUnitTagIndex"] = 21L,
                ["m_killerUnitTagRecycle"] = 22L,
            });
        Assert.Equal(21, withKillerTagFields.KillerUnitTagIndex);
        Assert.Equal(22, withKillerTagFields.KillerUnitTagRecycle);
    }

    private static T Decode<T>(int version, IReadOnlyDictionary<string, object?>? values = null)
        where T : TrackerEvent
    {
        S2ProtocolVersion protocol = TypeInfoLoader.LoadTypeInfos(version);
        string fullName = $"NNet.Replay.Tracker.{typeof(T).Name}";
        KeyValuePair<int, S2EventType> eventType = protocol.TrackerEvents.Single(
            item => string.Equals(item.Value.Name, fullName, StringComparison.Ordinal));
        byte[] encoded = VersionedTestEncoder.EncodeTrackerEvent(protocol, eventType.Key, values);
        return Assert.IsType<T>(Assert.Single(AllEvents(protocol.DecodeReplayTrackerEvents(encoded))));
    }

    private static IEnumerable<TrackerEvent> AllEvents(TrackerEvents events)
        => events.SPlayerSetupEvents.Cast<TrackerEvent>()
            .Concat(events.SPlayerStatsEvents)
            .Concat(events.SUnitBornEvents)
            .Concat(events.SUnitDiedEvents)
            .Concat(events.SUnitOwnerChangeEvents)
            .Concat(events.SUnitPositionsEvents)
            .Concat(events.SUnitTypeChangeEvents)
            .Concat(events.SUpgradeEvents)
            .Concat(events.SUnitInitEvents)
            .Concat(events.SUnitDoneEvents);

    private static string ShortName(string fullName) => fullName[(fullName.LastIndexOf('.') + 1)..];

    private static int[] CanonicalProtocolVersions()
        => typeof(TypeInfoLoader).Assembly.GetManifestResourceNames()
            .Select(name => ProtocolResourceRegex().Match(name))
            .Where(match => match.Success)
            .Select(match => int.Parse(match.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture))
            .Order()
            .ToArray();

    [GeneratedRegex(@"\.protocol(\d+)\.json$", RegexOptions.CultureInvariant)]
    private static partial Regex ProtocolResourceRegex();
}

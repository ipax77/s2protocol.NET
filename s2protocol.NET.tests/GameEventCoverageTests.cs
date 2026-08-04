using s2protocol.NET.Models;
using s2protocol.NET.S2Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace s2protocol.NET.tests;

public partial class GameEventCoverageTests
{
    [Fact]
    public void EveryBundledGameEventDecodesToItsPublicConcreteModel()
    {
        HashSet<string> union = new(StringComparer.Ordinal);
        int[] versions = CanonicalProtocolVersions();
        Assert.Equal(23, versions.Length);

        foreach (int version in versions)
        {
            S2ProtocolVersion protocol = TypeInfoLoader.LoadTypeInfos(version);
            foreach ((int eventId, S2EventType eventType) in protocol.GameEvents)
            {
                string eventName = eventType.Name[(eventType.Name.LastIndexOf('.') + 1)..];
                union.Add(eventName);

                byte[] encoded = BitPackedTestEncoder.EncodeGameEvent(protocol, eventId);
                GameEvent decoded;
                try
                {
                    decoded = Assert.Single(protocol.DecodeReplayGameEvents(encoded).BaseGameEvents);
                }
                catch (DecodeException exception)
                {
                    throw new InvalidOperationException(
                        $"Failed to decode {eventType.Name} ({eventId}) using protocol {version}.",
                        exception);
                }

                Assert.IsNotType<UnknownGameEvent>(decoded);
                Assert.Equal(eventName, decoded.GetType().Name);
                Assert.NotEqual(GameEventType.None, decoded.EventType);
            }
        }

        Assert.Equal(105, union.Count);
    }

    [Fact]
    public void EveryBundledNameHasASealedPublicModelAndEnumValue()
    {
        HashSet<string> union = CanonicalProtocolVersions()
            .Select(TypeInfoLoader.LoadTypeInfos)
            .SelectMany(protocol => protocol.GameEvents.Values)
            .Select(eventType => eventType.Name[(eventType.Name.LastIndexOf('.') + 1)..])
            .ToHashSet(StringComparer.Ordinal);

        Assert.Equal(105, union.Count);
        foreach (string eventName in union)
        {
            Type? model = typeof(GameEvent).Assembly.GetType($"s2protocol.NET.Models.{eventName}");
            Assert.NotNull(model);
            Assert.True(model.IsPublic, eventName);
            Assert.True(model.IsSealed, eventName);
            Assert.True(typeof(GameEvent).IsAssignableFrom(model), eventName);

            string enumName = eventName == "SCmdUpdateTargetUnitEvent"
                ? nameof(GameEventType.SCmdUpdateTargetUnitEvents)
                : eventName;
            Assert.True(Enum.TryParse(enumName, out GameEventType parsed), eventName);
            Assert.NotEqual(GameEventType.None, parsed);
        }
    }

    [Fact]
    public void DialogControlDecodesEveryChoiceBranch()
    {
        (string Name, object? Value, DialogControlEventDataKind Kind)[] cases =
        [
            ("None", null, DialogControlEventDataKind.None),
            ("Checked", true, DialogControlEventDataKind.Checked),
            ("ValueChanged", 123L, DialogControlEventDataKind.ValueChanged),
            ("SelectionChanged", 17L, DialogControlEventDataKind.SelectionChanged),
            ("TextChanged", "dialog text", DialogControlEventDataKind.TextChanged),
            ("MouseButton", 2L, DialogControlEventDataKind.MouseButton),
        ];

        foreach ((string name, object? value, DialogControlEventDataKind kind) in cases)
        {
            STriggerDialogControlEvent decoded = Decode<STriggerDialogControlEvent>(80949,
                new Dictionary<string, object?>
                {
                    ["m_controlId"] = 42L,
                    ["m_eventType"] = 7L,
                    ["m_eventData"] = new TestChoiceValue(name, value),
                });

            Assert.Equal(42, decoded.ControlId);
            Assert.Equal(7, decoded.EventTypeId);
            Assert.Equal(kind, decoded.EventDataKind);
            Assert.Equal(name == "Checked" ? true : null, decoded.Checked);
            Assert.Equal(name == "ValueChanged" ? 123 : null, decoded.ValueChanged);
            Assert.Equal(name == "SelectionChanged" ? 17 : null, decoded.SelectionChanged);
            Assert.Equal(name == "TextChanged" ? "dialog text" : null, decoded.TextChanged);
            Assert.Equal(name == "MouseButton" ? 2 : null, decoded.MouseButton);
        }
    }

    [Fact]
    public void CameraTargetSelectionAndCommandStateValuesDecode()
    {
        SCameraUpdateEvent camera = Decode<SCameraUpdateEvent>(80949,
            new Dictionary<string, object?>
            {
                ["m_target.x"] = 101L,
                ["m_target.y"] = 202L,
                ["m_distance"] = 303L,
                ["m_pitch"] = 11L,
                ["m_yaw"] = 12L,
                ["m_reason"] = 3L,
                ["m_follow"] = true,
            });
        Assert.Equal(101, camera.TargetX);
        Assert.Equal(202, camera.TargetY);
        Assert.Equal(303, camera.Distance);
        Assert.Equal(11, camera.Pitch);
        Assert.Equal(12, camera.Yaw);
        Assert.Equal("3", camera.Reason);
        Assert.True(camera.Follow);

        SCmdUpdateTargetPointEvent target = Decode<SCmdUpdateTargetPointEvent>(80949,
            new Dictionary<string, object?>
            {
                ["m_target.x"] = 1001L,
                ["m_target.y"] = 1002L,
                ["m_target.z"] = 1003L,
            });
        Assert.Equal(1001, target.TargetX);
        Assert.Equal(1002, target.TargetY);
        Assert.Equal(1003, target.TargetZ);

        SSelectionDeltaEvent selection = Decode<SSelectionDeltaEvent>(80949,
            new Dictionary<string, object?>
            {
                ["m_controlGroupId"] = 4L,
                ["m_delta.m_subgroupIndex"] = 2L,
                ["m_delta.m_removeMask"] = new TestChoiceValue("ZeroIndices", new object?[] { 6L, 8L }),
                ["m_delta.m_addUnitTags"] = new object?[] { 101L, 102L },
                ["m_delta.m_addSubgroups"] = new object?[] { null },
                ["m_delta.m_addSubgroups[0].m_unitLink"] = 9L,
                ["m_delta.m_addSubgroups[0].m_subgroupPriority"] = 3L,
                ["m_delta.m_addSubgroups[0].m_count"] = 2L,
                ["m_delta.m_addSubgroups[0].m_intraSubgroupPriority"] = 1L,
            });
        Assert.Equal(4, selection.ControlGroupId);
        Assert.Equal(2, selection.Delta.SubgroupIndex);
        Assert.Equal([6, 8], selection.Delta.ZeroIndices);
        Assert.Equal([101, 102], selection.Delta.AddUnitTags);
        SelectionDeltaEventDeltaSubGroup subgroup = Assert.Single(selection.Delta.AddSubgroups);
        Assert.Equal(9, subgroup.UnitLink);
        Assert.Equal(3, subgroup.SubgroupPriority);
        Assert.Equal(2, subgroup.Count);
        Assert.Equal(1, subgroup.IntraSubgroupPriority);

        SCommandManagerStateEvent command = Decode<SCommandManagerStateEvent>(80949,
            new Dictionary<string, object?> { ["m_state"] = 1L, ["m_sequence"] = 19L });
        Assert.Equal(1, command.State);
        Assert.Equal(19, command.Sequence);
    }

    [Fact]
    public void HistoricalOnlyAndAICommunicateLayoutsDecode()
    {
        SLagMessageEvent lag = Decode<SLagMessageEvent>(15405,
            new Dictionary<string, object?> { ["m_laggingPlayerId"] = 3L });
        Assert.Equal(3, lag.LaggingPlayerId);
        Assert.IsType<SPlayerLeaveEvent>(Decode<SPlayerLeaveEvent>(15405));
        STriggerCameraMoveEvent cameraMove = Decode<STriggerCameraMoveEvent>(15405,
            new Dictionary<string, object?> { ["m_reason"] = 2L });
        Assert.Equal(2, cameraMove.Reason);

        SAICommunicateEvent historical = Decode<SAICommunicateEvent>(15405,
            new Dictionary<string, object?>
            {
                ["m_beacon"] = 1L,
                ["m_ally"] = 2L,
                ["m_autocast"] = 3L,
                ["m_targetUnitTag"] = 100L,
                ["m_targetUnitSnapshotUnitLink"] = 7L,
                ["m_targetUnitSnapshotPlayerId"] = 4L,
                ["m_targetPoint.x"] = 11L,
                ["m_targetPoint.y"] = 12L,
                ["m_targetPoint.z"] = 13L,
            });
        Assert.Equal(3, historical.Autocast);
        Assert.Null(historical.Flags);
        Assert.Null(historical.Build);
        Assert.Equal(4, historical.TargetUnitSnapshotPlayerId);
        Assert.Equal(new GameEventPoint3D(11, 12, 13), historical.TargetPoint);

        SAICommunicateEvent current = Decode<SAICommunicateEvent>(80949,
            new Dictionary<string, object?>
            {
                ["m_beacon"] = 4L,
                ["m_ally"] = 5L,
                ["m_flags"] = 6L,
                ["m_build"] = 7L,
                ["m_targetUnitTag"] = 200L,
                ["m_targetUnitSnapshotUnitLink"] = 8L,
                ["m_targetUnitSnapshotUpkeepPlayerId"] = 9L,
                ["m_targetUnitSnapshotControlPlayerId"] = 10L,
                ["m_targetPoint.x"] = 21L,
                ["m_targetPoint.y"] = 22L,
                ["m_targetPoint.z"] = 23L,
            });
        Assert.Null(current.Autocast);
        Assert.Equal(6, current.Flags);
        Assert.Equal(7, current.Build);
        Assert.Equal(9, current.TargetUnitSnapshotUpkeepPlayerId);
        Assert.Equal(10, current.TargetUnitSnapshotControlPlayerId);
        Assert.Equal(new GameEventPoint3D(21, 22, 23), current.TargetPoint);
    }

    [Fact]
    public void DecrementSecondsAndMillisecondsLayoutsRemainDistinct()
    {
        SDecrementGameTimeRemainingEvent milliseconds = Decode<SDecrementGameTimeRemainingEvent>(15405,
            new Dictionary<string, object?> { ["m_decrementMs"] = 2500L });
        Assert.Equal(0, milliseconds.DecrementSeconds);
        Assert.Equal(2500, milliseconds.DecrementMilliseconds);

        SDecrementGameTimeRemainingEvent seconds = Decode<SDecrementGameTimeRemainingEvent>(80949,
            new Dictionary<string, object?> { ["m_decrementSeconds"] = 5L });
        Assert.Equal(5, seconds.DecrementSeconds);
        Assert.Null(seconds.DecrementMilliseconds);
    }

    private static T Decode<T>(
        int version,
        IReadOnlyDictionary<string, object?>? values = null)
        where T : GameEvent
    {
        S2ProtocolVersion protocol = TypeInfoLoader.LoadTypeInfos(version);
        string fullName = $"NNet.Game.{typeof(T).Name}";
        KeyValuePair<int, S2EventType> eventType = protocol.GameEvents.Single(
            item => string.Equals(item.Value.Name, fullName, StringComparison.Ordinal));
        byte[] encoded = BitPackedTestEncoder.EncodeGameEvent(protocol, eventType.Key, values);
        return Assert.IsType<T>(Assert.Single(protocol.DecodeReplayGameEvents(encoded).BaseGameEvents));
    }

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

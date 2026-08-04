namespace s2protocol.NET.Models;

/// <summary>A two-dimensional point contained in a game-event payload.</summary>
public readonly record struct GameEventPoint2D(long X, long Y);

/// <summary>A three-dimensional point contained in a game-event payload.</summary>
public readonly record struct GameEventPoint3D(long X, long Y, long Z);

/// <summary>An ability reference contained in a game-event payload.</summary>
public readonly record struct GameEventAbility(int AbilLink, int AbilCmdIndex, int? AbilCmdData);

/// <summary>The structured payload of <see cref="SGameCheatEvent"/>.</summary>
public readonly record struct GameCheatEventData(
    GameEventPoint2D Point,
    long Time,
    string Verb,
    string Arguments);

/// <summary>The selection checksums contained in <see cref="SSelectionSyncCheckEvent"/>.</summary>
public readonly record struct SelectionSyncCheckEventData(
    int Count,
    int SubgroupCount,
    int ActiveSubgroupIndex,
    long UnitTagsChecksum,
    long SubgroupIndicesChecksum,
    long SubgroupsChecksum);

/// <summary>The active payload branch in a dialog-control event.</summary>
public enum DialogControlEventDataKind
{
    None,
    Checked,
    ValueChanged,
    SelectionChanged,
    TextChanged,
    MouseButton,
}

using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>SUnitBornEvent</c> SUnitBornEvent</summary>
/// <comment>NNet.Replay.Tracker.SUnitBornEvent events appear for units that are created fully constructed.</comment>
///
public sealed class SUnitBornEvent(int playerId,
                                   int eventId,
                                   int bits,
                                   int gameloop,
                                   int unitTagIndex,
                                   int unitTagRecycle,
                                   string? creatorAbilityName,
                                   int? creatorUnitTagRecycle,
                                   int controlPlayerId,
                                   int x,
                                   int y,
                                   int upkeepPlayerId,
                                   string unitTypeName,
                                   int? creatorUnitTagIndex) : TrackerEvent(playerId, eventId, TrackerEventType.SUnitBornEvent, bits, gameloop)
{
    /// <summary>Event UnitIndex </summary>
    /// <comment>Convert unit tag index, recycle pairs into unit tags (as seen in game events) with protocol.unit_tag(index, recycle)</comment>
    ///
    public int UnitIndex { get; internal set; }
    /// <summary>Event UnitTagIndex</summary>
    ///
    public int UnitTagIndex { get; } = unitTagIndex;
    /// <summary>Event UnitTagRecycle</summary>
    ///
    public int UnitTagRecycle { get; } = unitTagRecycle;
    /// <summary>Event CreatorAbilityName</summary>
    ///
    public string? CreatorAbilityName { get; } = creatorAbilityName;
    /// <summary>Event CreatorUnitTagRecycle</summary>
    ///
    public int? CreatorUnitTagRecycle { get; } = creatorUnitTagRecycle;
    /// <summary>Event ControlPlayerId</summary>
    ///
    public int ControlPlayerId { get; } = controlPlayerId;
    /// <summary>Event Y</summary>
    ///
    public int Y { get; } = y;
    /// <summary>Event X</summary>
    ///
    public int X { get; } = x;
    /// <summary>Event UpkeepPlayerId</summary>
    ///
    public int UpkeepPlayerId { get; } = upkeepPlayerId;
    /// <summary>Event UnitTypeName</summary>
    ///
    public string UnitTypeName { get; } = unitTypeName;
    /// <summary>Event CreatorUnitTagIndex</summary>
    ///
    public int? CreatorUnitTagIndex { get; } = creatorUnitTagIndex;
    /// <summary>Event SUnitDiedEvent</summary>
    ///
    [JsonIgnore]
    public SUnitDiedEvent? SUnitDiedEvent { get; internal set; }
}

using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>SUnitDiedEvent</c> SUnitDiedEvent</summary>
///
/// <remarks>Record <c>SUnitDiedEvent</c> constructor</remarks>
/// <comment>You may receive a SUnitDiedEvent after either a UnitInit or UnitBorn event for the corresponding unit tag.</comment>
/// <comment>There's a known issue where revived units are not tracked, and placeholder units track death but not birth.</comment>
/// 
public sealed class SUnitDiedEvent(int playerId,
                                   int eventId,
                                   int bits,
                                   int gameloop,
                                   int unitTagIndex,
                                   int unitTagRecycle,
                                   int? killerPlayerId,
                                   int x,
                                   int y,
                                   int? killerUnitTagRecycle,
                                   int? killerUnitTagIndex) : TrackerEvent(playerId, eventId, TrackerEventType.SUnitDiedEvent, bits, gameloop)
{
    /// <summary>Event UnitIndex</summary>
    /// <comment>Convert unit tag index, recycle pairs into unit tags (as seen in game events) with protocol.unit_tag(index, recycle)</comment>
    /// 
    public int UnitIndex { get; internal set; }
    /// <summary>Event UnitTagIndex</summary>
    ///
    public int UnitTagIndex { get; } = unitTagIndex;
    /// <summary>Event UnitTagRecycle</summary>
    ///
    public int UnitTagRecycle { get; } = unitTagRecycle;
    /// <summary>Event ControlPlayerId</summary>
    ///
    public int? KillerPlayerId { get; } = killerPlayerId;
    /// <summary>Event Y</summary>
    ///
    public int Y { get; } = y;
    /// <summary>Event X</summary>
    ///
    public int X { get; } = x;
    /// <summary>Event KillerUnitTagRecycle</summary>
    ///
    public int? KillerUnitTagRecycle { get; } = killerUnitTagRecycle;
    /// <summary>Event KillerUnitTagIndex</summary>
    ///
    public int? KillerUnitTagIndex { get; } = killerUnitTagIndex;
    /// <summary>Event KillerUnitEvent - either SUnitBorn- or SUnitInitEvent</summary>
    ///
    public SUnitInitEvent? KillerUnitInitEvent { get; internal set; }
    /// <summary>Event KillerUnitEvent - either SUnitBorn- or SUnitInitEvent</summary>
    ///
    [JsonIgnore]
    public SUnitBornEvent? KillerUnitBornEvent { get; internal set; }

}

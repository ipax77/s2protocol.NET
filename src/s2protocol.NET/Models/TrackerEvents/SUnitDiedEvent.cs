using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>SUnitDiedEvent</c> SUnitDiedEvent</summary>
///
public record SUnitDiedEvent : TrackerEvent
{
    /// <summary>Record <c>SUnitDiedEvent</c> constructor</summary>
    /// <comment>You may receive a SUnitDiedEvent after either a UnitInit or UnitBorn event for the corresponding unit tag.</comment>
    /// <comment>There's a known issue where revived units are not tracked, and placeholder units track death but not birth.</comment>
    /// 
    public SUnitDiedEvent(TrackerEvent trackerEvent,
                          int unitTagIndex,
                          int unitTagRecycle,
                          int? killerPlayerId,
                          int x,
                          int y,
                          int? killerUnitTagRecycle,
                          int? killerUnitTagIndex) : base(trackerEvent)
    {
        UnitTagIndex = unitTagIndex;
        UnitTagRecycle = unitTagRecycle;
        KillerPlayerId = killerPlayerId;
        X = x;
        Y = y;
        KillerUnitTagRecycle = killerUnitTagRecycle;
        KillerUnitTagIndex = killerUnitTagIndex;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SUnitDiedEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }
    /// <summary>Event UnitIndex</summary>
    /// <comment>Convert unit tag index, recycle pairs into unit tags (as seen in game events) with protocol.unit_tag(index, recycle)</comment>
    /// 
    public int UnitIndex { get; internal set; }
    /// <summary>Event UnitTagIndex</summary>
    ///
    public int UnitTagIndex { get; init; }
    /// <summary>Event UnitTagRecycle</summary>
    ///
    public int UnitTagRecycle { get; init; }
    /// <summary>Event ControlPlayerId</summary>
    ///
    public int? KillerPlayerId { get; init; }
    /// <summary>Event Y</summary>
    ///
    public int Y { get; init; }
    /// <summary>Event X</summary>
    ///
    public int X { get; init; }
    /// <summary>Event KillerUnitTagRecycle</summary>
    ///
    public int? KillerUnitTagRecycle { get; init; }
    /// <summary>Event KillerUnitTagIndex</summary>
    ///
    public int? KillerUnitTagIndex { get; init; }
    /// <summary>Event KillerUnitEvent - either SUnitBorn- or SUnitInitEvent</summary>
    ///
    public SUnitInitEvent? KillerUnitInitEvent { get; internal set; }
    /// <summary>Event KillerUnitEvent - either SUnitBorn- or SUnitInitEvent</summary>
    ///
    public SUnitBornEvent? KillerUnitBornEvent { get; internal set; }

}

using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>SUnitOwnerChangeEvent</c> SUnitOwnerChangeEvent</summary>
///
public record SUnitOwnerChangeEvent : TrackerEvent
{
    /// <summary>Record <c>SUnitOwnerChangeEvent</c> constructor</summary>
    ///
    public SUnitOwnerChangeEvent(
        TrackerEvent trackerEvent,
        int unitTagIndex,
        int unitTagRecycle,
        int controlPlayerId,
        int upkeepPlayerId) : base(trackerEvent)
    {
        UnitTagIndex = unitTagIndex;
        UnitTagRecycle = unitTagRecycle;
        ControlPlayerId = controlPlayerId;
        UpkeepPlayerId = upkeepPlayerId;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SUnitOwnerChangeEvent()
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
    public int ControlPlayerId { get; init; }
    /// <summary>Event UpkeepPlayerId</summary>
    ///
    public int UpkeepPlayerId { get; init; }
}

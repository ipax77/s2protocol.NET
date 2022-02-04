using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>SUnitDoneEvent</c> SUnitDoneEvent</summary>
///
public record SUnitDoneEvent : TrackerEvent
{
    /// <summary>Record <c>SUnitDoneEvent</c> constructor</summary>
    ///
    public SUnitDoneEvent(TrackerEvent trackerEvent,
                         int unitTagIndex,
                         int unitTagRecycle) : base(trackerEvent)
    {
        UnitTagIndex = unitTagIndex; ;
        UnitTagRecycle = unitTagRecycle;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SUnitDoneEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Event UnitTagIndex</summary>
    ///
    public int UnitTagIndex { get; init; }
    /// <summary>Event UnitTagRecycle</summary>
    ///
    public int UnitTagRecycle { get; init; }
}

namespace s2protocol.NET.Models;
/// <summary>Record <c>SUnitTypeChangeEvent</c> SUnitTypeChangeEvent</summary>
///
public record SUnitTypeChangeEvent : TrackerEvent
{
    /// <summary>Record <c>SUnitTypeChangeEvent</c> constructor</summary>
    ///
    public SUnitTypeChangeEvent(
        TrackerEvent trackerEvent,
        int unitTagIndex,
        int unitTagRecycle,
        string unitTypeName) : base(trackerEvent)
    {
        UnitTagIndex = unitTagIndex;
        UnitTagRecycle = unitTagRecycle;
        UnitTypeName = unitTypeName;
    }

    /// <summary>Event UnitTagIndex</summary>
    ///
    public int UnitTagIndex { get; init; }
    /// <summary>Event UnitTagRecycle</summary>
    ///
    public int UnitTagRecycle { get; init; }
    /// <summary>Event UnitTypeName</summary>
    ///
    public string UnitTypeName { get; init; }
}

namespace s2protocol.NET.Models;
/// <summary>Record <c>SUnitInitEvent</c> SUnitInitEvent</summary>
///
public record SUnitInitEvent : TrackerEvent
{
    /// <summary>Record <c>SUnitInitEvent</c> constructor</summary>
    ///
    public SUnitInitEvent(TrackerEvent trackerEvent,
                          int unitTagIndex,
                          int unitTagRecycle,
                          int controlPlayerId,
                          int x,
                          int y,
                          int upkeepPlayerId,
                          string unitTypeName) : base(trackerEvent)
    {
        UnitTagIndex = unitTagIndex;
        UnitTagRecycle = unitTagRecycle;
        ControlPlayerId = controlPlayerId;
        X = x;
        Y = y;
        UpkeepPlayerId = upkeepPlayerId;
        UnitTypeName = unitTypeName;
    }

    /// <summary>Event UnitTagIndex</summary>
    ///
    public int UnitTagIndex { get; init; }
    /// <summary>Event UnitTagRecycle</summary>
    ///
    public int UnitTagRecycle { get; init; }
    /// <summary>Event ControlPlayerId</summary>
    ///
    public int ControlPlayerId { get; init; }
    /// <summary>Event Y</summary>
    ///
    public int Y { get; init; }
    /// <summary>Event X</summary>
    ///
    public int X { get; init; }
    /// <summary>Event UpkeepPlayerId</summary>
    ///
    public int UpkeepPlayerId { get; init; }
    /// <summary>Event UnitTypeName</summary>
    ///
    public string UnitTypeName { get; init; }
}

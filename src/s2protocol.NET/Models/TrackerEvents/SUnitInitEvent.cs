namespace s2protocol.NET.Models;
/// <summary>Record <c>SUnitInitEvent</c> SUnitInitEvent</summary>
///
/// <remarks>Record <c>SUnitInitEvent</c> constructor</remarks>
/// <comment>NNet.Replay.Tracker.SUnitInitEvent events appear for units under construction. When complete you'll see a NNet.Replay.Tracker.SUnitDoneEvent with the same unit tag.</comment>
public sealed class SUnitInitEvent(int playerId,
                                   int eventId,
                                   int bits,
                                   int gameloop,
                                   int unitTagIndex,
                                   int unitTagRecycle,
                                   int controlPlayerId,
                                   int x,
                                   int y,
                                   int upkeepPlayerId,
                                   string unitTypeName) : TrackerEvent(playerId, eventId, TrackerEventType.SUnitInitEvent, bits, gameloop)
{
    /// <summary>Event UnitIndex</summary>
    /// <comment>Convert unit tag index, recycle pairs into unit tags (as seen in game events) with protocol.unit_tag(index, recycle)</comment>
    /// 
    public int UnitIndex { get; internal set; }
    /// <summary>Event UnitTagIndex</summary>
    ///
    public int UnitTagIndex { get; init; } = unitTagIndex;
    /// <summary>Event UnitTagRecycle</summary>
    ///
    public int UnitTagRecycle { get; init; } = unitTagRecycle;
    /// <summary>Event ControlPlayerId</summary>
    ///
    public int ControlPlayerId { get; init; } = controlPlayerId;
    /// <summary>Event Y</summary>
    ///
    public int Y { get; init; } = y;
    /// <summary>Event X</summary>
    ///
    public int X { get; init; } = x;
    /// <summary>Event UpkeepPlayerId</summary>
    ///
    public int UpkeepPlayerId { get; init; } = upkeepPlayerId;
    /// <summary>Event UnitTypeName</summary>
    ///
    public string UnitTypeName { get; init; } = unitTypeName;
    /// <summary>Event SUnitDoneEvent</summary>
    ///
    public SUnitDoneEvent? SUnitDoneEvent { get; internal set; }
    /// <summary>Event SUnitDiedEvent</summary>
    ///
    public SUnitDiedEvent? SUnitDiedEvent { get; internal set; }
}

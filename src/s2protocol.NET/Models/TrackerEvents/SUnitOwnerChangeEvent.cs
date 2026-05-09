namespace s2protocol.NET.Models;
/// <summary>Record <c>SUnitOwnerChangeEvent</c> SUnitOwnerChangeEvent</summary>
///
/// <remarks>Record <c>SUnitOwnerChangeEvent</c> constructor</remarks>
///
public sealed class SUnitOwnerChangeEvent(int playerId,
                                          int eventId,
                                          int bits,
                                          int gameloop,
                                          int unitTagIndex,
                                          int unitTagRecycle,
                                          int controlPlayerId,
                                          int upkeepPlayerId) : TrackerEvent(playerId, eventId, TrackerEventType.SUnitOwnerChangeEvent, bits, gameloop)
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
    public int ControlPlayerId { get; } = controlPlayerId;
    /// <summary>Event UpkeepPlayerId</summary>
    ///
    public int UpkeepPlayerId { get; } = upkeepPlayerId;
}

namespace s2protocol.NET.Models;
/// <summary>Record <c>SUnitTypeChangeEvent</c> SUnitTypeChangeEvent</summary>
///
/// <remarks>Record <c>SUnitTypeChangeEvent</c> constructor</remarks>
///
public sealed class SUnitTypeChangeEvent(int playerId,
                                         int eventId,
                                         int bits,
                                         int gameloop,
                                         int unitTagIndex,
                                         int unitTagRecycle,
                                         string unitTypeName) : TrackerEvent(playerId, eventId, TrackerEventType.SUnitTypeChangeEvent, bits, gameloop)
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
    /// <summary>Event UnitTypeName</summary>
    ///
    public string UnitTypeName { get; } = unitTypeName;
}

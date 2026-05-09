namespace s2protocol.NET.Models;
/// <summary>Record <c>SUnitDoneEvent</c> SUnitDoneEvent</summary>
/// <comment>NNet.Replay.Tracker.SUnitInitEvent events appear for units under construction. When complete you'll see a NNet.Replay.Tracker.SUnitDoneEvent with the same unit tag.</comment>
///
/// <remarks>Record <c>SUnitDoneEvent</c> constructor</remarks>
///
public sealed class SUnitDoneEvent(int playerId,
                                   int eventId,
                                   int bits,
                                   int gameloop,
                                   int unitTagIndex,
                                   int unitTagRecycle) : TrackerEvent(playerId, eventId, TrackerEventType.SUnitDoneEvent, bits, gameloop)
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
}

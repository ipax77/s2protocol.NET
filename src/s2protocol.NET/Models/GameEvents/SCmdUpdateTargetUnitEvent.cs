namespace s2protocol.NET.Models;
/// <summary>Record <c>SCmdUpdateTargetUnitEvent</c> SCmdUpdateTargetUnitEvent</summary>
///
public sealed class SCmdUpdateTargetUnitEvent : GameEvent
{
    /// <summary>Record <c>SCmdUpdateTargetUnitEvent</c> constructor</summary>
    ///
    public SCmdUpdateTargetUnitEvent(int userId,
        int eventId,
        int bits,
        int gameloop,
        int snapshotControlPlayerId,
                                     long snapshotPointX,
                                     long snapshotPointY,
                                     long snapshotPointZ,
                                     int snapshotUpkeepPlayerId,
                                     int timer,
                                     int targetUnitFlags,
                                     int snapshotUnitLink,
                                     int tag) : base(userId, eventId, GameEventType.SCmdUpdateTargetUnitEvents, bits, gameloop)
    {
        SnapshotControlPlayerId = snapshotControlPlayerId;
        SnapshotPointX = snapshotPointX;
        SnapshotPointY = snapshotPointY;
        SnapshotPointZ = snapshotPointZ;
        SnapshotUpkeepPlayerId = snapshotUpkeepPlayerId;
        Timer = timer;
        TargetUnitFlags = targetUnitFlags;
        SnapshotUnitLink = snapshotUnitLink;
        Tag = tag;
    }

    /// <summary>Event Type</summary>
    ///
    public int SnapshotControlPlayerId { get; }
    /// <summary>Event SnapshotPointX</summary>
    ///    
    public long SnapshotPointX { get; }
    /// <summary>Event SnapshotPointY</summary>
    ///    
    public long SnapshotPointY { get; }
    /// <summary>Event SnapshotPointZ</summary>
    ///    
    public long SnapshotPointZ { get; }
    /// <summary>Event SnapshotUpkeepPlayerId</summary>
    ///    
    public int SnapshotUpkeepPlayerId { get; }
    /// <summary>Event Timer</summary>
    ///    
    public int Timer { get; }
    /// <summary>Event TargetUnitFlags</summary>
    ///    
    public int TargetUnitFlags { get; }
    /// <summary>Event SnapshotUnitLink</summary>
    ///    
    public int SnapshotUnitLink { get; }
    /// <summary>Event Tag</summary>
    ///    
    public int Tag { get; }
}
namespace s2protocol.NET.Models;
/// <summary>Record <c>SCmdUpdateTargetUnitEvent</c> SCmdUpdateTargetUnitEvent</summary>
///
/// <remarks>Record <c>SCmdUpdateTargetUnitEvent</c> constructor</remarks>
///
public sealed class SCmdUpdateTargetUnitEvent(int userId,
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
                                 int tag) : GameEvent(userId, eventId, GameEventType.SCmdUpdateTargetUnitEvents, bits, gameloop)
{

    /// <summary>Event Type</summary>
    ///
    public int SnapshotControlPlayerId { get; } = snapshotControlPlayerId;
    /// <summary>Event SnapshotPointX</summary>
    ///    
    public long SnapshotPointX { get; } = snapshotPointX;
    /// <summary>Event SnapshotPointY</summary>
    ///    
    public long SnapshotPointY { get; } = snapshotPointY;
    /// <summary>Event SnapshotPointZ</summary>
    ///    
    public long SnapshotPointZ { get; } = snapshotPointZ;
    /// <summary>Event SnapshotUpkeepPlayerId</summary>
    ///    
    public int SnapshotUpkeepPlayerId { get; } = snapshotUpkeepPlayerId;
    /// <summary>Event Timer</summary>
    ///    
    public int Timer { get; } = timer;
    /// <summary>Event TargetUnitFlags</summary>
    ///    
    public int TargetUnitFlags { get; } = targetUnitFlags;
    /// <summary>Event SnapshotUnitLink</summary>
    ///    
    public int SnapshotUnitLink { get; } = snapshotUnitLink;
    /// <summary>Event Tag</summary>
    ///    
    public int Tag { get; } = tag;
}
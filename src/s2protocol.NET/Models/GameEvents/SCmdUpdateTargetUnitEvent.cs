using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>SCmdUpdateTargetUnitEvent</c> SCmdUpdateTargetUnitEvent</summary>
///
public record SCmdUpdateTargetUnitEvent : GameEvent
{
    /// <summary>Record <c>SCmdUpdateTargetUnitEvent</c> constructor</summary>
    ///
    public SCmdUpdateTargetUnitEvent(GameEvent gameEvent,
                                     int snapshotControlPlayerId,
                                     long snapshotPointX,
                                     long snapshotPointY,
                                     long snapshotPointZ,
                                     int snapshotUpkeepPlayerId,
                                     int timer,
                                     int targetUnitFlags,
                                     int snapshotUnitLink,
                                     int tag) : base(gameEvent)
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

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SCmdUpdateTargetUnitEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Event Type</summary>
    ///
    public int SnapshotControlPlayerId { get; init; }
    /// <summary>Event SnapshotPointX</summary>
    ///    
    public long SnapshotPointX { get; init; }
    /// <summary>Event SnapshotPointY</summary>
    ///    
    public long SnapshotPointY { get; init; }
    /// <summary>Event SnapshotPointZ</summary>
    ///    
    public long SnapshotPointZ { get; init; }
    /// <summary>Event SnapshotUpkeepPlayerId</summary>
    ///    
    public int SnapshotUpkeepPlayerId { get; init; }
    /// <summary>Event Timer</summary>
    ///    
    public int Timer { get; init; }
    /// <summary>Event TargetUnitFlags</summary>
    ///    
    public int TargetUnitFlags { get; init; }
    /// <summary>Event SnapshotUnitLink</summary>
    ///    
    public int SnapshotUnitLink { get; init; }
    /// <summary>Event Tag</summary>
    ///    
    public int Tag { get; init; }
}
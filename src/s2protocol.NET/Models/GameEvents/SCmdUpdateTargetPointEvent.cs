namespace s2protocol.NET.Models;
/// <summary>Record <c>SCmdUpdateTargetPointEvent</c> SCmdUpdateTargetPointEvent</summary>
///
public sealed class SCmdUpdateTargetPointEvent : GameEvent
{
    /// <summary>Record <c>SCmdUpdateTargetPointEvent</c> constructor</summary>
    ///
    public SCmdUpdateTargetPointEvent(int userId,
        int eventId,
        int bits,
        int gameloop,
        long targetX,
        long targetY,
        long targetZ) : base(userId, eventId, GameEventType.SCmdUpdateTargetPointEvent, bits, gameloop)
    {
        TargetX = targetX;
        TargetY = targetY;
        TargetZ = targetZ;
    }

    /// <summary>Event TargetX</summary>
    ///
    public long TargetX { get; }
    /// <summary>Event TargetY</summary>
    ///
    public long TargetY { get; }
    /// <summary>Event TargetZ</summary>
    ///
    public long TargetZ { get; }
}

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
        long x,
        long y,
        long z) : base(userId, eventId, GameEventType.SCmdUpdateTargetPointEvent, bits, gameloop)
    {
        TargetX = x;
        TargetY = y;
        TargetZ = z;
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
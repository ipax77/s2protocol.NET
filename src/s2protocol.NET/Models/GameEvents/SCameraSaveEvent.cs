namespace s2protocol.NET.Models;
/// <summary>Record <c>SCameraSaveEvent</c> SCameraSaveEvent</summary>
///
public sealed class SCameraSaveEvent : GameEvent
{
    /// <summary>Record <c>SCameraSaveEvent</c> constructor</summary>
    ///
    public SCameraSaveEvent(int userId,
        int eventId,
        int bits,
        int gameloop,
        int which,
                            long targetX,
                            long targetY) : base(userId, eventId, GameEventType.SCameraSaveEvent, bits, gameloop)
    {
        Which = which;
        TargetX = targetX;
        TargetY = targetY;
    }

    /// <summary>Event Which</summary>
    ///
    public int Which { get; }
    /// <summary>Event TargetX</summary>
    ///
    public long TargetX { get; }
    /// <summary>Event TargetY</summary>
    ///
    public long TargetY { get; }
}
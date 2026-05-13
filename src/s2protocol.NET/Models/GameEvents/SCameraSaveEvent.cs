namespace s2protocol.NET.Models;
/// <summary>Record <c>SCameraSaveEvent</c> SCameraSaveEvent</summary>
///
/// <remarks>Record <c>SCameraSaveEvent</c> constructor</remarks>
///
public sealed class SCameraSaveEvent(int userId,
    int eventId,
    int bits,
    int gameloop,
    int which,
                        long targetX,
                        long targetY) : GameEvent(userId, eventId, GameEventType.SCameraSaveEvent, bits, gameloop)
{

    /// <summary>Event Which</summary>
    ///
    public int Which { get; } = which;
    /// <summary>Event TargetX</summary>
    ///
    public long TargetX { get; } = targetX;
    /// <summary>Event TargetY</summary>
    ///
    public long TargetY { get; } = targetY;
}
namespace s2protocol.NET.Models;
/// <summary>Record <c>SCmdUpdateTargetPointEvent</c> SCmdUpdateTargetPointEvent</summary>
///
/// <remarks>Record <c>SCmdUpdateTargetPointEvent</c> constructor</remarks>
///
public sealed class SCmdUpdateTargetPointEvent(int userId,
    int eventId,
    int bits,
    int gameloop,
    long x,
    long y,
    long z) : GameEvent(userId, eventId, GameEventType.SCmdUpdateTargetPointEvent, bits, gameloop)
{

    /// <summary>Event TargetX</summary>
    ///
    public long TargetX { get; } = x;
    /// <summary>Event TargetY</summary>
    ///
    public long TargetY { get; } = y;
    /// <summary>Event TargetZ</summary>
    ///
    public long TargetZ { get; } = z;
}
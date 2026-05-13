namespace s2protocol.NET.Models;
/// <summary>Record <c>SGameUserLeaveEvent</c> SGameUserLeaveEvent</summary>
///
/// <remarks>Record <c>SGameUserLeaveEvent</c> constructor</remarks>
///
public sealed class SGameUserLeaveEvent(int userId,
    int eventId,
    int bits,
    int gameloop,
    int leaveReason) : GameEvent(userId, eventId, GameEventType.SGameUserLeaveEvent, bits, gameloop)
{

    /// <summary>Event LeaveReason</summary>
    ///
    public int LeaveReason { get; } = leaveReason;
}
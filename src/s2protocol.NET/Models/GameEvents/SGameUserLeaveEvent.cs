namespace s2protocol.NET.Models;
/// <summary>Record <c>SGameUserLeaveEvent</c> SGameUserLeaveEvent</summary>
///
public sealed class SGameUserLeaveEvent : GameEvent
{
    /// <summary>Record <c>SGameUserLeaveEvent</c> constructor</summary>
    ///
    public SGameUserLeaveEvent(int userId,
        int eventId,
        int bits,
        int gameloop,
        int leaveReason) : base(userId, eventId, GameEventType.SGameUserLeaveEvent, bits, gameloop)
    {
        LeaveReason = leaveReason;
    }

    /// <summary>Event LeaveReason</summary>
    ///
    public int LeaveReason { get; }
}
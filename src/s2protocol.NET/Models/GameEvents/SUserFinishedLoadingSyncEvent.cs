namespace s2protocol.NET.Models;
/// <summary>Record <c>SUserFinishedLoadingSyncEvent</c> SUserFinishedLoadingSyncEvent</summary>
///
public sealed class SUserFinishedLoadingSyncEvent : GameEvent
{
    /// <summary>Record <c>SUserFinishedLoadingSyncEvent</c> constructor</summary>
    ///
    public SUserFinishedLoadingSyncEvent(int userId,
        int eventId,
        int bits,
        int gameloop) : base(userId, eventId, GameEventType.SUserFinishedLoadingSyncEvent, bits, gameloop)
    {
    }
}
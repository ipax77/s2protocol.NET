namespace s2protocol.NET.Models;
/// <summary>Record <c>SUserFinishedLoadingSyncEvent</c> SUserFinishedLoadingSyncEvent</summary>
///
/// <remarks>Record <c>SUserFinishedLoadingSyncEvent</c> constructor</remarks>
///
public sealed class SUserFinishedLoadingSyncEvent(int userId,
    int eventId,
    int bits,
    int gameloop) : GameEvent(userId, eventId, GameEventType.SUserFinishedLoadingSyncEvent, bits, gameloop)
{
}
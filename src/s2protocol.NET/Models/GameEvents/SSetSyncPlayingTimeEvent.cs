namespace s2protocol.NET.Models;
/// <summary>Record <c>SSetSyncPlayingTimeEvent</c> SSetSyncPlayingTimeEvent</summary>
///
/// <remarks>Record <c>SSetSyncPlayingTimeEvent</c> constructor</remarks>
///
public sealed class SSetSyncPlayingTimeEvent(int userId,
    int eventId,
    int bits,
    int gameloop,
    int syncTime) : GameEvent(userId, eventId, GameEventType.SSetSyncPlayingTimeEvent, bits, gameloop)
{

    /// <summary>Event SyncTime</summary>
    ///
    public int SyncTime { get; } = syncTime;
}
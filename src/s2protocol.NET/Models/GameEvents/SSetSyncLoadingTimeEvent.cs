namespace s2protocol.NET.Models;
/// <summary>Record <c>SSetSyncLoadingTimeEvent</c> SSetSyncLoadingTimeEvent</summary>
///
/// <remarks>Record <c>SSetSyncLoadingTimeEvent</c> constructor</remarks>
///
public sealed class SSetSyncLoadingTimeEvent(int userId,
    int eventId,
    int bits,
    int gameloop,
    int syncTime) : GameEvent(userId, eventId, GameEventType.SSetSyncLoadingTimeEvent, bits, gameloop)
{

    /// <summary>Event Type</summary>
    ///
    public int SyncTime { get; } = syncTime;
}
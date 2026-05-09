namespace s2protocol.NET.Models;
/// <summary>Record <c>SSetSyncLoadingTimeEvent</c> SSetSyncLoadingTimeEvent</summary>
///
public sealed class SSetSyncLoadingTimeEvent : GameEvent
{
    /// <summary>Record <c>SSetSyncLoadingTimeEvent</c> constructor</summary>
    ///
    public SSetSyncLoadingTimeEvent(int userId,
        int eventId,
        int bits,
        int gameloop,
        int syncTime) : base(userId, eventId, GameEventType.SSetSyncLoadingTimeEvent, bits, gameloop)
    {
        SyncTime = syncTime;
    }

    /// <summary>Event Type</summary>
    ///
    public int SyncTime { get; }
}
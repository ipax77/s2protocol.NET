namespace s2protocol.NET.Models;
/// <summary>Record <c>SSetSyncPlayingTimeEvent</c> SSetSyncPlayingTimeEvent</summary>
///
public sealed class SSetSyncPlayingTimeEvent : GameEvent
{
    /// <summary>Record <c>SSetSyncPlayingTimeEvent</c> constructor</summary>
    ///
    public SSetSyncPlayingTimeEvent(int userId,
        int eventId,
        int bits,
        int gameloop,
        int syncTime) : base(userId, eventId, GameEventType.SSetSyncPlayingTimeEvent, bits, gameloop)
    {
        SyncTime = syncTime;
    }

    /// <summary>Event SyncTime</summary>
    ///
    public int SyncTime { get; }
}
using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>SSetSyncPlayingTimeEvent</c> SSetSyncPlayingTimeEvent</summary>
///
public record SSetSyncPlayingTimeEvent : GameEvent
{
    /// <summary>Record <c>SSetSyncPlayingTimeEvent</c> constructor</summary>
    ///
    public SSetSyncPlayingTimeEvent(
        GameEvent gameEvent,
        int syncTime) : base(gameEvent)
    {
        SyncTime = syncTime;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SSetSyncPlayingTimeEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Event SyncTime</summary>
    ///
    public int SyncTime { get; init; }
}
using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>SSetSyncLoadingTimeEvent</c> SSetSyncLoadingTimeEvent</summary>
///
public record SSetSyncLoadingTimeEvent : GameEvent
{
    /// <summary>Record <c>SSetSyncLoadingTimeEvent</c> constructor</summary>
    ///
    public SSetSyncLoadingTimeEvent(
        GameEvent gameEvent,
        int syncTime) : base(gameEvent)
    {
        SyncTime = syncTime;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SSetSyncLoadingTimeEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Event Type</summary>
    ///
    public int SyncTime { get; init; }
}
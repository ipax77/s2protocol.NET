using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>SUserFinishedLoadingSyncEvent</c> SUserFinishedLoadingSyncEvent</summary>
///
public record SUserFinishedLoadingSyncEvent : GameEvent
{
    /// <summary>Record <c>SUserFinishedLoadingSyncEvent</c> constructor</summary>
    ///
    public SUserFinishedLoadingSyncEvent(GameEvent gameEvent) : base(gameEvent)
    {
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SUserFinishedLoadingSyncEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }
}
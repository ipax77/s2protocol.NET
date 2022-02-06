using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>SDecrementGameTimeRemainingEvent</c> SDecrementGameTimeRemainingEvent</summary>
///
public record SDecrementGameTimeRemainingEvent : GameEvent
{
    /// <summary>Record <c>SDecrementGameTimeRemainingEvent</c> constructor</summary>
    ///
    public SDecrementGameTimeRemainingEvent(
        GameEvent gameEvent,
        int decrementSeconds) : base(gameEvent)
    {
        DecrementSeconds = decrementSeconds;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SDecrementGameTimeRemainingEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Event DecrementSeconds</summary>
    ///
    public int DecrementSeconds { get; init; }
}
using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerTransmissionOffsetEvent</c> STriggerTransmissionOffsetEvent</summary>
///
public record STriggerTransmissionOffsetEvent : GameEvent
{
    /// <summary>Record <c>STriggerTransmissionOffsetEvent</c> constructor</summary>
    ///
    public STriggerTransmissionOffsetEvent(
        GameEvent gameEvent,
        int achievementLink) : base(gameEvent)
    {
        AchievementLink = achievementLink;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public STriggerTransmissionOffsetEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
    }
    /// <summary>Event Flags</summary>
    ///
    public int AchievementLink { get; init; }
}
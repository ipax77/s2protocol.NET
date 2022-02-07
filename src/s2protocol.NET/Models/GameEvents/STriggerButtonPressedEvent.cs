using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerButtonPressedEvent</c> STriggerButtonPressedEvent</summary>
///
public record STriggerButtonPressedEvent : GameEvent
{
    /// <summary>Record <c>STriggerButtonPressedEvent</c> constructor</summary>
    ///
    public STriggerButtonPressedEvent(
        GameEvent gameEvent,
        int button) : base(gameEvent)
    {
        Button = button;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public STriggerButtonPressedEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
    }
    /// <summary>Event Button</summary>
    ///
    public int Button { get; init; }
}
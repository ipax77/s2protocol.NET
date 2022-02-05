using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerKeyPressedEvent</c> STriggerKeyPressedEvent</summary>
///
public record STriggerKeyPressedEvent : GameEvent
{
    /// <summary>Record <c>STriggerKeyPressedEvent</c> constructor</summary>
    ///
    public STriggerKeyPressedEvent(
        GameEvent gameEvent) : base(gameEvent)
    {
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public STriggerKeyPressedEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }
}
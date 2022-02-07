using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerGameMenuItemSelectedEvent</c> STriggerGameMenuItemSelectedEvent</summary>
///
public record STriggerGameMenuItemSelectedEvent : GameEvent
{
    /// <summary>Record <c>STriggerGameMenuItemSelectedEvent</c> constructor</summary>
    ///
    public STriggerGameMenuItemSelectedEvent(
        GameEvent gameEvent,
        long gameMenuItemIndex) : base(gameEvent)
    {
        GameMenuItemIndex = gameMenuItemIndex;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public STriggerGameMenuItemSelectedEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
    }
    /// <summary>Event GameMenuItemIndex</summary>
    ///
    public long GameMenuItemIndex { get; init; }
}
using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerDialogControlEvent</c> STriggerDialogControlEvent</summary>
///
public record STriggerDialogControlEvent : GameEvent
{
    /// <summary>Record <c>STriggerDialogControlEvent</c> constructor</summary>
    ///
    public STriggerDialogControlEvent(GameEvent gameEvent,
                                      long controlId,
                                      int? mouseButton,
                                      string? textChanged,
                                      long eventTypeId) : base(gameEvent)
    {
        ControlId = controlId;
        MouseButton = mouseButton;
        TextChanged = textChanged;
        EventTypeId = eventTypeId;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public STriggerDialogControlEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }



    /// <summary>Event Type</summary>
    ///
    public long ControlId { get; init; }
    /// <summary>Event MouseButton</summary>
    ///
    public int? MouseButton { get; init; }
    /// <summary>Event TextChanged</summary>
    ///
    public string? TextChanged { get; init; }
    /// <summary>Event EventType</summary>
    ///
    public long EventTypeId { get; init; }

}
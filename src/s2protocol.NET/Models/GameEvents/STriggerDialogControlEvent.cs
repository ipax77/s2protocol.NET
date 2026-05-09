namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerDialogControlEvent</c> STriggerDialogControlEvent</summary>
///
public sealed class STriggerDialogControlEvent : GameEvent
{
    /// <summary>Record <c>STriggerDialogControlEvent</c> constructor</summary>
    ///
    public STriggerDialogControlEvent(int userId,
        int eventId,
        int bits,
        int gameloop,
        long controlId,
                                      int? mouseButton,
                                      string? textChanged,
                                      long eventTypeId) : base(userId, eventId, GameEventType.STriggerDialogControlEvent, bits, gameloop)
    {
        ControlId = controlId;
        MouseButton = mouseButton;
        TextChanged = textChanged;
        EventTypeId = eventTypeId;
    }



    /// <summary>Event Type</summary>
    ///
    public long ControlId { get; }
    /// <summary>Event MouseButton</summary>
    ///
    public int? MouseButton { get; }
    /// <summary>Event TextChanged</summary>
    ///
    public string? TextChanged { get; }
    /// <summary>Event EventType</summary>
    ///
    public long EventTypeId { get; }

}
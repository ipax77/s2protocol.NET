namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerDialogControlEvent</c> STriggerDialogControlEvent</summary>
///
/// <remarks>Record <c>STriggerDialogControlEvent</c> constructor</remarks>
///
public sealed class STriggerDialogControlEvent(int userId,
    int eventId,
    int bits,
    int gameloop,
    long controlId,
                                  int? mouseButton,
                                  string? textChanged,
                                  long eventTypeId) : GameEvent(userId, eventId, GameEventType.STriggerDialogControlEvent, bits, gameloop)
{



    /// <summary>Event Type</summary>
    ///
    public long ControlId { get; } = controlId;
    /// <summary>Event MouseButton</summary>
    ///
    public int? MouseButton { get; } = mouseButton;
    /// <summary>Event TextChanged</summary>
    ///
    public string? TextChanged { get; } = textChanged;
    /// <summary>Event EventType</summary>
    ///
    public long EventTypeId { get; } = eventTypeId;

}
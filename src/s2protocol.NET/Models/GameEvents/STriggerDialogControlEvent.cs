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
                                      long eventTypeId)
        : this(userId, eventId, bits, gameloop, controlId, mouseButton, textChanged, eventTypeId,
            textChanged is not null
                ? DialogControlEventDataKind.TextChanged
                : mouseButton.HasValue
                    ? DialogControlEventDataKind.MouseButton
                    : DialogControlEventDataKind.None,
            null, null, null)
    {
    }

    /// <summary>Creates a dialog-control event with every protocol choice exposed.</summary>
    public STriggerDialogControlEvent(
        int userId,
        int eventId,
        int bits,
        int gameloop,
        long controlId,
        int? mouseButton,
        string? textChanged,
        long eventTypeId,
        DialogControlEventDataKind eventDataKind,
        bool? @checked,
        long? valueChanged,
        long? selectionChanged) : base(userId, eventId, GameEventType.STriggerDialogControlEvent, bits, gameloop)
    {
        ControlId = controlId;
        MouseButton = mouseButton;
        TextChanged = textChanged;
        EventTypeId = eventTypeId;
        EventDataKind = eventDataKind;
        Checked = @checked;
        ValueChanged = valueChanged;
        SelectionChanged = selectionChanged;
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

    /// <summary>The active event-data choice.</summary>
    public DialogControlEventDataKind EventDataKind { get; }

    /// <summary>The checked value when <see cref="EventDataKind"/> is <see cref="DialogControlEventDataKind.Checked"/>.</summary>
    public bool? Checked { get; }

    /// <summary>The changed numeric value when present.</summary>
    public long? ValueChanged { get; }

    /// <summary>The changed selection index when present.</summary>
    public long? SelectionChanged { get; }

}

namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerButtonPressedEvent</c> STriggerButtonPressedEvent</summary>
///
public sealed class STriggerButtonPressedEvent : GameEvent
{
    /// <summary>Record <c>STriggerButtonPressedEvent</c> constructor</summary>
    ///
    public STriggerButtonPressedEvent(int userId,
        int eventId,
        int bits,
        int gameloop,
        int button) : base(userId, eventId, GameEventType.STriggerButtonPressedEvent, bits, gameloop)
    {
        Button = button;
    }
    /// <summary>Event Button</summary>
    ///
    public int Button { get; }
}
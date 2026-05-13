namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerButtonPressedEvent</c> STriggerButtonPressedEvent</summary>
///
/// <remarks>Record <c>STriggerButtonPressedEvent</c> constructor</remarks>
///
public sealed class STriggerButtonPressedEvent(int userId,
    int eventId,
    int bits,
    int gameloop,
    int button) : GameEvent(userId, eventId, GameEventType.STriggerButtonPressedEvent, bits, gameloop)
{
    /// <summary>Event Button</summary>
    ///
    public int Button { get; } = button;
}
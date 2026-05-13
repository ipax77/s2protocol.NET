namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerKeyPressedEvent</c> STriggerKeyPressedEvent</summary>
///
/// <remarks>Record <c>STriggerKeyPressedEvent</c> constructor</remarks>
///
public sealed class STriggerKeyPressedEvent(int userId,
    int eventId,
    int bits,
    int gameloop,
    int flags,
    int key) : GameEvent(userId, eventId, GameEventType.STriggerKeyPressedEvent, bits, gameloop)
{
    /// <summary>Event Flags</summary>
    ///
    public int Flags { get; } = flags;
    /// <summary>Event Key</summary>
    ///
    public int Key { get; } = key;
}
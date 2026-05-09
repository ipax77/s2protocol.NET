namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerKeyPressedEvent</c> STriggerKeyPressedEvent</summary>
///
public sealed class STriggerKeyPressedEvent : GameEvent
{
    /// <summary>Record <c>STriggerKeyPressedEvent</c> constructor</summary>
    ///
    public STriggerKeyPressedEvent(int userId,
        int eventId,
        int bits,
        int gameloop,
        int flags,
        int key) : base(userId, eventId, GameEventType.STriggerKeyPressedEvent, bits, gameloop)
    {
        Flags = flags;
        Key = key;
    }
    /// <summary>Event Flags</summary>
    ///
    public int Flags { get; }
    /// <summary>Event Key</summary>
    ///
    public int Key { get; }
}
namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerMouseMovedEvent</c> STriggerMouseMovedEvent</summary>
///
public sealed class STriggerMouseMovedEvent : GameEvent
{
    /// <summary>Record <c>STriggerMouseMovedEvent</c> constructor</summary>
    ///
    public STriggerMouseMovedEvent(int userId,
        int eventId,
        int bits,
        int gameloop,
        int flags,
        long posX,
        long posY) : base(userId, eventId, GameEventType.STriggerMouseMovedEvent, bits, gameloop)
    {
        Flags = flags;
        PosUIX = posX;
        PosUIY = posY;
    }

    /// <summary>Event Flags</summary>
    ///
    public int Flags { get; }
    /// <summary>Event PosUIX</summary>
    ///
    public long PosUIX { get; }
    /// <summary>Event PosUIY</summary>
    ///
    public long PosUIY { get; }
}
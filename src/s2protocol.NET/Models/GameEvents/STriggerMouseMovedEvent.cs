namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerMouseMovedEvent</c> STriggerMouseMovedEvent</summary>
///
/// <remarks>Record <c>STriggerMouseMovedEvent</c> constructor</remarks>
///
public sealed class STriggerMouseMovedEvent(int userId,
    int eventId,
    int bits,
    int gameloop,
    int flags,
    long posX,
    long posY) : GameEvent(userId, eventId, GameEventType.STriggerMouseMovedEvent, bits, gameloop)
{

    /// <summary>Event Flags</summary>
    ///
    public int Flags { get; } = flags;
    /// <summary>Event PosUIX</summary>
    ///
    public long PosUIX { get; } = posX;
    /// <summary>Event PosUIY</summary>
    ///
    public long PosUIY { get; } = posY;
}
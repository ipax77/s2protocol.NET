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
        long posY)
        : this(userId, eventId, bits, gameloop, flags, posX, posY, null, null, null)
    {
    }

    /// <summary>Creates a mouse-move event including world coordinates when supplied by the protocol.</summary>
    public STriggerMouseMovedEvent(
        int userId,
        int eventId,
        int bits,
        int gameloop,
        int flags,
        long posX,
        long posY,
        long? posWorldX,
        long? posWorldY,
        long? posWorldZ) : base(userId, eventId, GameEventType.STriggerMouseMovedEvent, bits, gameloop)
    {
        Flags = flags;
        PosUIX = posX;
        PosUIY = posY;
        PosWorldX = posWorldX;
        PosWorldY = posWorldY;
        PosWorldZ = posWorldZ;
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

    /// <summary>World X coordinate, when present.</summary>
    public long? PosWorldX { get; }

    /// <summary>World Y coordinate, when present.</summary>
    public long? PosWorldY { get; }

    /// <summary>World Z coordinate, when present.</summary>
    public long? PosWorldZ { get; }
}

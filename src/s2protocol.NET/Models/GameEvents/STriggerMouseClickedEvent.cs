namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerMouseClickedEvent</c> STriggerMouseClickedEvent</summary>
///
public sealed class STriggerMouseClickedEvent : GameEvent
{
    /// <summary>Record <c>STriggerMouseClickedEvent</c> constructor</summary>
    ///
    public STriggerMouseClickedEvent(int userId,
        int eventId,
        int bits,
        int gameloop,
        bool down,
                                     int button,
                                     int flags,
                                     long posUIX,
                                     long posUIY)
        : this(userId, eventId, bits, gameloop, down, button, flags, posUIX, posUIY, null, null, null)
    {
    }

    /// <summary>Creates a mouse-click event including world coordinates when supplied by the protocol.</summary>
    [System.Text.Json.Serialization.JsonConstructor]
    public STriggerMouseClickedEvent(
        int userId,
        int eventId,
        int bits,
        int gameloop,
        bool down,
        int button,
        int flags,
        long posUIX,
        long posUIY,
        long? posWorldX,
        long? posWorldY,
        long? posWorldZ) : base(userId, eventId, GameEventType.STriggerMouseClickedEvent, bits, gameloop)
    {
        Down = down;
        Button = button;
        Flags = flags;
        PosUIX = posUIX;
        PosUIY = posUIY;
        PosWorldX = posWorldX;
        PosWorldY = posWorldY;
        PosWorldZ = posWorldZ;
    }

    /// <summary>Event Down</summary>
    ///
    public bool Down { get; }
    /// <summary>Event Button</summary>
    ///    
    public int Button { get; }
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

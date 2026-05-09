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
                                     long posUIY) : base(userId, eventId, GameEventType.STriggerMouseClickedEvent, bits, gameloop)
    {
        Down = down;
        Button = button;
        Flags = flags;
        PosUIX = posUIX;
        PosUIY = posUIY;
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
}
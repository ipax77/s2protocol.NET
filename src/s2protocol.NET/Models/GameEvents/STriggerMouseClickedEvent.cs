namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerMouseClickedEvent</c> STriggerMouseClickedEvent</summary>
///
/// <remarks>Record <c>STriggerMouseClickedEvent</c> constructor</remarks>
///
public sealed class STriggerMouseClickedEvent(int userId,
    int eventId,
    int bits,
    int gameloop,
    bool down,
                                 int button,
                                 int flags,
                                 long posUIX,
                                 long posUIY) : GameEvent(userId, eventId, GameEventType.STriggerMouseClickedEvent, bits, gameloop)
{

    /// <summary>Event Down</summary>
    ///
    public bool Down { get; } = down;
    /// <summary>Event Button</summary>
    ///    
    public int Button { get; } = button;
    /// <summary>Event Flags</summary>
    ///    
    public int Flags { get; } = flags;
    /// <summary>Event PosUIX</summary>
    ///    
    public long PosUIX { get; } = posUIX;
    /// <summary>Event PosUIY</summary>
    ///    
    public long PosUIY { get; } = posUIY;
}
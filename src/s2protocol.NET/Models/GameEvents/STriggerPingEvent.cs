namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerPingEvent</c> STriggerPingEvent</summary>
///
/// <remarks>Record <c>STriggerPingEvent</c> constructor</remarks>
///
public sealed class STriggerPingEvent(int userId,
    int eventId,
    int bits,
    int gameloop,
    bool pingedMinimap,
                         int unitLink,
                         bool unitIsUnderConstruction,
                         long option,
                         int unit,
                         long unitX,
                         long unitY,
                         long unitZ,
                         int? unitControlPlayerId,
                         long pointX,
                         long pointY,
                         int? unitUpkeepPlayerId) : GameEvent(userId, eventId, GameEventType.STriggerPingEvent, bits, gameloop)
{

    /// <summary>Event PingedMinimap</summary>
    ///
    public bool PingedMinimap { get; } = pingedMinimap;
    /// <summary>Event UnitLink</summary>
    ///    
    public int UnitLink { get; } = unitLink;
    /// <summary>Event UnitIsUnderConstruction</summary>
    ///    
    public bool UnitIsUnderConstruction { get; } = unitIsUnderConstruction;
    /// <summary>Event Option</summary>
    ///    
    public long Option { get; } = option;
    /// <summary>Event Unit</summary>
    ///    
    public int Unit { get; } = unit;
    /// <summary>Event UnitX</summary>
    ///    
    public long UnitX { get; } = unitX;
    /// <summary>Event UnitY</summary>
    ///    
    public long UnitY { get; } = unitY;
    /// <summary>Event UnitZ</summary>
    ///    
    public long UnitZ { get; } = unitZ;
    /// <summary>Event UnitControlPlayerId</summary>
    ///    
    public int? UnitControlPlayerId { get; } = unitControlPlayerId;
    /// <summary>Event PointX</summary>
    ///    
    public long PointX { get; } = pointX;
    /// <summary>Event PointY</summary>
    ///    
    public long PointY { get; } = pointY;
    /// <summary>Event UnitUpkeepPlayerId</summary>
    ///    
    public int? UnitUpkeepPlayerId { get; } = unitUpkeepPlayerId;

}
namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerPingEvent</c> STriggerPingEvent</summary>
///
public sealed class STriggerPingEvent : GameEvent
{
    /// <summary>Record <c>STriggerPingEvent</c> constructor</summary>
    ///
    public STriggerPingEvent(int userId,
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
                             int? unitUpkeepPlayerId) : base(userId, eventId, GameEventType.STriggerPingEvent, bits, gameloop)
    {
        PingedMinimap = pingedMinimap;
        UnitLink = unitLink;
        UnitIsUnderConstruction = unitIsUnderConstruction;
        Option = option;
        Unit = unit;
        UnitX = unitX;
        UnitY = unitY;
        UnitZ = unitZ;
        UnitControlPlayerId = unitControlPlayerId;
        PointX = pointX;
        PointY = pointY;
        UnitUpkeepPlayerId = unitUpkeepPlayerId;
    }

    /// <summary>Event PingedMinimap</summary>
    ///
    public bool PingedMinimap { get; }
    /// <summary>Event UnitLink</summary>
    ///    
    public int UnitLink { get; }
    /// <summary>Event UnitIsUnderConstruction</summary>
    ///    
    public bool UnitIsUnderConstruction { get; }
    /// <summary>Event Option</summary>
    ///    
    public long Option { get; }
    /// <summary>Event Unit</summary>
    ///    
    public int Unit { get; }
    /// <summary>Event UnitX</summary>
    ///    
    public long UnitX { get; }
    /// <summary>Event UnitY</summary>
    ///    
    public long UnitY { get; }
    /// <summary>Event UnitZ</summary>
    ///    
    public long UnitZ { get; }
    /// <summary>Event UnitControlPlayerId</summary>
    ///    
    public int? UnitControlPlayerId { get; }
    /// <summary>Event PointX</summary>
    ///    
    public long PointX { get; }
    /// <summary>Event PointY</summary>
    ///    
    public long PointY { get; }
    /// <summary>Event UnitUpkeepPlayerId</summary>
    ///    
    public int? UnitUpkeepPlayerId { get; }

}
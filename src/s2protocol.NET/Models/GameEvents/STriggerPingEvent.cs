using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerPingEvent</c> STriggerPingEvent</summary>
///
public record STriggerPingEvent : GameEvent
{
    /// <summary>Record <c>STriggerPingEvent</c> constructor</summary>
    ///
    public STriggerPingEvent(GameEvent gameEvent,
                             bool pingedMinimap,
                             int unitLink,
                             bool unitIsUnderConstruction,
                             int option,
                             int unit,
                             long unitX,
                             long unitY,
                             long unitZ,
                             int? unitControlPlayerId,
                             long pointX,
                             long pointY,
                             int? unitUpkeepPlayerId) : base(gameEvent)
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

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public STriggerPingEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Event PingedMinimap</summary>
    ///
    public bool PingedMinimap { get; init; }
    /// <summary>Event UnitLink</summary>
    ///    
    public int UnitLink { get; init; }
    /// <summary>Event UnitIsUnderConstruction</summary>
    ///    
    public bool UnitIsUnderConstruction { get; init; }
    /// <summary>Event Option</summary>
    ///    
    public int Option { get; init; }
    /// <summary>Event Unit</summary>
    ///    
    public int Unit { get; init; }
    /// <summary>Event UnitX</summary>
    ///    
    public long UnitX { get; init; }
    /// <summary>Event UnitY</summary>
    ///    
    public long UnitY { get; init; }
    /// <summary>Event UnitZ</summary>
    ///    
    public long UnitZ { get; init; }
    /// <summary>Event UnitControlPlayerId</summary>
    ///    
    public int? UnitControlPlayerId { get; init; }
    /// <summary>Event PointX</summary>
    ///    
    public long PointX { get; init; }
    /// <summary>Event PointY</summary>
    ///    
    public long PointY { get; init; }
    /// <summary>Event UnitUpkeepPlayerId</summary>
    ///    
    public int? UnitUpkeepPlayerId { get; init; }

}
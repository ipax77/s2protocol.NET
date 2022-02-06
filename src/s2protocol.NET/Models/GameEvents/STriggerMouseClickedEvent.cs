using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerMouseClickedEvent</c> STriggerMouseClickedEvent</summary>
///
public record STriggerMouseClickedEvent : GameEvent
{
    /// <summary>Record <c>STriggerMouseClickedEvent</c> constructor</summary>
    ///
    public STriggerMouseClickedEvent(GameEvent gameEvent,
                                     bool down,
                                     int button,
                                     int flags,
                                     long posUIX,
                                     long posUIY) : base(gameEvent)
    {
        Down = down;
        Button = button;
        Flags = flags;
        PosUIX = posUIX;
        PosUIY = posUIY;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public STriggerMouseClickedEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Event Down</summary>
    ///
    public bool Down { get; init; }
    /// <summary>Event Button</summary>
    ///    
    public int Button { get; init; }
    /// <summary>Event Flags</summary>
    ///    
    public int Flags { get; init; }
    /// <summary>Event PosUIX</summary>
    ///    
    public long PosUIX { get; init; }
    /// <summary>Event PosUIY</summary>
    ///    
    public long PosUIY { get; init; }
}
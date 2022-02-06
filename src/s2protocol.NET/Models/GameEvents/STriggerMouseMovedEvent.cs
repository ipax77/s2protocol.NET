using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerMouseMovedEvent</c> STriggerMouseMovedEvent</summary>
///
public record STriggerMouseMovedEvent : GameEvent
{
    /// <summary>Record <c>STriggerMouseMovedEvent</c> constructor</summary>
    ///
    public STriggerMouseMovedEvent(
        GameEvent gameEvent,
        int flags,
        long posX,
        long posY) : base(gameEvent)
    {
        Flags = flags;
        PosUIX = posX;
        PosUIY = posY;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public STriggerMouseMovedEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

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
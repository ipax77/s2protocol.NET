using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>SCameraUpdateEvent</c> SCameraUpdateEvent</summary>
///
public record SCameraUpdateEvent : GameEvent
{
    /// <summary>Record <c>SCameraUpdateEvent</c> constructor</summary>
    ///
    public SCameraUpdateEvent(GameEvent gameEvent,
                          string? reason,
                          int? distance,
                          int? targetX,
                          int? targetY,
                          int? yaw,
                          int? pitch,
                          bool follow) : base(gameEvent)
    {
        Reason = reason;
        Distance = distance;
        TargetX = targetX;
        TargetY = targetY;
        Yaw = yaw;
        Pitch = pitch;
        Follow = follow;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SCameraUpdateEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Event Reason</summary>
    ///
    public string? Reason { get; init; }
    /// <summary>Event Distance</summary>
    ///
    public int? Distance { get; init; }
    /// <summary>Event TargetX</summary>
    ///
    public int? TargetX { get; init; }
    /// <summary>Event TargetY</summary>
    ///
    public int? TargetY { get; init; }
    /// <summary>Event Yaw</summary>
    ///
    public int? Yaw { get; init; }
    /// <summary>Event Pitch</summary>
    ///
    public int? Pitch { get; init; }
    /// <summary>Event Follow</summary>
    ///
    public bool Follow { get; init; }
}
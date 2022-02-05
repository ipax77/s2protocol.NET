using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>SCmdUpdateTargetPointEvent</c> SCmdUpdateTargetPointEvent</summary>
///
public record SCmdUpdateTargetPointEvent : GameEvent
{
    /// <summary>Record <c>SCmdUpdateTargetPointEvent</c> constructor</summary>
    ///
    public SCmdUpdateTargetPointEvent(
        GameEvent gameEvent,
        int x,
        int y,
        long z) : base(gameEvent)
    {
        TargetX = x;
        TargetY = y;
        TargetZ = z;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SCmdUpdateTargetPointEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Event TargetX</summary>
    ///
    public int TargetX { get; init; }
    /// <summary>Event TargetY</summary>
    ///
    public int TargetY { get; init; }
    /// <summary>Event TargetZ</summary>
    ///
    public long TargetZ { get; init; }
}
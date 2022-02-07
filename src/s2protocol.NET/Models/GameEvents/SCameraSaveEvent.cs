using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>SCameraSaveEvent</c> SCameraSaveEvent</summary>
///
public record SCameraSaveEvent : GameEvent
{
    /// <summary>Record <c>SCameraSaveEvent</c> constructor</summary>
    ///
    public SCameraSaveEvent(GameEvent gameEvent,
                            int which,
                            long targetX,
                            long targetY) : base(gameEvent)
    {
        Which = which;
        TargetX = targetX;
        TargetY = targetY;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SCameraSaveEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Event Which</summary>
    ///
    public int Which { get; init; }
    /// <summary>Event TargetX</summary>
    ///
    public long TargetX { get; init; }
    /// <summary>Event TargetY</summary>
    ///
    public long TargetY { get; init; }
}
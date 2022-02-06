using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>SUnitClickEvent</c> SUnitClickEvent</summary>
///
public record SUnitClickEvent : GameEvent
{
    /// <summary>Record <c>SUnitClickEvent</c> constructor</summary>
    ///
    public SUnitClickEvent(
        GameEvent gameEvent,
        int unitTag) : base(gameEvent)
    {
        UnitTag = unitTag;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SUnitClickEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Event TargetX</summary>
    ///
    public int UnitTag { get; init; }
}
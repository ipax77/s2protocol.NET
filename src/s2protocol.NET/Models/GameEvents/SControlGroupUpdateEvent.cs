using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>SControlGroupUpdateEvent</c> SControlGroupUpdateEvent</summary>
///
public record SControlGroupUpdateEvent : GameEvent
{
    /// <summary>Record <c>SControlGroupUpdateEvent</c> constructor</summary>
    ///
    public SControlGroupUpdateEvent(
        GameEvent gameEvent,
        string name) : base(gameEvent)
    {
        Name = name;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SControlGroupUpdateEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Event Type</summary>
    ///
    public string Name { get; init; }
}
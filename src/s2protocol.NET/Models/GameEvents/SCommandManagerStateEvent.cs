using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>SCommandManagerStateEvent</c> SCommandManagerStateEvent</summary>
///
public record SCommandManagerStateEvent : GameEvent
{
    /// <summary>Record <c>SCommandManagerStateEvent</c> constructor</summary>
    ///
    public SCommandManagerStateEvent(GameEvent gameEvent,
                                     int state,
                                     int? sequence) : base(gameEvent)
    {
        State = state;
        Sequence = sequence;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SCommandManagerStateEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Event State</summary>
    ///
    public int State { get; init; }
    /// <summary>Event Sequence</summary>
    ///
    public int? Sequence { get; init; }
}
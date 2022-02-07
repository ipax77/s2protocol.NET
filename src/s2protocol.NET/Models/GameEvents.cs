using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;

/// <summary>Record <c>GameEvents</c> GameEvents</summary>
///
public sealed record GameEvents
{
    /// <summary>Record <c>GameEvents</c> constructor</summary>
    ///
    public GameEvents(ICollection<GameEvent> gameEvents)
    {
        BaseGameEvents = gameEvents;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public GameEvents()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>BaseGameEvents</summary>
    ///
    public ICollection<GameEvent> BaseGameEvents { get; init; }
    /// <summary>GetGameEvents of spcified Type</summary>
    ///
    public ICollection<T> GetGameEvents<T>()
    {
        return BaseGameEvents.OfType<T>().ToList();
    }
}

using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;

/// <summary>Decoded replay game events in stream order.</summary>
public sealed class GameEvents
{
    /// <summary>Creates an ordered game-event collection.</summary>
    [JsonConstructor]
    public GameEvents(ICollection<GameEvent> baseGameEvents)
    {
        ArgumentNullException.ThrowIfNull(baseGameEvents);
        BaseGameEvents = baseGameEvents;
    }

    /// <summary>All game events in encoded stream order.</summary>
    public ICollection<GameEvent> BaseGameEvents { get; init; }

    /// <summary>Gets game events of the specified type.</summary>
    public ICollection<T> GetGameEvents<T>()
    {
        return BaseGameEvents.OfType<T>().ToList();
    }
}

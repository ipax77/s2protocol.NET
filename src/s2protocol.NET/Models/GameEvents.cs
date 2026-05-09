namespace s2protocol.NET.Models;

/// <summary>Record <c>GameEvents</c> GameEvents</summary>
///
/// <remarks>Record <c>GameEvents</c> constructor</remarks>
///
public sealed class GameEvents(ICollection<GameEvent> gameEvents)
{
    /// <summary>BaseGameEvents</summary>
    ///
    public ICollection<GameEvent> BaseGameEvents { get; init; } = gameEvents;
    /// <summary>GetGameEvents of spcified Type</summary>
    ///
    public ICollection<T> GetGameEvents<T>()
    {
        return BaseGameEvents.OfType<T>().ToList();
    }
}

namespace s2protocol.NET.Models;
/// <summary>Record <c>Event</c> Event baseclass</summary>
///
public abstract class TrackerEvent(
    int playerId,
    int eventId,
    TrackerEventType eventType,
    int bits,
    int gameloop)
{
    public int PlayerId { get; } = playerId;
    public int EventId { get; } = eventId;
    public TrackerEventType EventType { get; } = eventType;
    public int Bits { get; } = bits;
    public int Gameloop { get; } = gameloop;
}

public sealed class UnknownTrackerEvent(
    int playerId,
    int eventId,
    TrackerEventType eventType,
    int bits,
    int gameloop) : TrackerEvent(playerId, eventId, eventType, bits, gameloop)
{
}

/// <summary>Enum <c>EventType</c> Event type</summary>
///
public enum TrackerEventType
{
    None = 0,
    SPlayerSetupEvent = 1,
    SPlayerStatsEvent = 2,
    SUnitBornEvent = 3,
    SUnitDiedEvent = 4,
    SUnitOwnerChangeEvent = 5,
    SUnitPositionsEvent = 6,
    SUnitTypeChangeEvent = 7,
    SUpgradeEvent = 8,
    SUnitInitEvent = 9,
    SUnitDoneEvent = 10
}

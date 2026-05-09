namespace s2protocol.NET.Models;
/// <summary>Record <c>Event</c> Event baseclass</summary>
///
public abstract class TrackerEvent
{
    protected TrackerEvent(
        int playerId,
        int eventId,
        TrackerEventType eventType,
        int bits,
        int gameloop)
    {
        PlayerId = playerId;
        EventId = eventId;
        EventType = eventType;
        Bits = bits;
        Gameloop = gameloop;
    }

    public int PlayerId { get; }
    public int EventId { get; }
    public TrackerEventType EventType { get; }
    public int Bits { get; }
    public int Gameloop { get; }
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

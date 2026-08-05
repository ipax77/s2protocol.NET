using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;

/// <summary>Base class for replay tracker events.</summary>
[JsonDerivedType(typeof(SPlayerSetupEvent), nameof(SPlayerSetupEvent))]
[JsonDerivedType(typeof(SPlayerStatsEvent), nameof(SPlayerStatsEvent))]
[JsonDerivedType(typeof(SUnitBornEvent), nameof(SUnitBornEvent))]
[JsonDerivedType(typeof(SUnitDiedEvent), nameof(SUnitDiedEvent))]
[JsonDerivedType(typeof(SUnitOwnerChangeEvent), nameof(SUnitOwnerChangeEvent))]
[JsonDerivedType(typeof(SUnitPositionsEvent), nameof(SUnitPositionsEvent))]
[JsonDerivedType(typeof(SUnitTypeChangeEvent), nameof(SUnitTypeChangeEvent))]
[JsonDerivedType(typeof(SUpgradeEvent), nameof(SUpgradeEvent))]
[JsonDerivedType(typeof(SUnitInitEvent), nameof(SUnitInitEvent))]
[JsonDerivedType(typeof(SUnitDoneEvent), nameof(SUnitDoneEvent))]
[JsonDerivedType(typeof(UnknownTrackerEvent), nameof(UnknownTrackerEvent))]
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

/// <summary>A tracker event whose schema name is not known by this library.</summary>
public sealed class UnknownTrackerEvent(
    int playerId,
    int eventId,
    TrackerEventType eventType,
    int bits,
    int gameloop,
    string eventTypeName = "") : TrackerEvent(playerId, eventId, eventType, bits, gameloop)
{
    /// <summary>Full Blizzard schema name.</summary>
    public string EventTypeName { get; } = eventTypeName;
}

/// <summary>Known replay tracker event types.</summary>
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

namespace s2protocol.NET.Models;

/// <summary>Contains the strongly typed tracker events decoded from a replay.</summary>
public sealed class TrackerEvents
{
    /// <summary>Creates a tracker-event collection from arrays of each supported event type.</summary>
    public TrackerEvents(
        SPlayerSetupEvent[] sPlayerSetupEvents,
        SPlayerStatsEvent[] sPlayerStatsEvents,
        SUnitBornEvent[] sUnitBornEvents,
        SUnitDiedEvent[] sUnitDiedEvents,
        SUnitOwnerChangeEvent[] sUnitOwnerChangeEvents,
        SUnitPositionsEvent[] sUnitPositionsEvents,
        SUnitTypeChangeEvent[] sUnitTypeChangeEvents,
        SUpgradeEvent[] sUpgradeEvents,
        SUnitInitEvent[] sUnitInitEvents,
        SUnitDoneEvent[] sUnitDoneEvents)
        : this(
            (ICollection<SPlayerSetupEvent>)sPlayerSetupEvents,
            sPlayerStatsEvents,
            sUnitBornEvents,
            sUnitDiedEvents,
            sUnitOwnerChangeEvents,
            sUnitPositionsEvents,
            sUnitTypeChangeEvents,
            sUpgradeEvents,
            sUnitInitEvents,
            sUnitDoneEvents)
    {
    }

    internal TrackerEvents(
        ICollection<SPlayerSetupEvent> sPlayerSetupEvents,
        ICollection<SPlayerStatsEvent> sPlayerStatsEvents,
        ICollection<SUnitBornEvent> sUnitBornEvents,
        ICollection<SUnitDiedEvent> sUnitDiedEvents,
        ICollection<SUnitOwnerChangeEvent> sUnitOwnerChangeEvents,
        ICollection<SUnitPositionsEvent> sUnitPositionsEvents,
        ICollection<SUnitTypeChangeEvent> sUnitTypeChangeEvents,
        ICollection<SUpgradeEvent> sUpgradeEvents,
        ICollection<SUnitInitEvent> sUnitInitEvents,
        ICollection<SUnitDoneEvent> sUnitDoneEvents)
    {
        SPlayerSetupEvents = sPlayerSetupEvents;
        SPlayerStatsEvents = sPlayerStatsEvents;
        SUnitBornEvents = sUnitBornEvents;
        SUnitDiedEvents = sUnitDiedEvents;
        SUnitOwnerChangeEvents = sUnitOwnerChangeEvents;
        SUnitPositionsEvents = sUnitPositionsEvents;
        SUnitTypeChangeEvents = sUnitTypeChangeEvents;
        SUpgradeEvents = sUpgradeEvents;
        SUnitInitEvents = sUnitInitEvents;
        SUnitDoneEvents = sUnitDoneEvents;
    }

    /// <summary>Player setup events.</summary>
    public ICollection<SPlayerSetupEvent> SPlayerSetupEvents { get; init; }

    /// <summary>Player statistics events.</summary>
    public ICollection<SPlayerStatsEvent> SPlayerStatsEvents { get; init; }

    /// <summary>Events for units created fully constructed.</summary>
    public ICollection<SUnitBornEvent> SUnitBornEvents { get; init; }

    /// <summary>Unit death events.</summary>
    public ICollection<SUnitDiedEvent> SUnitDiedEvents { get; init; }

    /// <summary>Unit owner-change events.</summary>
    public ICollection<SUnitOwnerChangeEvent> SUnitOwnerChangeEvents { get; init; }

    /// <summary>Unit position events.</summary>
    public ICollection<SUnitPositionsEvent> SUnitPositionsEvents { get; init; }

    /// <summary>Unit type-change events.</summary>
    public ICollection<SUnitTypeChangeEvent> SUnitTypeChangeEvents { get; init; }

    /// <summary>Upgrade events.</summary>
    public ICollection<SUpgradeEvent> SUpgradeEvents { get; init; }

    /// <summary>Events for units beginning construction.</summary>
    public ICollection<SUnitInitEvent> SUnitInitEvents { get; init; }

    /// <summary>Unit construction completion events.</summary>
    public ICollection<SUnitDoneEvent> SUnitDoneEvents { get; init; }
}

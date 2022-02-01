namespace s2protocol.NET.Models;
/// <summary>Record <c>TrackerEvents</c> TrackerEvents</summary>
///
public sealed record TrackerEvents
{
    /// <summary>Record <c>TrackerEvents</c> constructor</summary>
    ///
    public TrackerEvents(SPlayerSetupEvent[] sPlayerSetupEvents,
                         SPlayerStatsEvent[] sPlayerStatsEvents,
                         SUnitBornEvent[] sUnitBornEvents,
                         SUnitDiedEvent[] sUnitDiedEvents,
                         SUnitOwnerChangeEvent[] sUnitOwnerChangeEvents,
                         SUnitPositionsEvent[] sUnitPositionsEvents,
                         SUnitTypeChangeEvent[] sUnitTypeChangeEvents,
                         SUpgradeEvent[] sUpgradeEvents,
                         SUnitInitEvent[] sUnitInitEvents,
                         SUnitDoneEvent[] sUnitDoneEvents)
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

    /// <summary>Event ControlPlayerId</summary>
    ///
    public ICollection<SPlayerSetupEvent> SPlayerSetupEvents { get; init; }
    /// <summary>Event ControlPlayerId</summary>
    ///
    public ICollection<SPlayerStatsEvent> SPlayerStatsEvents { get; init; }
    /// <summary>Event ControlPlayerId</summary>
    ///
    public ICollection<SUnitBornEvent> SUnitBornEvents { get; init; }
    /// <summary>Event ControlPlayerId</summary>
    ///
    public ICollection<SUnitDiedEvent> SUnitDiedEvents { get; init; }
    /// <summary>Event ControlPlayerId</summary>
    ///
    public ICollection<SUnitOwnerChangeEvent> SUnitOwnerChangeEvents { get; init; }
    /// <summary>Event ControlPlayerId</summary>
    ///
    public ICollection<SUnitPositionsEvent> SUnitPositionsEvents { get; init; }
    /// <summary>Event ControlPlayerId</summary>
    ///
    public ICollection<SUnitTypeChangeEvent> SUnitTypeChangeEvents { get; init; }
    /// <summary>Event ControlPlayerId</summary>
    ///
    public ICollection<SUpgradeEvent> SUpgradeEvents { get; init; }
    /// <summary>Event ControlPlayerId</summary>
    ///
    public ICollection<SUnitInitEvent> SUnitInitEvents { get; init; }
    /// <summary>Event ControlPlayerId</summary>
    ///
    public ICollection<SUnitDoneEvent> SUnitDoneEvents { get; init; }
}

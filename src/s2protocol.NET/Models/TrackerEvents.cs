namespace s2protocol.NET.Models;
/// <summary>Record <c>TrackerEvents</c> TrackerEvents</summary>
///
/// <remarks>Record <c>TrackerEvents</c> constructor</remarks>
///
public sealed class TrackerEvents(SPlayerSetupEvent[] sPlayerSetupEvents,
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
    /// <summary>Event ControlPlayerId</summary>
    ///
    public ICollection<SPlayerSetupEvent> SPlayerSetupEvents { get; init; } = sPlayerSetupEvents;
    /// <summary>Event ControlPlayerId</summary>
    ///
    public ICollection<SPlayerStatsEvent> SPlayerStatsEvents { get; init; } = sPlayerStatsEvents;
    /// <summary>Event ControlPlayerId</summary>
    ///
    public ICollection<SUnitBornEvent> SUnitBornEvents { get; init; } = sUnitBornEvents;
    /// <summary>Event ControlPlayerId</summary>
    ///
    public ICollection<SUnitDiedEvent> SUnitDiedEvents { get; init; } = sUnitDiedEvents;
    /// <summary>Event ControlPlayerId</summary>
    ///
    public ICollection<SUnitOwnerChangeEvent> SUnitOwnerChangeEvents { get; init; } = sUnitOwnerChangeEvents;
    /// <summary>Event ControlPlayerId</summary>
    ///
    public ICollection<SUnitPositionsEvent> SUnitPositionsEvents { get; init; } = sUnitPositionsEvents;
    /// <summary>Event ControlPlayerId</summary>
    ///
    public ICollection<SUnitTypeChangeEvent> SUnitTypeChangeEvents { get; init; } = sUnitTypeChangeEvents;
    /// <summary>Event ControlPlayerId</summary>
    ///
    public ICollection<SUpgradeEvent> SUpgradeEvents { get; init; } = sUpgradeEvents;
    /// <summary>Event ControlPlayerId</summary>
    ///
    public ICollection<SUnitInitEvent> SUnitInitEvents { get; init; } = sUnitInitEvents;
    /// <summary>Event ControlPlayerId</summary>
    ///
    public ICollection<SUnitDoneEvent> SUnitDoneEvents { get; init; } = sUnitDoneEvents;
}

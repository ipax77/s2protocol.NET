using System.Text.Json.Serialization;

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

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public TrackerEvents()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

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

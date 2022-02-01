using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public SPlayerSetupEvent[] SPlayerSetupEvents { get; init; }
    /// <summary>Event ControlPlayerId</summary>
    ///
    public SPlayerStatsEvent[] SPlayerStatsEvents { get; init; }
    /// <summary>Event ControlPlayerId</summary>
    ///
    public SUnitBornEvent[] SUnitBornEvents { get; init; }
    /// <summary>Event ControlPlayerId</summary>
    ///
    public SUnitDiedEvent[] SUnitDiedEvents { get; init; }
    /// <summary>Event ControlPlayerId</summary>
    ///
    public SUnitOwnerChangeEvent[] SUnitOwnerChangeEvents { get; init; }
    /// <summary>Event ControlPlayerId</summary>
    ///
    public SUnitPositionsEvent[] SUnitPositionsEvents { get; init; }
    /// <summary>Event ControlPlayerId</summary>
    ///
    public SUnitTypeChangeEvent[] SUnitTypeChangeEvents { get; init; }
    /// <summary>Event ControlPlayerId</summary>
    ///
    public SUpgradeEvent[] SUpgradeEvents { get; init; }
    /// <summary>Event ControlPlayerId</summary>
    ///
    public SUnitInitEvent[] SUnitInitEvents { get; init; }
    /// <summary>Event ControlPlayerId</summary>
    ///
    public SUnitDoneEvent[] SUnitDoneEvents { get; init; }
}

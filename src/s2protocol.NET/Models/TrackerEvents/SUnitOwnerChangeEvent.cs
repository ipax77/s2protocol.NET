using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s2protocol.NET.Models;
/// <summary>Record <c>SUnitOwnerChangeEvent</c> SUnitOwnerChangeEvent</summary>
///
public record SUnitOwnerChangeEvent : TrackerEvent
{
    /// <summary>Record <c>SUnitOwnerChangeEvent</c> constructor</summary>
    ///
    public SUnitOwnerChangeEvent(
        TrackerEvent trackerEvent,
        int unitTagIndex,
        int unitTagRecycle,
        int controlPlayerId,
        int upkeepPlayerId) : base(trackerEvent)
    {
        UnitTagIndex = unitTagIndex;
        UnitTagRecycle = unitTagRecycle;
        ControlPlayerId = controlPlayerId;
        UpkeepPlayerId = upkeepPlayerId;
    }

    /// <summary>Event UnitTagIndex</summary>
    ///
    public int UnitTagIndex { get; init; }
    /// <summary>Event UnitTagRecycle</summary>
    ///
    public int UnitTagRecycle { get; init; }
    /// <summary>Event ControlPlayerId</summary>
    ///
    public int ControlPlayerId { get; init; }
    /// <summary>Event UpkeepPlayerId</summary>
    ///
    public int UpkeepPlayerId { get; init; }
}

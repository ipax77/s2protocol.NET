using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s2protocol.NET.Models;
/// <summary>Record <c>Event</c> Event baseclass</summary>
///
public record TrackerEvent
{
    /// <summary>Record <c>Event</c> base constructor</summary>
    ///
    public TrackerEvent(int playerId, int eventId, TrackerEventType eventType, int bits, int gameloop)
    {
        PlayerId = playerId;
        EventId = eventId;
        EventType = eventType;
        Bits = bits;
        Gameloop = gameloop;
    }
    /// <summary>Event PlayerId</summary>
    ///
    public int PlayerId { get; init; }

    /// <summary>Event EventId</summary>
    ///
    public int EventId { get; init; }
    /// <summary>Event EventType</summary>
    ///
    public TrackerEventType EventType { get; init; }
    /// <summary>Event Bits</summary>
    ///
    public int Bits { get; init; }
    /// <summary>Event Gameloop</summary>
    ///
    public int Gameloop { get; init; }

}


/// <summary>Enum <c>EventType</c> Event type</summary>
///
public enum TrackerEventType
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    None = 0,
    SPlayerSetupEvent = 1,
    SPlayerStatsEvent = 2,
    SUnitBornEvent = 3,
    SUnitDiedEvent = 4,
    SUnitOwnerChangeEvent = 5,
    SUnitPositionsEvent = 6,
    SUnitTypeChangeEvent = 7,
    SUpgradeEvent = 8
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}

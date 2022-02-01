namespace s2protocol.NET.Models;
/// <summary>Record <c>Event</c> Event baseclass</summary>
///
public record TrackerEvent
{
    /// <summary>Record <c>Event</c> base constructor</summary>
    /// <comment>Tracker events are new in version 2.0.8, they do not exist in replays recorded with older versions of the game.</comment>
    /// 
    public TrackerEvent(int playerId, int eventId, string eventType, int bits, int gameloop)
    {
        PlayerId = playerId;
        EventId = eventId;
        Bits = bits;
        Gameloop = gameloop;
        EventType = eventType switch
        {
            "NNet.Replay.Tracker.SPlayerSetupEvent" => TrackerEventType.SPlayerSetupEvent,
            "NNet.Replay.Tracker.SPlayerStatsEvent" => TrackerEventType.SPlayerStatsEvent,
            "NNet.Replay.Tracker.SUnitBornEvent" => TrackerEventType.SUnitBornEvent,
            "NNet.Replay.Tracker.SUnitDiedEvent" => TrackerEventType.SUnitDiedEvent,
            "NNet.Replay.Tracker.SUnitOwnerChangeEvent" => TrackerEventType.SUnitOwnerChangeEvent,
            "NNet.Replay.Tracker.SUnitPositionsEvent" => TrackerEventType.SUnitPositionsEvent,
            "NNet.Replay.Tracker.SUnitTypeChangeEvent" => TrackerEventType.SUnitTypeChangeEvent,
            "NNet.Replay.Tracker.SUpgradeEvent" => TrackerEventType.SUpgradeEvent,
            "NNet.Replay.Tracker.SUnitInitEvent" => TrackerEventType.SUnitInitEvent,
            "NNet.Replay.Tracker.SUnitDoneEvent" => TrackerEventType.SUnitDoneEvent,
            _ => TrackerEventType.None
        };
    }

    /// <summary>Record <c>Event</c> base clone constructor</summary>
    /// <comment>Tracker events are new in version 2.0.8, they do not exist in replays recorded with older versions of the game.</comment>
    /// 

    public TrackerEvent(TrackerEvent trackerEvent)
    {
        if (trackerEvent == null)
        {
            throw new ArgumentNullException(nameof(trackerEvent));
        }
        PlayerId = trackerEvent.PlayerId;
        EventId = trackerEvent.EventId;
        Bits = trackerEvent.Bits;
        Gameloop = trackerEvent.Gameloop;
        EventType = trackerEvent.EventType;
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
    SUpgradeEvent = 8,
    SUnitInitEvent = 9,
    SUnitDoneEvent = 10
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}

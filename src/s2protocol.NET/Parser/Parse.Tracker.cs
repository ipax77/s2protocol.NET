using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;

internal static partial class Parse
{
    internal static TrackerEvents Tracker(IEnumerable<Dictionary<string, object?>> trackerEvents)
    {
        TrackerEventStore events = new();

        foreach (var eventDic in trackerEvents)
        {
            if (eventDic is not Dictionary<string, object> cleanEventDic)
            {
                continue;
            }

            events.Add(GetTrackerEventTyped((Dictionary<string, object>)(object)cleanEventDic));
        }

        return new TrackerEvents(events);
    }

    internal static TrackerEvent GetTrackerEventTyped(Dictionary<string, object> eventDic)
    {
        var header = GetTrackerEventHeader(eventDic);

        return header.EventType switch
        {
            TrackerEventType.SPlayerSetupEvent => GetSPlayerSetupEvent(eventDic, header),
            TrackerEventType.SPlayerStatsEvent => GetSPlayerStatsEvent(eventDic, header),
            TrackerEventType.SUnitBornEvent => GetSUnitBornEvent(eventDic, header),
            TrackerEventType.SUnitDiedEvent => GetSUnitDiedEvent(eventDic, header),
            TrackerEventType.SUnitOwnerChangeEvent => GetSUnitOwnerChangeEvent(eventDic, header),
            TrackerEventType.SUnitPositionsEvent => GetSUnitPositionsEvent(eventDic, header),
            TrackerEventType.SUnitTypeChangeEvent => GetSUnitTypeChangeEvent(eventDic, header),
            TrackerEventType.SUpgradeEvent => GetSUpgradeEvent(eventDic, header),
            TrackerEventType.SUnitInitEvent => GetSUnitInitEvent(eventDic, header),
            TrackerEventType.SUnitDoneEvent => GetSUnitDoneEvent(eventDic, header),
            _ => GetUnknownEvent(header)
        };
    }

    private static UnknownTrackerEvent GetUnknownEvent(TrackerEventHeader header)
    {
        return new UnknownTrackerEvent(
            header.PlayerId,
            header.EventId,
            header.EventType,
            header.Bits,
            header.Gameloop,
            header.EventTypeName);
    }

    internal static void SetTrackerEventsUnitConnections(TrackerEvents trackerEvents)
    {
        var diedByUnitIndex = new Dictionary<int, SUnitDiedEvent>(trackerEvents.SUnitDiedEvents.Count);
        var doneByUnitIndex = new Dictionary<int, SUnitDoneEvent>(trackerEvents.SUnitDoneEvents.Count);
        var bornByUnitTag = new Dictionary<UnitTag, SUnitBornEvent>(trackerEvents.SUnitBornEvents.Count);
        var initByUnitTag = new Dictionary<UnitTag, SUnitInitEvent>(trackerEvents.SUnitInitEvents.Count);

        foreach (TrackerEvent trackerEvent in trackerEvents.BaseTrackerEvents)
        {
            switch (trackerEvent)
            {
                case SUnitDiedEvent e:
                    diedByUnitIndex.TryAdd(e.UnitIndex, e);
                    break;
                case SUnitDoneEvent e:
                    doneByUnitIndex.TryAdd(e.UnitIndex, e);
                    break;
                case SUnitBornEvent e:
                    bornByUnitTag.TryAdd(new UnitTag(e.UnitTagIndex, e.UnitTagRecycle), e);
                    break;
                case SUnitInitEvent e:
                    initByUnitTag.TryAdd(new UnitTag(e.UnitTagIndex, e.UnitTagRecycle), e);
                    break;
            }
        }

        foreach (TrackerEvent trackerEvent in trackerEvents.BaseTrackerEvents)
        {
            switch (trackerEvent)
            {
                case SUnitBornEvent e:
                    diedByUnitIndex.TryGetValue(e.UnitIndex, out var diedEvent);
                    e.SUnitDiedEvent = diedEvent;
                    break;
                case SUnitInitEvent e:
                    diedByUnitIndex.TryGetValue(e.UnitIndex, out var initDiedEvent);
                    doneByUnitIndex.TryGetValue(e.UnitIndex, out var doneEvent);
                    e.SUnitDiedEvent = initDiedEvent;
                    e.SUnitDoneEvent = doneEvent;
                    break;
                case SUnitDiedEvent e when e.KillerUnitTagIndex is not null && e.KillerUnitTagRecycle is not null:
                    var killerTag = new UnitTag(e.KillerUnitTagIndex.Value, e.KillerUnitTagRecycle.Value);
                    bornByUnitTag.TryGetValue(killerTag, out var bornEvent);
                    initByUnitTag.TryGetValue(killerTag, out var initEvent);
                    e.KillerUnitBornEvent = bornEvent;
                    e.KillerUnitInitEvent = initEvent;
                    break;
            }
        }
    }

    private static SUnitDoneEvent GetSUnitDoneEvent(Dictionary<string, object> pydic, TrackerEventHeader header)
    {
        int unitTagIndex = GetInt(pydic, "m_unitTagIndex");
        int unitTagRecycle = GetInt(pydic, "m_unitTagRecycle");
        return new SUnitDoneEvent(header.PlayerId,
                                  header.EventId,
                                  header.Bits,
                                  header.Gameloop,
                                  unitTagIndex,
                                  unitTagRecycle);
    }


    private static SUnitInitEvent GetSUnitInitEvent(Dictionary<string, object> pydic, TrackerEventHeader header)
    {
        int unitTagIndex = GetInt(pydic, "m_unitTagIndex");
        int unitTagRecycle = GetInt(pydic, "m_unitTagRecycle");
        string unitTypeName = GetString(pydic, "m_unitTypeName");
        int controlPlayerId = GetInt(pydic, "m_controlPlayerId");
        int x = GetInt(pydic, "m_x");
        int y = GetInt(pydic, "m_y");
        int upkeepPlayerId = GetInt(pydic, "m_upkeepPlayerId");
        return new SUnitInitEvent(header.PlayerId,
                                  header.EventId,
                                  header.Bits,
                                  header.Gameloop,
                                  unitTagIndex,
                                  unitTagRecycle,
                                  controlPlayerId,
                                  x,
                                  y,
                                  upkeepPlayerId,
                                  unitTypeName);
    }

    private static SUpgradeEvent GetSUpgradeEvent(Dictionary<string, object> pydic, TrackerEventHeader header)
    {
        int count = GetInt(pydic, "m_count");
        string upgradeTypeName = GetString(pydic, "m_upgradeTypeName");
        return new SUpgradeEvent(header.PlayerId,
                                 header.EventId,
                                 header.Bits,
                                 header.Gameloop,
                                 count,
                                 upgradeTypeName);
    }

    private static SUnitTypeChangeEvent GetSUnitTypeChangeEvent(Dictionary<string, object> pydic, TrackerEventHeader header)
    {
        int unitTagIndex = GetInt(pydic, "m_unitTagIndex");
        int unitTagRecycle = GetInt(pydic, "m_unitTagRecycle");
        string unitTypeName = GetString(pydic, "m_unitTypeName");
        return new SUnitTypeChangeEvent(header.PlayerId,
                                        header.EventId,
                                        header.Bits,
                                        header.Gameloop,
                                        unitTagIndex,
                                        unitTagRecycle,
                                        unitTypeName);
    }

    private static SUnitPositionsEvent GetSUnitPositionsEvent(Dictionary<string, object> pydic, TrackerEventHeader header)
    {
        int firstUnitIndex = GetInt(pydic, "m_firstUnitIndex");
        List<int> items = new();
        if (pydic.ContainsKey("m_items"))
        {
            if (pydic["m_items"] is ICollection<object> nums)
            {
                foreach (var num in nums)
                {
                    var n = num as int?;
                    if (n != null)
                    {
                        items.Add(n.Value);
                    }
                }
            }
        }
        return new SUnitPositionsEvent(header.PlayerId,
                                       header.EventId,
                                       header.Bits,
                                       header.Gameloop,
                                       firstUnitIndex,
                                       [.. items]);
    }

    private static SUnitOwnerChangeEvent GetSUnitOwnerChangeEvent(Dictionary<string, object> pydic, TrackerEventHeader header)
    {
        int unitTagIndex = GetInt(pydic, "m_unitTagIndex");
        int unitTagRecycle = GetInt(pydic, "m_unitTagRecycle");
        int controlPlayerId = GetInt(pydic, "m_controlPlayerId");
        int upkeepPlayerId = GetInt(pydic, "m_upkeepPlayerId");
        return new SUnitOwnerChangeEvent(header.PlayerId,
                                         header.EventId,
                                         header.Bits,
                                         header.Gameloop,
                                         unitTagIndex,
                                         unitTagRecycle,
                                         controlPlayerId,
                                         upkeepPlayerId);
    }

    private static SUnitDiedEvent GetSUnitDiedEvent(Dictionary<string, object> pydic, TrackerEventHeader header)
    {
        int unitTagIndex = GetInt(pydic, "m_unitTagIndex");
        int unitTagRecycle = GetInt(pydic, "m_unitTagRecycle");
        int? killerPlayerId = GetNullableInt(pydic, "m_killerPlayerId");
        int x = GetInt(pydic, "m_x");
        int y = GetInt(pydic, "m_y");
        int? killerUnitTagRecycle = GetNullableInt(pydic, "m_killerUnitTagRecycle");
        int? killerUnitTagIndex = GetNullableInt(pydic, "m_killerUnitTagIndex");
        return new SUnitDiedEvent(header.PlayerId,
                                  header.EventId,
                                  header.Bits,
                                  header.Gameloop,
                                  unitTagIndex,
                                  unitTagRecycle,
                                  killerPlayerId,
                                  x,
                                  y,
                                  killerUnitTagRecycle,
                                  killerUnitTagIndex);
    }

    private static SUnitBornEvent GetSUnitBornEvent(Dictionary<string, object> pydic, TrackerEventHeader header)
    {
        int unitTagIndex = GetInt(pydic, "m_unitTagIndex");
        int unitTagRecycle = GetInt(pydic, "m_unitTagRecycle");
        string? creatorAbilityName = GetNullableString(pydic, "m_creatorAbilityName");
        int? creatorUnitTagRecylce = GetNullableInt(pydic, "m_creatorUnitTagRecycle");
        int controlPlayerId = GetInt(pydic, "m_controlPlayerId");
        int x = GetInt(pydic, "m_x");
        int y = GetInt(pydic, "m_y");
        int upkeepPlayerId = GetInt(pydic, "m_upkeepPlayerId");
        string unitTypeName = GetString(pydic, "m_unitTypeName");
        int? creatorUnitTagIndex = GetNullableInt(pydic, "m_creatorUnitTagIndex");
        return new SUnitBornEvent(header.PlayerId,
                                  header.EventId,
                                  header.Bits,
                                  header.Gameloop,
                                  unitTagIndex,
                                  unitTagRecycle,
                                  creatorAbilityName,
                                  creatorUnitTagRecylce,
                                  controlPlayerId,
                                  x,
                                  y,
                                  upkeepPlayerId,
                                  unitTypeName,
                                  creatorUnitTagIndex);
    }

    private static SPlayerSetupEvent GetSPlayerSetupEvent(Dictionary<string, object> pydic, TrackerEventHeader header)
    {
        int type = GetInt(pydic, "m_type");
        int? userId = GetNullableInt(pydic, "m_userId");
        int? slotId = GetNullableInt(pydic, "m_slotId");
        return new SPlayerSetupEvent(header.PlayerId,
                                     header.EventId,
                                     header.Bits,
                                     header.Gameloop,
                                     type,
                                     userId,
                                     slotId);
    }

    private static TrackerEventHeader GetTrackerEventHeader(Dictionary<string, object> pydic)
    {
        var playerId = GetInt(pydic, "m_playerId");
        var eventId = GetInt(pydic, "_eventid");
        string eventTypeName = GetString(pydic, "_event");
        var eventType = GetTrackerEventType(eventTypeName);
        var bits = GetInt(pydic, "_bits");
        var gameloop = GetInt(pydic, "_gameloop");

        return new TrackerEventHeader(
            playerId,
            eventId,
            eventType,
            bits,
            gameloop,
            eventTypeName);
    }

    private static TrackerEventType GetTrackerEventType(string eventType) => eventType switch
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

    private readonly record struct TrackerEventHeader(
    int PlayerId,
    int EventId,
    TrackerEventType EventType,
    int Bits,
    int Gameloop,
    string EventTypeName);

    private readonly record struct UnitTag(int Index, int Recycle);
}

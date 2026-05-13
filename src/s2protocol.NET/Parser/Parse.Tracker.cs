using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;

internal static partial class Parse
{
    internal static TrackerEvents Tracker(IEnumerable<Dictionary<string, object?>> trackerEvents)
    {
        var playerSetupEvents = new List<SPlayerSetupEvent>();
        var playerStatsEvents = new List<SPlayerStatsEvent>();
        var unitBornEvents = new List<SUnitBornEvent>();
        var unitDiedEvents = new List<SUnitDiedEvent>();
        var unitOwnerChangeEvents = new List<SUnitOwnerChangeEvent>();
        var unitPositionsEvents = new List<SUnitPositionsEvent>();
        var unitTypeChangeEvents = new List<SUnitTypeChangeEvent>();
        var upgradeEvents = new List<SUpgradeEvent>();
        var unitInitEvents = new List<SUnitInitEvent>();
        var unitDoneEvents = new List<SUnitDoneEvent>();

        foreach (var eventDic in trackerEvents)
        {
            if (eventDic is not Dictionary<string, object> cleanEventDic)
            {
                continue;
            }

            var parsedEvent = GetTrackerEventTyped(cleanEventDic as Dictionary<string, object>);

            switch (parsedEvent)
            {
                case SPlayerSetupEvent e:
                    playerSetupEvents.Add(e);
                    break;

                case SPlayerStatsEvent e:
                    playerStatsEvents.Add(e);
                    break;

                case SUnitBornEvent e:
                    unitBornEvents.Add(e);
                    break;

                case SUnitDiedEvent e:
                    unitDiedEvents.Add(e);
                    break;

                case SUnitOwnerChangeEvent e:
                    unitOwnerChangeEvents.Add(e);
                    break;

                case SUnitPositionsEvent e:
                    unitPositionsEvents.Add(e);
                    break;

                case SUnitTypeChangeEvent e:
                    unitTypeChangeEvents.Add(e);
                    break;

                case SUpgradeEvent e:
                    upgradeEvents.Add(e);
                    break;

                case SUnitInitEvent e:
                    unitInitEvents.Add(e);
                    break;

                case SUnitDoneEvent e:
                    unitDoneEvents.Add(e);
                    break;
            }
        }

        var events = new TrackerEvents(
            playerSetupEvents.ToArray(),
            playerStatsEvents.ToArray(),
            unitBornEvents.ToArray(),
            unitDiedEvents.ToArray(),
            unitOwnerChangeEvents.ToArray(),
            unitPositionsEvents.ToArray(),
            unitTypeChangeEvents.ToArray(),
            upgradeEvents.ToArray(),
            unitInitEvents.ToArray(),
            unitDoneEvents.ToArray());

        return events;
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
            TrackerEventType.None => throw new NotImplementedException(),
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
            header.Gameloop);
    }

    internal static void SetTrackerEventsUnitConnections(TrackerEvents trackerEvents)
    {
        var diedByUnitIndex = new Dictionary<int, SUnitDiedEvent>();
        var doneByUnitIndex = new Dictionary<int, SUnitDoneEvent>();
        var bornByUnitTag = new Dictionary<UnitTag, SUnitBornEvent>();
        var initByUnitTag = new Dictionary<UnitTag, SUnitInitEvent>();

        foreach (var e in trackerEvents.SUnitDiedEvents)
        {
            diedByUnitIndex.TryAdd(e.UnitIndex, e);
        }

        foreach (var e in trackerEvents.SUnitDoneEvents)
        {
            doneByUnitIndex.TryAdd(e.UnitIndex, e);
        }

        foreach (var e in trackerEvents.SUnitBornEvents)
        {
            bornByUnitTag.TryAdd(new UnitTag(e.UnitTagIndex, e.UnitTagRecycle), e);
        }

        foreach (var e in trackerEvents.SUnitInitEvents)
        {
            initByUnitTag.TryAdd(new UnitTag(e.UnitTagIndex, e.UnitTagRecycle), e);
        }

        foreach (var e in trackerEvents.SUnitBornEvents)
        {
            diedByUnitIndex.TryGetValue(e.UnitIndex, out var diedEvent);
            e.SUnitDiedEvent = diedEvent;
        }

        foreach (var e in trackerEvents.SUnitInitEvents)
        {
            diedByUnitIndex.TryGetValue(e.UnitIndex, out var diedEvent);
            doneByUnitIndex.TryGetValue(e.UnitIndex, out var doneEvent);

            e.SUnitDiedEvent = diedEvent;
            e.SUnitDoneEvent = doneEvent;
        }

        foreach (var e in trackerEvents.SUnitDiedEvents)
        {
            if (e.KillerUnitTagIndex is null || e.KillerUnitTagRecycle is null)
            {
                continue;
            }

            var killerTag = new UnitTag(
                e.KillerUnitTagIndex.Value,
                e.KillerUnitTagRecycle.Value);

            bornByUnitTag.TryGetValue(killerTag, out var bornEvent);
            initByUnitTag.TryGetValue(killerTag, out var initEvent);

            e.KillerUnitBornEvent = bornEvent;
            e.KillerUnitInitEvent = initEvent;
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
        List<int> items = [];
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
        int slotId = GetInt(pydic, "m_slotId");
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
        var eventType = GetTrackerEventType(GetString(pydic, "_event"));
        var bits = GetInt(pydic, "_bits");
        var gameloop = GetInt(pydic, "_gameloop");

        return new TrackerEventHeader(
            playerId,
            eventId,
            eventType,
            bits,
            gameloop);
    }

    private static TrackerEventType GetTrackerEventType(string eventType)
    {
        return eventType switch
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

    private readonly record struct TrackerEventHeader(
    int PlayerId,
    int EventId,
    TrackerEventType EventType,
    int Bits,
    int Gameloop);

    private readonly record struct UnitTag(int Index, int Recycle);
}

using IronPython.Runtime;
using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal partial class Parse
{
    internal static TrackerEvents Tracker(dynamic pydic)
    {
        List<TrackerEvent> trackerevents = new List<TrackerEvent>();

        foreach (var ent in pydic)
        {
            PythonDictionary? eventDic = ent as PythonDictionary;
            if (eventDic != null)
            {
                TrackerEvent trackerEvent = GetTrackerEvent(eventDic);

                TrackerEvent detailEvent = trackerEvent.EventType switch
                {
                    TrackerEventType.SPlayerSetupEvent => GetSPlayerSetupEvent(eventDic, trackerEvent),
                    TrackerEventType.SPlayerStatsEvent => GetSPlayerStatsEvent(eventDic, trackerEvent),
                    TrackerEventType.SUnitBornEvent => GetSUnitBornEvent(eventDic, trackerEvent),
                    TrackerEventType.SUnitDiedEvent => GetSUnitDiedEvent(eventDic, trackerEvent),
                    TrackerEventType.SUnitOwnerChangeEvent => GetSUnitOwnerChangeEvent(eventDic, trackerEvent),
                    TrackerEventType.SUnitPositionsEvent => GetSUnitPositionsEvent(eventDic, trackerEvent),
                    TrackerEventType.SUnitTypeChangeEvent => GetSUnitTypeChangeEvent(eventDic, trackerEvent),
                    TrackerEventType.SUpgradeEvent => GetSUpgradeEvent(eventDic, trackerEvent),
                    TrackerEventType.SUnitInitEvent => GetSUnitInitEvent(eventDic, trackerEvent),
                    TrackerEventType.SUnitDoneEvent => GetSUnitDoneEvent(eventDic, trackerEvent),
                    _ => GetUnknownEvent(eventDic, trackerEvent)
                };
                trackerevents.Add(detailEvent);
            }
        }

        return new TrackerEvents(
            trackerevents.OfType<SPlayerSetupEvent>().ToArray(),
            trackerevents.OfType<SPlayerStatsEvent>().ToArray(),
            trackerevents.OfType<SUnitBornEvent>().ToArray(),
            trackerevents.OfType<SUnitDiedEvent>().ToArray(),
            trackerevents.OfType<SUnitOwnerChangeEvent>().ToArray(),
            trackerevents.OfType<SUnitPositionsEvent>().ToArray(),
            trackerevents.OfType<SUnitTypeChangeEvent>().ToArray(),
            trackerevents.OfType<SUpgradeEvent>().ToArray(),
            trackerevents.OfType<SUnitInitEvent>().ToArray(),
            trackerevents.OfType<SUnitDoneEvent>().ToArray()
        );
    }

    private static TrackerEvent GetTrackerEvent(PythonDictionary pydic)
    {
        int playerId = GetInt(pydic, "m_playerId");
        int eventId = GetInt(pydic, "_eventid");
        string type = GetString(pydic, "_event");
        int bits = GetInt(pydic, "_bits");
        int gameloop = GetInt(pydic, "_gameloop");
        return new TrackerEvent(playerId, eventId, type, bits, gameloop);
    }

    private static TrackerEvent GetUnknownEvent(PythonDictionary pydic, TrackerEvent trackerEvent)
    {
        ReplayDecoder.logger.DecodeWarning($"Game event type unknown: {GetString(pydic, "_event")}");
        return trackerEvent;
    }

    private static SUnitDoneEvent GetSUnitDoneEvent(PythonDictionary pydic, TrackerEvent trackerEvent)
    {
        int unitTagIndex = GetInt(pydic, "m_unitTagIndex");
        int unitTagRecycle = GetInt(pydic, "m_unitTagRecycle");
        return new SUnitDoneEvent(trackerEvent, unitTagIndex, unitTagRecycle);
    }


    private static SUnitInitEvent GetSUnitInitEvent(PythonDictionary pydic, TrackerEvent trackerEvent)
    {
        int unitTagIndex = GetInt(pydic, "m_unitTagIndex");
        int unitTagRecycle = GetInt(pydic, "m_unitTagRecycle");
        string unitTypeName = GetString(pydic, "m_unitTypeName");
        int controlPlayerId = GetInt(pydic, "m_controlPlayerId");
        int x = GetInt(pydic, "m_x");
        int y = GetInt(pydic, "m_y");
        int upkeepPlayerId = GetInt(pydic, "m_upkeepPlayerId");
        return new SUnitInitEvent(trackerEvent, unitTagIndex, unitTagRecycle, controlPlayerId, x, y, upkeepPlayerId, unitTypeName);
    }

    private static SUpgradeEvent GetSUpgradeEvent(PythonDictionary pydic, TrackerEvent trackerEvent)
    {
        int count = GetInt(pydic, "m_count");
        string upgradeTypeName = GetString(pydic, "m_upgradeTypeName");
        return new SUpgradeEvent(trackerEvent, count, upgradeTypeName);
    }

    private static SUnitTypeChangeEvent GetSUnitTypeChangeEvent(PythonDictionary pydic, TrackerEvent trackerEvent)
    {
        int unitTagIndex = GetInt(pydic, "m_unitTagIndex");
        int unitTagRecycle = GetInt(pydic, "m_unitTagRecycle");
        string unitTypeName = GetString(pydic, "m_unitTypeName");
        return new SUnitTypeChangeEvent(trackerEvent, unitTagIndex, unitTagRecycle, unitTypeName);
    }

    private static SUnitPositionsEvent GetSUnitPositionsEvent(PythonDictionary pydic, TrackerEvent trackerEvent)
    {
        int firstUnitIndex = GetInt(pydic, "m_firstUnitIndex");
        List<int> items = new List<int>();
        if (pydic.ContainsKey("m_items"))
        {
            var nums = pydic["m_items"] as ICollection<object>;
            if (nums != null)
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
        return new SUnitPositionsEvent(trackerEvent, firstUnitIndex, items.ToArray());
    }

    private static SUnitOwnerChangeEvent GetSUnitOwnerChangeEvent(PythonDictionary pydic, TrackerEvent trackerEvent)
    {
        int unitTagIndex = GetInt(pydic, "m_unitTagIndex");
        int unitTagRecycle = GetInt(pydic, "m_unitTagRecycle");
        int controlPlayerId = GetInt(pydic, "m_controlPlayerId");
        int upkeepPlayerId = GetInt(pydic, "m_upkeepPlayerId");
        return new SUnitOwnerChangeEvent(trackerEvent, unitTagIndex, unitTagRecycle, controlPlayerId, upkeepPlayerId);
    }

    private static SUnitDiedEvent GetSUnitDiedEvent(PythonDictionary pydic, TrackerEvent trackerEvent)
    {
        int unitTagIndex = GetInt(pydic, "m_unitTagIndex");
        int unitTagRecycle = GetInt(pydic, "m_unitTagRecycle");
        int? killerPlayerId = GetNullableInt(pydic, "m_killerPlayerId");
        int x = GetInt(pydic, "m_x");
        int y = GetInt(pydic, "m_y");
        int? killerUnitTagRecycle = GetNullableInt(pydic, "m_killerUnitTagRecycle");
        int? killerUnitTagIndex = GetNullableInt(pydic, "m_killerUnitTagIndex");
        return new SUnitDiedEvent(trackerEvent, unitTagIndex, unitTagRecycle, killerPlayerId, x, y, killerUnitTagRecycle, killerUnitTagIndex);
    }

    private static SUnitBornEvent GetSUnitBornEvent(PythonDictionary pydic, TrackerEvent trackerEvent)
    {
        int unitTagIndex = GetInt(pydic, "m_unitTagIndex");
        int unitTagRecycle = GetInt(pydic, "m_unitTagRecycle");
        string? creatorAbilityName = GetNullableString(pydic, "m_creatorAbilityName");
        string? creatorUnitTagRecylce = GetNullableString(pydic, "m_creatorUnitTagRecycle");
        int controlPlayerId = GetInt(pydic, "m_controlPlayerId");
        int x = GetInt(pydic, "m_x");
        int y = GetInt(pydic, "m_y");
        int upkeepPlayerId = GetInt(pydic, "m_upkeepPlayerId");
        string unitTypeName = GetString(pydic, "m_unitTypeName");
        int? creatorUnitTagIndex = GetNullableInt(pydic, "m_creatorUnitTagIndex");
        return new SUnitBornEvent(trackerEvent, unitTagIndex, unitTagRecycle, creatorAbilityName, creatorUnitTagIndex, controlPlayerId, x, y, upkeepPlayerId, unitTypeName, creatorUnitTagIndex);
    }

    private static SPlayerSetupEvent GetSPlayerSetupEvent(PythonDictionary pydic, TrackerEvent trackerEvent)
    {
        int type = GetInt(pydic, "m_type");
        int? userId = GetNullableInt(pydic, "m_userId");
        int slotId = GetInt(pydic, "m_slotId");
        return new SPlayerSetupEvent(trackerEvent, type, userId, slotId);
    }
}

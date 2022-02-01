using IronPython.Runtime;
using s2protocol.NET.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s2protocol.NET.Parser;
internal partial class Parse
{
    public static TrackerEvents Tracker(dynamic pydic)
    {
        List<SPlayerSetupEvent> SPlayerSetupEvents = new();
        List<SPlayerStatsEvent> SPlayerStatsEvents = new();
        List<SUnitBornEvent> SUnitBornEvents = new();
        List<SUnitDiedEvent> SUnitDiedEvents = new();
        List<SUnitOwnerChangeEvent> SUnitOwnerChangeEvents = new();
        List<SUnitPositionsEvent> SUnitPositionsEvents = new();
        List<SUnitTypeChangeEvent> SUnitTypeChangeEvents = new();
        List<SUpgradeEvent> SUpgradeEvents = new();
        List<SUnitInitEvent> SUnitInitEvents = new();
        List<SUnitDoneEvent> SUnitDoneEvent = new();

        foreach (var ent in pydic)
        {
            PythonDictionary? eventDic = ent as PythonDictionary;
            if (eventDic != null)
            {
                TrackerEvent trackerEvent = GetTrackerEvent(eventDic);

                if (trackerEvent.EventType == TrackerEventType.SUnitBornEvent)
                {
                    SUnitBornEvents.Add(GetSUnitBornEvent(eventDic, trackerEvent));
                }
                else if (trackerEvent.EventType == TrackerEventType.SPlayerSetupEvent)
                {
                    SPlayerSetupEvents.Add(GetSPlayerSetupEvent(eventDic, trackerEvent));
                }
                else if (trackerEvent.EventType == TrackerEventType.SUnitDiedEvent)
                {
                    SUnitDiedEvents.Add(GetSUnitDiedEvent(eventDic, trackerEvent));
                }
                else if (trackerEvent.EventType == TrackerEventType.SPlayerStatsEvent)
                {
                    SPlayerStatsEvents.Add(GetSPlayerStatsEvent(eventDic, trackerEvent));
                }
                else if (trackerEvent.EventType == TrackerEventType.SUnitOwnerChangeEvent)
                {
                    SUnitOwnerChangeEvents.Add(GetSUnitOwnerChangeEvent(eventDic, trackerEvent));
                }
                else if (trackerEvent.EventType == TrackerEventType.SUnitPositionsEvent)
                {
                    SUnitPositionsEvents.Add(GetSUnitPositionsEvent(eventDic, trackerEvent));
                }
                else if (trackerEvent.EventType == TrackerEventType.SUnitTypeChangeEvent)
                {
                    SUnitTypeChangeEvents.Add(GetSUnitTypeChangeEvent(eventDic, trackerEvent));
                }
                else if (trackerEvent.EventType == TrackerEventType.SUpgradeEvent)
                {
                    SUpgradeEvents.Add(GetSUpgradeEvent(eventDic, trackerEvent));
                }
                else if (trackerEvent.EventType == TrackerEventType.SUnitInitEvent)
                {
                    SUnitInitEvents.Add(GetSUnitInitEvent(eventDic, trackerEvent));
                }
            }
        }

        //SUnitInitEvent = 9,

        return new TrackerEvents(
            SPlayerSetupEvents.ToArray(),
            SPlayerStatsEvents.ToArray(),
            SUnitBornEvents.ToArray(),
            SUnitDiedEvents.ToArray(),
            SUnitOwnerChangeEvents.ToArray(),
            SUnitPositionsEvents.ToArray(),
            SUnitTypeChangeEvents.ToArray(),
            SUpgradeEvents.ToArray(),
            SUnitInitEvents.ToArray(),
            SUnitDoneEvent.ToArray()
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

    private static SPlayerStatsEvent GetSPlayerStatsEvent(PythonDictionary pydic, TrackerEvent trackerEvent)
    {
        if (pydic.ContainsKey("m_stats"))
        {
            PythonDictionary? statsDic = pydic["m_stats"] as PythonDictionary;
            if (statsDic != null)
            {
                int scoreValueVespeneUsedCurrentTechnology = GetInt(statsDic, "m_scoreValueVespeneUsedCurrentTechnology");
                int scoreValueVespeneFriendlyFireArmy = GetInt(statsDic, "m_scoreValueVespeneFriendlyFireArmy");
                int scoreValueMineralsFriendlyFireTechnology = GetInt(statsDic, "m_scoreValueMineralsFriendlyFireTechnology");
                int scoreValueMineralsUsedCurrentEconomy = GetInt(statsDic, "m_scoreValueMineralsUsedCurrentEconomy");
                int scoreValueVespeneLostEconomy = GetInt(statsDic, "m_scoreValueVespeneLostEconomy");
                int scoreValueMineralsUsedCurrentArmy = GetInt(statsDic, "m_scoreValueMineralsUsedCurrentArmy");
                int scoreValueVespeneUsedInProgressArmy = GetInt(statsDic, "m_scoreValueVespeneUsedInProgressArmy");
                int scoreValueVespeneCollectionRate = GetInt(statsDic, "m_scoreValueVespeneCollectionRate");
                int scoreValueMineralsUsedInProgressTechnology = GetInt(statsDic, "m_scoreValueMineralsUsedInProgressTechnology");
                int scoreValueMineralsCollectionRate = GetInt(statsDic, "m_scoreValueMineralsCollectionRate");
                int scoreValueWorkersActiveCount = GetInt(statsDic, "m_scoreValueWorkersActiveCount");
                int scoreValueMineralsUsedInProgressArmy = GetInt(statsDic, "m_scoreValueMineralsUsedInProgressArmy");
                int scoreValueVespeneLostArmy = GetInt(statsDic, "m_scoreValueVespeneLostArmy");
                int scoreValueMineralsKilledEconomy = GetInt(statsDic, "m_scoreValueMineralsKilledEconomy");
                int scoreValueMineralsUsedCurrentTechnology = GetInt(statsDic, "m_scoreValueMineralsUsedCurrentTechnology");
                int scoreValueMineralsKilledArmy = GetInt(statsDic, "m_scoreValueMineralsKilledArmy");
                int scoreValueMineralsLostEconomy = GetInt(statsDic, "m_scoreValueMineralsLostEconomy");
                int scoreValueMineralsCurrent = GetInt(statsDic, "m_scoreValueMineralsCurrent");
                int scoreValueMineralsLostArmy = GetInt(statsDic, "m_scoreValueMineralsLostArmy");
                int scoreValueVespeneKilledArmy = GetInt(statsDic, "m_scoreValueVespeneKilledArmy");
                int scoreValueVespeneKilledTechnology = GetInt(statsDic, "m_scoreValueVespeneKilledTechnology");
                int scoreValueVespeneKilledEconomy = GetInt(statsDic, "m_scoreValueVespeneKilledEconomy");
                int scoreValueMineralsUsedActiveForces = GetInt(statsDic, "m_scoreValueMineralsUsedActiveForces");
                int scoreValueVespeneUsedCurrentArmy = GetInt(statsDic, "m_scoreValueVespeneUsedCurrentArmy");
                int scoreValueMineralsFriendlyFireArmy = GetInt(statsDic, "m_scoreValueMineralsFriendlyFireArmy");
                int scoreValueVespeneUsedActiveForces = GetInt(statsDic, "m_scoreValueVespeneUsedActiveForces");
                int scoreValueVespeneCurrent = GetInt(statsDic, "m_scoreValueVespeneCurrent");
                int scoreValueMineralsLostTechnology = GetInt(statsDic, "m_scoreValueMineralsLostTechnology");
                int scoreValueMineralsUsedInProgressEconomy = GetInt(statsDic, "m_scoreValueMineralsUsedInProgressEconomy");
                int scoreValueMineralsFriendlyFireEconomy = GetInt(statsDic, "m_scoreValueMineralsFriendlyFireEconomy");
                int scoreValueVespeneUsedInProgressTechnology = GetInt(statsDic, "m_scoreValueVespeneUsedInProgressTechnology");
                int scoreValueFoodMade = GetInt(statsDic, "m_scoreValueFoodMade");
                int scoreValueMineralsKilledTechnology = GetInt(statsDic, "m_scoreValueMineralsKilledTechnology");
                int scoreValueVespeneLostTechnology = GetInt(statsDic, "m_scoreValueVespeneLostTechnology");
                int scoreValueVespeneFriendlyFireEconomy = GetInt(statsDic, "m_scoreValueVespeneFriendlyFireEconomy");
                int scoreValueVespeneUsedInProgressEconomy = GetInt(statsDic, "m_scoreValueVespeneUsedInProgressEconomy");
                int scoreValueVespeneUsedCurrentEconomy = GetInt(statsDic, "m_scoreValueVespeneUsedCurrentEconomy");
                int scoreValueVespeneFriendlyFireTechnology = GetInt(statsDic, "m_scoreValueVespeneFriendlyFireTechnology");
                int scoreValueFoodUsed = GetInt(statsDic, "m_scoreValueFoodUsed");
                return new SPlayerStatsEvent
                    (
                        trackerEvent,
                        scoreValueVespeneUsedCurrentTechnology,
                        scoreValueVespeneFriendlyFireArmy,
                        scoreValueMineralsFriendlyFireTechnology,
                        scoreValueMineralsUsedCurrentEconomy,
                        scoreValueVespeneLostEconomy,
                        scoreValueMineralsUsedCurrentArmy,
                        scoreValueVespeneUsedInProgressArmy,
                        scoreValueVespeneCollectionRate,
                        scoreValueMineralsUsedInProgressTechnology,
                        scoreValueMineralsCollectionRate,
                        scoreValueWorkersActiveCount,
                        scoreValueMineralsUsedInProgressArmy,
                        scoreValueVespeneLostArmy,
                        scoreValueMineralsKilledEconomy,
                        scoreValueMineralsUsedCurrentTechnology,
                        scoreValueMineralsKilledArmy,
                        scoreValueMineralsLostEconomy,
                        scoreValueMineralsCurrent,
                        scoreValueMineralsLostArmy,
                        scoreValueVespeneKilledArmy,
                        scoreValueVespeneKilledTechnology,
                        scoreValueVespeneKilledEconomy,
                        scoreValueMineralsUsedActiveForces,
                        scoreValueVespeneUsedCurrentArmy,
                        scoreValueMineralsFriendlyFireArmy,
                        scoreValueVespeneUsedActiveForces,
                        scoreValueVespeneCurrent,
                        scoreValueMineralsLostTechnology,
                        scoreValueMineralsUsedInProgressEconomy,
                        scoreValueMineralsFriendlyFireEconomy,
                        scoreValueVespeneUsedInProgressTechnology,
                        scoreValueFoodMade,
                        scoreValueMineralsKilledTechnology,
                        scoreValueVespeneLostTechnology,
                        scoreValueVespeneFriendlyFireEconomy,
                        scoreValueVespeneUsedInProgressEconomy,
                        scoreValueVespeneUsedCurrentEconomy,
                        scoreValueVespeneFriendlyFireTechnology,
                        scoreValueFoodUsed
                    );
            }
        }
        return new SPlayerStatsEvent
            (
                trackerEvent,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
            );
    }

    private static SUnitDiedEvent GetSUnitDiedEvent(PythonDictionary pydic, TrackerEvent trackerEvent)
    {
        int unitTagIndex = GetInt(pydic, "m_unitTagIndex");
        int unitTagRecycle = GetInt(pydic, "m_unitTagRecycle");
        int killerPlayerId = GetInt(pydic, "m_killerPlayerId");
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
        int userId = GetInt(pydic, "m_userId");
        int slotId = GetInt(pydic, "m_slotId");
        return new SPlayerSetupEvent(trackerEvent, type, userId, slotId);
    }
}

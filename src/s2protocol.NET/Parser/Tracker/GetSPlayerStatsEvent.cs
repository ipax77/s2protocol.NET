using IronPython.Runtime;
using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;

internal partial class Parse
{
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
}


namespace s2protocol.NET.Models;
/// <summary>Record <c>SPlayerStatsEvent</c> SPlayerStatsEvent</summary>
///
/// <remarks>Record <c>SPlayerStatsEvent</c> constructor</remarks>
public sealed class SPlayerStatsEvent(int playerId,
                                      int eventId,
                                      int bits,
                                      int gameloop,
                                      int vespeneUsedCurrentTechnology,
                                      int vespeneFriendlyFireArmy,
                                      int mineralsFriendlyFireTechnology,
                                      int mineralsUsedCurrentEconomy,
                                      int vespeneLostEconomy,
                                      int mineralsUsedCurrentArmy,
                                      int vespeneUsedInProgressArmy,
                                      int vespeneCollectionRate,
                                      int mineralsUsedInProgressTechnology,
                                      int mineralsCollectionRate,
                                      int workersActiveCount,
                                      int mineralsUsedInProgressArmy,
                                      int vespeneLostArmy,
                                      int mineralsKilledEconomy,
                                      int mineralsUsedCurrentTechnology,
                                      int mineralsKilledArmy,
                                      int mineralsLostEconomy,
                                      int mineralsCurrent,
                                      int mineralsLostArmy,
                                      int vespeneKilledArmy,
                                      int vespeneKilledTechnology,
                                      int vespeneKilledEconomy,
                                      int mineralsUsedActiveForces,
                                      int vespeneUsedCurrentArmy,
                                      int mineralsFriendlyFireArmy,
                                      int vespeneUsedActiveForces,
                                      int vespeneCurrent,
                                      int mineralsLostTechnology,
                                      int mineralsUsedInProgressEconomy,
                                      int mineralsFriendlyFireEconomy,
                                      int vespeneUsedInProgressTechnology,
                                      int foodMade,
                                      int mineralsKilledTechnology,
                                      int vespeneLostTechnology,
                                      int vespeneFriendlyFireEconomy,
                                      int vespeneUsedInProgressEconomy,
                                      int vespeneUsedCurrentEconomy,
                                      int vespeneFriendlyFireTechnology,
                                      int foodUsed) : TrackerEvent(playerId, eventId, TrackerEventType.SPlayerStatsEvent, bits, gameloop)
{
    public int VespeneUsedCurrentTechnology { get; } = vespeneUsedCurrentTechnology;
    public int VespeneFriendlyFireArmy { get; } = vespeneFriendlyFireArmy;
    public int MineralsFriendlyFireTechnology { get; } = mineralsFriendlyFireTechnology;
    public int MineralsUsedCurrentEconomy { get; } = mineralsUsedCurrentEconomy;
    public int VespeneLostEconomy { get; } = vespeneLostEconomy;
    public int MineralsUsedCurrentArmy { get; } = mineralsUsedCurrentArmy;
    public int VespeneUsedInProgressArmy { get; } = vespeneUsedInProgressArmy;
    public int VespeneCollectionRate { get; } = vespeneCollectionRate;
    public int MineralsUsedInProgressTechnology { get; } = mineralsUsedInProgressTechnology;
    public int MineralsCollectionRate { get; } = mineralsCollectionRate;
    public int WorkersActiveCount { get; } = workersActiveCount;
    public int MineralsUsedInProgressArmy { get; } = mineralsUsedInProgressArmy;
    public int VespeneLostArmy { get; } = vespeneLostArmy;
    public int MineralsKilledEconomy { get; } = mineralsKilledEconomy;
    public int MineralsUsedCurrentTechnology { get; } = mineralsUsedCurrentTechnology;
    public int MineralsKilledArmy { get; } = mineralsKilledArmy;
    public int MineralsLostEconomy { get; } = mineralsLostEconomy;
    public int MineralsCurrent { get; } = mineralsCurrent;
    public int MineralsLostArmy { get; } = mineralsLostArmy;
    public int VespeneKilledArmy { get; } = vespeneKilledArmy;
    public int VespeneKilledTechnology { get; } = vespeneKilledTechnology;
    public int VespeneKilledEconomy { get; } = vespeneKilledEconomy;
    public int MineralsUsedActiveForces { get; } = mineralsUsedActiveForces;
    public int VespeneUsedCurrentArmy { get; } = vespeneUsedCurrentArmy;
    public int MineralsFriendlyFireArmy { get; } = mineralsFriendlyFireArmy;
    public int VespeneUsedActiveForces { get; } = vespeneUsedActiveForces;
    public int VespeneCurrent { get; } = vespeneCurrent;
    public int MineralsLostTechnology { get; } = mineralsLostTechnology;
    public int MineralsUsedInProgressEconomy { get; } = mineralsUsedInProgressEconomy;
    public int MineralsFriendlyFireEconomy { get; } = mineralsFriendlyFireEconomy;
    public int VespeneUsedInProgressTechnology { get; } = vespeneUsedInProgressTechnology;
    public int FoodMade { get; } = foodMade;
    public int MineralsKilledTechnology { get; } = mineralsKilledTechnology;
    public int VespeneLostTechnology { get; } = vespeneLostTechnology;
    public int VespeneFriendlyFireEconomy { get; } = vespeneFriendlyFireEconomy;
    public int VespeneUsedInProgressEconomy { get; } = vespeneUsedInProgressEconomy;
    public int VespeneUsedCurrentEconomy { get; } = vespeneUsedCurrentEconomy;
    public int VespeneFriendlyFireTechnology { get; } = vespeneFriendlyFireTechnology;
    public int FoodUsed { get; } = foodUsed;
}

using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>SPlayerStatsEvent</c> SPlayerStatsEvent</summary>
///
public record SPlayerStatsEvent : TrackerEvent
{
    /// <summary>Record <c>SPlayerStatsEvent</c> constructor</summary>
    ///
    public SPlayerStatsEvent(TrackerEvent trackerEvent,
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
                             int foodUsed) : base(trackerEvent)
    {
        VespeneUsedCurrentTechnology = vespeneUsedCurrentTechnology;
        VespeneFriendlyFireArmy = vespeneFriendlyFireArmy;
        MineralsFriendlyFireTechnology = mineralsFriendlyFireTechnology;
        MineralsUsedCurrentEconomy = mineralsUsedCurrentEconomy;
        VespeneLostEconomy = vespeneLostEconomy;
        MineralsUsedCurrentArmy = mineralsUsedCurrentArmy;
        VespeneUsedInProgressArmy = vespeneUsedInProgressArmy;
        VespeneCollectionRate = vespeneCollectionRate;
        MineralsUsedInProgressTechnology = mineralsUsedInProgressTechnology;
        MineralsCollectionRate = mineralsCollectionRate;
        WorkersActiveCount = workersActiveCount;
        MineralsUsedInProgressArmy = mineralsUsedInProgressArmy;
        VespeneLostArmy = vespeneLostArmy;
        MineralsKilledEconomy = mineralsKilledEconomy;
        MineralsUsedCurrentTechnology = mineralsUsedCurrentTechnology;
        MineralsKilledArmy = mineralsKilledArmy;
        MineralsLostEconomy = mineralsLostEconomy;
        MineralsCurrent = mineralsCurrent;
        MineralsLostArmy = mineralsLostArmy;
        VespeneKilledArmy = vespeneKilledArmy;
        VespeneKilledTechnology = vespeneKilledTechnology;
        VespeneKilledEconomy = vespeneKilledEconomy;
        MineralsUsedActiveForces = mineralsUsedActiveForces;
        VespeneUsedCurrentArmy = vespeneUsedCurrentArmy;
        MineralsFriendlyFireArmy = mineralsFriendlyFireArmy;
        VespeneUsedActiveForces = vespeneUsedActiveForces;
        VespeneCurrent = vespeneCurrent;
        MineralsLostTechnology = mineralsLostTechnology;
        MineralsUsedInProgressEconomy = mineralsUsedInProgressEconomy;
        MineralsFriendlyFireEconomy = mineralsFriendlyFireEconomy;
        VespeneUsedInProgressTechnology = vespeneUsedInProgressTechnology;
        FoodMade = foodMade;
        MineralsKilledTechnology = mineralsKilledTechnology;
        VespeneLostTechnology = vespeneLostTechnology;
        VespeneFriendlyFireEconomy = vespeneFriendlyFireEconomy;
        VespeneUsedInProgressEconomy = vespeneUsedInProgressEconomy;
        VespeneUsedCurrentEconomy = vespeneUsedCurrentEconomy;
        VespeneFriendlyFireTechnology = vespeneFriendlyFireTechnology;
        FoodUsed = foodUsed;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SPlayerStatsEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public int VespeneUsedCurrentTechnology { get; init; }
    public int VespeneFriendlyFireArmy { get; init; }
    public int MineralsFriendlyFireTechnology { get; init; }
    public int MineralsUsedCurrentEconomy { get; init; }
    public int VespeneLostEconomy { get; init; }
    public int MineralsUsedCurrentArmy { get; init; }
    public int VespeneUsedInProgressArmy { get; init; }
    public int VespeneCollectionRate { get; init; }
    public int MineralsUsedInProgressTechnology { get; init; }
    public int MineralsCollectionRate { get; init; }
    public int WorkersActiveCount { get; init; }
    public int MineralsUsedInProgressArmy { get; init; }
    public int VespeneLostArmy { get; init; }
    public int MineralsKilledEconomy { get; init; }
    public int MineralsUsedCurrentTechnology { get; init; }
    public int MineralsKilledArmy { get; init; }
    public int MineralsLostEconomy { get; init; }
    public int MineralsCurrent { get; init; }
    public int MineralsLostArmy { get; init; }
    public int VespeneKilledArmy { get; init; }
    public int VespeneKilledTechnology { get; init; }
    public int VespeneKilledEconomy { get; init; }
    public int MineralsUsedActiveForces { get; init; }
    public int VespeneUsedCurrentArmy { get; init; }
    public int MineralsFriendlyFireArmy { get; init; }
    public int VespeneUsedActiveForces { get; init; }
    public int VespeneCurrent { get; init; }
    public int MineralsLostTechnology { get; init; }
    public int MineralsUsedInProgressEconomy { get; init; }
    public int MineralsFriendlyFireEconomy { get; init; }
    public int VespeneUsedInProgressTechnology { get; init; }
    public int FoodMade { get; init; }
    public int MineralsKilledTechnology { get; init; }
    public int VespeneLostTechnology { get; init; }
    public int VespeneFriendlyFireEconomy { get; init; }
    public int VespeneUsedInProgressEconomy { get; init; }
    public int VespeneUsedCurrentEconomy { get; init; }
    public int VespeneFriendlyFireTechnology { get; init; }
    public int FoodUsed { get; init; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
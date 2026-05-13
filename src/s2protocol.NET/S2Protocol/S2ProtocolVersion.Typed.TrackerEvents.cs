using s2protocol.NET.Models;

namespace s2protocol.NET.S2Protocol;

public sealed partial record S2ProtocolVersion
{
    private static SPlayerSetupEvent ReadSPlayerSetupEvent(TypedProtocolDecoder decoder, int typeId, int eventId, int bits, int gameloop)
    {
        SPlayerSetupEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new SPlayerSetupEvent(state.PlayerId, eventId, bits, gameloop, state.Type, state.UserId, state.SlotId);
    }

    private struct SPlayerSetupEventReadState : IStructFieldReader
    {
        public int PlayerId;
        public int Type;
        public int? UserId;
        public int SlotId;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_playerId":
                    PlayerId = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_type":
                    Type = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_userId":
                    UserId = decoder.ReadNullableInt(fieldTypeId);
                    return true;
                case "m_slotId":
                    SlotId = decoder.ReadInt(fieldTypeId);
                    return true;
                default:
                    return false;
            }
        }
    }

    private static SPlayerStatsEvent ReadSPlayerStatsEvent(TypedProtocolDecoder decoder, int typeId, int eventId, int bits, int gameloop)
    {
        SPlayerStatsEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return !state.HasStats
            ? throw new InvalidOperationException("SPlayerStatsEvent was missing m_stats.")
            : new SPlayerStatsEvent(state.PlayerId, eventId, bits, gameloop,
            state.Stats.ScoreValueVespeneUsedCurrentTechnology,
            state.Stats.ScoreValueVespeneFriendlyFireArmy,
            state.Stats.ScoreValueMineralsFriendlyFireTechnology,
            state.Stats.ScoreValueMineralsUsedCurrentEconomy,
            state.Stats.ScoreValueVespeneLostEconomy,
            state.Stats.ScoreValueMineralsUsedCurrentArmy,
            state.Stats.ScoreValueVespeneUsedInProgressArmy,
            state.Stats.ScoreValueVespeneCollectionRate,
            state.Stats.ScoreValueMineralsUsedInProgressTechnology,
            state.Stats.ScoreValueMineralsCollectionRate,
            state.Stats.ScoreValueWorkersActiveCount,
            state.Stats.ScoreValueMineralsUsedInProgressArmy,
            state.Stats.ScoreValueVespeneLostArmy,
            state.Stats.ScoreValueMineralsKilledEconomy,
            state.Stats.ScoreValueMineralsUsedCurrentTechnology,
            state.Stats.ScoreValueMineralsKilledArmy,
            state.Stats.ScoreValueMineralsLostEconomy,
            state.Stats.ScoreValueMineralsCurrent,
            state.Stats.ScoreValueMineralsLostArmy,
            state.Stats.ScoreValueVespeneKilledArmy,
            state.Stats.ScoreValueVespeneKilledTechnology,
            state.Stats.ScoreValueVespeneKilledEconomy,
            state.Stats.ScoreValueMineralsUsedActiveForces,
            state.Stats.ScoreValueVespeneUsedCurrentArmy,
            state.Stats.ScoreValueMineralsFriendlyFireArmy,
            state.Stats.ScoreValueVespeneUsedActiveForces,
            state.Stats.ScoreValueVespeneCurrent,
            state.Stats.ScoreValueMineralsLostTechnology,
            state.Stats.ScoreValueMineralsUsedInProgressEconomy,
            state.Stats.ScoreValueMineralsFriendlyFireEconomy,
            state.Stats.ScoreValueVespeneUsedInProgressTechnology,
            state.Stats.ScoreValueFoodMade,
            state.Stats.ScoreValueMineralsKilledTechnology,
            state.Stats.ScoreValueVespeneLostTechnology,
            state.Stats.ScoreValueVespeneFriendlyFireEconomy,
            state.Stats.ScoreValueVespeneUsedInProgressEconomy,
            state.Stats.ScoreValueVespeneUsedCurrentEconomy,
            state.Stats.ScoreValueVespeneFriendlyFireTechnology,
            state.Stats.ScoreValueFoodUsed);
    }

    private struct SPlayerStatsEventReadState : IStructFieldReader
    {
        public int PlayerId;
        public bool HasStats;
        public SPlayerStatsValuesReadState Stats;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_playerId":
                    PlayerId = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_stats":
                    Stats = default;
                    decoder.ReadStruct(fieldTypeId, ref Stats);
                    HasStats = true;
                    return true;
                default:
                    return false;
            }
        }
    }

    private struct SPlayerStatsValuesReadState : IStructFieldReader
    {
        public int ScoreValueVespeneUsedCurrentTechnology;
        public int ScoreValueVespeneFriendlyFireArmy;
        public int ScoreValueMineralsFriendlyFireTechnology;
        public int ScoreValueMineralsUsedCurrentEconomy;
        public int ScoreValueVespeneLostEconomy;
        public int ScoreValueMineralsUsedCurrentArmy;
        public int ScoreValueVespeneUsedInProgressArmy;
        public int ScoreValueVespeneCollectionRate;
        public int ScoreValueMineralsUsedInProgressTechnology;
        public int ScoreValueMineralsCollectionRate;
        public int ScoreValueWorkersActiveCount;
        public int ScoreValueMineralsUsedInProgressArmy;
        public int ScoreValueVespeneLostArmy;
        public int ScoreValueMineralsKilledEconomy;
        public int ScoreValueMineralsUsedCurrentTechnology;
        public int ScoreValueMineralsKilledArmy;
        public int ScoreValueMineralsLostEconomy;
        public int ScoreValueMineralsCurrent;
        public int ScoreValueMineralsLostArmy;
        public int ScoreValueVespeneKilledArmy;
        public int ScoreValueVespeneKilledTechnology;
        public int ScoreValueVespeneKilledEconomy;
        public int ScoreValueMineralsUsedActiveForces;
        public int ScoreValueVespeneUsedCurrentArmy;
        public int ScoreValueMineralsFriendlyFireArmy;
        public int ScoreValueVespeneUsedActiveForces;
        public int ScoreValueVespeneCurrent;
        public int ScoreValueMineralsLostTechnology;
        public int ScoreValueMineralsUsedInProgressEconomy;
        public int ScoreValueMineralsFriendlyFireEconomy;
        public int ScoreValueVespeneUsedInProgressTechnology;
        public int ScoreValueFoodMade;
        public int ScoreValueMineralsKilledTechnology;
        public int ScoreValueVespeneLostTechnology;
        public int ScoreValueVespeneFriendlyFireEconomy;
        public int ScoreValueVespeneUsedInProgressEconomy;
        public int ScoreValueVespeneUsedCurrentEconomy;
        public int ScoreValueVespeneFriendlyFireTechnology;
        public int ScoreValueFoodUsed;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_scoreValueVespeneUsedCurrentTechnology":
                    ScoreValueVespeneUsedCurrentTechnology = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueVespeneFriendlyFireArmy":
                    ScoreValueVespeneFriendlyFireArmy = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueMineralsFriendlyFireTechnology":
                    ScoreValueMineralsFriendlyFireTechnology = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueMineralsUsedCurrentEconomy":
                    ScoreValueMineralsUsedCurrentEconomy = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueVespeneLostEconomy":
                    ScoreValueVespeneLostEconomy = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueMineralsUsedCurrentArmy":
                    ScoreValueMineralsUsedCurrentArmy = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueVespeneUsedInProgressArmy":
                    ScoreValueVespeneUsedInProgressArmy = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueVespeneCollectionRate":
                    ScoreValueVespeneCollectionRate = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueMineralsUsedInProgressTechnology":
                    ScoreValueMineralsUsedInProgressTechnology = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueMineralsCollectionRate":
                    ScoreValueMineralsCollectionRate = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueWorkersActiveCount":
                    ScoreValueWorkersActiveCount = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueMineralsUsedInProgressArmy":
                    ScoreValueMineralsUsedInProgressArmy = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueVespeneLostArmy":
                    ScoreValueVespeneLostArmy = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueMineralsKilledEconomy":
                    ScoreValueMineralsKilledEconomy = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueMineralsUsedCurrentTechnology":
                    ScoreValueMineralsUsedCurrentTechnology = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueMineralsKilledArmy":
                    ScoreValueMineralsKilledArmy = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueMineralsLostEconomy":
                    ScoreValueMineralsLostEconomy = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueMineralsCurrent":
                    ScoreValueMineralsCurrent = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueMineralsLostArmy":
                    ScoreValueMineralsLostArmy = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueVespeneKilledArmy":
                    ScoreValueVespeneKilledArmy = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueVespeneKilledTechnology":
                    ScoreValueVespeneKilledTechnology = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueVespeneKilledEconomy":
                    ScoreValueVespeneKilledEconomy = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueMineralsUsedActiveForces":
                    ScoreValueMineralsUsedActiveForces = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueVespeneUsedCurrentArmy":
                    ScoreValueVespeneUsedCurrentArmy = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueMineralsFriendlyFireArmy":
                    ScoreValueMineralsFriendlyFireArmy = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueVespeneUsedActiveForces":
                    ScoreValueVespeneUsedActiveForces = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueVespeneCurrent":
                    ScoreValueVespeneCurrent = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueMineralsLostTechnology":
                    ScoreValueMineralsLostTechnology = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueMineralsUsedInProgressEconomy":
                    ScoreValueMineralsUsedInProgressEconomy = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueMineralsFriendlyFireEconomy":
                    ScoreValueMineralsFriendlyFireEconomy = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueVespeneUsedInProgressTechnology":
                    ScoreValueVespeneUsedInProgressTechnology = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueFoodMade":
                    ScoreValueFoodMade = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueMineralsKilledTechnology":
                    ScoreValueMineralsKilledTechnology = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueVespeneLostTechnology":
                    ScoreValueVespeneLostTechnology = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueVespeneFriendlyFireEconomy":
                    ScoreValueVespeneFriendlyFireEconomy = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueVespeneUsedInProgressEconomy":
                    ScoreValueVespeneUsedInProgressEconomy = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueVespeneUsedCurrentEconomy":
                    ScoreValueVespeneUsedCurrentEconomy = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueVespeneFriendlyFireTechnology":
                    ScoreValueVespeneFriendlyFireTechnology = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_scoreValueFoodUsed":
                    ScoreValueFoodUsed = decoder.ReadInt(fieldTypeId);
                    return true;
                default:
                    return false;
            }
        }
    }

    private static SUnitBornEvent ReadSUnitBornEvent(TypedProtocolDecoder decoder, int typeId, int eventId, int bits, int gameloop)
    {
        SUnitBornEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new SUnitBornEvent(state.PlayerId, eventId, bits, gameloop, state.UnitTagIndex,
            state.UnitTagRecycle, state.CreatorAbilityName, state.CreatorUnitTagRecycle, state.ControlPlayerId,
            state.X, state.Y, state.UpkeepPlayerId, state.UnitTypeName ?? string.Empty, state.CreatorUnitTagIndex);
    }

    private struct SUnitBornEventReadState : IStructFieldReader
    {
        public int PlayerId;
        public int UnitTagIndex;
        public int UnitTagRecycle;
        public string? CreatorAbilityName;
        public int? CreatorUnitTagRecycle;
        public int ControlPlayerId;
        public int X;
        public int Y;
        public int UpkeepPlayerId;
        public string? UnitTypeName;
        public int? CreatorUnitTagIndex;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_playerId":
                    PlayerId = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_unitTagIndex":
                    UnitTagIndex = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_unitTagRecycle":
                    UnitTagRecycle = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_creatorAbilityName":
                    CreatorAbilityName = decoder.ReadNullableString(fieldTypeId);
                    return true;
                case "m_creatorUnitTagRecycle":
                    CreatorUnitTagRecycle = decoder.ReadNullableInt(fieldTypeId);
                    return true;
                case "m_controlPlayerId":
                    ControlPlayerId = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_x":
                    X = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_y":
                    Y = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_upkeepPlayerId":
                    UpkeepPlayerId = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_unitTypeName":
                    UnitTypeName = decoder.ReadString(fieldTypeId);
                    return true;
                case "m_creatorUnitTagIndex":
                    CreatorUnitTagIndex = decoder.ReadNullableInt(fieldTypeId);
                    return true;
                default:
                    return false;
            }
        }
    }

    private static SUnitDiedEvent ReadSUnitDiedEvent(TypedProtocolDecoder decoder, int typeId, int eventId, int bits, int gameloop)
    {
        SUnitDiedEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new SUnitDiedEvent(state.PlayerId, eventId, bits, gameloop, state.UnitTagIndex,
            state.UnitTagRecycle, state.KillerPlayerId, state.X, state.Y, state.KillerUnitTagRecycle,
            state.KillerUnitTagIndex);
    }

    private struct SUnitDiedEventReadState : IStructFieldReader
    {
        public int PlayerId;
        public int UnitTagIndex;
        public int UnitTagRecycle;
        public int? KillerPlayerId;
        public int X;
        public int Y;
        public int? KillerUnitTagRecycle;
        public int? KillerUnitTagIndex;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_playerId":
                    PlayerId = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_unitTagIndex":
                    UnitTagIndex = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_unitTagRecycle":
                    UnitTagRecycle = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_killerPlayerId":
                    KillerPlayerId = decoder.ReadNullableInt(fieldTypeId);
                    return true;
                case "m_x":
                    X = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_y":
                    Y = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_killerUnitTagRecycle":
                    KillerUnitTagRecycle = decoder.ReadNullableInt(fieldTypeId);
                    return true;
                case "m_killerUnitTagIndex":
                    KillerUnitTagIndex = decoder.ReadNullableInt(fieldTypeId);
                    return true;
                default:
                    return false;
            }
        }
    }

    private static SUnitOwnerChangeEvent ReadSUnitOwnerChangeEvent(TypedProtocolDecoder decoder, int typeId, int eventId, int bits, int gameloop)
    {
        SUnitOwnerChangeEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new SUnitOwnerChangeEvent(state.PlayerId, eventId, bits, gameloop, state.UnitTagIndex,
            state.UnitTagRecycle, state.ControlPlayerId, state.UpkeepPlayerId);
    }

    private struct SUnitOwnerChangeEventReadState : IStructFieldReader
    {
        public int PlayerId;
        public int UnitTagIndex;
        public int UnitTagRecycle;
        public int ControlPlayerId;
        public int UpkeepPlayerId;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_playerId":
                    PlayerId = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_unitTagIndex":
                    UnitTagIndex = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_unitTagRecycle":
                    UnitTagRecycle = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_controlPlayerId":
                    ControlPlayerId = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_upkeepPlayerId":
                    UpkeepPlayerId = decoder.ReadInt(fieldTypeId);
                    return true;
                default:
                    return false;
            }
        }
    }

    private static SUnitPositionsEvent ReadSUnitPositionsEvent(TypedProtocolDecoder decoder, int typeId, int eventId, int bits, int gameloop)
    {
        SUnitPositionsEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new SUnitPositionsEvent(state.PlayerId, eventId, bits, gameloop, state.FirstUnitIndex,
            state.Items is null ? [] : [.. state.Items]);
    }

    private struct SUnitPositionsEventReadState : IStructFieldReader
    {
        public int PlayerId;
        public int FirstUnitIndex;
        public List<int>? Items;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_playerId":
                    PlayerId = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_firstUnitIndex":
                    FirstUnitIndex = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_items":
                    Items = decoder.ReadIntList(fieldTypeId);
                    return true;
                default:
                    return false;
            }
        }
    }

    private static SUnitTypeChangeEvent ReadSUnitTypeChangeEvent(TypedProtocolDecoder decoder, int typeId, int eventId, int bits, int gameloop)
    {
        SUnitTypeChangeEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new SUnitTypeChangeEvent(state.PlayerId, eventId, bits, gameloop, state.UnitTagIndex,
            state.UnitTagRecycle, state.UnitTypeName ?? string.Empty);
    }

    private struct SUnitTypeChangeEventReadState : IStructFieldReader
    {
        public int PlayerId;
        public int UnitTagIndex;
        public int UnitTagRecycle;
        public string? UnitTypeName;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_playerId":
                    PlayerId = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_unitTagIndex":
                    UnitTagIndex = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_unitTagRecycle":
                    UnitTagRecycle = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_unitTypeName":
                    UnitTypeName = decoder.ReadString(fieldTypeId);
                    return true;
                default:
                    return false;
            }
        }
    }

    private static SUpgradeEvent ReadSUpgradeEvent(TypedProtocolDecoder decoder, int typeId, int eventId, int bits, int gameloop)
    {
        SUpgradeEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new SUpgradeEvent(state.PlayerId, eventId, bits, gameloop, state.Count, state.UpgradeTypeName ?? string.Empty);
    }

    private struct SUpgradeEventReadState : IStructFieldReader
    {
        public int PlayerId;
        public int Count;
        public string? UpgradeTypeName;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_playerId":
                    PlayerId = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_count":
                    Count = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_upgradeTypeName":
                    UpgradeTypeName = decoder.ReadString(fieldTypeId);
                    return true;
                default:
                    return false;
            }
        }
    }

    private static SUnitInitEvent ReadSUnitInitEvent(TypedProtocolDecoder decoder, int typeId, int eventId, int bits, int gameloop)
    {
        SUnitInitEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new SUnitInitEvent(state.PlayerId, eventId, bits, gameloop, state.UnitTagIndex,
            state.UnitTagRecycle, state.ControlPlayerId, state.X, state.Y, state.UpkeepPlayerId,
            state.UnitTypeName ?? string.Empty);
    }

    private struct SUnitInitEventReadState : IStructFieldReader
    {
        public int PlayerId;
        public int UnitTagIndex;
        public int UnitTagRecycle;
        public int ControlPlayerId;
        public int X;
        public int Y;
        public int UpkeepPlayerId;
        public string? UnitTypeName;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_playerId":
                    PlayerId = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_unitTagIndex":
                    UnitTagIndex = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_unitTagRecycle":
                    UnitTagRecycle = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_controlPlayerId":
                    ControlPlayerId = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_x":
                    X = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_y":
                    Y = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_upkeepPlayerId":
                    UpkeepPlayerId = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_unitTypeName":
                    UnitTypeName = decoder.ReadString(fieldTypeId);
                    return true;
                default:
                    return false;
            }
        }
    }

    private static SUnitDoneEvent ReadSUnitDoneEvent(TypedProtocolDecoder decoder, int typeId, int eventId, int bits, int gameloop)
    {
        SUnitDoneEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new SUnitDoneEvent(state.PlayerId, eventId, bits, gameloop, state.UnitTagIndex, state.UnitTagRecycle);
    }

    private struct SUnitDoneEventReadState : IStructFieldReader
    {
        public int PlayerId;
        public int UnitTagIndex;
        public int UnitTagRecycle;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_playerId":
                    PlayerId = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_unitTagIndex":
                    UnitTagIndex = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_unitTagRecycle":
                    UnitTagRecycle = decoder.ReadInt(fieldTypeId);
                    return true;
                default:
                    return false;
            }
        }
    }
}

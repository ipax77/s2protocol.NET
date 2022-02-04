﻿using IronPython.Runtime;
using s2protocol.NET.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s2protocol.NET.Parser;
internal partial class Parse
{
    internal static Initdata? InitData(dynamic pydic)
    {
        if (pydic.ContainsKey("m_syncLobbyState"))
        {
            PythonDictionary? initDic = pydic["m_syncLobbyState"] as PythonDictionary;
            if (initDic != null)
            {
                List<UserInitialData> userInitialDatas = GetUserInitialData(initDic);
                LobbyState lobbyState = GetLobbyState(initDic);
                GameDescription gameDescription = GetGameDescription(initDic);
                return new Initdata(userInitialDatas, lobbyState, gameDescription);
            }
        }
        return null;
    }

    private static GameDescription GetGameDescription(PythonDictionary pydic)
    {
        if (pydic.ContainsKey("m_gameDescription"))
        {
            PythonDictionary? descDic = pydic["m_gameDescription"] as PythonDictionary;
            if (descDic != null)
            {
                int maxRaces = GetInt(descDic, "m_maxRaces");
                int maxTeams = GetInt(descDic, "m_maxTeams");
                bool hasExtensionMod = GetBool(descDic, "m_hasExtensionMod");
                int maxColors = GetInt(descDic, "m_maxColors");
                bool isBlizzardMap = GetBool(descDic, "m_isBlizzardMap");
                GameOptions gameOptions = GetGameOptions(descDic);
                int defaultDifficulty = GetInt(descDic, "m_defaultDifficulty");
                bool isCoopMode = GetBool(descDic, "m_isCoopMode");
                string mapFileName = GetString(descDic, "m_mapFileName");
                int defaultAIBuild = GetInt(descDic, "m_defaultAIBuild");
                int gameType = GetInt(descDic, "m_gameType");
                bool hasNonBlizzardExtensionMod = GetBool(descDic, "m_hasNonBlizzardExtensionMod");
                long randomValue = GetBigInt(descDic, "m_randomValue");
                int maxObservers = GetInt(descDic, "m_maxObservers");
                bool isRealtimeMode = GetBool(descDic, "m_isRealtimeMode");
                int maxUsers = GetInt(descDic, "m_maxUsers");
                long modFileSyncChecksum = GetBigInt(descDic, "m_modFileSyncChecksum");
                long mapFileSyncChecksum = GetBigInt(descDic, "m_mapFileSyncChecksum");
                int maxPlayers = GetInt(descDic, "m_maxPlayers");
                List<string> cacheHandles = GetStringList(descDic, "m_cacheHandles");
                int gameSpeed = GetInt(descDic, "m_gameSpeed");
                int maxControls = GetInt(descDic, "m_maxControls");
                string gameCacheName = GetString(descDic, "m_gameCacheName");
                string mapAuthorName = GetString(descDic, "m_mapAuthorName");
                List<SlotDescription> slotDescriptions = GetSlotDescriptions(descDic);
                int mapSizeY = GetInt(descDic, "m_mapSizeY");
                int mapSizeX = GetInt(descDic, "m_mapSizeX");
                bool isPremadeFFA = GetBool(descDic, "m_isPremadeFFA");
                return new GameDescription(
                           maxRaces,
                           maxTeams,
                           hasExtensionMod,
                           maxColors,
                           isBlizzardMap,
                           gameOptions,
                           defaultDifficulty,
                           isCoopMode,
                           mapFileName,
                           defaultAIBuild,
                           gameType,
                           hasNonBlizzardExtensionMod,
                           randomValue,
                           maxObservers,
                           isRealtimeMode,
                           maxUsers,
                           modFileSyncChecksum,
                           mapFileSyncChecksum,
                           maxPlayers,
                           cacheHandles,
                           gameSpeed,
                           maxControls,
                           gameCacheName,
                           mapAuthorName,
                           slotDescriptions,
                           mapSizeY,
                           mapSizeX,
                           isPremadeFFA
                    );
            }
        }
        return new GameDescription(0, 0, false, 0, false, GetGameOptions(pydic), 0, false, "", 0, 0, false, 0, 0, false, 0, 0, 0, 0, new List<string>(), 0, 0, "", "", new List<SlotDescription>(), 0, 0, false);
    }

    private static GameOptions GetGameOptions(PythonDictionary pydic)
    {
        if (pydic.ContainsKey("m_gameOptions"))
        {
            PythonDictionary? optDic = pydic["m_gameOptions"] as PythonDictionary;
            if (optDic != null)
            {
                bool competitive = GetBool(optDic, "m_competitive");
                bool practice = GetBool(optDic, "m_practice");
                bool lockTeams = GetBool(optDic, "m_lockTeams");
                bool amm = GetBool(optDic, "m_amm");
                bool battleNet = GetBool(optDic, "m_battleNet");
                int fog = GetInt(optDic, "m_fog");
                bool noVictoryOrDefeat = GetBool(optDic, "m_noVictoryOrDefeat");
                bool heroDuplicatesAllowed = GetBool(optDic, "m_heroDuplicatesAllowed");
                int userDifficulty = GetInt(optDic, "m_userDifficulty");
                bool advancedSharedControl = GetBool(optDic, "m_advancedSharedControl");
                bool cooperative = GetBool(optDic, "m_cooperative");
                long clientDebugFlags = GetBigInt(optDic, "m_clientDebugFlags");
                int observers = GetInt(optDic, "m_observers");
                bool teamsTogether = GetBool(optDic, "m_teamsTogether");
                bool randomRaces = GetBool(optDic, "m_randomRaces");
                bool buildCoachEnabled = GetBool(optDic, "m_buildCoachEnabled");
                return new GameOptions(
                    competitive,
                    practice,
                    lockTeams,
                    amm,
                    battleNet,
                    fog,
                    noVictoryOrDefeat,
                    heroDuplicatesAllowed,
                    userDifficulty,
                    advancedSharedControl,
                    cooperative,
                    clientDebugFlags,
                    observers,
                    teamsTogether,
                    randomRaces,
                    buildCoachEnabled
                );
            }
        }
        return new GameOptions(false, false, false, false, false, 0, false, false, 0, false, false, 0, 0, false, false, false);
    }

    private static List<SlotDescription> GetSlotDescriptions(PythonDictionary pydic)
    {
        List<SlotDescription> slotDescscitions = new List<SlotDescription>();
        if (pydic.ContainsKey("m_slotDescriptions"))
        {
            List? slotDescs = pydic["m_slotDescriptions"] as List;
            if (slotDescs != null)
            {
                foreach (var slotDesc in slotDescs)
                {
                    PythonDictionary? slotDic = slotDesc as PythonDictionary;
                    if (slotDic != null)
                    {
                        var allowedRaces = GetIntBigTuple(slotDic, "m_allowedRaces");
                        var allowedColors = GetIntBigTuple(slotDic, "m_allowedColors");
                        var allowedAIBuilds = GetIntBigTuple(slotDic, "m_allowedAIBuilds");
                        var allowedDifficulty = GetIntBigTuple(slotDic, "m_allowedDifficulty");
                        var allowedObserveTypes = GetIntBigTuple(slotDic, "m_allowedObserveTypes");
                        var allowedControls = GetIntBigTuple(slotDic, "m_allowedControls");
                        slotDescscitions.Add(new SlotDescription(
                            allowedRaces,
                            allowedColors,
                            allowedAIBuilds,
                            allowedDifficulty,
                            allowedObserveTypes,
                            allowedControls
                        ));
                    }
                }
            }
        }
        return slotDescscitions;
    }

    private static LobbyState GetLobbyState(PythonDictionary pydic)
    {
        if (pydic.ContainsKey("m_lobbyState"))
        {
            PythonDictionary? lobbyDic = pydic["m_lobbyState"] as PythonDictionary;
            if (lobbyDic != null)
            {
                int maxUser = GetInt(lobbyDic, "m_maxUsers");
                List<Slot> slots = GetSlots(lobbyDic);
                int defaultDifficulty = GetInt(lobbyDic, "m_defaultDifficulty");
                bool isSinglePlayer = GetBool(lobbyDic, "m_isSinglePlayer");
                int phase = GetInt(lobbyDic, "m_phase");
                int? hostUserId = GetNullableInt(lobbyDic, "m_hostUserId");
                int maxObs = GetInt(lobbyDic, "m_maxObservers");
                int defaultAIBuild = GetInt(lobbyDic, "m_defaultAIBuild");
                int pickedMapTag = GetInt(lobbyDic, "m_pickedMapTag");
                long randomSeed = GetBigInt(lobbyDic, "m_randomSeed");
                int gameDuration = GetInt(lobbyDic, "m_gameDuration");
                return new LobbyState(maxUser,
                                      slots,
                                      defaultDifficulty,
                                      isSinglePlayer,
                                      phase,
                                      hostUserId,
                                      maxObs,
                                      defaultAIBuild,
                                      pickedMapTag,
                                      randomSeed,
                                      gameDuration);
            }
        }
        return new LobbyState(0, new List<Slot>(), 0, false, 0, 0, 0, 0, 0, 0, 0);
    }

    private static List<Slot> GetSlots(PythonDictionary pydic)
    {
        List<Slot> slots = new List<Slot>();
        if (pydic.ContainsKey("m_slots"))
        {
            var slotList = pydic["m_slots"] as List;
            if (slotList != null)
            {
                foreach (var item in slotList)
                {
                    PythonDictionary? slotDic = item as PythonDictionary;
                    if (slotDic != null)
                    {
                        int aCEnemyRace = GetInt(slotDic, "m_aCEnemyRace");
                        string toonHandle = GetString(slotDic, "m_toonHandle");
                        List<int> rewardOverrides = GetIntList(slotDic, "m_rewardOverrides");
                        int? userId = GetNullableInt(slotDic, "m_userId");
                        string skin = GetString(slotDic, "m_skin");
                        List<int> commanderMasteryTalents = GetIntList(slotDic, "m_commanderMasteryTalents");
                        int aiBuild = GetInt(slotDic, "m_aiBuild");
                        int teamId = GetInt(slotDic, "m_teamId");
                        List<int> rewards = GetIntList(slotDic, "m_rewards");
                        int commanderLevel = GetInt(slotDic, "m_commanderLevel");
                        int logoIndex = GetInt(slotDic, "m_logoIndex");
                        List<string> artifacts = GetStringList(slotDic, "m_artifacts");
                        int difficulty = GetInt(slotDic, "m_difficulty");
                        int? tandemLeaderId = GetNullableInt(slotDic, "m_tandemLeaderId");
                        int commanderMasteryLevel = GetInt(slotDic, "m_commanderMasteryLevel");
                        int trophyId = GetInt(slotDic, "m_trophyId");
                        int brutalPlusDifficulty = GetInt(slotDic, "m_brutalPlusDifficulty");
                        int? racePref = GetRacePreference(slotDic, "m_racePref");
                        int? tandemId = GetNullableInt(slotDic, "m_tandemId");
                        string hero = GetString(slotDic, "m_hero");
                        string commander = GetString(slotDic, "m_commander");
                        string mount = GetString(slotDic, "m_mount");
                        int handicap = GetInt(slotDic, "m_handicap");
                        int observe = GetInt(slotDic, "m_observe");
                        int aCEnemyWaveType = GetInt(slotDic, "m_aCEnemyWaveType");
                        int control = GetInt(slotDic, "m_control");
                        List<int> licenses = GetIntList(slotDic, "m_licenses");
                        int? colorPref = GetColorPreference(slotDic);
                        bool hasSilencePenalty = GetBool(slotDic, "m_hasSilencePenalty");
                        int workingSetSlotId = GetInt(slotDic, "m_workingSetSlotId");
                        List<int> retryMutationIndexes = GetIntList(slotDic, "m_retryMutationIndexes");
                        int selectedCommanderPrestige = GetInt(slotDic, "m_selectedCommanderPrestige");
                        slots.Add(new Slot(aCEnemyRace,
                                            toonHandle,
                                            rewardOverrides,
                                            userId,
                                            skin,
                                            commanderMasteryTalents,
                                            aiBuild,
                                            teamId,
                                            rewards,
                                            commanderLevel,
                                            logoIndex,
                                            artifacts,
                                            difficulty,
                                            tandemLeaderId,
                                            commanderMasteryLevel,
                                            trophyId,
                                            brutalPlusDifficulty,
                                            racePref,
                                            tandemId,
                                            hero,
                                            commander,
                                            mount,
                                            handicap,
                                            observe,
                                            aCEnemyWaveType,
                                            control,
                                            licenses,
                                            colorPref,
                                            hasSilencePenalty,
                                            workingSetSlotId,
                                            retryMutationIndexes,
                                            selectedCommanderPrestige));
                    }
                }
            }
        }
        return slots;
    }

    private static List<UserInitialData> GetUserInitialData(PythonDictionary pydic)
    {
        List<UserInitialData> initDatas = new List<UserInitialData>();
        if (pydic.ContainsKey("m_userInitialData"))
        {
            List? userInitalDatas = pydic["m_userInitialData"] as List;

            if (userInitalDatas != null)
            {
                foreach (var userInitalData in userInitalDatas)
                {
                    PythonDictionary? initDic = userInitalData as PythonDictionary;
                    if (initDic != null)
                    {
                        string mount = GetString(initDic, "m_mount");
                        string skin = GetString(initDic, "m_skin");
                        int observe = GetInt(initDic, "m_observe");
                        int? teamPref = GetTeamPreference(initDic);
                        string toonHandle = GetString(initDic, "m_toonHandle");
                        long combinedRaceLevels = GetBigInt(initDic, "m_combinedRaceLevels");
                        int highestLeague = GetInt(initDic, "m_highestLeague");
                        string clanTag = GetString(initDic, "m_clanTag");
                        bool testMap = GetBool(initDic, "m_testMap");
                        bool testAuto = GetBool(initDic, "m_testAuto");
                        bool examine = GetBool(initDic, "m_examine");
                        int testType = GetInt(initDic, "m_testType");
                        bool customInterface = GetBool(initDic, "m_customInterface");
                        string clanLogo = GetString(initDic, "m_clanLogo");
                        string name = GetString(initDic, "m_name");
                        int? racePreference = GetRacePreference(initDic);
                        int randomSeed = GetInt(initDic, "m_randomSeed");
                        string hero = GetString(initDic, "m_hero");
                        long? scaledRating = GetNullableBigInt(initDic, "m_scaledRating");
                        UserInitialData initData = new UserInitialData(mount,
                                                                       skin,
                                                                       observe,
                                                                       teamPref,
                                                                       toonHandle,
                                                                       combinedRaceLevels,
                                                                       highestLeague,
                                                                       clanTag,
                                                                       testMap,
                                                                       testAuto,
                                                                       examine,
                                                                       testType,
                                                                       customInterface,
                                                                       clanLogo,
                                                                       name,
                                                                       racePreference,
                                                                       randomSeed,
                                                                       hero,
                                                                       scaledRating);
                        initDatas.Add(initData);
                    }
                }
            }
        }
        return initDatas;
    }

    private static int? GetTeamPreference(PythonDictionary pydic)
    {
        if (pydic.ContainsKey("m_teamPreference"))
        {
            PythonDictionary? teamDic = pydic["m_teamPreference"] as PythonDictionary;
            if (teamDic != null)
            {
                return GetNullableInt(teamDic, "m_team");
            }
        }
        return null;
    }

    private static int? GetRacePreference(PythonDictionary pydic, string property = "m_racePreference")
    {
        if (pydic.ContainsKey(property))
        {
            PythonDictionary? raceDic = pydic[property] as PythonDictionary;
            if (raceDic != null)
            {
                return GetNullableInt(raceDic, "m_race");
            }
        }
        return null;
    }
    private static int? GetColorPreference(PythonDictionary pydic)
    {
        if (pydic.ContainsKey("m_colorPref"))
        {
            PythonDictionary? colDic = pydic["m_colorPref"] as PythonDictionary;
            if (colDic != null)
            {
                return GetNullableInt(colDic, "m_color");
            }
        }
        return null;
    }
}
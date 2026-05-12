using s2protocol.NET.Models;
using System.Numerics;
using System.Text;

namespace s2protocol.NET.S2Protocol;

public sealed partial record S2ProtocolVersion
{
    public Header DecodeReplayHeader(byte[] content) => DecodeReplayHeader((ReadOnlyMemory<byte>)content);

    public Header DecodeReplayHeader(ReadOnlyMemory<byte> content)
    {
        VersionedTypedDecoder decoder = new(content, TypeInfos);

        bool useScaledTime = false;
        string signature = string.Empty;
        Version version = new();
        int flags = 0;
        int build = 0;
        int baseBuild = 0;
        int elapsed = 0;
        int dataBuild = 0;
        int type = 0;

        decoder.ReadStruct(ReplayHeaderTypeId ?? 18, (name, typeId) =>
        {
            switch (name)
            {
                case "m_signature":
                    signature = decoder.ReadString(typeId);
                    return true;
                case "m_version":
                    decoder.ReadStruct(typeId, (versionName, versionTypeId) =>
                    {
                        switch (versionName)
                        {
                            case "m_flags":
                                flags = decoder.ReadInt(versionTypeId);
                                return true;
                            case "m_major":
                                int major = decoder.ReadInt(versionTypeId);
                                version = new Version(major, version.Minor < 0 ? 0 : version.Minor, version.Build < 0 ? 0 : version.Build);
                                return true;
                            case "m_minor":
                                int minor = decoder.ReadInt(versionTypeId);
                                version = new Version(version.Major < 0 ? 0 : version.Major, minor, version.Build < 0 ? 0 : version.Build);
                                return true;
                            case "m_revision":
                                int revision = decoder.ReadInt(versionTypeId);
                                version = new Version(version.Major < 0 ? 0 : version.Major, version.Minor < 0 ? 0 : version.Minor, revision);
                                return true;
                            case "m_build":
                                build = decoder.ReadInt(versionTypeId);
                                return true;
                            case "m_baseBuild":
                                baseBuild = decoder.ReadInt(versionTypeId);
                                return true;
                            default:
                                return false;
                        }
                    });
                    return true;
                case "m_type":
                    type = decoder.ReadInt(typeId);
                    return true;
                case "m_elapsedGameLoops":
                    elapsed = decoder.ReadInt(typeId);
                    return true;
                case "m_useScaledTime":
                    useScaledTime = decoder.ReadBool(typeId);
                    return true;
                case "m_dataBuildNum":
                    dataBuild = decoder.ReadInt(typeId);
                    return true;
                default:
                    return false;
            }
        });

        return new Header(dataBuild, elapsed, useScaledTime, version, signature, string.Empty, string.Empty, type, flags, build, baseBuild);
    }

    public Details DecodeReplayDetails(byte[] content) => DecodeReplayDetails((ReadOnlyMemory<byte>)content);

    public Details DecodeReplayDetails(ReadOnlyMemory<byte> content)
    {
        VersionedTypedDecoder decoder = new(content, TypeInfos);

        int campaignIndex = 0;
        int defaultDifficulty = 0;
        string description = string.Empty;
        string difficulty = string.Empty;
        bool disableRecoverGame = false;
        int gameSpeed = 0;
        string imageFilePath = string.Empty;
        bool isBlizzardMap = false;
        string mapFileName = string.Empty;
        bool miniSave = false;
        bool restartAsTransitionMap = false;
        long timeLocalOffset = 0;
        long timeUTC = 0;
        string title = string.Empty;
        List<DetailsPlayer> players = [];

        decoder.ReadStruct(GameDetailsTypeId ?? 40, (name, typeId) =>
        {
            switch (name)
            {
                case "m_campaignIndex":
                    campaignIndex = decoder.ReadInt(typeId);
                    return true;
                case "m_defaultDifficulty":
                    defaultDifficulty = decoder.ReadInt(typeId);
                    return true;
                case "m_description":
                    description = decoder.ReadString(typeId);
                    return true;
                case "m_difficulty":
                    difficulty = decoder.ReadString(typeId);
                    return true;
                case "m_disableRecoverGame":
                    disableRecoverGame = decoder.ReadBool(typeId);
                    return true;
                case "m_gameSpeed":
                    gameSpeed = decoder.ReadInt(typeId);
                    return true;
                case "m_imageFilePath":
                    imageFilePath = decoder.ReadString(typeId);
                    return true;
                case "m_isBlizzardMap":
                    isBlizzardMap = decoder.ReadBool(typeId);
                    return true;
                case "m_mapFileName":
                    mapFileName = decoder.ReadString(typeId);
                    return true;
                case "m_miniSave":
                    miniSave = decoder.ReadBool(typeId);
                    return true;
                case "m_restartAsTransitionMap":
                    restartAsTransitionMap = decoder.ReadBool(typeId);
                    return true;
                case "m_timeLocalOffset":
                    timeLocalOffset = decoder.ReadLong(typeId);
                    return true;
                case "m_timeUTC":
                    timeUTC = decoder.ReadLong(typeId);
                    return true;
                case "m_title":
                    title = decoder.ReadString(typeId);
                    return true;
                case "m_playerList":
                    decoder.ReadArray(typeId, itemTypeId =>
                    {
                        players.Add(ReadDetailsPlayer(decoder, itemTypeId));
                        return true;
                    });
                    return true;
                default:
                    return false;
            }
        });

        return new Details(campaignIndex, defaultDifficulty, description, difficulty, disableRecoverGame, gameSpeed,
            imageFilePath, isBlizzardMap, mapFileName, miniSave, restartAsTransitionMap, timeLocalOffset, timeUTC,
            title, players);
    }

    public Initdata? DecodeReplayInitData(byte[] content) => DecodeReplayInitData((ReadOnlyMemory<byte>)content);

    public Initdata? DecodeReplayInitData(ReadOnlyMemory<byte> content)
    {
        BitPackedTypedDecoder decoder = new(content, TypeInfos);
        Initdata? initdata = null;

        decoder.ReadStruct(ReplayInitDataTypeId ?? 73, (name, typeId) =>
        {
            if (name != "m_syncLobbyState")
            {
                return false;
            }

            List<UserInitialData> userInitialData = [];
            LobbyState lobbyState = new(0, [], 0, false, 0, 0, 0, 0, 0, 0, 0);
            GameDescription gameDescription = EmptyGameDescription();

            decoder.ReadStruct(typeId, (syncName, syncTypeId) =>
            {
                switch (syncName)
                {
                    case "m_userInitialData":
                        decoder.ReadArray(syncTypeId, itemTypeId =>
                        {
                            userInitialData.Add(ReadUserInitialData(decoder, itemTypeId));
                            return true;
                        });
                        return true;
                    case "m_lobbyState":
                        lobbyState = ReadLobbyState(decoder, syncTypeId);
                        return true;
                    case "m_gameDescription":
                        gameDescription = ReadGameDescription(decoder, syncTypeId);
                        return true;
                    default:
                        return false;
                }
            });

            initdata = new Initdata(userInitialData, lobbyState, gameDescription);
            return true;
        });

        return initdata;
    }

    public MessageEvents DecodeReplayMessageEvents(byte[] content) => DecodeReplayMessageEvents((ReadOnlyMemory<byte>)content);

    public MessageEvents DecodeReplayMessageEvents(ReadOnlyMemory<byte> content)
    {
        BitPackedTypedDecoder decoder = new(content, TypeInfos);
        List<ChatMessageEvent> chatMessages = [];
        List<PingMessageEvent> pingMessages = [];

        DecodeEventStream(decoder, MessageEventIdTypeId ?? 1, MessageEvents, decodeUserId: true,
            (eventId, eventName, bits, gameloop, userId, typeId) =>
            {
                if (eventName == "NNet.Game.SChatMessage")
                {
                    int recipient = 0;
                    string message = string.Empty;
                    decoder.ReadStruct(typeId, (name, fieldTypeId) =>
                    {
                        switch (name)
                        {
                            case "m_recipient":
                                recipient = decoder.ReadInt(fieldTypeId);
                                return true;
                            case "m_string":
                                message = decoder.ReadString(fieldTypeId);
                                return true;
                            default:
                                return false;
                        }
                    });
                    chatMessages.Add(new ChatMessageEvent(recipient, userId, message, gameloop));
                }
                else if (eventName == "NNet.Game.SPingMessage")
                {
                    int recipient = 0;
                    long x = 0;
                    long y = 0;
                    decoder.ReadStruct(typeId, (name, fieldTypeId) =>
                    {
                        switch (name)
                        {
                            case "m_recipient":
                                recipient = decoder.ReadInt(fieldTypeId);
                                return true;
                            case "m_point":
                                decoder.ReadStruct(fieldTypeId, (pointName, pointTypeId) =>
                                {
                                    if (pointName == "x")
                                    {
                                        x = decoder.ReadLong(pointTypeId);
                                        return true;
                                    }

                                    if (pointName == "y")
                                    {
                                        y = decoder.ReadLong(pointTypeId);
                                        return true;
                                    }

                                    return false;
                                });
                                return true;
                            default:
                                return false;
                        }
                    });
                    pingMessages.Add(new PingMessageEvent(recipient, userId, gameloop, x, y));
                }
                else
                {
                    decoder.SkipType(typeId);
                }
            });

        return new MessageEvents(chatMessages, pingMessages);
    }

    public GameEvents DecodeReplayGameEvents(byte[] content) => DecodeReplayGameEvents((ReadOnlyMemory<byte>)content);

    public GameEvents DecodeReplayGameEvents(ReadOnlyMemory<byte> content)
    {
        BitPackedTypedDecoder decoder = new(content, TypeInfos);
        List<GameEvent> events = [];

        DecodeEventStream(decoder, GameEventIdTypeId ?? 0, GameEvents, decodeUserId: true,
            (eventId, eventName, bits, gameloop, userId, typeId) =>
            {
                GameEventType eventType = GetGameEventType(eventName);
                GameEvent gameEvent = eventType switch
                {
                    GameEventType.SUserFinishedLoadingSyncEvent => ReadEmptyGameEvent(decoder, typeId, userId, eventId, bits, gameloop),
                    GameEventType.STriggerSoundLengthSyncEvent => ReadSoundLengthSyncEvent(decoder, typeId, userId, eventId, bits, gameloop),
                    _ => ReadGenericKnownGameEvent(decoder, typeId, userId, eventId, bits, gameloop, eventName, eventType),
                };
                events.Add(gameEvent);
            });

        return new GameEvents(events);
    }

    public TrackerEvents DecodeReplayTrackerEvents(byte[] content) => DecodeReplayTrackerEvents((ReadOnlyMemory<byte>)content);

    public TrackerEvents DecodeReplayTrackerEvents(ReadOnlyMemory<byte> content)
    {
        VersionedTypedDecoder decoder = new(content, TypeInfos);
        List<SPlayerSetupEvent> playerSetupEvents = [];
        List<SPlayerStatsEvent> playerStatsEvents = [];
        List<SUnitBornEvent> unitBornEvents = [];
        List<SUnitDiedEvent> unitDiedEvents = [];
        List<SUnitOwnerChangeEvent> unitOwnerChangeEvents = [];
        List<SUnitPositionsEvent> unitPositionsEvents = [];
        List<SUnitTypeChangeEvent> unitTypeChangeEvents = [];
        List<SUpgradeEvent> upgradeEvents = [];
        List<SUnitInitEvent> unitInitEvents = [];
        List<SUnitDoneEvent> unitDoneEvents = [];

        DecodeEventStream(decoder, TrackerEventIdTypeId ?? 2, TrackerEvents, decodeUserId: false,
            (eventId, eventName, bits, gameloop, _, typeId) =>
            {
                TrackerEventType eventType = GetTrackerEventType(eventName);
                TrackerEvent parsedEvent = ReadTrackerEvent(decoder, typeId, eventId, bits, gameloop, eventType);

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
            });

        return new TrackerEvents(
            [.. playerSetupEvents],
            [.. playerStatsEvents],
            [.. unitBornEvents],
            [.. unitDiedEvents],
            [.. unitOwnerChangeEvents],
            [.. unitPositionsEvents],
            [.. unitTypeChangeEvents],
            [.. upgradeEvents],
            [.. unitInitEvents],
            [.. unitDoneEvents]);
    }

    private static TrackerEvent ReadTrackerEvent(
        TypedProtocolDecoder decoder,
        int typeId,
        int eventId,
        int bits,
        int gameloop,
        TrackerEventType eventType)
    {
        int playerId = 0;
        int type = 0;
        int? userId = null;
        int slotId = 0;
        int unitTagIndex = 0;
        int unitTagRecycle = 0;
        int? killerPlayerId = null;
        int x = 0;
        int y = 0;
        int? killerUnitTagRecycle = null;
        int? killerUnitTagIndex = null;
        string? creatorAbilityName = null;
        int? creatorUnitTagRecycle = null;
        int controlPlayerId = 0;
        int upkeepPlayerId = 0;
        string unitTypeName = string.Empty;
        int? creatorUnitTagIndex = null;
        int firstUnitIndex = 0;
        List<int> items = [];
        int count = 0;
        string upgradeTypeName = string.Empty;
        int[] stats = new int[39];

        decoder.ReadStruct(typeId, (name, fieldTypeId) =>
        {
            switch (name)
            {
                case "m_playerId":
                    playerId = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_type":
                    type = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_userId":
                    userId = decoder.ReadNullableInt(fieldTypeId);
                    return true;
                case "m_slotId":
                    slotId = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_unitTagIndex":
                    unitTagIndex = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_unitTagRecycle":
                    unitTagRecycle = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_killerPlayerId":
                    killerPlayerId = decoder.ReadNullableInt(fieldTypeId);
                    return true;
                case "m_x":
                    x = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_y":
                    y = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_killerUnitTagRecycle":
                    killerUnitTagRecycle = decoder.ReadNullableInt(fieldTypeId);
                    return true;
                case "m_killerUnitTagIndex":
                    killerUnitTagIndex = decoder.ReadNullableInt(fieldTypeId);
                    return true;
                case "m_creatorAbilityName":
                    creatorAbilityName = decoder.ReadNullableString(fieldTypeId);
                    return true;
                case "m_creatorUnitTagRecycle":
                    creatorUnitTagRecycle = decoder.ReadNullableInt(fieldTypeId);
                    return true;
                case "m_controlPlayerId":
                    controlPlayerId = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_upkeepPlayerId":
                    upkeepPlayerId = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_unitTypeName":
                    unitTypeName = decoder.ReadString(fieldTypeId);
                    return true;
                case "m_creatorUnitTagIndex":
                    creatorUnitTagIndex = decoder.ReadNullableInt(fieldTypeId);
                    return true;
                case "m_firstUnitIndex":
                    firstUnitIndex = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_items":
                    items = decoder.ReadIntList(fieldTypeId);
                    return true;
                case "m_count":
                    count = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_upgradeTypeName":
                    upgradeTypeName = decoder.ReadString(fieldTypeId);
                    return true;
                case "m_stats":
                    stats = ReadPlayerStats(decoder, fieldTypeId);
                    return true;
                default:
                    return false;
            }
        });

        return eventType switch
        {
            TrackerEventType.SPlayerSetupEvent => new SPlayerSetupEvent(playerId, eventId, bits, gameloop, type, userId, slotId),
            TrackerEventType.SPlayerStatsEvent => new SPlayerStatsEvent(playerId, eventId, bits, gameloop,
                stats[0], stats[1], stats[2], stats[3], stats[4], stats[5], stats[6], stats[7], stats[8], stats[9],
                stats[10], stats[11], stats[12], stats[13], stats[14], stats[15], stats[16], stats[17], stats[18],
                stats[19], stats[20], stats[21], stats[22], stats[23], stats[24], stats[25], stats[26], stats[27],
                stats[28], stats[29], stats[30], stats[31], stats[32], stats[33], stats[34], stats[35], stats[36],
                stats[37], stats[38]),
            TrackerEventType.SUnitBornEvent => new SUnitBornEvent(playerId, eventId, bits, gameloop, unitTagIndex,
                unitTagRecycle, creatorAbilityName, creatorUnitTagRecycle, controlPlayerId, x, y, upkeepPlayerId,
                unitTypeName, creatorUnitTagIndex),
            TrackerEventType.SUnitDiedEvent => new SUnitDiedEvent(playerId, eventId, bits, gameloop, unitTagIndex,
                unitTagRecycle, killerPlayerId, x, y, killerUnitTagRecycle, killerUnitTagIndex),
            TrackerEventType.SUnitOwnerChangeEvent => new SUnitOwnerChangeEvent(playerId, eventId, bits, gameloop,
                unitTagIndex, unitTagRecycle, controlPlayerId, upkeepPlayerId),
            TrackerEventType.SUnitPositionsEvent => new SUnitPositionsEvent(playerId, eventId, bits, gameloop,
                firstUnitIndex, [.. items]),
            TrackerEventType.SUnitTypeChangeEvent => new SUnitTypeChangeEvent(playerId, eventId, bits, gameloop,
                unitTagIndex, unitTagRecycle, unitTypeName),
            TrackerEventType.SUpgradeEvent => new SUpgradeEvent(playerId, eventId, bits, gameloop, count, upgradeTypeName),
            TrackerEventType.SUnitInitEvent => new SUnitInitEvent(playerId, eventId, bits, gameloop, unitTagIndex,
                unitTagRecycle, controlPlayerId, x, y, upkeepPlayerId, unitTypeName),
            TrackerEventType.SUnitDoneEvent => new SUnitDoneEvent(playerId, eventId, bits, gameloop, unitTagIndex, unitTagRecycle),
            _ => new UnknownTrackerEvent(playerId, eventId, eventType, bits, gameloop),
        };
    }

    private static int[] ReadPlayerStats(TypedProtocolDecoder decoder, int typeId)
    {
        int[] stats = new int[39];
        decoder.ReadStruct(typeId, (name, fieldTypeId) =>
        {
            int index = name switch
            {
                "m_scoreValueVespeneUsedCurrentTechnology" => 0,
                "m_scoreValueVespeneFriendlyFireArmy" => 1,
                "m_scoreValueMineralsFriendlyFireTechnology" => 2,
                "m_scoreValueMineralsUsedCurrentEconomy" => 3,
                "m_scoreValueVespeneLostEconomy" => 4,
                "m_scoreValueMineralsUsedCurrentArmy" => 5,
                "m_scoreValueVespeneUsedInProgressArmy" => 6,
                "m_scoreValueVespeneCollectionRate" => 7,
                "m_scoreValueMineralsUsedInProgressTechnology" => 8,
                "m_scoreValueMineralsCollectionRate" => 9,
                "m_scoreValueWorkersActiveCount" => 10,
                "m_scoreValueMineralsUsedInProgressArmy" => 11,
                "m_scoreValueVespeneLostArmy" => 12,
                "m_scoreValueMineralsKilledEconomy" => 13,
                "m_scoreValueMineralsUsedCurrentTechnology" => 14,
                "m_scoreValueMineralsKilledArmy" => 15,
                "m_scoreValueMineralsLostEconomy" => 16,
                "m_scoreValueMineralsCurrent" => 17,
                "m_scoreValueMineralsLostArmy" => 18,
                "m_scoreValueVespeneKilledArmy" => 19,
                "m_scoreValueVespeneKilledTechnology" => 20,
                "m_scoreValueVespeneKilledEconomy" => 21,
                "m_scoreValueMineralsUsedActiveForces" => 22,
                "m_scoreValueVespeneUsedCurrentArmy" => 23,
                "m_scoreValueMineralsFriendlyFireArmy" => 24,
                "m_scoreValueVespeneUsedActiveForces" => 25,
                "m_scoreValueVespeneCurrent" => 26,
                "m_scoreValueMineralsLostTechnology" => 27,
                "m_scoreValueMineralsUsedInProgressEconomy" => 28,
                "m_scoreValueMineralsFriendlyFireEconomy" => 29,
                "m_scoreValueVespeneUsedInProgressTechnology" => 30,
                "m_scoreValueFoodMade" => 31,
                "m_scoreValueMineralsKilledTechnology" => 32,
                "m_scoreValueVespeneLostTechnology" => 33,
                "m_scoreValueVespeneFriendlyFireEconomy" => 34,
                "m_scoreValueVespeneUsedInProgressEconomy" => 35,
                "m_scoreValueVespeneUsedCurrentEconomy" => 36,
                "m_scoreValueVespeneFriendlyFireTechnology" => 37,
                "m_scoreValueFoodUsed" => 38,
                _ => -1,
            };

            if (index < 0)
            {
                return false;
            }

            stats[index] = decoder.ReadInt(fieldTypeId);
            return true;
        });
        return stats;
    }

    public static AttributeEvents DecodeReplayAttributeEvents(byte[] content) => DecodeReplayAttributeEvents((ReadOnlyMemory<byte>)content);

    public static AttributeEvents DecodeReplayAttributeEvents(ReadOnlyMemory<byte> content)
    {
        BitPackedBuffer buffer = new(content, "little");
        if (buffer.Done())
        {
            return new AttributeEvents(0, 0, []);
        }

        int source = (int)buffer.ReadBits(8);
        int mapNamespace = (int)buffer.ReadBits(32);
        _ = buffer.ReadBits(32);
        List<AttributeEventScope> scopes = [];
        Span<byte> rawValue = stackalloc byte[4];

        while (!buffer.Done())
        {
            int ns = (int)buffer.ReadBits(32);
            int attrId = (int)buffer.ReadBits(32);
            int scope = (int)buffer.ReadBits(8);
            buffer.ReadAlignedSpan(4).CopyTo(rawValue);
            rawValue.Reverse();
            string value = Encoding.UTF8.GetString(rawValue).TrimEnd('\0');
            rawValue.Reverse();
            scopes.Add(new AttributeEventScope(scope, attrId, ns, attrId, value));
        }

        return new AttributeEvents(source, mapNamespace, scopes);
    }

    private static DetailsPlayer ReadDetailsPlayer(TypedProtocolDecoder decoder, int typeId)
    {
        PlayerColor color = new(0, 0, 0, 0);
        int control = 0;
        int handicap = 0;
        string hero = string.Empty;
        string nameValue = string.Empty;
        int observe = 0;
        string race = string.Empty;
        int result = 0;
        int team = 0;
        Toon toon = new(0, string.Empty, 0, 0);
        int slot = 0;

        decoder.ReadStruct(typeId, (name, fieldTypeId) =>
        {
            switch (name)
            {
                case "m_color":
                    color = ReadColor(decoder, fieldTypeId);
                    return true;
                case "m_control":
                    control = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_handicap":
                    handicap = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_hero":
                    hero = decoder.ReadString(fieldTypeId);
                    return true;
                case "m_name":
                    nameValue = decoder.ReadString(fieldTypeId);
                    return true;
                case "m_observe":
                    observe = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_race":
                    race = decoder.ReadString(fieldTypeId);
                    return true;
                case "m_result":
                    result = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_teamId":
                    team = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_toon":
                    toon = ReadToon(decoder, fieldTypeId);
                    return true;
                case "m_workingSetSlotId":
                    slot = decoder.ReadInt(fieldTypeId);
                    return true;
                default:
                    return false;
            }
        });

        return new DetailsPlayer(color, control, handicap, hero, nameValue, observe, race, result, team, toon, slot);
    }

    private static PlayerColor ReadColor(TypedProtocolDecoder decoder, int typeId)
    {
        int a = 0;
        int b = 0;
        int g = 0;
        int r = 0;
        decoder.ReadStruct(typeId, (name, fieldTypeId) =>
        {
            switch (name)
            {
                case "m_a":
                    a = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_b":
                    b = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_g":
                    g = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_r":
                    r = decoder.ReadInt(fieldTypeId);
                    return true;
                default:
                    return false;
            }
        });
        return new PlayerColor(a, b, g, r);
    }

    private static Toon ReadToon(TypedProtocolDecoder decoder, int typeId)
    {
        int id = 0;
        string programId = string.Empty;
        int realm = 0;
        int region = 0;
        decoder.ReadStruct(typeId, (name, fieldTypeId) =>
        {
            switch (name)
            {
                case "m_id":
                    id = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_programId":
                    programId = decoder.ReadString(fieldTypeId);
                    return true;
                case "m_realm":
                    realm = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_region":
                    region = decoder.ReadInt(fieldTypeId);
                    return true;
                default:
                    return false;
            }
        });
        return new Toon(id, programId, realm, region);
    }

    private static SUserFinishedLoadingSyncEvent ReadEmptyGameEvent(TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        decoder.SkipType(typeId);
        return new SUserFinishedLoadingSyncEvent(userId, eventId, bits, gameloop);
    }

    private static STriggerSoundLengthSyncEvent ReadSoundLengthSyncEvent(TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        decoder.SkipType(typeId);
        return new STriggerSoundLengthSyncEvent(userId, eventId, bits, gameloop);
    }

    private static GameEvent ReadGenericKnownGameEvent(
        TypedProtocolDecoder decoder,
        int typeId,
        int userId,
        int eventId,
        int bits,
        int gameloop,
        string eventName,
        GameEventType eventType)
    {
        string name = string.Empty;
        string data = string.Empty;
        int intValue = 0;
        long longValue = 0;

        decoder.ReadStruct(typeId, (fieldName, fieldTypeId) =>
        {
            switch (fieldName)
            {
                case "m_name":
                case "m_fileName":
                    name = decoder.ReadString(fieldTypeId);
                    return true;
                case "m_data":
                case "m_chatMessage":
                    data = decoder.ReadString(fieldTypeId);
                    return true;
                case "m_type":
                case "m_state":
                case "m_leaveReason":
                case "m_controlGroupUpdate":
                case "m_syncTime":
                case "m_button":
                case "m_key":
                case "m_flags":
                case "m_unitTag":
                case "m_sound":
                case "m_soundtrack":
                case "m_achievementLink":
                    intValue = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_cutsceneId":
                case "m_gameMenuItemIndex":
                case "m_transmissionId":
                case "m_decrementSeconds":
                    longValue = decoder.ReadLong(fieldTypeId);
                    return true;
                default:
                    return false;
            }
        });

        return eventType switch
        {
            GameEventType.SBankFileEvent => new SBankFileEvent(userId, eventId, bits, gameloop, name),
            GameEventType.SBankKeyEvent => new SBankKeyEvent(userId, eventId, bits, gameloop, name, data, intValue),
            GameEventType.SBankSectionEvent => new SBankSectionEvent(userId, eventId, bits, gameloop, name),
            GameEventType.SBankValueEvent => new SBankValueEvent(userId, eventId, bits, gameloop, name, data, intValue),
            GameEventType.SControlGroupUpdateEvent => new SControlGroupUpdateEvent(userId, eventId, bits, gameloop, intValue),
            GameEventType.SGameUserLeaveEvent => new SGameUserLeaveEvent(userId, eventId, bits, gameloop, intValue),
            GameEventType.SSetSyncLoadingTimeEvent => new SSetSyncLoadingTimeEvent(userId, eventId, bits, gameloop, intValue),
            GameEventType.SSetSyncPlayingTimeEvent => new SSetSyncPlayingTimeEvent(userId, eventId, bits, gameloop, intValue),
            GameEventType.STriggerButtonPressedEvent => new STriggerButtonPressedEvent(userId, eventId, bits, gameloop, intValue),
            GameEventType.STriggerChatMessageEvent => new STriggerChatMessageEvent(userId, eventId, bits, gameloop, data),
            GameEventType.STriggerCutsceneEndSceneFiredEvent => new STriggerCutsceneEndSceneFiredEvent(userId, eventId, bits, gameloop, longValue),
            GameEventType.STriggerGameMenuItemSelectedEvent => new STriggerGameMenuItemSelectedEvent(userId, eventId, bits, gameloop, longValue),
            GameEventType.STriggerSoundOffsetEvent => new STriggerSoundOffsetEvent(userId, eventId, bits, gameloop, intValue),
            GameEventType.STriggerSoundtrackDoneEvent => new STriggerSoundtrackDoneEvent(userId, eventId, bits, gameloop, intValue),
            GameEventType.STriggerTransmissionCompleteEvent => new STriggerTransmissionCompleteEvent(userId, eventId, bits, gameloop, longValue),
            GameEventType.STriggerTransmissionOffsetEvent => new STriggerTransmissionOffsetEvent(userId, eventId, bits, gameloop, intValue),
            GameEventType.SUnitClickEvent => new SUnitClickEvent(userId, eventId, bits, gameloop, intValue),
            _ => new UnknownGameEvent(userId, eventId, bits, gameloop, eventName),
        };
    }

    private void DecodeEventStream(
        TypedProtocolDecoder decoder,
        int eventIdTypeId,
        Dictionary<int, S2EventType> eventTypes,
        bool decodeUserId,
        Action<int, string, int, int, int, int> handleEvent)
    {
        int gameloop = 0;

        while (!decoder.Done())
        {
            long startBits = decoder.UsedBits();
            gameloop += decoder.ReadInt(SVarUint32TypeId ?? 7);

            int userId = -1;
            if (decodeUserId)
            {
                userId = decoder.ReadInt(ReplayUserIdTypeId ?? 8);
            }

            int eventId = decoder.ReadInt(eventIdTypeId);
            if (!eventTypes.TryGetValue(eventId, out var eventTypeInfo))
            {
                eventTypeInfo = new S2EventType(-1, "UnknownEvent");
            }

            int typeId = eventTypeInfo.TypeId;
            string typeName = eventTypeInfo.Name;
            handleEvent(eventId, typeName, 0, gameloop, userId, typeId);
            decoder.ByteAlign();
            _ = decoder.UsedBits() - startBits;
        }
    }

    private static GameEventType GetGameEventType(string eventTypeName) => eventTypeName switch
    {
        "NNet.Game.SBankFileEvent" => GameEventType.SBankFileEvent,
        "NNet.Game.SBankKeyEvent" => GameEventType.SBankKeyEvent,
        "NNet.Game.SBankSectionEvent" => GameEventType.SBankSectionEvent,
        "NNet.Game.SBankSignatureEvent" => GameEventType.SBankSignatureEvent,
        "NNet.Game.SBankValueEvent" => GameEventType.SBankValueEvent,
        "NNet.Game.SCameraUpdateEvent" => GameEventType.SCameraUpdateEvent,
        "NNet.Game.SCmdEvent" => GameEventType.SCmdEvent,
        "NNet.Game.SCmdUpdateTargetPointEvent" => GameEventType.SCmdUpdateTargetPointEvent,
        "NNet.Game.SCommandManagerStateEvent" => GameEventType.SCommandManagerStateEvent,
        "NNet.Game.SControlGroupUpdateEvent" => GameEventType.SControlGroupUpdateEvent,
        "NNet.Game.SGameUserLeaveEvent" => GameEventType.SGameUserLeaveEvent,
        "NNet.Game.SSelectionDeltaEvent" => GameEventType.SSelectionDeltaEvent,
        "NNet.Game.SSetSyncLoadingTimeEvent" => GameEventType.SSetSyncLoadingTimeEvent,
        "NNet.Game.SSetSyncPlayingTimeEvent" => GameEventType.SSetSyncPlayingTimeEvent,
        "NNet.Game.STriggerDialogControlEvent" => GameEventType.STriggerDialogControlEvent,
        "NNet.Game.STriggerPingEvent" => GameEventType.STriggerPingEvent,
        "NNet.Game.STriggerSoundLengthSyncEvent" => GameEventType.STriggerSoundLengthSyncEvent,
        "NNet.Game.SUserFinishedLoadingSyncEvent" => GameEventType.SUserFinishedLoadingSyncEvent,
        "NNet.Game.SUserOptionsEvent" => GameEventType.SUserOptionsEvent,
        "NNet.Game.SCmdUpdateTargetUnitEvent" => GameEventType.SCmdUpdateTargetUnitEvents,
        "NNet.Game.STriggerKeyPressedEvent" => GameEventType.STriggerKeyPressedEvent,
        "NNet.Game.SUnitClickEvent" => GameEventType.SUnitClickEvent,
        "NNet.Game.SDecrementGameTimeRemainingEvent" => GameEventType.SDecrementGameTimeRemainingEvent,
        "NNet.Game.STriggerChatMessageEvent" => GameEventType.STriggerChatMessageEvent,
        "NNet.Game.STriggerMouseClickedEvent" => GameEventType.STriggerMouseClickedEvent,
        "NNet.Game.STriggerSoundtrackDoneEvent" => GameEventType.STriggerSoundtrackDoneEvent,
        "NNet.Game.SCameraSaveEvent" => GameEventType.SCameraSaveEvent,
        "NNet.Game.STriggerCutsceneBookmarkFiredEvent" => GameEventType.STriggerCutsceneBookmarkFiredEvent,
        "NNet.Game.STriggerCutsceneEndSceneFiredEvent" => GameEventType.STriggerCutsceneEndSceneFiredEvent,
        "NNet.Game.STriggerSoundLengthQueryEvent" => GameEventType.STriggerSoundLengthQueryEvent,
        "NNet.Game.STriggerSoundOffsetEvent" => GameEventType.STriggerSoundOffsetEvent,
        "NNet.Game.STriggerTargetModeUpdateEvent" => GameEventType.STriggerTargetModeUpdateEvent,
        "NNet.Game.STriggerTransmissionCompleteEvent" => GameEventType.STriggerTransmissionCompleteEvent,
        "NNet.Game.SAchievementAwardedEvent" => GameEventType.SAchievementAwardedEvent,
        "NNet.Game.STriggerTransmissionOffsetEvent" => GameEventType.STriggerTransmissionOffsetEvent,
        "NNet.Game.STriggerButtonPressedEvent" => GameEventType.STriggerButtonPressedEvent,
        "NNet.Game.STriggerGameMenuItemSelectedEvent" => GameEventType.STriggerGameMenuItemSelectedEvent,
        "NNet.Game.STriggerMouseMovedEvent" => GameEventType.STriggerMouseMovedEvent,
        _ => GameEventType.None
    };

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

    private static GameDescription EmptyGameDescription()
        => new(0, 0, false, 0, false, new GameOptions(false, false, false, false, false, 0, false, false, 0, false, false, 0, 0, false, false, false), 0, false, string.Empty, 0, 0, false, 0, 0, false, 0, 0, 0, 0, [], 0, 0, string.Empty, string.Empty, [], 0, 0, false);

    private static UserInitialData ReadUserInitialData(TypedProtocolDecoder decoder, int typeId)
    {
        string name = string.Empty;
        string toonHandle = string.Empty;
        string clanTag = string.Empty;
        string clanLogo = string.Empty;
        string hero = string.Empty;
        string skin = string.Empty;
        string mount = string.Empty;
        int observe = 0;
        int? teamPref = null;
        long combinedRaceLevels = 0;
        int highestLeague = 0;
        bool testMap = false;
        bool testAuto = false;
        bool examine = false;
        int testType = 0;
        bool customInterface = false;
        int? racePreference = null;
        int randomSeed = 0;
        long? scaledRating = null;

        decoder.ReadStruct(typeId, (field, fieldTypeId) =>
        {
            switch (field)
            {
                case "m_name": name = decoder.ReadString(fieldTypeId); return true;
                case "m_toonHandle": toonHandle = decoder.ReadString(fieldTypeId); return true;
                case "m_clanTag": clanTag = decoder.ReadString(fieldTypeId); return true;
                case "m_clanLogo": clanLogo = decoder.ReadString(fieldTypeId); return true;
                case "m_hero": hero = decoder.ReadString(fieldTypeId); return true;
                case "m_skin": skin = decoder.ReadString(fieldTypeId); return true;
                case "m_mount": mount = decoder.ReadString(fieldTypeId); return true;
                case "m_observe": observe = decoder.ReadInt(fieldTypeId); return true;
                case "m_teamPreference": teamPref = ReadSingleNullableStructInt(decoder, fieldTypeId, "m_team"); return true;
                case "m_combinedRaceLevels": combinedRaceLevels = decoder.ReadLong(fieldTypeId); return true;
                case "m_highestLeague": highestLeague = decoder.ReadInt(fieldTypeId); return true;
                case "m_testMap": testMap = decoder.ReadBool(fieldTypeId); return true;
                case "m_testAuto": testAuto = decoder.ReadBool(fieldTypeId); return true;
                case "m_examine": examine = decoder.ReadBool(fieldTypeId); return true;
                case "m_testType": testType = decoder.ReadInt(fieldTypeId); return true;
                case "m_customInterface": customInterface = decoder.ReadBool(fieldTypeId); return true;
                case "m_racePreference": racePreference = ReadSingleNullableStructInt(decoder, fieldTypeId, "m_race"); return true;
                case "m_randomSeed": randomSeed = decoder.ReadInt(fieldTypeId); return true;
                case "m_scaledRating": scaledRating = decoder.ReadNullableLong(fieldTypeId); return true;
                default: return false;
            }
        });

        return new UserInitialData(mount, skin, observe, teamPref, toonHandle, combinedRaceLevels, highestLeague,
            clanTag, testMap, testAuto, examine, testType, customInterface, clanLogo, name, racePreference,
            randomSeed, hero, scaledRating);
    }

    private static LobbyState ReadLobbyState(TypedProtocolDecoder decoder, int typeId)
    {
        int maxUsers = 0;
        List<Slot> slots = [];
        int defaultDifficulty = 0;
        bool isSinglePlayer = false;
        int phase = 0;
        int? hostUserId = null;
        int maxObservers = 0;
        int defaultAIBuild = 0;
        int pickedMapTag = 0;
        long randomSeed = 0;
        int gameDuration = 0;

        decoder.ReadStruct(typeId, (field, fieldTypeId) =>
        {
            switch (field)
            {
                case "m_maxUsers": maxUsers = decoder.ReadInt(fieldTypeId); return true;
                case "m_slots":
                    decoder.ReadArray(fieldTypeId, itemTypeId =>
                    {
                        slots.Add(ReadSlot(decoder, itemTypeId));
                        return true;
                    });
                    return true;
                case "m_defaultDifficulty": defaultDifficulty = decoder.ReadInt(fieldTypeId); return true;
                case "m_isSinglePlayer": isSinglePlayer = decoder.ReadBool(fieldTypeId); return true;
                case "m_phase": phase = decoder.ReadInt(fieldTypeId); return true;
                case "m_hostUserId": hostUserId = decoder.ReadNullableInt(fieldTypeId); return true;
                case "m_maxObservers": maxObservers = decoder.ReadInt(fieldTypeId); return true;
                case "m_defaultAIBuild": defaultAIBuild = decoder.ReadInt(fieldTypeId); return true;
                case "m_pickedMapTag": pickedMapTag = decoder.ReadInt(fieldTypeId); return true;
                case "m_randomSeed": randomSeed = decoder.ReadLong(fieldTypeId); return true;
                case "m_gameDuration": gameDuration = decoder.ReadInt(fieldTypeId); return true;
                default: return false;
            }
        });

        return new LobbyState(maxUsers, slots, defaultDifficulty, isSinglePlayer, phase, hostUserId, maxObservers,
            defaultAIBuild, pickedMapTag, randomSeed, gameDuration);
    }

    private static Slot ReadSlot(TypedProtocolDecoder decoder, int typeId)
    {
        int aiBuild = 0;
        int teamId = 0;
        int difficulty = 0;
        int handicap = 0;
        int observe = 0;
        int control = 0;
        int workingSetSlotId = 0;
        int? userId = null;
        int? racePref = null;
        int? colorPref = null;
        string toonHandle = string.Empty;
        string skin = string.Empty;
        string hero = string.Empty;
        string commander = string.Empty;
        string mount = string.Empty;
        List<int> rewards = [];
        List<int> rewardOverrides = [];
        List<int> licenses = [];
        List<int> commanderMasteryTalents = [];
        List<string> artifacts = [];
        bool hasSilencePenalty = false;

        decoder.ReadStruct(typeId, (field, fieldTypeId) =>
        {
            switch (field)
            {
                case "m_aiBuild": aiBuild = decoder.ReadInt(fieldTypeId); return true;
                case "m_teamId": teamId = decoder.ReadInt(fieldTypeId); return true;
                case "m_difficulty": difficulty = decoder.ReadInt(fieldTypeId); return true;
                case "m_handicap": handicap = decoder.ReadInt(fieldTypeId); return true;
                case "m_observe": observe = decoder.ReadInt(fieldTypeId); return true;
                case "m_control": control = decoder.ReadInt(fieldTypeId); return true;
                case "m_workingSetSlotId": workingSetSlotId = decoder.ReadInt(fieldTypeId); return true;
                case "m_userId": userId = decoder.ReadNullableInt(fieldTypeId); return true;
                case "m_racePref": racePref = ReadSingleNullableStructInt(decoder, fieldTypeId, "m_race"); return true;
                case "m_colorPref": colorPref = ReadSingleNullableStructInt(decoder, fieldTypeId, "m_color"); return true;
                case "m_toonHandle": toonHandle = decoder.ReadString(fieldTypeId); return true;
                case "m_skin": skin = decoder.ReadString(fieldTypeId); return true;
                case "m_hero": hero = decoder.ReadString(fieldTypeId); return true;
                case "m_commander": commander = decoder.ReadString(fieldTypeId); return true;
                case "m_mount": mount = decoder.ReadString(fieldTypeId); return true;
                case "m_rewards": rewards = decoder.ReadIntList(fieldTypeId); return true;
                case "m_rewardOverrides": rewardOverrides = decoder.ReadIntList(fieldTypeId); return true;
                case "m_licenses": licenses = decoder.ReadIntList(fieldTypeId); return true;
                case "m_commanderMasteryTalents": commanderMasteryTalents = decoder.ReadIntList(fieldTypeId); return true;
                case "m_artifacts": artifacts = decoder.ReadStringList(fieldTypeId); return true;
                case "m_hasSilencePenalty": hasSilencePenalty = decoder.ReadBool(fieldTypeId); return true;
                default: return false;
            }
        });

        return new Slot(0, toonHandle, rewardOverrides, userId, skin, commanderMasteryTalents, aiBuild, teamId,
            rewards, 0, 0, artifacts, difficulty, null, 0, 0, 0, racePref, null, hero, commander, mount, handicap,
            observe, 0, control, licenses, colorPref, hasSilencePenalty, workingSetSlotId, [], 0);
    }

    private static GameDescription ReadGameDescription(TypedProtocolDecoder decoder, int typeId)
    {
        GameOptions gameOptions = new(false, false, false, false, false, 0, false, false, 0, false, false, 0, 0, false, false, false);
        List<string> cacheHandles = [];
        List<SlotDescription> slotDescriptions = [];
        int maxRaces = 0;
        int maxTeams = 0;
        int maxColors = 0;
        int defaultDifficulty = 0;
        int defaultAIBuild = 0;
        int gameType = 0;
        int maxObservers = 0;
        int maxUsers = 0;
        int maxPlayers = 0;
        int gameSpeed = 0;
        int maxControls = 0;
        int mapSizeY = 0;
        int mapSizeX = 0;
        bool hasExtensionMod = false;
        bool isBlizzardMap = false;
        bool isCoopMode = false;
        bool hasNonBlizzardExtensionMod = false;
        bool isRealtimeMode = false;
        bool isPremadeFFA = false;
        string mapFileName = string.Empty;
        string gameCacheName = string.Empty;
        string mapAuthorName = string.Empty;
        long randomValue = 0;
        long modFileSyncChecksum = 0;
        long mapFileSyncChecksum = 0;

        decoder.ReadStruct(typeId, (field, fieldTypeId) =>
        {
            switch (field)
            {
                case "m_maxRaces": maxRaces = decoder.ReadInt(fieldTypeId); return true;
                case "m_maxTeams": maxTeams = decoder.ReadInt(fieldTypeId); return true;
                case "m_maxColors": maxColors = decoder.ReadInt(fieldTypeId); return true;
                case "m_defaultDifficulty": defaultDifficulty = decoder.ReadInt(fieldTypeId); return true;
                case "m_defaultAIBuild": defaultAIBuild = decoder.ReadInt(fieldTypeId); return true;
                case "m_gameType": gameType = decoder.ReadInt(fieldTypeId); return true;
                case "m_maxObservers": maxObservers = decoder.ReadInt(fieldTypeId); return true;
                case "m_maxUsers": maxUsers = decoder.ReadInt(fieldTypeId); return true;
                case "m_maxPlayers": maxPlayers = decoder.ReadInt(fieldTypeId); return true;
                case "m_gameSpeed": gameSpeed = decoder.ReadInt(fieldTypeId); return true;
                case "m_maxControls": maxControls = decoder.ReadInt(fieldTypeId); return true;
                case "m_mapSizeY": mapSizeY = decoder.ReadInt(fieldTypeId); return true;
                case "m_mapSizeX": mapSizeX = decoder.ReadInt(fieldTypeId); return true;
                case "m_hasExtensionMod": hasExtensionMod = decoder.ReadBool(fieldTypeId); return true;
                case "m_isBlizzardMap": isBlizzardMap = decoder.ReadBool(fieldTypeId); return true;
                case "m_isCoopMode": isCoopMode = decoder.ReadBool(fieldTypeId); return true;
                case "m_hasNonBlizzardExtensionMod": hasNonBlizzardExtensionMod = decoder.ReadBool(fieldTypeId); return true;
                case "m_isRealtimeMode": isRealtimeMode = decoder.ReadBool(fieldTypeId); return true;
                case "m_isPremadeFFA": isPremadeFFA = decoder.ReadBool(fieldTypeId); return true;
                case "m_mapFileName": mapFileName = decoder.ReadString(fieldTypeId); return true;
                case "m_gameCacheName": gameCacheName = decoder.ReadString(fieldTypeId); return true;
                case "m_mapAuthorName": mapAuthorName = decoder.ReadString(fieldTypeId); return true;
                case "m_randomValue": randomValue = decoder.ReadLong(fieldTypeId); return true;
                case "m_modFileSyncChecksum": modFileSyncChecksum = decoder.ReadLong(fieldTypeId); return true;
                case "m_mapFileSyncChecksum": mapFileSyncChecksum = decoder.ReadLong(fieldTypeId); return true;
                case "m_cacheHandles": cacheHandles = decoder.ReadStringList(fieldTypeId); return true;
                case "m_gameOptions": gameOptions = ReadGameOptions(decoder, fieldTypeId); return true;
                case "m_slotDescriptions":
                    decoder.ReadArray(fieldTypeId, itemTypeId =>
                    {
                        slotDescriptions.Add(ReadSlotDescription(decoder, itemTypeId));
                        return true;
                    });
                    return true;
                default: return false;
            }
        });

        return new GameDescription(maxRaces, maxTeams, hasExtensionMod, maxColors, isBlizzardMap, gameOptions,
            defaultDifficulty, isCoopMode, mapFileName, defaultAIBuild, gameType, hasNonBlizzardExtensionMod,
            randomValue, maxObservers, isRealtimeMode, maxUsers, modFileSyncChecksum, mapFileSyncChecksum,
            maxPlayers, cacheHandles, gameSpeed, maxControls, gameCacheName, mapAuthorName, slotDescriptions,
            mapSizeY, mapSizeX, isPremadeFFA);
    }

    private static GameOptions ReadGameOptions(TypedProtocolDecoder decoder, int typeId)
    {
        bool competitive = false, practice = false, lockTeams = false, amm = false, battleNet = false,
            noVictoryOrDefeat = false, heroDuplicatesAllowed = false, advancedSharedControl = false,
            cooperative = false, teamsTogether = false, randomRaces = false, buildCoachEnabled = false;
        int fog = 0, userDifficulty = 0, observers = 0;
        long clientDebugFlags = 0;

        decoder.ReadStruct(typeId, (field, fieldTypeId) =>
        {
            switch (field)
            {
                case "m_competitive": competitive = decoder.ReadBool(fieldTypeId); return true;
                case "m_practice": practice = decoder.ReadBool(fieldTypeId); return true;
                case "m_lockTeams": lockTeams = decoder.ReadBool(fieldTypeId); return true;
                case "m_amm": amm = decoder.ReadBool(fieldTypeId); return true;
                case "m_battleNet": battleNet = decoder.ReadBool(fieldTypeId); return true;
                case "m_noVictoryOrDefeat": noVictoryOrDefeat = decoder.ReadBool(fieldTypeId); return true;
                case "m_heroDuplicatesAllowed": heroDuplicatesAllowed = decoder.ReadBool(fieldTypeId); return true;
                case "m_advancedSharedControl": advancedSharedControl = decoder.ReadBool(fieldTypeId); return true;
                case "m_cooperative": cooperative = decoder.ReadBool(fieldTypeId); return true;
                case "m_teamsTogether": teamsTogether = decoder.ReadBool(fieldTypeId); return true;
                case "m_randomRaces": randomRaces = decoder.ReadBool(fieldTypeId); return true;
                case "m_buildCoachEnabled": buildCoachEnabled = decoder.ReadBool(fieldTypeId); return true;
                case "m_fog": fog = decoder.ReadInt(fieldTypeId); return true;
                case "m_userDifficulty": userDifficulty = decoder.ReadInt(fieldTypeId); return true;
                case "m_observers": observers = decoder.ReadInt(fieldTypeId); return true;
                case "m_clientDebugFlags": clientDebugFlags = decoder.ReadLong(fieldTypeId); return true;
                default: return false;
            }
        });

        return new GameOptions(competitive, practice, lockTeams, amm, battleNet, fog, noVictoryOrDefeat,
            heroDuplicatesAllowed, userDifficulty, advancedSharedControl, cooperative, clientDebugFlags,
            observers, teamsTogether, randomRaces, buildCoachEnabled);
    }

    private static SlotDescription ReadSlotDescription(TypedProtocolDecoder decoder, int typeId)
    {
        KeyValuePair<int, BigInteger> allowedRaces = default;
        KeyValuePair<int, BigInteger> allowedColors = default;
        KeyValuePair<int, BigInteger> allowedAIBuilds = default;
        KeyValuePair<int, BigInteger> allowedDifficulty = default;
        KeyValuePair<int, BigInteger> allowedObserveTypes = default;
        KeyValuePair<int, BigInteger> allowedControls = default;

        decoder.ReadStruct(typeId, (field, fieldTypeId) =>
        {
            switch (field)
            {
                case "m_allowedRaces": allowedRaces = decoder.ReadBitArray(fieldTypeId); return true;
                case "m_allowedColors": allowedColors = decoder.ReadBitArray(fieldTypeId); return true;
                case "m_allowedAIBuilds": allowedAIBuilds = decoder.ReadBitArray(fieldTypeId); return true;
                case "m_allowedDifficulty": allowedDifficulty = decoder.ReadBitArray(fieldTypeId); return true;
                case "m_allowedObserveTypes": allowedObserveTypes = decoder.ReadBitArray(fieldTypeId); return true;
                case "m_allowedControls": allowedControls = decoder.ReadBitArray(fieldTypeId); return true;
                default: return false;
            }
        });

        return new SlotDescription(allowedRaces, allowedColors, allowedAIBuilds, allowedDifficulty, allowedObserveTypes, allowedControls);
    }

    private static int? ReadSingleNullableStructInt(TypedProtocolDecoder decoder, int typeId, string fieldName)
    {
        int? result = null;
        decoder.ReadStruct(typeId, (name, innerTypeId) =>
        {
            if (name != fieldName)
            {
                return false;
            }

            result = decoder.ReadNullableInt(innerTypeId);
            return true;
        });
        return result;
    }
}

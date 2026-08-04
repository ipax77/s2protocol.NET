using s2protocol.NET.Models;
using System.Numerics;
using System.Text;

namespace s2protocol.NET.S2Protocol;

public sealed partial record S2ProtocolVersion
{
    public Header DecodeReplayHeader(ReadOnlyMemory<byte> content)
    {
        VersionedTypedDecoder decoder = new(content, TypeInfos);
        HeaderReadState state = default;

        decoder.ReadStruct(ReplayHeaderTypeId ?? 18, ref state);

        return new Header(state.DataBuild, state.Elapsed, state.UseScaledTime, state.Version ?? new Version(),
            state.Signature ?? string.Empty, string.Empty, string.Empty, state.Type, state.Flags, state.Build, state.BaseBuild);
    }

    public Details DecodeReplayDetails(ReadOnlyMemory<byte> content)
    {
        VersionedTypedDecoder decoder = new(content, TypeInfos);
        DetailsReadState state = new()
        {
            Players = [],
        };

        decoder.ReadStruct(GameDetailsTypeId ?? 40, ref state);

        return new Details(state.CampaignIndex, state.DefaultDifficulty, state.Description ?? string.Empty,
            state.Difficulty ?? string.Empty, state.DisableRecoverGame, state.GameSpeed, state.ImageFilePath ?? string.Empty,
            state.IsBlizzardMap, state.MapFileName ?? string.Empty, state.MiniSave, state.RestartAsTransitionMap,
            state.TimeLocalOffset, state.TimeUTC, state.Title ?? string.Empty, state.Players);
    }

    public Initdata? DecodeReplayInitData(ReadOnlyMemory<byte> content)
    {
        BitPackedTypedDecoder decoder = new(content, TypeInfos);
        InitDataReadState state = default;

        decoder.ReadStruct(ReplayInitDataTypeId ?? 73, ref state);

        return state.Initdata;
    }

    public MessageEvents DecodeReplayMessageEvents(ReadOnlyMemory<byte> content)
    {
        BitPackedTypedDecoder decoder = new(content, TypeInfos);
        List<ChatMessageEvent> chatMessages = [];
        List<PingMessageEvent> pingMessages = [];
        MessageEventHandler handler = new(decoder, chatMessages, pingMessages);

        DecodeEventStream(decoder, MessageEventIdTypeId ?? 1, MessageEvents, decodeUserId: true, ref handler);

        return new MessageEvents(chatMessages, pingMessages);
    }

    public GameEvents DecodeReplayGameEvents(ReadOnlyMemory<byte> content)
    {
        BitPackedTypedDecoder decoder = new(content, TypeInfos);
        List<GameEvent> events = [];
        GameEventHandler handler = new(decoder, events);

        DecodeEventStream(decoder, GameEventIdTypeId ?? 0, GameEvents, decodeUserId: true, ref handler);

        return new GameEvents(events);
    }

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
        TrackerEventHandler handler = new(
            decoder,
            playerSetupEvents,
            playerStatsEvents,
            unitBornEvents,
            unitDiedEvents,
            unitOwnerChangeEvents,
            unitPositionsEvents,
            unitTypeChangeEvents,
            upgradeEvents,
            unitInitEvents,
            unitDoneEvents);

        DecodeEventStream(decoder, TrackerEventIdTypeId ?? 2, TrackerEvents, decodeUserId: false, ref handler);

        return new TrackerEvents(
            playerSetupEvents,
            playerStatsEvents,
            unitBornEvents,
            unitDiedEvents,
            unitOwnerChangeEvents,
            unitPositionsEvents,
            unitTypeChangeEvents,
            upgradeEvents,
            unitInitEvents,
            unitDoneEvents);
    }

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

    private struct HeaderReadState : IStructFieldReader
    {
        public bool UseScaledTime;
        public string? Signature;
        public Version? Version;
        public int Flags;
        public int Build;
        public int BaseBuild;
        public int Elapsed;
        public int DataBuild;
        public int Type;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_signature":
                    Signature = decoder.ReadString(fieldTypeId);
                    return true;
                case "m_version":
                    HeaderVersionReadState version = new(Version ?? new Version(), Flags, Build, BaseBuild);
                    decoder.ReadStruct(fieldTypeId, ref version);
                    Version = version.Version;
                    Flags = version.Flags;
                    Build = version.Build;
                    BaseBuild = version.BaseBuild;
                    return true;
                case "m_type":
                    Type = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_elapsedGameLoops":
                    Elapsed = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_useScaledTime":
                    UseScaledTime = decoder.ReadBool(fieldTypeId);
                    return true;
                case "m_dataBuildNum":
                    DataBuild = decoder.ReadInt(fieldTypeId);
                    return true;
                default:
                    return false;
            }
        }
    }

    private struct HeaderVersionReadState : IStructFieldReader
    {
        public HeaderVersionReadState(Version version, int flags, int build, int baseBuild)
        {
            Version = version;
            Flags = flags;
            Build = build;
            BaseBuild = baseBuild;
        }

        public Version Version;
        public int Flags;
        public int Build;
        public int BaseBuild;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_flags":
                    Flags = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_major":
                    int major = decoder.ReadInt(fieldTypeId);
                    Version = new Version(major, Version.Minor < 0 ? 0 : Version.Minor, Version.Build < 0 ? 0 : Version.Build);
                    return true;
                case "m_minor":
                    int minor = decoder.ReadInt(fieldTypeId);
                    Version = new Version(Version.Major < 0 ? 0 : Version.Major, minor, Version.Build < 0 ? 0 : Version.Build);
                    return true;
                case "m_revision":
                    int revision = decoder.ReadInt(fieldTypeId);
                    Version = new Version(Version.Major < 0 ? 0 : Version.Major, Version.Minor < 0 ? 0 : Version.Minor, revision);
                    return true;
                case "m_build":
                    Build = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_baseBuild":
                    BaseBuild = decoder.ReadInt(fieldTypeId);
                    return true;
                default:
                    return false;
            }
        }
    }

    private struct DetailsReadState : IStructFieldReader
    {
        public int CampaignIndex;
        public int DefaultDifficulty;
        public string? Description;
        public string? Difficulty;
        public bool DisableRecoverGame;
        public int GameSpeed;
        public string? ImageFilePath;
        public bool IsBlizzardMap;
        public string? MapFileName;
        public bool MiniSave;
        public bool RestartAsTransitionMap;
        public long TimeLocalOffset;
        public long TimeUTC;
        public string? Title;
        public List<DetailsPlayer> Players;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_campaignIndex": CampaignIndex = decoder.ReadInt(fieldTypeId); return true;
                case "m_defaultDifficulty": DefaultDifficulty = decoder.ReadInt(fieldTypeId); return true;
                case "m_description": Description = decoder.ReadString(fieldTypeId); return true;
                case "m_difficulty": Difficulty = decoder.ReadString(fieldTypeId); return true;
                case "m_disableRecoverGame": DisableRecoverGame = decoder.ReadBool(fieldTypeId); return true;
                case "m_gameSpeed": GameSpeed = decoder.ReadInt(fieldTypeId); return true;
                case "m_imageFilePath": ImageFilePath = decoder.ReadString(fieldTypeId); return true;
                case "m_isBlizzardMap": IsBlizzardMap = decoder.ReadBool(fieldTypeId); return true;
                case "m_mapFileName": MapFileName = decoder.ReadString(fieldTypeId); return true;
                case "m_miniSave": MiniSave = decoder.ReadBool(fieldTypeId); return true;
                case "m_restartAsTransitionMap": RestartAsTransitionMap = decoder.ReadBool(fieldTypeId); return true;
                case "m_timeLocalOffset": TimeLocalOffset = decoder.ReadLong(fieldTypeId); return true;
                case "m_timeUTC": TimeUTC = decoder.ReadLong(fieldTypeId); return true;
                case "m_title": Title = decoder.ReadString(fieldTypeId); return true;
                case "m_playerList":
                    DetailsPlayerArrayReader reader = new(Players);
                    decoder.ReadArray(fieldTypeId, ref reader);
                    return true;
                default:
                    return false;
            }
        }
    }

    private readonly struct DetailsPlayerArrayReader(List<DetailsPlayer> players) : IArrayItemReader
    {
        public bool ReadItem(TypedProtocolDecoder decoder, int itemTypeId)
        {
            players.Add(ReadDetailsPlayer(decoder, itemTypeId));
            return true;
        }
    }

    private struct InitDataReadState : IStructFieldReader
    {
        public Initdata? Initdata;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            if (name != "m_syncLobbyState")
            {
                return false;
            }

            SyncLobbyStateReadState state = new()
            {
                UserInitialData = [],
                LobbyState = new LobbyState(0, [], 0, false, 0, 0, 0, 0, 0, 0, 0),
                GameDescription = EmptyGameDescription(),
            };

            decoder.ReadStruct(fieldTypeId, ref state);
            Initdata = new Initdata(state.UserInitialData, state.LobbyState, state.GameDescription);
            return true;
        }
    }

    private struct SyncLobbyStateReadState : IStructFieldReader
    {
        public List<UserInitialData> UserInitialData;
        public LobbyState LobbyState;
        public GameDescription GameDescription;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_userInitialData":
                    UserInitialDataArrayReader reader = new(UserInitialData);
                    decoder.ReadArray(fieldTypeId, ref reader);
                    return true;
                case "m_lobbyState":
                    LobbyState = ReadLobbyState(decoder, fieldTypeId);
                    return true;
                case "m_gameDescription":
                    GameDescription = ReadGameDescription(decoder, fieldTypeId);
                    return true;
                default:
                    return false;
            }
        }
    }

    private readonly struct UserInitialDataArrayReader(List<UserInitialData> userInitialData) : IArrayItemReader
    {
        public bool ReadItem(TypedProtocolDecoder decoder, int itemTypeId)
        {
            userInitialData.Add(ReadUserInitialData(decoder, itemTypeId));
            return true;
        }
    }

    private struct DetailsPlayerReadState : IStructFieldReader
    {
        public PlayerColor Color;
        public int Control;
        public int Handicap;
        public string? Hero;
        public string? NameValue;
        public int Observe;
        public string? Race;
        public int Result;
        public int Team;
        public Toon Toon;
        public int Slot;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_color": Color = ReadColor(decoder, fieldTypeId); return true;
                case "m_control": Control = decoder.ReadInt(fieldTypeId); return true;
                case "m_handicap": Handicap = decoder.ReadInt(fieldTypeId); return true;
                case "m_hero": Hero = decoder.ReadString(fieldTypeId); return true;
                case "m_name": NameValue = decoder.ReadString(fieldTypeId); return true;
                case "m_observe": Observe = decoder.ReadInt(fieldTypeId); return true;
                case "m_race": Race = decoder.ReadString(fieldTypeId); return true;
                case "m_result": Result = decoder.ReadInt(fieldTypeId); return true;
                case "m_teamId": Team = decoder.ReadInt(fieldTypeId); return true;
                case "m_toon": Toon = ReadToon(decoder, fieldTypeId); return true;
                case "m_workingSetSlotId": Slot = decoder.ReadInt(fieldTypeId); return true;
                default: return false;
            }
        }
    }

    private struct ColorReadState : IStructFieldReader
    {
        public int A;
        public int B;
        public int G;
        public int R;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_a": A = decoder.ReadInt(fieldTypeId); return true;
                case "m_b": B = decoder.ReadInt(fieldTypeId); return true;
                case "m_g": G = decoder.ReadInt(fieldTypeId); return true;
                case "m_r": R = decoder.ReadInt(fieldTypeId); return true;
                default: return false;
            }
        }
    }

    private struct ToonReadState : IStructFieldReader
    {
        public int Id;
        public string? ProgramId;
        public int Realm;
        public int Region;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_id": Id = decoder.ReadInt(fieldTypeId); return true;
                case "m_programId": ProgramId = decoder.ReadString(fieldTypeId); return true;
                case "m_realm": Realm = decoder.ReadInt(fieldTypeId); return true;
                case "m_region": Region = decoder.ReadInt(fieldTypeId); return true;
                default: return false;
            }
        }
    }

    private static DetailsPlayer ReadDetailsPlayer(TypedProtocolDecoder decoder, int typeId)
    {
        DetailsPlayerReadState state = new()
        {
            Color = new PlayerColor(0, 0, 0, 0),
            Toon = new Toon(0, string.Empty, 0, 0),
        };

        decoder.ReadStruct(typeId, ref state);

        return new DetailsPlayer(state.Color, state.Control, state.Handicap, state.Hero ?? string.Empty,
            state.NameValue ?? string.Empty, state.Observe, state.Race ?? string.Empty, state.Result, state.Team,
            state.Toon, state.Slot);
    }

    private static PlayerColor ReadColor(TypedProtocolDecoder decoder, int typeId)
    {
        ColorReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new PlayerColor(state.A, state.B, state.G, state.R);
    }

    private static Toon ReadToon(TypedProtocolDecoder decoder, int typeId)
    {
        ToonReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new Toon(state.Id, state.ProgramId ?? string.Empty, state.Realm, state.Region);
    }

    private interface IEventHandler
    {
        void Handle(in DecodedEvent decodedEvent);
    }

    private readonly struct DecodedEvent(
        int eventId,
        string typeName,
        int bits,
        int gameloop,
        int userId,
        int typeId)
    {
        public int EventId { get; } = eventId;
        public string TypeName { get; } = typeName;
        public int Bits { get; } = bits;
        public int Gameloop { get; } = gameloop;
        public int UserId { get; } = userId;
        public int TypeId { get; } = typeId;
    }

    private readonly struct MessageEventHandler(
        TypedProtocolDecoder decoder,
        List<ChatMessageEvent> chatMessages,
        List<PingMessageEvent> pingMessages) : IEventHandler
    {
        public void Handle(in DecodedEvent decodedEvent)
        {
            if (decodedEvent.TypeName == "NNet.Game.SChatMessage")
            {
                chatMessages.Add(ReadSChatMessage(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.Gameloop));
            }
            else if (decodedEvent.TypeName == "NNet.Game.SPingMessage")
            {
                pingMessages.Add(ReadSPingMessage(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.Gameloop));
            }
            else
            {
                decoder.SkipType(decodedEvent.TypeId);
            }
        }
    }

    private readonly struct GameEventHandler(TypedProtocolDecoder decoder, List<GameEvent> events) : IEventHandler
    {
        public void Handle(in DecodedEvent decodedEvent)
        {
            GameEventType eventType = GetGameEventType(decodedEvent.TypeName);
#pragma warning disable CS8524 // Every named value is explicit; unnamed numeric enum values are invalid input.
            GameEvent gameEvent = eventType switch
            {
                GameEventType.SBankFileEvent => ReadSBankFileEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SBankKeyEvent => ReadSBankKeyEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SBankSectionEvent => ReadSBankSectionEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SBankSignatureEvent => ReadSBankSignatureEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SBankValueEvent => ReadSBankValueEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SCameraUpdateEvent => ReadSCameraUpdateEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SCmdEvent => ReadSCmdEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SCmdUpdateTargetPointEvent => ReadSCmdUpdateTargetPointEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SCommandManagerStateEvent => ReadSCommandManagerStateEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SControlGroupUpdateEvent => ReadSControlGroupUpdateEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SGameUserLeaveEvent => ReadSGameUserLeaveEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SSelectionDeltaEvent => ReadSSelectionDeltaEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SSetSyncLoadingTimeEvent => ReadSSetSyncLoadingTimeEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SSetSyncPlayingTimeEvent => ReadSSetSyncPlayingTimeEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerDialogControlEvent => ReadSTriggerDialogControlEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerPingEvent => ReadSTriggerPingEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerSoundLengthSyncEvent => ReadSoundLengthSyncEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SUserFinishedLoadingSyncEvent => ReadEmptyGameEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SUserOptionsEvent => ReadSUserOptionsEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SCmdUpdateTargetUnitEvents => ReadSCmdUpdateTargetUnitEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerKeyPressedEvent => ReadSTriggerKeyPressedEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SUnitClickEvent => ReadSUnitClickEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SDecrementGameTimeRemainingEvent => ReadSDecrementGameTimeRemainingEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerChatMessageEvent => ReadSTriggerChatMessageEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerMouseClickedEvent => ReadSTriggerMouseClickedEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerSoundtrackDoneEvent => ReadSTriggerSoundtrackDoneEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SCameraSaveEvent => ReadSCameraSaveEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerCutsceneBookmarkFiredEvent => ReadSTriggerCutsceneBookmarkFiredEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerCutsceneEndSceneFiredEvent => ReadSTriggerCutsceneEndSceneFiredEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerSoundLengthQueryEvent => ReadSTriggerSoundLengthQueryEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerSoundOffsetEvent => ReadSTriggerSoundOffsetEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerTargetModeUpdateEvent => ReadSTriggerTargetModeUpdateEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerTransmissionCompleteEvent => ReadSTriggerTransmissionCompleteEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SAchievementAwardedEvent => ReadSAchievementAwardedEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerTransmissionOffsetEvent => ReadSTriggerTransmissionOffsetEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerButtonPressedEvent => ReadSTriggerButtonPressedEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerGameMenuItemSelectedEvent => ReadSTriggerGameMenuItemSelectedEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerMouseMovedEvent => ReadSTriggerMouseMovedEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SHijackReplayGameEvent => ReadSHijackReplayGameEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SGameUserJoinEvent => ReadSGameUserJoinEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SSaveGameEvent => ReadSSaveGameEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SSaveGameDoneEvent => ReadMarkerGameEvent(decoder, decodedEvent.TypeId, new SSaveGameDoneEvent(decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop)),
                GameEventType.SLoadGameDoneEvent => ReadMarkerGameEvent(decoder, decodedEvent.TypeId, new SLoadGameDoneEvent(decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop)),
                GameEventType.SCommandManagerResetEvent => ReadSCommandManagerResetEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SGameCheatEvent => ReadSGameCheatEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SSelectionSyncCheckEvent => ReadSSelectionSyncCheckEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SResourceTradeEvent => ReadSResourceTradeEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SAICommunicateEvent => ReadSAICommunicateEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SSetAbsoluteGameSpeedEvent => ReadSSetAbsoluteGameSpeedEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SAddAbsoluteGameSpeedEvent => ReadSAddAbsoluteGameSpeedEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SBroadcastCheatEvent => ReadSBroadcastCheatEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SAllianceEvent => ReadSAllianceEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SUnitHighlightEvent => ReadSUnitHighlightEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerReplySelectedEvent => ReadSTriggerReplySelectedEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerSkippedEvent => ReadMarkerGameEvent(decoder, decodedEvent.TypeId, new STriggerSkippedEvent(decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop)),
                GameEventType.STriggerAbortMissionEvent => ReadMarkerGameEvent(decoder, decodedEvent.TypeId, new STriggerAbortMissionEvent(decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop)),
                GameEventType.STriggerPurchaseMadeEvent => ReadSTriggerPurchaseMadeEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerPurchaseExitEvent => ReadMarkerGameEvent(decoder, decodedEvent.TypeId, new STriggerPurchaseExitEvent(decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop)),
                GameEventType.STriggerPlanetMissionLaunchedEvent => ReadSTriggerPlanetMissionLaunchedEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerPlanetPanelCanceledEvent => ReadMarkerGameEvent(decoder, decodedEvent.TypeId, new STriggerPlanetPanelCanceledEvent(decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop)),
                GameEventType.STriggerConversationSkippedEvent => ReadSTriggerConversationSkippedEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerHotkeyPressedEvent => ReadSTriggerHotkeyPressedEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerPlanetPanelReplayEvent => ReadMarkerGameEvent(decoder, decodedEvent.TypeId, new STriggerPlanetPanelReplayEvent(decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop)),
                GameEventType.STriggerPlanetMissionSelectedEvent => ReadSTriggerPlanetMissionSelectedEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerMovieFunctionEvent => ReadSTriggerMovieFunctionEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerPlanetPanelBirthCompleteEvent => ReadMarkerGameEvent(decoder, decodedEvent.TypeId, new STriggerPlanetPanelBirthCompleteEvent(decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop)),
                GameEventType.STriggerPlanetPanelDeathCompleteEvent => ReadMarkerGameEvent(decoder, decodedEvent.TypeId, new STriggerPlanetPanelDeathCompleteEvent(decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop)),
                GameEventType.SResourceRequestEvent => ReadSResourceRequestEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SResourceRequestFulfillEvent => ReadSResourceRequestFulfillEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SResourceRequestCancelEvent => ReadSResourceRequestCancelEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerResearchPanelExitEvent => ReadMarkerGameEvent(decoder, decodedEvent.TypeId, new STriggerResearchPanelExitEvent(decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop)),
                GameEventType.STriggerResearchPanelPurchaseEvent => ReadMarkerGameEvent(decoder, decodedEvent.TypeId, new STriggerResearchPanelPurchaseEvent(decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop)),
                GameEventType.STriggerResearchPanelSelectionChangedEvent => ReadSTriggerResearchPanelSelectionChangedEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerCommandErrorEvent => ReadSTriggerCommandErrorEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerMercenaryPanelExitEvent => ReadMarkerGameEvent(decoder, decodedEvent.TypeId, new STriggerMercenaryPanelExitEvent(decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop)),
                GameEventType.STriggerMercenaryPanelPurchaseEvent => ReadMarkerGameEvent(decoder, decodedEvent.TypeId, new STriggerMercenaryPanelPurchaseEvent(decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop)),
                GameEventType.STriggerMercenaryPanelSelectionChangedEvent => ReadSTriggerMercenaryPanelSelectionChangedEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerVictoryPanelExitEvent => ReadMarkerGameEvent(decoder, decodedEvent.TypeId, new STriggerVictoryPanelExitEvent(decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop)),
                GameEventType.STriggerBattleReportPanelExitEvent => ReadMarkerGameEvent(decoder, decodedEvent.TypeId, new STriggerBattleReportPanelExitEvent(decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop)),
                GameEventType.STriggerBattleReportPanelPlayMissionEvent => ReadSTriggerBattleReportPanelPlayMissionEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerBattleReportPanelPlaySceneEvent => ReadSTriggerBattleReportPanelPlaySceneEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerBattleReportPanelSelectionChangedEvent => ReadSTriggerBattleReportPanelSelectionChangedEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerVictoryPanelPlayMissionAgainEvent => ReadSTriggerVictoryPanelPlayMissionAgainEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerMovieStartedEvent => ReadMarkerGameEvent(decoder, decodedEvent.TypeId, new STriggerMovieStartedEvent(decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop)),
                GameEventType.STriggerMovieFinishedEvent => ReadMarkerGameEvent(decoder, decodedEvent.TypeId, new STriggerMovieFinishedEvent(decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop)),
                GameEventType.STriggerPortraitLoadedEvent => ReadSTriggerPortraitLoadedEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerCustomDialogDismissedEvent => ReadSTriggerCustomDialogDismissedEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerMouseWheelEvent => ReadSTriggerMouseWheelEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerPurchasePanelSelectedPurchaseItemChangedEvent => ReadSTriggerPurchasePanelSelectedPurchaseItemChangedEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerPurchasePanelSelectedPurchaseCategoryChangedEvent => ReadSTriggerPurchasePanelSelectedPurchaseCategoryChangedEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerGameCreditsFinishedEvent => ReadMarkerGameEvent(decoder, decodedEvent.TypeId, new STriggerGameCreditsFinishedEvent(decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop)),
                GameEventType.STriggerCutsceneConversationLineEvent => ReadSTriggerCutsceneConversationLineEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerCutsceneConversationLineMissingEvent => ReadSTriggerCutsceneConversationLineMissingEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerAnimLengthQueryByNameEvent => ReadSTriggerAnimLengthQueryByNameEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerAnimLengthQueryByPropsEvent => ReadSTriggerAnimLengthQueryByPropsEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerAnimOffsetEvent => ReadSTriggerAnimOffsetEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SCatalogModifyEvent => ReadSCatalogModifyEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SHeroTalentTreeSelectedEvent => ReadSHeroTalentTreeSelectedEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.STriggerProfilerLoggingFinishedEvent => ReadMarkerGameEvent(decoder, decodedEvent.TypeId, new STriggerProfilerLoggingFinishedEvent(decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop)),
                GameEventType.SHeroTalentTreeSelectionPanelToggledEvent => ReadSHeroTalentTreeSelectionPanelToggledEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SPeerSetSyncLoadingTimeEvent => ReadSPeerSetSyncLoadingTimeEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SPeerSetSyncPlayingTimeEvent => ReadSPeerSetSyncPlayingTimeEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SLagMessageEvent => ReadSLagMessageEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.SPlayerLeaveEvent => ReadMarkerGameEvent(decoder, decodedEvent.TypeId, new SPlayerLeaveEvent(decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop)),
                GameEventType.STriggerCameraMoveEvent => ReadSTriggerCameraMoveEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop),
                GameEventType.None => ReadUnknownGameEvent(decoder, decodedEvent.TypeId, decodedEvent.UserId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop, decodedEvent.TypeName),
            };
#pragma warning restore CS8524

            events.Add(gameEvent);
        }
    }

    private readonly struct TrackerEventHandler(
        TypedProtocolDecoder decoder,
        List<SPlayerSetupEvent> playerSetupEvents,
        List<SPlayerStatsEvent> playerStatsEvents,
        List<SUnitBornEvent> unitBornEvents,
        List<SUnitDiedEvent> unitDiedEvents,
        List<SUnitOwnerChangeEvent> unitOwnerChangeEvents,
        List<SUnitPositionsEvent> unitPositionsEvents,
        List<SUnitTypeChangeEvent> unitTypeChangeEvents,
        List<SUpgradeEvent> upgradeEvents,
        List<SUnitInitEvent> unitInitEvents,
        List<SUnitDoneEvent> unitDoneEvents) : IEventHandler
    {
        public void Handle(in DecodedEvent decodedEvent)
        {
            TrackerEventType eventType = GetTrackerEventType(decodedEvent.TypeName);
            switch (eventType)
            {
                case TrackerEventType.SPlayerSetupEvent:
                    playerSetupEvents.Add(ReadSPlayerSetupEvent(decoder, decodedEvent.TypeId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop));
                    break;
                case TrackerEventType.SPlayerStatsEvent:
                    playerStatsEvents.Add(ReadSPlayerStatsEvent(decoder, decodedEvent.TypeId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop));
                    break;
                case TrackerEventType.SUnitBornEvent:
                    unitBornEvents.Add(ReadSUnitBornEvent(decoder, decodedEvent.TypeId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop));
                    break;
                case TrackerEventType.SUnitDiedEvent:
                    unitDiedEvents.Add(ReadSUnitDiedEvent(decoder, decodedEvent.TypeId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop));
                    break;
                case TrackerEventType.SUnitOwnerChangeEvent:
                    unitOwnerChangeEvents.Add(ReadSUnitOwnerChangeEvent(decoder, decodedEvent.TypeId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop));
                    break;
                case TrackerEventType.SUnitPositionsEvent:
                    unitPositionsEvents.Add(ReadSUnitPositionsEvent(decoder, decodedEvent.TypeId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop));
                    break;
                case TrackerEventType.SUnitTypeChangeEvent:
                    unitTypeChangeEvents.Add(ReadSUnitTypeChangeEvent(decoder, decodedEvent.TypeId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop));
                    break;
                case TrackerEventType.SUpgradeEvent:
                    upgradeEvents.Add(ReadSUpgradeEvent(decoder, decodedEvent.TypeId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop));
                    break;
                case TrackerEventType.SUnitInitEvent:
                    unitInitEvents.Add(ReadSUnitInitEvent(decoder, decodedEvent.TypeId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop));
                    break;
                case TrackerEventType.SUnitDoneEvent:
                    unitDoneEvents.Add(ReadSUnitDoneEvent(decoder, decodedEvent.TypeId, decodedEvent.EventId, decodedEvent.Bits, decodedEvent.Gameloop));
                    break;
                default:
                    decoder.SkipType(decodedEvent.TypeId);
                    break;
            }
        }
    }

    private void DecodeEventStream<THandler>(
        TypedProtocolDecoder decoder,
        int eventIdTypeId,
        Dictionary<int, S2EventType> eventTypes,
        bool decodeUserId,
        ref THandler handler)
        where THandler : struct, IEventHandler
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

            DecodedEvent decodedEvent = new(eventId, eventTypeInfo.Name, 0, gameloop, userId, eventTypeInfo.TypeId);
            handler.Handle(in decodedEvent);
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
        "NNet.Game.SGameUserJoinEvent" => GameEventType.SGameUserJoinEvent,
        "NNet.Game.SHijackReplayGameEvent" => GameEventType.SHijackReplayGameEvent,
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
        "NNet.Game.SSaveGameEvent" => GameEventType.SSaveGameEvent,
        "NNet.Game.SSaveGameDoneEvent" => GameEventType.SSaveGameDoneEvent,
        "NNet.Game.SLoadGameDoneEvent" => GameEventType.SLoadGameDoneEvent,
        "NNet.Game.SCommandManagerResetEvent" => GameEventType.SCommandManagerResetEvent,
        "NNet.Game.SGameCheatEvent" => GameEventType.SGameCheatEvent,
        "NNet.Game.SSelectionSyncCheckEvent" => GameEventType.SSelectionSyncCheckEvent,
        "NNet.Game.SResourceTradeEvent" => GameEventType.SResourceTradeEvent,
        "NNet.Game.SAICommunicateEvent" => GameEventType.SAICommunicateEvent,
        "NNet.Game.SSetAbsoluteGameSpeedEvent" => GameEventType.SSetAbsoluteGameSpeedEvent,
        "NNet.Game.SAddAbsoluteGameSpeedEvent" => GameEventType.SAddAbsoluteGameSpeedEvent,
        "NNet.Game.SBroadcastCheatEvent" => GameEventType.SBroadcastCheatEvent,
        "NNet.Game.SAllianceEvent" => GameEventType.SAllianceEvent,
        "NNet.Game.SUnitHighlightEvent" => GameEventType.SUnitHighlightEvent,
        "NNet.Game.STriggerReplySelectedEvent" => GameEventType.STriggerReplySelectedEvent,
        "NNet.Game.STriggerSkippedEvent" => GameEventType.STriggerSkippedEvent,
        "NNet.Game.STriggerAbortMissionEvent" => GameEventType.STriggerAbortMissionEvent,
        "NNet.Game.STriggerPurchaseMadeEvent" => GameEventType.STriggerPurchaseMadeEvent,
        "NNet.Game.STriggerPurchaseExitEvent" => GameEventType.STriggerPurchaseExitEvent,
        "NNet.Game.STriggerPlanetMissionLaunchedEvent" => GameEventType.STriggerPlanetMissionLaunchedEvent,
        "NNet.Game.STriggerPlanetPanelCanceledEvent" => GameEventType.STriggerPlanetPanelCanceledEvent,
        "NNet.Game.STriggerConversationSkippedEvent" => GameEventType.STriggerConversationSkippedEvent,
        "NNet.Game.STriggerHotkeyPressedEvent" => GameEventType.STriggerHotkeyPressedEvent,
        "NNet.Game.STriggerPlanetPanelReplayEvent" => GameEventType.STriggerPlanetPanelReplayEvent,
        "NNet.Game.STriggerPlanetMissionSelectedEvent" => GameEventType.STriggerPlanetMissionSelectedEvent,
        "NNet.Game.STriggerMovieFunctionEvent" => GameEventType.STriggerMovieFunctionEvent,
        "NNet.Game.STriggerPlanetPanelBirthCompleteEvent" => GameEventType.STriggerPlanetPanelBirthCompleteEvent,
        "NNet.Game.STriggerPlanetPanelDeathCompleteEvent" => GameEventType.STriggerPlanetPanelDeathCompleteEvent,
        "NNet.Game.SResourceRequestEvent" => GameEventType.SResourceRequestEvent,
        "NNet.Game.SResourceRequestFulfillEvent" => GameEventType.SResourceRequestFulfillEvent,
        "NNet.Game.SResourceRequestCancelEvent" => GameEventType.SResourceRequestCancelEvent,
        "NNet.Game.STriggerResearchPanelExitEvent" => GameEventType.STriggerResearchPanelExitEvent,
        "NNet.Game.STriggerResearchPanelPurchaseEvent" => GameEventType.STriggerResearchPanelPurchaseEvent,
        "NNet.Game.STriggerResearchPanelSelectionChangedEvent" => GameEventType.STriggerResearchPanelSelectionChangedEvent,
        "NNet.Game.STriggerCommandErrorEvent" => GameEventType.STriggerCommandErrorEvent,
        "NNet.Game.STriggerMercenaryPanelExitEvent" => GameEventType.STriggerMercenaryPanelExitEvent,
        "NNet.Game.STriggerMercenaryPanelPurchaseEvent" => GameEventType.STriggerMercenaryPanelPurchaseEvent,
        "NNet.Game.STriggerMercenaryPanelSelectionChangedEvent" => GameEventType.STriggerMercenaryPanelSelectionChangedEvent,
        "NNet.Game.STriggerVictoryPanelExitEvent" => GameEventType.STriggerVictoryPanelExitEvent,
        "NNet.Game.STriggerBattleReportPanelExitEvent" => GameEventType.STriggerBattleReportPanelExitEvent,
        "NNet.Game.STriggerBattleReportPanelPlayMissionEvent" => GameEventType.STriggerBattleReportPanelPlayMissionEvent,
        "NNet.Game.STriggerBattleReportPanelPlaySceneEvent" => GameEventType.STriggerBattleReportPanelPlaySceneEvent,
        "NNet.Game.STriggerBattleReportPanelSelectionChangedEvent" => GameEventType.STriggerBattleReportPanelSelectionChangedEvent,
        "NNet.Game.STriggerVictoryPanelPlayMissionAgainEvent" => GameEventType.STriggerVictoryPanelPlayMissionAgainEvent,
        "NNet.Game.STriggerMovieStartedEvent" => GameEventType.STriggerMovieStartedEvent,
        "NNet.Game.STriggerMovieFinishedEvent" => GameEventType.STriggerMovieFinishedEvent,
        "NNet.Game.STriggerPortraitLoadedEvent" => GameEventType.STriggerPortraitLoadedEvent,
        "NNet.Game.STriggerCustomDialogDismissedEvent" => GameEventType.STriggerCustomDialogDismissedEvent,
        "NNet.Game.STriggerMouseWheelEvent" => GameEventType.STriggerMouseWheelEvent,
        "NNet.Game.STriggerPurchasePanelSelectedPurchaseItemChangedEvent" => GameEventType.STriggerPurchasePanelSelectedPurchaseItemChangedEvent,
        "NNet.Game.STriggerPurchasePanelSelectedPurchaseCategoryChangedEvent" => GameEventType.STriggerPurchasePanelSelectedPurchaseCategoryChangedEvent,
        "NNet.Game.STriggerGameCreditsFinishedEvent" => GameEventType.STriggerGameCreditsFinishedEvent,
        "NNet.Game.STriggerCutsceneConversationLineEvent" => GameEventType.STriggerCutsceneConversationLineEvent,
        "NNet.Game.STriggerCutsceneConversationLineMissingEvent" => GameEventType.STriggerCutsceneConversationLineMissingEvent,
        "NNet.Game.STriggerAnimLengthQueryByNameEvent" => GameEventType.STriggerAnimLengthQueryByNameEvent,
        "NNet.Game.STriggerAnimLengthQueryByPropsEvent" => GameEventType.STriggerAnimLengthQueryByPropsEvent,
        "NNet.Game.STriggerAnimOffsetEvent" => GameEventType.STriggerAnimOffsetEvent,
        "NNet.Game.SCatalogModifyEvent" => GameEventType.SCatalogModifyEvent,
        "NNet.Game.SHeroTalentTreeSelectedEvent" => GameEventType.SHeroTalentTreeSelectedEvent,
        "NNet.Game.STriggerProfilerLoggingFinishedEvent" => GameEventType.STriggerProfilerLoggingFinishedEvent,
        "NNet.Game.SHeroTalentTreeSelectionPanelToggledEvent" => GameEventType.SHeroTalentTreeSelectionPanelToggledEvent,
        "NNet.Game.SPeerSetSyncLoadingTimeEvent" => GameEventType.SPeerSetSyncLoadingTimeEvent,
        "NNet.Game.SPeerSetSyncPlayingTimeEvent" => GameEventType.SPeerSetSyncPlayingTimeEvent,
        "NNet.Game.SLagMessageEvent" => GameEventType.SLagMessageEvent,
        "NNet.Game.SPlayerLeaveEvent" => GameEventType.SPlayerLeaveEvent,
        "NNet.Game.STriggerCameraMoveEvent" => GameEventType.STriggerCameraMoveEvent,
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

    private struct UserInitialDataReadState : IStructFieldReader
    {
        public string? Name;
        public string? ToonHandle;
        public string? ClanTag;
        public string? ClanLogo;
        public string? Hero;
        public string? Skin;
        public string? Mount;
        public int Observe;
        public int? TeamPref;
        public long CombinedRaceLevels;
        public int HighestLeague;
        public bool TestMap;
        public bool TestAuto;
        public bool Examine;
        public int TestType;
        public bool CustomInterface;
        public int? RacePreference;
        public int RandomSeed;
        public long? ScaledRating;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_name": Name = decoder.ReadString(fieldTypeId); return true;
                case "m_toonHandle": ToonHandle = decoder.ReadString(fieldTypeId); return true;
                case "m_clanTag": ClanTag = decoder.ReadString(fieldTypeId); return true;
                case "m_clanLogo": ClanLogo = decoder.ReadString(fieldTypeId); return true;
                case "m_hero": Hero = decoder.ReadString(fieldTypeId); return true;
                case "m_skin": Skin = decoder.ReadString(fieldTypeId); return true;
                case "m_mount": Mount = decoder.ReadString(fieldTypeId); return true;
                case "m_observe": Observe = decoder.ReadInt(fieldTypeId); return true;
                case "m_teamPreference": TeamPref = ReadSingleNullableStructInt(decoder, fieldTypeId, "m_team"); return true;
                case "m_combinedRaceLevels": CombinedRaceLevels = decoder.ReadLong(fieldTypeId); return true;
                case "m_highestLeague": HighestLeague = decoder.ReadInt(fieldTypeId); return true;
                case "m_testMap": TestMap = decoder.ReadBool(fieldTypeId); return true;
                case "m_testAuto": TestAuto = decoder.ReadBool(fieldTypeId); return true;
                case "m_examine": Examine = decoder.ReadBool(fieldTypeId); return true;
                case "m_testType": TestType = decoder.ReadInt(fieldTypeId); return true;
                case "m_customInterface": CustomInterface = decoder.ReadBool(fieldTypeId); return true;
                case "m_racePreference": RacePreference = ReadSingleNullableStructInt(decoder, fieldTypeId, "m_race"); return true;
                case "m_randomSeed": RandomSeed = decoder.ReadInt(fieldTypeId); return true;
                case "m_scaledRating": ScaledRating = decoder.ReadNullableLong(fieldTypeId); return true;
                default: return false;
            }
        }
    }

    private struct LobbyStateReadState : IStructFieldReader
    {
        public int MaxUsers;
        public List<Slot> Slots;
        public int DefaultDifficulty;
        public bool IsSinglePlayer;
        public int Phase;
        public int? HostUserId;
        public int MaxObservers;
        public int DefaultAIBuild;
        public int PickedMapTag;
        public long RandomSeed;
        public int GameDuration;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_maxUsers": MaxUsers = decoder.ReadInt(fieldTypeId); return true;
                case "m_slots":
                    SlotArrayReader reader = new(Slots);
                    decoder.ReadArray(fieldTypeId, ref reader);
                    return true;
                case "m_defaultDifficulty": DefaultDifficulty = decoder.ReadInt(fieldTypeId); return true;
                case "m_isSinglePlayer": IsSinglePlayer = decoder.ReadBool(fieldTypeId); return true;
                case "m_phase": Phase = decoder.ReadInt(fieldTypeId); return true;
                case "m_hostUserId": HostUserId = decoder.ReadNullableInt(fieldTypeId); return true;
                case "m_maxObservers": MaxObservers = decoder.ReadInt(fieldTypeId); return true;
                case "m_defaultAIBuild": DefaultAIBuild = decoder.ReadInt(fieldTypeId); return true;
                case "m_pickedMapTag": PickedMapTag = decoder.ReadInt(fieldTypeId); return true;
                case "m_randomSeed": RandomSeed = decoder.ReadLong(fieldTypeId); return true;
                case "m_gameDuration": GameDuration = decoder.ReadInt(fieldTypeId); return true;
                default: return false;
            }
        }
    }

    private readonly struct SlotArrayReader(List<Slot> slots) : IArrayItemReader
    {
        public bool ReadItem(TypedProtocolDecoder decoder, int itemTypeId)
        {
            slots.Add(ReadSlot(decoder, itemTypeId));
            return true;
        }
    }

    private struct SlotReadState : IStructFieldReader
    {
        public int AiBuild;
        public int TeamId;
        public int Difficulty;
        public int Handicap;
        public int Observe;
        public int Control;
        public int WorkingSetSlotId;
        public int? UserId;
        public int? RacePref;
        public int? ColorPref;
        public string? ToonHandle;
        public string? Skin;
        public string? Hero;
        public string? Commander;
        public string? Mount;
        public List<int> Rewards;
        public List<int> RewardOverrides;
        public List<int> Licenses;
        public List<int> CommanderMasteryTalents;
        public List<string> Artifacts;
        public bool HasSilencePenalty;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_aiBuild": AiBuild = decoder.ReadInt(fieldTypeId); return true;
                case "m_teamId": TeamId = decoder.ReadInt(fieldTypeId); return true;
                case "m_difficulty": Difficulty = decoder.ReadInt(fieldTypeId); return true;
                case "m_handicap": Handicap = decoder.ReadInt(fieldTypeId); return true;
                case "m_observe": Observe = decoder.ReadInt(fieldTypeId); return true;
                case "m_control": Control = decoder.ReadInt(fieldTypeId); return true;
                case "m_workingSetSlotId": WorkingSetSlotId = decoder.ReadInt(fieldTypeId); return true;
                case "m_userId": UserId = decoder.ReadNullableInt(fieldTypeId); return true;
                case "m_racePref": RacePref = ReadSingleNullableStructInt(decoder, fieldTypeId, "m_race"); return true;
                case "m_colorPref": ColorPref = ReadSingleNullableStructInt(decoder, fieldTypeId, "m_color"); return true;
                case "m_toonHandle": ToonHandle = decoder.ReadString(fieldTypeId); return true;
                case "m_skin": Skin = decoder.ReadString(fieldTypeId); return true;
                case "m_hero": Hero = decoder.ReadString(fieldTypeId); return true;
                case "m_commander": Commander = decoder.ReadString(fieldTypeId); return true;
                case "m_mount": Mount = decoder.ReadString(fieldTypeId); return true;
                case "m_rewards": Rewards = decoder.ReadIntList(fieldTypeId); return true;
                case "m_rewardOverrides": RewardOverrides = decoder.ReadIntList(fieldTypeId); return true;
                case "m_licenses": Licenses = decoder.ReadIntList(fieldTypeId); return true;
                case "m_commanderMasteryTalents": CommanderMasteryTalents = decoder.ReadIntList(fieldTypeId); return true;
                case "m_artifacts": Artifacts = decoder.ReadStringList(fieldTypeId); return true;
                case "m_hasSilencePenalty": HasSilencePenalty = decoder.ReadBool(fieldTypeId); return true;
                default: return false;
            }
        }
    }

    private struct GameDescriptionReadState : IStructFieldReader
    {
        public GameOptions GameOptions;
        public List<string> CacheHandles;
        public List<SlotDescription> SlotDescriptions;
        public int MaxRaces;
        public int MaxTeams;
        public int MaxColors;
        public int DefaultDifficulty;
        public int DefaultAIBuild;
        public int GameType;
        public int MaxObservers;
        public int MaxUsers;
        public int MaxPlayers;
        public int GameSpeed;
        public int MaxControls;
        public int MapSizeY;
        public int MapSizeX;
        public bool HasExtensionMod;
        public bool IsBlizzardMap;
        public bool IsCoopMode;
        public bool HasNonBlizzardExtensionMod;
        public bool IsRealtimeMode;
        public bool IsPremadeFFA;
        public string? MapFileName;
        public string? GameCacheName;
        public string? MapAuthorName;
        public long RandomValue;
        public long ModFileSyncChecksum;
        public long MapFileSyncChecksum;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_maxRaces": MaxRaces = decoder.ReadInt(fieldTypeId); return true;
                case "m_maxTeams": MaxTeams = decoder.ReadInt(fieldTypeId); return true;
                case "m_maxColors": MaxColors = decoder.ReadInt(fieldTypeId); return true;
                case "m_defaultDifficulty": DefaultDifficulty = decoder.ReadInt(fieldTypeId); return true;
                case "m_defaultAIBuild": DefaultAIBuild = decoder.ReadInt(fieldTypeId); return true;
                case "m_gameType": GameType = decoder.ReadInt(fieldTypeId); return true;
                case "m_maxObservers": MaxObservers = decoder.ReadInt(fieldTypeId); return true;
                case "m_maxUsers": MaxUsers = decoder.ReadInt(fieldTypeId); return true;
                case "m_maxPlayers": MaxPlayers = decoder.ReadInt(fieldTypeId); return true;
                case "m_gameSpeed": GameSpeed = decoder.ReadInt(fieldTypeId); return true;
                case "m_maxControls": MaxControls = decoder.ReadInt(fieldTypeId); return true;
                case "m_mapSizeY": MapSizeY = decoder.ReadInt(fieldTypeId); return true;
                case "m_mapSizeX": MapSizeX = decoder.ReadInt(fieldTypeId); return true;
                case "m_hasExtensionMod": HasExtensionMod = decoder.ReadBool(fieldTypeId); return true;
                case "m_isBlizzardMap": IsBlizzardMap = decoder.ReadBool(fieldTypeId); return true;
                case "m_isCoopMode": IsCoopMode = decoder.ReadBool(fieldTypeId); return true;
                case "m_hasNonBlizzardExtensionMod": HasNonBlizzardExtensionMod = decoder.ReadBool(fieldTypeId); return true;
                case "m_isRealtimeMode": IsRealtimeMode = decoder.ReadBool(fieldTypeId); return true;
                case "m_isPremadeFFA": IsPremadeFFA = decoder.ReadBool(fieldTypeId); return true;
                case "m_mapFileName": MapFileName = decoder.ReadString(fieldTypeId); return true;
                case "m_gameCacheName": GameCacheName = decoder.ReadString(fieldTypeId); return true;
                case "m_mapAuthorName": MapAuthorName = decoder.ReadString(fieldTypeId); return true;
                case "m_randomValue": RandomValue = decoder.ReadLong(fieldTypeId); return true;
                case "m_modFileSyncChecksum": ModFileSyncChecksum = decoder.ReadLong(fieldTypeId); return true;
                case "m_mapFileSyncChecksum": MapFileSyncChecksum = decoder.ReadLong(fieldTypeId); return true;
                case "m_cacheHandles": CacheHandles = decoder.ReadStringList(fieldTypeId); return true;
                case "m_gameOptions": GameOptions = ReadGameOptions(decoder, fieldTypeId); return true;
                case "m_slotDescriptions":
                    SlotDescriptionArrayReader reader = new(SlotDescriptions);
                    decoder.ReadArray(fieldTypeId, ref reader);
                    return true;
                default: return false;
            }
        }
    }

    private readonly struct SlotDescriptionArrayReader(List<SlotDescription> slotDescriptions) : IArrayItemReader
    {
        public bool ReadItem(TypedProtocolDecoder decoder, int itemTypeId)
        {
            slotDescriptions.Add(ReadSlotDescription(decoder, itemTypeId));
            return true;
        }
    }

    private struct GameOptionsReadState : IStructFieldReader
    {
        public bool Competitive;
        public bool Practice;
        public bool LockTeams;
        public bool Amm;
        public bool BattleNet;
        public bool NoVictoryOrDefeat;
        public bool HeroDuplicatesAllowed;
        public bool AdvancedSharedControl;
        public bool Cooperative;
        public bool TeamsTogether;
        public bool RandomRaces;
        public bool BuildCoachEnabled;
        public int Fog;
        public int UserDifficulty;
        public int Observers;
        public long ClientDebugFlags;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_competitive": Competitive = decoder.ReadBool(fieldTypeId); return true;
                case "m_practice": Practice = decoder.ReadBool(fieldTypeId); return true;
                case "m_lockTeams": LockTeams = decoder.ReadBool(fieldTypeId); return true;
                case "m_amm": Amm = decoder.ReadBool(fieldTypeId); return true;
                case "m_battleNet": BattleNet = decoder.ReadBool(fieldTypeId); return true;
                case "m_noVictoryOrDefeat": NoVictoryOrDefeat = decoder.ReadBool(fieldTypeId); return true;
                case "m_heroDuplicatesAllowed": HeroDuplicatesAllowed = decoder.ReadBool(fieldTypeId); return true;
                case "m_advancedSharedControl": AdvancedSharedControl = decoder.ReadBool(fieldTypeId); return true;
                case "m_cooperative": Cooperative = decoder.ReadBool(fieldTypeId); return true;
                case "m_teamsTogether": TeamsTogether = decoder.ReadBool(fieldTypeId); return true;
                case "m_randomRaces": RandomRaces = decoder.ReadBool(fieldTypeId); return true;
                case "m_buildCoachEnabled": BuildCoachEnabled = decoder.ReadBool(fieldTypeId); return true;
                case "m_fog": Fog = decoder.ReadInt(fieldTypeId); return true;
                case "m_userDifficulty": UserDifficulty = decoder.ReadInt(fieldTypeId); return true;
                case "m_observers": Observers = decoder.ReadInt(fieldTypeId); return true;
                case "m_clientDebugFlags": ClientDebugFlags = decoder.ReadLong(fieldTypeId); return true;
                default: return false;
            }
        }
    }

    private struct SlotDescriptionReadState : IStructFieldReader
    {
        public KeyValuePair<int, BigInteger> AllowedRaces;
        public KeyValuePair<int, BigInteger> AllowedColors;
        public KeyValuePair<int, BigInteger> AllowedAIBuilds;
        public KeyValuePair<int, BigInteger> AllowedDifficulty;
        public KeyValuePair<int, BigInteger> AllowedObserveTypes;
        public KeyValuePair<int, BigInteger> AllowedControls;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_allowedRaces": AllowedRaces = decoder.ReadBitArray(fieldTypeId); return true;
                case "m_allowedColors": AllowedColors = decoder.ReadBitArray(fieldTypeId); return true;
                case "m_allowedAIBuilds": AllowedAIBuilds = decoder.ReadBitArray(fieldTypeId); return true;
                case "m_allowedDifficulty": AllowedDifficulty = decoder.ReadBitArray(fieldTypeId); return true;
                case "m_allowedObserveTypes": AllowedObserveTypes = decoder.ReadBitArray(fieldTypeId); return true;
                case "m_allowedControls": AllowedControls = decoder.ReadBitArray(fieldTypeId); return true;
                default: return false;
            }
        }
    }

    private struct SingleNullableStructIntReadState(string fieldName) : IStructFieldReader
    {
        public int? Result;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            if (name != fieldName)
            {
                return false;
            }

            Result = decoder.ReadNullableInt(fieldTypeId);
            return true;
        }
    }

    private static UserInitialData ReadUserInitialData(TypedProtocolDecoder decoder, int typeId)
    {
        UserInitialDataReadState state = default;
        decoder.ReadStruct(typeId, ref state);

        return new UserInitialData(state.Mount ?? string.Empty, state.Skin ?? string.Empty, state.Observe, state.TeamPref,
            state.ToonHandle ?? string.Empty, state.CombinedRaceLevels, state.HighestLeague, state.ClanTag ?? string.Empty,
            state.TestMap, state.TestAuto, state.Examine, state.TestType, state.CustomInterface,
            state.ClanLogo ?? string.Empty, state.Name ?? string.Empty, state.RacePreference, state.RandomSeed,
            state.Hero ?? string.Empty, state.ScaledRating);
    }

    private static LobbyState ReadLobbyState(TypedProtocolDecoder decoder, int typeId)
    {
        LobbyStateReadState state = new()
        {
            Slots = [],
        };

        decoder.ReadStruct(typeId, ref state);

        return new LobbyState(state.MaxUsers, state.Slots, state.DefaultDifficulty, state.IsSinglePlayer, state.Phase,
            state.HostUserId, state.MaxObservers, state.DefaultAIBuild, state.PickedMapTag, state.RandomSeed,
            state.GameDuration);
    }

    private static Slot ReadSlot(TypedProtocolDecoder decoder, int typeId)
    {
        SlotReadState state = new()
        {
            Rewards = [],
            RewardOverrides = [],
            Licenses = [],
            CommanderMasteryTalents = [],
            Artifacts = [],
        };

        decoder.ReadStruct(typeId, ref state);

        return new Slot(0, state.ToonHandle ?? string.Empty, state.RewardOverrides, state.UserId,
            state.Skin ?? string.Empty, state.CommanderMasteryTalents, state.AiBuild, state.TeamId,
            state.Rewards, 0, 0, state.Artifacts, state.Difficulty, null, 0, 0, 0, state.RacePref, null,
            state.Hero ?? string.Empty, state.Commander ?? string.Empty, state.Mount ?? string.Empty, state.Handicap,
            state.Observe, 0, state.Control, state.Licenses, state.ColorPref, state.HasSilencePenalty,
            state.WorkingSetSlotId, [], 0);
    }

    private static GameDescription ReadGameDescription(TypedProtocolDecoder decoder, int typeId)
    {
        GameDescriptionReadState state = new()
        {
            GameOptions = new GameOptions(false, false, false, false, false, 0, false, false, 0, false, false, 0, 0, false, false, false),
            CacheHandles = [],
            SlotDescriptions = [],
        };

        decoder.ReadStruct(typeId, ref state);

        return new GameDescription(state.MaxRaces, state.MaxTeams, state.HasExtensionMod, state.MaxColors,
            state.IsBlizzardMap, state.GameOptions, state.DefaultDifficulty, state.IsCoopMode,
            state.MapFileName ?? string.Empty, state.DefaultAIBuild, state.GameType, state.HasNonBlizzardExtensionMod,
            state.RandomValue, state.MaxObservers, state.IsRealtimeMode, state.MaxUsers, state.ModFileSyncChecksum,
            state.MapFileSyncChecksum, state.MaxPlayers, state.CacheHandles, state.GameSpeed, state.MaxControls,
            state.GameCacheName ?? string.Empty, state.MapAuthorName ?? string.Empty, state.SlotDescriptions,
            state.MapSizeY, state.MapSizeX, state.IsPremadeFFA);
    }

    private static GameOptions ReadGameOptions(TypedProtocolDecoder decoder, int typeId)
    {
        GameOptionsReadState state = default;
        decoder.ReadStruct(typeId, ref state);

        return new GameOptions(state.Competitive, state.Practice, state.LockTeams, state.Amm, state.BattleNet,
            state.Fog, state.NoVictoryOrDefeat, state.HeroDuplicatesAllowed, state.UserDifficulty,
            state.AdvancedSharedControl, state.Cooperative, state.ClientDebugFlags, state.Observers,
            state.TeamsTogether, state.RandomRaces, state.BuildCoachEnabled);
    }

    private static SlotDescription ReadSlotDescription(TypedProtocolDecoder decoder, int typeId)
    {
        SlotDescriptionReadState state = default;
        decoder.ReadStruct(typeId, ref state);

        return new SlotDescription(state.AllowedRaces, state.AllowedColors, state.AllowedAIBuilds,
            state.AllowedDifficulty, state.AllowedObserveTypes, state.AllowedControls);
    }

    private static int? ReadSingleNullableStructInt(TypedProtocolDecoder decoder, int typeId, string fieldName)
    {
        SingleNullableStructIntReadState state = new(fieldName);
        decoder.ReadStruct(typeId, ref state);
        return state.Result;
    }
}

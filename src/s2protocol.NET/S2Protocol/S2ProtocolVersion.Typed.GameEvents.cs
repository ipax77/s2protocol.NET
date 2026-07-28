using s2protocol.NET.Models;

namespace s2protocol.NET.S2Protocol;

public sealed partial record S2ProtocolVersion
{
    private static SUserFinishedLoadingSyncEvent ReadEmptyGameEvent(
        TypedProtocolDecoder decoder,
        int typeId,
        int userId,
        int eventId,
        int bits,
        int gameloop)
    {
        decoder.SkipType(typeId);
        return new SUserFinishedLoadingSyncEvent(userId, eventId, bits, gameloop);
    }

    private static STriggerSoundLengthSyncEvent ReadSoundLengthSyncEvent(
        TypedProtocolDecoder decoder,
        int typeId,
        int userId,
        int eventId,
        int bits,
        int gameloop)
    {
        decoder.SkipType(typeId);
        return new STriggerSoundLengthSyncEvent(userId, eventId, bits, gameloop);
    }

    private static UnknownGameEvent ReadUnknownGameEvent(
        TypedProtocolDecoder decoder,
        int typeId,
        int userId,
        int eventId,
        int bits,
        int gameloop,
        string eventName)
    {
        decoder.SkipType(typeId);
        return new UnknownGameEvent(userId, eventId, bits, gameloop, eventName);
    }

    private static SCmdEvent ReadSCmdEvent(
        TypedProtocolDecoder decoder,
        int typeId,
        int userId,
        int eventId,
        int bits,
        int gameloop)
    {
        SCmdEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new SCmdEvent(
            userId,
            eventId,
            bits,
            gameloop,
            state.UnitGroup,
            state.AbilLink,
            state.AbilCmdIndex,
            state.AbilCmdData,
            state.TargetX,
            state.TargetY,
            state.TargetZ,
            state.CmdFlags,
            state.Sequence,
            state.OtherUnit,
            state.TargetUnitTag);
    }

    private struct SCmdEventReadState : IStructFieldReader
    {
        public int? UnitGroup;
        public int AbilLink;
        public int AbilCmdIndex;
        public string? AbilCmdData;
        public long? TargetX;
        public long? TargetY;
        public long? TargetZ;
        public int CmdFlags;
        public int Sequence;
        public int? OtherUnit;
        public int? TargetUnitTag;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_unitGroup":
                    UnitGroup = decoder.ReadNullableInt(fieldTypeId);
                    return true;
                case "m_abil":
                    SCmdAbilityReadState ability = default;
                    decoder.ReadStruct(fieldTypeId, ref ability);
                    AbilLink = ability.AbilLink;
                    AbilCmdIndex = ability.AbilCmdIndex;
                    AbilCmdData = ability.AbilCmdData;
                    return true;
                case "m_data":
                    SCmdDataChoiceHandler data = default;
                    decoder.ReadChoice(fieldTypeId, ref data);
                    TargetX = data.TargetX;
                    TargetY = data.TargetY;
                    TargetZ = data.TargetZ;
                    TargetUnitTag = data.TargetUnitTag;
                    return true;
                case "m_cmdFlags":
                    CmdFlags = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_sequence":
                    Sequence = decoder.ReadNullableInt(fieldTypeId) ?? 0;
                    return true;
                case "m_otherUnit":
                    OtherUnit = decoder.ReadNullableInt(fieldTypeId);
                    return true;
                default:
                    return false;
            }
        }
    }

    private struct SCmdAbilityReadState : IStructFieldReader
    {
        public int AbilLink;
        public int AbilCmdIndex;
        public string? AbilCmdData;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_abilLink":
                    AbilLink = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_abilCmdIndex":
                    AbilCmdIndex = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_abilCmdData":
                    AbilCmdData = decoder.ReadNullableString(fieldTypeId);
                    return true;
                default:
                    return false;
            }
        }
    }

    private struct SCmdDataChoiceHandler : IChoiceHandler
    {
        public long? TargetX;
        public long? TargetY;
        public long? TargetZ;
        public int? TargetUnitTag;

        public bool HandleChoice(TypedProtocolDecoder decoder, string name, int choiceTypeId)
        {
            switch (name)
            {
                case "TargetPoint":
                    SCmdTargetPointReadState point = default;
                    decoder.ReadStruct(choiceTypeId, ref point);
                    SetTarget(point);
                    return true;
                case "TargetUnit":
                    SCmdTargetUnitReadState unit = default;
                    decoder.ReadStruct(choiceTypeId, ref unit);
                    TargetUnitTag = unit.Tag;
                    if (unit.HasSnapshotPoint)
                    {
                        SetTarget(unit.SnapshotPoint);
                    }
                    return true;
                default:
                    return false;
            }
        }

        private void SetTarget(SCmdTargetPointReadState point)
        {
            TargetX = point.X;
            TargetY = point.Y;
            TargetZ = point.Z;
        }
    }

    private struct SCmdTargetUnitReadState : IStructFieldReader
    {
        public bool HasSnapshotPoint;
        public SCmdTargetPointReadState SnapshotPoint;
        public int? Tag;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            if (name == "m_tag")
            {
                Tag = decoder.ReadInt(fieldTypeId);
                return true;
            }

            if (name != "m_snapshotPoint")
            {
                return false;
            }

            decoder.ReadStruct(fieldTypeId, ref SnapshotPoint);
            HasSnapshotPoint = true;
            return true;
        }
    }

    private struct SCmdTargetPointReadState : IStructFieldReader
    {
        public long X;
        public long Y;
        public long Z;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "x":
                    X = decoder.ReadLong(fieldTypeId);
                    return true;
                case "y":
                    Y = decoder.ReadLong(fieldTypeId);
                    return true;
                case "z":
                    Z = decoder.ReadLong(fieldTypeId);
                    return true;
                default:
                    return false;
            }
        }
    }

    private static SCommandManagerStateEvent ReadSCommandManagerStateEvent(
        TypedProtocolDecoder decoder,
        int typeId,
        int userId,
        int eventId,
        int bits,
        int gameloop)
    {
        SCommandManagerStateEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new SCommandManagerStateEvent(
            userId,
            eventId,
            bits,
            gameloop,
            state.State,
            state.Sequence);
    }

    private struct SCommandManagerStateEventReadState : IStructFieldReader
    {
        public int State;
        public int? Sequence;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_state":
                    State = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_sequence":
                    Sequence = decoder.ReadNullableInt(fieldTypeId);
                    return true;
                default:
                    return false;
            }
        }
    }

    private static SSelectionDeltaEvent ReadSSelectionDeltaEvent(
        TypedProtocolDecoder decoder,
        int typeId,
        int userId,
        int eventId,
        int bits,
        int gameloop)
    {
        SSelectionDeltaEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);

        ICollection<int> addUnitTags = state.AddUnitTags is { } unitTags
            ? unitTags
            : Array.Empty<int>();
        ICollection<SelectionDeltaEventDeltaSubGroup> addSubgroups =
            state.AddSubgroups is { } subgroups
                ? subgroups
                : Array.Empty<SelectionDeltaEventDeltaSubGroup>();
        ICollection<int> zeroIndices = state.ZeroIndices is { } indices
            ? indices
            : Array.Empty<int>();
        SelectionDeltaEventDelta delta = new(
            addUnitTags,
            addSubgroups,
            zeroIndices,
            state.SubgroupIndex);

        return new SSelectionDeltaEvent(
            userId,
            eventId,
            bits,
            gameloop,
            delta,
            state.ControlGroupId);
    }

    private struct SSelectionDeltaEventReadState : IStructFieldReader
    {
        public List<int>? AddUnitTags;
        public List<SelectionDeltaEventDeltaSubGroup>? AddSubgroups;
        public List<int>? ZeroIndices;
        public int SubgroupIndex;
        public int ControlGroupId;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_delta":
                    SelectionDeltaReadState delta = default;
                    decoder.ReadStruct(fieldTypeId, ref delta);
                    AddUnitTags = delta.AddUnitTags;
                    AddSubgroups = delta.AddSubgroups;
                    ZeroIndices = delta.ZeroIndices;
                    SubgroupIndex = delta.SubgroupIndex;
                    return true;
                case "m_controlGroupId":
                    ControlGroupId = decoder.ReadInt(fieldTypeId);
                    return true;
                default:
                    return false;
            }
        }
    }

    private struct SelectionDeltaReadState : IStructFieldReader
    {
        public List<int>? AddUnitTags;
        public List<SelectionDeltaEventDeltaSubGroup>? AddSubgroups;
        public List<int>? ZeroIndices;
        public int SubgroupIndex;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_addUnitTags":
                    IntArrayReader unitTagReader = default;
                    decoder.ReadArray(fieldTypeId, ref unitTagReader);
                    AddUnitTags = unitTagReader.Values;
                    return true;
                case "m_addSubgroups":
                    SelectionSubgroupArrayReader subgroupReader = default;
                    decoder.ReadArray(fieldTypeId, ref subgroupReader);
                    AddSubgroups = subgroupReader.Values;
                    return true;
                case "m_removeMask":
                    SelectionRemoveMaskChoiceHandler removeMask = default;
                    decoder.ReadChoice(fieldTypeId, ref removeMask);
                    ZeroIndices = removeMask.ZeroIndices;
                    return true;
                case "m_subgroupIndex":
                    SubgroupIndex = decoder.ReadInt(fieldTypeId);
                    return true;
                default:
                    return false;
            }
        }
    }

    private struct IntArrayReader : IArrayItemReader
    {
        public List<int>? Values;

        public bool ReadItem(TypedProtocolDecoder decoder, int itemTypeId)
        {
            (Values ??= []).Add(decoder.ReadInt(itemTypeId));
            return true;
        }
    }

    private struct SelectionSubgroupArrayReader : IArrayItemReader
    {
        public List<SelectionDeltaEventDeltaSubGroup>? Values;

        public bool ReadItem(TypedProtocolDecoder decoder, int itemTypeId)
        {
            SelectionSubgroupReadState state = default;
            decoder.ReadStruct(itemTypeId, ref state);
            (Values ??= []).Add(new SelectionDeltaEventDeltaSubGroup(
                state.UnitLink,
                state.SubgroupPriority,
                state.Count,
                state.IntraSubgroupPriority));
            return true;
        }
    }

    private struct SelectionSubgroupReadState : IStructFieldReader
    {
        public int UnitLink;
        public int SubgroupPriority;
        public int Count;
        public int IntraSubgroupPriority;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_unitLink":
                    UnitLink = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_subgroupPriority":
                    SubgroupPriority = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_count":
                    Count = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_intraSubgroupPriority":
                    IntraSubgroupPriority = decoder.ReadInt(fieldTypeId);
                    return true;
                default:
                    return false;
            }
        }
    }

    private struct SelectionRemoveMaskChoiceHandler : IChoiceHandler
    {
        public List<int>? ZeroIndices;

        public bool HandleChoice(TypedProtocolDecoder decoder, string name, int choiceTypeId)
        {
            if (name != "ZeroIndices")
            {
                return false;
            }

            IntArrayReader reader = default;
            decoder.ReadArray(choiceTypeId, ref reader);
            ZeroIndices = reader.Values;
            return true;
        }
    }

    private static SHijackReplayGameEvent ReadSHijackReplayGameEvent(
        TypedProtocolDecoder decoder,
        int typeId,
        int userId,
        int eventId,
        int bits,
        int gameloop)
    {
        SHijackReplayGameEventReadState state = new() { UserInfos = [] };
        decoder.ReadStruct(typeId, ref state);
        return new SHijackReplayGameEvent(userId, eventId, bits, gameloop, state.UserInfos, state.Method);
    }

    private struct SHijackReplayGameEventReadState : IStructFieldReader
    {
        public List<HijackReplayUserInfo> UserInfos;
        public int Method;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_userInfos":
                    HijackReplayUserInfoArrayReader users = new(UserInfos);
                    decoder.ReadArray(fieldTypeId, ref users);
                    return true;
                case "m_method":
                    Method = decoder.ReadInt(fieldTypeId);
                    return true;
                default:
                    return false;
            }
        }
    }

    private readonly struct HijackReplayUserInfoArrayReader(List<HijackReplayUserInfo> users) : IArrayItemReader
    {
        public bool ReadItem(TypedProtocolDecoder decoder, int itemTypeId)
        {
            HijackReplayUserInfoReadState state = default;
            decoder.ReadStruct(itemTypeId, ref state);
            users.Add(new HijackReplayUserInfo(
                state.GameUserId,
                state.Observe,
                state.Name ?? string.Empty,
                state.ToonHandle,
                state.ClanTag,
                state.ClanLogo));
            return true;
        }
    }

    private struct HijackReplayUserInfoReadState : IStructFieldReader
    {
        public int GameUserId;
        public int Observe;
        public string? Name;
        public string? ToonHandle;
        public string? ClanTag;
        public string? ClanLogo;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_gameUserId":
                    GameUserId = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_observe":
                    Observe = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_name":
                    Name = decoder.ReadString(fieldTypeId);
                    return true;
                case "m_toonHandle":
                    ToonHandle = decoder.ReadNullableString(fieldTypeId);
                    return true;
                case "m_clanTag":
                    ClanTag = decoder.ReadNullableString(fieldTypeId);
                    return true;
                case "m_clanLogo":
                    ClanLogo = decoder.ReadNullableString(fieldTypeId);
                    return true;
                default:
                    return false;
            }
        }
    }

    private static SGameUserJoinEvent ReadSGameUserJoinEvent(
        TypedProtocolDecoder decoder,
        int typeId,
        int userId,
        int eventId,
        int bits,
        int gameloop)
    {
        SGameUserJoinEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new SGameUserJoinEvent(
            userId,
            eventId,
            bits,
            gameloop,
            state.Observe,
            state.Name ?? string.Empty,
            state.ToonHandle,
            state.ClanTag,
            state.ClanLogo,
            state.Hijack,
            state.HijackCloneGameUserId);
    }

    private struct SGameUserJoinEventReadState : IStructFieldReader
    {
        public int Observe;
        public string? Name;
        public string? ToonHandle;
        public string? ClanTag;
        public string? ClanLogo;
        public bool Hijack;
        public int? HijackCloneGameUserId;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_observe":
                    Observe = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_name":
                    Name = decoder.ReadString(fieldTypeId);
                    return true;
                case "m_toonHandle":
                    ToonHandle = decoder.ReadNullableString(fieldTypeId);
                    return true;
                case "m_clanTag":
                    ClanTag = decoder.ReadNullableString(fieldTypeId);
                    return true;
                case "m_clanLogo":
                    ClanLogo = decoder.ReadNullableString(fieldTypeId);
                    return true;
                case "m_hijack":
                    Hijack = decoder.ReadBool(fieldTypeId);
                    return true;
                case "m_hijackCloneGameUserId":
                    HijackCloneGameUserId = decoder.ReadNullableInt(fieldTypeId);
                    return true;
                default:
                    return false;
            }
        }
    }

    private static SBankFileEvent ReadSBankFileEvent(TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        SBankFileEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new SBankFileEvent(userId, eventId, bits, gameloop, state.Name ?? string.Empty);
    }

    private struct SBankFileEventReadState : IStructFieldReader
    {
        public string? Name;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_name":
                case "m_fileName":
                    Name = decoder.ReadString(fieldTypeId);
                    return true;
                default:
                    return false;
            }
        }
    }

    private static SBankKeyEvent ReadSBankKeyEvent(TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        SBankKeyEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new SBankKeyEvent(userId, eventId, bits, gameloop, state.Name ?? string.Empty, state.Data ?? string.Empty, state.Type);
    }

    private struct SBankKeyEventReadState : IStructFieldReader
    {
        public string? Name;
        public string? Data;
        public int Type;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_name":
                case "m_fileName":
                    Name = decoder.ReadString(fieldTypeId);
                    return true;
                case "m_data":
                    Data = decoder.ReadString(fieldTypeId);
                    return true;
                case "m_type":
                    Type = decoder.ReadInt(fieldTypeId);
                    return true;
                default:
                    return false;
            }
        }
    }

    private static SBankSectionEvent ReadSBankSectionEvent(TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        SBankSectionEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new SBankSectionEvent(userId, eventId, bits, gameloop, state.Name ?? string.Empty);
    }

    private struct SBankSectionEventReadState : IStructFieldReader
    {
        public string? Name;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_name":
                case "m_fileName":
                    Name = decoder.ReadString(fieldTypeId);
                    return true;
                default:
                    return false;
            }
        }
    }

    private static SBankValueEvent ReadSBankValueEvent(TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        SBankValueEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new SBankValueEvent(userId, eventId, bits, gameloop, state.Name ?? string.Empty, state.Data ?? string.Empty, state.Type);
    }

    private struct SBankValueEventReadState : IStructFieldReader
    {
        public string? Name;
        public string? Data;
        public int Type;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_name":
                case "m_fileName":
                    Name = decoder.ReadString(fieldTypeId);
                    return true;
                case "m_data":
                    Data = decoder.ReadString(fieldTypeId);
                    return true;
                case "m_type":
                    Type = decoder.ReadInt(fieldTypeId);
                    return true;
                default:
                    return false;
            }
        }
    }

    private static SControlGroupUpdateEvent ReadSControlGroupUpdateEvent(TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        SControlGroupUpdateEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new SControlGroupUpdateEvent(userId, eventId, bits, gameloop, state.ControlGroupUpdate);
    }

    private struct SControlGroupUpdateEventReadState : IStructFieldReader
    {
        public int ControlGroupUpdate;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            if (name != "m_controlGroupUpdate")
            {
                return false;
            }

            ControlGroupUpdate = decoder.ReadInt(fieldTypeId);
            return true;
        }
    }

    private static SGameUserLeaveEvent ReadSGameUserLeaveEvent(TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        SGameUserLeaveEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new SGameUserLeaveEvent(userId, eventId, bits, gameloop, state.LeaveReason);
    }

    private struct SGameUserLeaveEventReadState : IStructFieldReader
    {
        public int LeaveReason;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            if (name != "m_leaveReason")
            {
                return false;
            }

            LeaveReason = decoder.ReadInt(fieldTypeId);
            return true;
        }
    }

    private static SSetSyncLoadingTimeEvent ReadSSetSyncLoadingTimeEvent(TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        SSyncTimeEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new SSetSyncLoadingTimeEvent(userId, eventId, bits, gameloop, state.SyncTime);
    }

    private static SSetSyncPlayingTimeEvent ReadSSetSyncPlayingTimeEvent(TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        SSyncTimeEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new SSetSyncPlayingTimeEvent(userId, eventId, bits, gameloop, state.SyncTime);
    }

    private struct SSyncTimeEventReadState : IStructFieldReader
    {
        public int SyncTime;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            if (name != "m_syncTime")
            {
                return false;
            }

            SyncTime = decoder.ReadInt(fieldTypeId);
            return true;
        }
    }

    private static STriggerButtonPressedEvent ReadSTriggerButtonPressedEvent(TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        STriggerButtonPressedEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new STriggerButtonPressedEvent(userId, eventId, bits, gameloop, state.Button);
    }

    private struct STriggerButtonPressedEventReadState : IStructFieldReader
    {
        public int Button;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            if (name != "m_button")
            {
                return false;
            }

            Button = decoder.ReadInt(fieldTypeId);
            return true;
        }
    }

    private static STriggerChatMessageEvent ReadSTriggerChatMessageEvent(TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        STriggerChatMessageEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new STriggerChatMessageEvent(userId, eventId, bits, gameloop, state.ChatMessage ?? string.Empty);
    }

    private struct STriggerChatMessageEventReadState : IStructFieldReader
    {
        public string? ChatMessage;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            if (name != "m_chatMessage")
            {
                return false;
            }

            ChatMessage = decoder.ReadString(fieldTypeId);
            return true;
        }
    }

    private static STriggerCutsceneEndSceneFiredEvent ReadSTriggerCutsceneEndSceneFiredEvent(TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        STriggerCutsceneEndSceneFiredEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new STriggerCutsceneEndSceneFiredEvent(userId, eventId, bits, gameloop, state.CutsceneId);
    }

    private struct STriggerCutsceneEndSceneFiredEventReadState : IStructFieldReader
    {
        public long CutsceneId;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            if (name != "m_cutsceneId")
            {
                return false;
            }

            CutsceneId = decoder.ReadLong(fieldTypeId);
            return true;
        }
    }

    private static STriggerGameMenuItemSelectedEvent ReadSTriggerGameMenuItemSelectedEvent(TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        STriggerGameMenuItemSelectedEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new STriggerGameMenuItemSelectedEvent(userId, eventId, bits, gameloop, state.GameMenuItemIndex);
    }

    private struct STriggerGameMenuItemSelectedEventReadState : IStructFieldReader
    {
        public long GameMenuItemIndex;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            if (name != "m_gameMenuItemIndex")
            {
                return false;
            }

            GameMenuItemIndex = decoder.ReadLong(fieldTypeId);
            return true;
        }
    }

    private static STriggerSoundOffsetEvent ReadSTriggerSoundOffsetEvent(TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        STriggerSoundOffsetEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new STriggerSoundOffsetEvent(userId, eventId, bits, gameloop, state.Sound);
    }

    private struct STriggerSoundOffsetEventReadState : IStructFieldReader
    {
        public int Sound;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            if (name != "m_sound")
            {
                return false;
            }

            Sound = decoder.ReadInt(fieldTypeId);
            return true;
        }
    }

    private static STriggerSoundtrackDoneEvent ReadSTriggerSoundtrackDoneEvent(TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        STriggerSoundtrackDoneEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new STriggerSoundtrackDoneEvent(userId, eventId, bits, gameloop, state.Soundtrack);
    }

    private struct STriggerSoundtrackDoneEventReadState : IStructFieldReader
    {
        public int Soundtrack;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            if (name != "m_soundtrack")
            {
                return false;
            }

            Soundtrack = decoder.ReadInt(fieldTypeId);
            return true;
        }
    }

    private static STriggerTransmissionCompleteEvent ReadSTriggerTransmissionCompleteEvent(TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        STriggerTransmissionCompleteEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new STriggerTransmissionCompleteEvent(userId, eventId, bits, gameloop, state.TransmissionId);
    }

    private struct STriggerTransmissionCompleteEventReadState : IStructFieldReader
    {
        public long TransmissionId;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            if (name != "m_transmissionId")
            {
                return false;
            }

            TransmissionId = decoder.ReadLong(fieldTypeId);
            return true;
        }
    }

    private static STriggerTransmissionOffsetEvent ReadSTriggerTransmissionOffsetEvent(TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        STriggerTransmissionOffsetEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new STriggerTransmissionOffsetEvent(userId, eventId, bits, gameloop, state.AchievementLink);
    }

    private struct STriggerTransmissionOffsetEventReadState : IStructFieldReader
    {
        public int AchievementLink;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            if (name != "m_achievementLink")
            {
                return false;
            }

            AchievementLink = decoder.ReadInt(fieldTypeId);
            return true;
        }
    }

    private static SUnitClickEvent ReadSUnitClickEvent(TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        SUnitClickEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new SUnitClickEvent(userId, eventId, bits, gameloop, state.UnitTag);
    }

    private struct SUnitClickEventReadState : IStructFieldReader
    {
        public int UnitTag;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            if (name != "m_unitTag")
            {
                return false;
            }

            UnitTag = decoder.ReadInt(fieldTypeId);
            return true;
        }
    }
}

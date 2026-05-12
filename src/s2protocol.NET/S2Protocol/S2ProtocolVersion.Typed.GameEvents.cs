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

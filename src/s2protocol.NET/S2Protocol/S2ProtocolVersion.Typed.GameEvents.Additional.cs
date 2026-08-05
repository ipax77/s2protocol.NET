using s2protocol.NET.Models;

namespace s2protocol.NET.S2Protocol;

public sealed partial record S2ProtocolVersion
{
    private struct SSaveGameEventReadState : IStructFieldReader
    {
        public string? FileName;
        public bool Automatic;
        public bool Overwrite;
        public string? Name;
        public string? Description;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_fileName": FileName = decoder.ReadString(fieldTypeId); return true;
                case "m_automatic": Automatic = decoder.ReadBool(fieldTypeId); return true;
                case "m_overwrite": Overwrite = decoder.ReadBool(fieldTypeId); return true;
                case "m_name": Name = decoder.ReadString(fieldTypeId); return true;
                case "m_description": Description = decoder.ReadString(fieldTypeId); return true;
                default: return false;
            }
        }
    }

    private static SSaveGameEvent ReadSSaveGameEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        SSaveGameEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new SSaveGameEvent(userId, eventId, bits, gameloop, state.FileName ?? string.Empty,
            state.Automatic, state.Overwrite, state.Name ?? string.Empty, state.Description ?? string.Empty);
    }

    private static SCommandManagerResetEvent ReadSCommandManagerResetEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        LongFieldReadState state = new("m_sequence");
        decoder.ReadStruct(typeId, ref state);
        return new SCommandManagerResetEvent(userId, eventId, bits, gameloop, state.Value);
    }

    private struct GameCheatEventDataReadState : IStructFieldReader
    {
        public GameEventPoint2D Point;
        public long Time;
        public string? Verb;
        public string? Arguments;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_point":
                    GameEventPoint2DReadState point = default;
                    decoder.ReadStruct(fieldTypeId, ref point);
                    Point = point.ToValue();
                    return true;
                case "m_time": Time = decoder.ReadLong(fieldTypeId); return true;
                case "m_verb": Verb = decoder.ReadString(fieldTypeId); return true;
                case "m_arguments": Arguments = decoder.ReadString(fieldTypeId); return true;
                default: return false;
            }
        }
    }

    private struct SGameCheatEventReadState : IStructFieldReader
    {
        public GameCheatEventData Data;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            if (name != "m_data")
            {
                return false;
            }

            GameCheatEventDataReadState state = default;
            decoder.ReadStruct(fieldTypeId, ref state);
            Data = new GameCheatEventData(
                state.Point, state.Time, state.Verb ?? string.Empty, state.Arguments ?? string.Empty);
            return true;
        }
    }

    private static SGameCheatEvent ReadSGameCheatEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        SGameCheatEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new SGameCheatEvent(userId, eventId, bits, gameloop, state.Data);
    }

    private struct SelectionSyncCheckDataReadState : IStructFieldReader
    {
        public int Count;
        public int SubgroupCount;
        public int ActiveSubgroupIndex;
        public long UnitTagsChecksum;
        public long SubgroupIndicesChecksum;
        public long SubgroupsChecksum;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_count": Count = decoder.ReadInt(fieldTypeId); return true;
                case "m_subgroupCount": SubgroupCount = decoder.ReadInt(fieldTypeId); return true;
                case "m_activeSubgroupIndex": ActiveSubgroupIndex = decoder.ReadInt(fieldTypeId); return true;
                case "m_unitTagsChecksum": UnitTagsChecksum = decoder.ReadLong(fieldTypeId); return true;
                case "m_subgroupIndicesChecksum": SubgroupIndicesChecksum = decoder.ReadLong(fieldTypeId); return true;
                case "m_subgroupsChecksum": SubgroupsChecksum = decoder.ReadLong(fieldTypeId); return true;
                default: return false;
            }
        }

        public readonly SelectionSyncCheckEventData ToValue() => new(
            Count, SubgroupCount, ActiveSubgroupIndex, UnitTagsChecksum,
            SubgroupIndicesChecksum, SubgroupsChecksum);
    }

    private struct SSelectionSyncCheckEventReadState : IStructFieldReader
    {
        public int ControlGroupId;
        public SelectionSyncCheckEventData Data;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_controlGroupId": ControlGroupId = decoder.ReadInt(fieldTypeId); return true;
                case "m_selectionSyncData":
                    SelectionSyncCheckDataReadState state = default;
                    decoder.ReadStruct(fieldTypeId, ref state);
                    Data = state.ToValue();
                    return true;
                default: return false;
            }
        }
    }

    private static SSelectionSyncCheckEvent ReadSSelectionSyncCheckEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        SSelectionSyncCheckEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new SSelectionSyncCheckEvent(
            userId, eventId, bits, gameloop, state.ControlGroupId, state.Data);
    }

    private struct ResourceEventReadState : IStructFieldReader
    {
        public int RecipientId;
        public List<long>? Resources;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_recipientId": RecipientId = decoder.ReadInt(fieldTypeId); return true;
                case "m_resources":
                    LongArrayReader reader = default;
                    decoder.ReadArray(fieldTypeId, ref reader);
                    Resources = reader.Values;
                    return true;
                default: return false;
            }
        }
    }

    private static SResourceTradeEvent ReadSResourceTradeEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        ResourceEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        IReadOnlyList<long> resources = state.Resources is { } values ? values : Array.Empty<long>();
        return new SResourceTradeEvent(userId, eventId, bits, gameloop, state.RecipientId, resources);
    }

    private static SResourceRequestEvent ReadSResourceRequestEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        ResourceEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        IReadOnlyList<long> resources = state.Resources is { } values ? values : Array.Empty<long>();
        return new SResourceRequestEvent(userId, eventId, bits, gameloop, resources);
    }

    private struct SAICommunicateEventReadState : IStructFieldReader
    {
        public int Beacon;
        public int Ally;
        public int? Autocast;
        public int? Flags;
        public int? Build;
        public long TargetUnitTag;
        public int TargetUnitSnapshotUnitLink;
        public int? TargetUnitSnapshotPlayerId;
        public int? TargetUnitSnapshotUpkeepPlayerId;
        public int? TargetUnitSnapshotControlPlayerId;
        public GameEventPoint3D TargetPoint;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_beacon": Beacon = decoder.ReadInt(fieldTypeId); return true;
                case "m_ally": Ally = decoder.ReadInt(fieldTypeId); return true;
                case "m_autocast": Autocast = decoder.ReadInt(fieldTypeId); return true;
                case "m_flags": Flags = decoder.ReadInt(fieldTypeId); return true;
                case "m_build": Build = decoder.ReadInt(fieldTypeId); return true;
                case "m_targetUnitTag": TargetUnitTag = decoder.ReadLong(fieldTypeId); return true;
                case "m_targetUnitSnapshotUnitLink": TargetUnitSnapshotUnitLink = decoder.ReadInt(fieldTypeId); return true;
                case "m_targetUnitSnapshotPlayerId": TargetUnitSnapshotPlayerId = decoder.ReadNullableInt(fieldTypeId); return true;
                case "m_targetUnitSnapshotUpkeepPlayerId": TargetUnitSnapshotUpkeepPlayerId = decoder.ReadNullableInt(fieldTypeId); return true;
                case "m_targetUnitSnapshotControlPlayerId": TargetUnitSnapshotControlPlayerId = decoder.ReadNullableInt(fieldTypeId); return true;
                case "m_targetPoint":
                    GameEventPoint3DReadState point = default;
                    decoder.ReadStruct(fieldTypeId, ref point);
                    TargetPoint = point.ToValue();
                    return true;
                default: return false;
            }
        }
    }

    private static SAICommunicateEvent ReadSAICommunicateEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        SAICommunicateEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new SAICommunicateEvent(userId, eventId, bits, gameloop, state.Beacon, state.Ally,
            state.Autocast, state.Flags, state.Build, state.TargetUnitTag,
            state.TargetUnitSnapshotUnitLink, state.TargetUnitSnapshotPlayerId,
            state.TargetUnitSnapshotUpkeepPlayerId, state.TargetUnitSnapshotControlPlayerId, state.TargetPoint);
    }

    private static SSetAbsoluteGameSpeedEvent ReadSSetAbsoluteGameSpeedEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        IntFieldReadState state = new("m_speed");
        decoder.ReadStruct(typeId, ref state);
        return new SSetAbsoluteGameSpeedEvent(userId, eventId, bits, gameloop, state.Value);
    }

    private static SAddAbsoluteGameSpeedEvent ReadSAddAbsoluteGameSpeedEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        IntFieldReadState state = new("m_delta");
        decoder.ReadStruct(typeId, ref state);
        return new SAddAbsoluteGameSpeedEvent(userId, eventId, bits, gameloop, state.Value);
    }

    private static SBroadcastCheatEvent ReadSBroadcastCheatEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        StringPairReadState state = new("m_verb", "m_arguments");
        decoder.ReadStruct(typeId, ref state);
        return new SBroadcastCheatEvent(
            userId, eventId, bits, gameloop, state.First ?? string.Empty, state.Second ?? string.Empty);
    }

    private static SAllianceEvent ReadSAllianceEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        LongPairReadState state = new("m_alliance", "m_control");
        decoder.ReadStruct(typeId, ref state);
        return new SAllianceEvent(userId, eventId, bits, gameloop, state.First, state.Second);
    }

    private struct UnitHighlightEventReadState : IStructFieldReader
    {
        public long UnitTag;
        public int Flags;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_unitTag": UnitTag = decoder.ReadLong(fieldTypeId); return true;
                case "m_flags": Flags = decoder.ReadInt(fieldTypeId); return true;
                default: return false;
            }
        }
    }

    private static SUnitHighlightEvent ReadSUnitHighlightEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        UnitHighlightEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new SUnitHighlightEvent(userId, eventId, bits, gameloop, state.UnitTag, state.Flags);
    }

    private static STriggerReplySelectedEvent ReadSTriggerReplySelectedEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        LongPairReadState state = new("m_conversationId", "m_replyId");
        decoder.ReadStruct(typeId, ref state);
        return new STriggerReplySelectedEvent(userId, eventId, bits, gameloop, state.First, state.Second);
    }

    private static STriggerPurchaseMadeEvent ReadSTriggerPurchaseMadeEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        LongFieldReadState state = new("m_purchaseItemId");
        decoder.ReadStruct(typeId, ref state);
        return new STriggerPurchaseMadeEvent(userId, eventId, bits, gameloop, state.Value);
    }

    private static STriggerPlanetMissionLaunchedEvent ReadSTriggerPlanetMissionLaunchedEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        LongFieldReadState state = new("m_difficultyLevel");
        decoder.ReadStruct(typeId, ref state);
        return new STriggerPlanetMissionLaunchedEvent(userId, eventId, bits, gameloop, state.Value);
    }

    private static STriggerConversationSkippedEvent ReadSTriggerConversationSkippedEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        IntFieldReadState state = new("m_skipType");
        decoder.ReadStruct(typeId, ref state);
        return new STriggerConversationSkippedEvent(userId, eventId, bits, gameloop, state.Value);
    }

    private struct HotkeyPressedEventReadState : IStructFieldReader
    {
        public long Hotkey;
        public bool Down;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_hotkey": Hotkey = decoder.ReadLong(fieldTypeId); return true;
                case "m_down": Down = decoder.ReadBool(fieldTypeId); return true;
                default: return false;
            }
        }
    }

    private static STriggerHotkeyPressedEvent ReadSTriggerHotkeyPressedEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        HotkeyPressedEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new STriggerHotkeyPressedEvent(userId, eventId, bits, gameloop, state.Hotkey, state.Down);
    }

    private static STriggerPlanetMissionSelectedEvent ReadSTriggerPlanetMissionSelectedEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        LongFieldReadState state = new("m_planetId");
        decoder.ReadStruct(typeId, ref state);
        return new STriggerPlanetMissionSelectedEvent(userId, eventId, bits, gameloop, state.Value);
    }

    private static STriggerMovieFunctionEvent ReadSTriggerMovieFunctionEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        StringFieldReadState state = new("m_functionName");
        decoder.ReadStruct(typeId, ref state);
        return new STriggerMovieFunctionEvent(
            userId, eventId, bits, gameloop, state.Value ?? string.Empty);
    }

    private static SResourceRequestFulfillEvent ReadSResourceRequestFulfillEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        LongFieldReadState state = new("m_fulfillRequestId");
        decoder.ReadStruct(typeId, ref state);
        return new SResourceRequestFulfillEvent(userId, eventId, bits, gameloop, state.Value);
    }

    private static SResourceRequestCancelEvent ReadSResourceRequestCancelEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        LongFieldReadState state = new("m_cancelRequestId");
        decoder.ReadStruct(typeId, ref state);
        return new SResourceRequestCancelEvent(userId, eventId, bits, gameloop, state.Value);
    }

    private static STriggerResearchPanelSelectionChangedEvent ReadSTriggerResearchPanelSelectionChangedEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        LongFieldReadState state = new("m_researchItemId");
        decoder.ReadStruct(typeId, ref state);
        return new STriggerResearchPanelSelectionChangedEvent(userId, eventId, bits, gameloop, state.Value);
    }

    private struct GameEventAbilityReadState : IStructFieldReader
    {
        public int AbilLink;
        public int AbilCmdIndex;
        public int? AbilCmdData;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_abilLink": AbilLink = decoder.ReadInt(fieldTypeId); return true;
                case "m_abilCmdIndex": AbilCmdIndex = decoder.ReadInt(fieldTypeId); return true;
                case "m_abilCmdData": AbilCmdData = decoder.ReadNullableInt(fieldTypeId); return true;
                default: return false;
            }
        }

        public readonly GameEventAbility ToValue() => new(AbilLink, AbilCmdIndex, AbilCmdData);
    }

    private struct TriggerCommandErrorEventReadState : IStructFieldReader
    {
        public long Error;
        public GameEventAbility? Abil;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_error": Error = decoder.ReadLong(fieldTypeId); return true;
                case "m_abil":
                    GameEventAbilityReadState ability = default;
                    if (decoder.ReadNullableStruct(fieldTypeId, ref ability))
                    {
                        Abil = ability.ToValue();
                    }
                    return true;
                default: return false;
            }
        }
    }

    private static STriggerCommandErrorEvent ReadSTriggerCommandErrorEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        TriggerCommandErrorEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new STriggerCommandErrorEvent(userId, eventId, bits, gameloop, state.Error, state.Abil);
    }

    private static STriggerMercenaryPanelSelectionChangedEvent ReadSTriggerMercenaryPanelSelectionChangedEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        LongFieldReadState state = new("m_mercenaryId");
        decoder.ReadStruct(typeId, ref state);
        return new STriggerMercenaryPanelSelectionChangedEvent(userId, eventId, bits, gameloop, state.Value);
    }

    private struct BattleReportMissionEventReadState : IStructFieldReader
    {
        public long BattleReportId;
        public long DifficultyLevel;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_battleReportId": BattleReportId = decoder.ReadLong(fieldTypeId); return true;
                case "m_difficultyLevel": DifficultyLevel = decoder.ReadLong(fieldTypeId); return true;
                default: return false;
            }
        }
    }

    private static STriggerBattleReportPanelPlayMissionEvent ReadSTriggerBattleReportPanelPlayMissionEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        BattleReportMissionEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new STriggerBattleReportPanelPlayMissionEvent(
            userId, eventId, bits, gameloop, state.BattleReportId, state.DifficultyLevel);
    }

    private static STriggerBattleReportPanelPlaySceneEvent ReadSTriggerBattleReportPanelPlaySceneEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        LongFieldReadState state = new("m_battleReportId");
        decoder.ReadStruct(typeId, ref state);
        return new STriggerBattleReportPanelPlaySceneEvent(userId, eventId, bits, gameloop, state.Value);
    }

    private static STriggerBattleReportPanelSelectionChangedEvent ReadSTriggerBattleReportPanelSelectionChangedEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        LongFieldReadState state = new("m_battleReportId");
        decoder.ReadStruct(typeId, ref state);
        return new STriggerBattleReportPanelSelectionChangedEvent(userId, eventId, bits, gameloop, state.Value);
    }

    private static STriggerVictoryPanelPlayMissionAgainEvent ReadSTriggerVictoryPanelPlayMissionAgainEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        LongFieldReadState state = new("m_difficultyLevel");
        decoder.ReadStruct(typeId, ref state);
        return new STriggerVictoryPanelPlayMissionAgainEvent(userId, eventId, bits, gameloop, state.Value);
    }

    private static STriggerPortraitLoadedEvent ReadSTriggerPortraitLoadedEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        LongFieldReadState state = new("m_portraitId");
        decoder.ReadStruct(typeId, ref state);
        return new STriggerPortraitLoadedEvent(userId, eventId, bits, gameloop, state.Value);
    }

    private static STriggerCustomDialogDismissedEvent ReadSTriggerCustomDialogDismissedEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        LongFieldReadState state = new("m_result");
        decoder.ReadStruct(typeId, ref state);
        return new STriggerCustomDialogDismissedEvent(userId, eventId, bits, gameloop, state.Value);
    }

    private struct MouseWheelEventReadState : IStructFieldReader
    {
        public int WheelSpin;
        public int Flags;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_wheelSpin": WheelSpin = decoder.ReadInt(fieldTypeId); return true;
                case "m_flags": Flags = decoder.ReadInt(fieldTypeId); return true;
                default: return false;
            }
        }
    }

    private static STriggerMouseWheelEvent ReadSTriggerMouseWheelEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        MouseWheelEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new STriggerMouseWheelEvent(userId, eventId, bits, gameloop, state.WheelSpin, state.Flags);
    }

    private static STriggerPurchasePanelSelectedPurchaseItemChangedEvent ReadSTriggerPurchasePanelSelectedPurchaseItemChangedEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        LongFieldReadState state = new("m_purchaseItemId");
        decoder.ReadStruct(typeId, ref state);
        return new STriggerPurchasePanelSelectedPurchaseItemChangedEvent(
            userId, eventId, bits, gameloop, state.Value);
    }

    private static STriggerPurchasePanelSelectedPurchaseCategoryChangedEvent ReadSTriggerPurchasePanelSelectedPurchaseCategoryChangedEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        LongFieldReadState state = new("m_purchaseCategoryId");
        decoder.ReadStruct(typeId, ref state);
        return new STriggerPurchasePanelSelectedPurchaseCategoryChangedEvent(
            userId, eventId, bits, gameloop, state.Value);
    }

    private struct CutsceneConversationLineEventReadState : IStructFieldReader
    {
        public long CutsceneId;
        public string? ConversationLine;
        public string? AltConversationLine;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_cutsceneId": CutsceneId = decoder.ReadLong(fieldTypeId); return true;
                case "m_conversationLine": ConversationLine = decoder.ReadString(fieldTypeId); return true;
                case "m_altConversationLine": AltConversationLine = decoder.ReadString(fieldTypeId); return true;
                default: return false;
            }
        }
    }

    private static STriggerCutsceneConversationLineEvent ReadSTriggerCutsceneConversationLineEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        CutsceneConversationLineEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new STriggerCutsceneConversationLineEvent(userId, eventId, bits, gameloop, state.CutsceneId,
            state.ConversationLine ?? string.Empty, state.AltConversationLine ?? string.Empty);
    }

    private static STriggerCutsceneConversationLineMissingEvent ReadSTriggerCutsceneConversationLineMissingEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        LongStringReadState state = new("m_cutsceneId", "m_conversationLine");
        decoder.ReadStruct(typeId, ref state);
        return new STriggerCutsceneConversationLineMissingEvent(
            userId, eventId, bits, gameloop, state.LongValue, state.StringValue ?? string.Empty);
    }

    private struct AnimLengthQueryReadState : IStructFieldReader
    {
        public int QueryId;
        public long LengthMs;
        public long FinishGameLoop;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_queryId": QueryId = decoder.ReadInt(fieldTypeId); return true;
                case "m_lengthMs": LengthMs = decoder.ReadLong(fieldTypeId); return true;
                case "m_finishGameLoop": FinishGameLoop = decoder.ReadLong(fieldTypeId); return true;
                default: return false;
            }
        }
    }

    private static STriggerAnimLengthQueryByNameEvent ReadSTriggerAnimLengthQueryByNameEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        AnimLengthQueryReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new STriggerAnimLengthQueryByNameEvent(
            userId, eventId, bits, gameloop, state.QueryId, state.LengthMs, state.FinishGameLoop);
    }

    private static STriggerAnimLengthQueryByPropsEvent ReadSTriggerAnimLengthQueryByPropsEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        AnimLengthQueryReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new STriggerAnimLengthQueryByPropsEvent(
            userId, eventId, bits, gameloop, state.QueryId, state.LengthMs);
    }

    private static STriggerAnimOffsetEvent ReadSTriggerAnimOffsetEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        IntFieldReadState state = new("m_animWaitQueryId");
        decoder.ReadStruct(typeId, ref state);
        return new STriggerAnimOffsetEvent(userId, eventId, bits, gameloop, state.Value);
    }

    private struct CatalogModifyEventReadState : IStructFieldReader
    {
        public int Catalog;
        public int Entry;
        public string? Field;
        public string? Value;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_catalog": Catalog = decoder.ReadInt(fieldTypeId); return true;
                case "m_entry": Entry = decoder.ReadInt(fieldTypeId); return true;
                case "m_field": Field = decoder.ReadString(fieldTypeId); return true;
                case "m_value": Value = decoder.ReadString(fieldTypeId); return true;
                default: return false;
            }
        }
    }

    private static SCatalogModifyEvent ReadSCatalogModifyEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        CatalogModifyEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new SCatalogModifyEvent(userId, eventId, bits, gameloop, state.Catalog, state.Entry,
            state.Field ?? string.Empty, state.Value ?? string.Empty);
    }

    private static SHeroTalentTreeSelectedEvent ReadSHeroTalentTreeSelectedEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        LongFieldReadState state = new("m_index");
        decoder.ReadStruct(typeId, ref state);
        return new SHeroTalentTreeSelectedEvent(userId, eventId, bits, gameloop, state.Value);
    }

    private static SHeroTalentTreeSelectionPanelToggledEvent ReadSHeroTalentTreeSelectionPanelToggledEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        BoolFieldReadState state = new("m_shown");
        decoder.ReadStruct(typeId, ref state);
        return new SHeroTalentTreeSelectionPanelToggledEvent(userId, eventId, bits, gameloop, state.Value);
    }

    private static SPeerSetSyncLoadingTimeEvent ReadSPeerSetSyncLoadingTimeEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        LongFieldReadState state = new("m_syncTime");
        decoder.ReadStruct(typeId, ref state);
        return new SPeerSetSyncLoadingTimeEvent(userId, eventId, bits, gameloop, state.Value);
    }

    private static SPeerSetSyncPlayingTimeEvent ReadSPeerSetSyncPlayingTimeEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        LongFieldReadState state = new("m_syncTime");
        decoder.ReadStruct(typeId, ref state);
        return new SPeerSetSyncPlayingTimeEvent(userId, eventId, bits, gameloop, state.Value);
    }

    private static SLagMessageEvent ReadSLagMessageEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        IntFieldReadState state = new("m_laggingPlayerId");
        decoder.ReadStruct(typeId, ref state);
        return new SLagMessageEvent(userId, eventId, bits, gameloop, state.Value);
    }

    private static STriggerCameraMoveEvent ReadSTriggerCameraMoveEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        IntFieldReadState state = new("m_reason");
        decoder.ReadStruct(typeId, ref state);
        return new STriggerCameraMoveEvent(userId, eventId, bits, gameloop, state.Value);
    }
}

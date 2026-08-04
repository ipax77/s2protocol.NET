using s2protocol.NET.Models;

namespace s2protocol.NET.S2Protocol;

public sealed partial record S2ProtocolVersion
{
    private static TEvent ReadMarkerGameEvent<TEvent>(TypedProtocolDecoder decoder, int typeId, TEvent gameEvent)
        where TEvent : GameEvent
    {
        decoder.SkipType(typeId);
        return gameEvent;
    }

    private struct GameEventPoint2DReadState : IStructFieldReader
    {
        public long X;
        public long Y;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "x": X = decoder.ReadLong(fieldTypeId); return true;
                case "y": Y = decoder.ReadLong(fieldTypeId); return true;
                default: return false;
            }
        }

        public readonly GameEventPoint2D ToValue() => new(X, Y);
    }

    private struct GameEventPoint3DReadState : IStructFieldReader
    {
        public long X;
        public long Y;
        public long Z;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "x": X = decoder.ReadLong(fieldTypeId); return true;
                case "y": Y = decoder.ReadLong(fieldTypeId); return true;
                case "z": Z = decoder.ReadLong(fieldTypeId); return true;
                default: return false;
            }
        }

        public readonly GameEventPoint3D ToValue() => new(X, Y, Z);
    }

    private struct LongArrayReader : IArrayItemReader
    {
        public List<long>? Values;

        public bool ReadItem(TypedProtocolDecoder decoder, int itemTypeId)
        {
            (Values ??= []).Add(decoder.ReadLong(itemTypeId));
            return true;
        }
    }

    private struct SBankSignatureEventReadState : IStructFieldReader
    {
        public List<int>? Signature;
        public string? ToonHandle;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_signature": Signature = decoder.ReadIntList(fieldTypeId); return true;
                case "m_toonHandle": ToonHandle = decoder.ReadString(fieldTypeId); return true;
                default: return false;
            }
        }
    }

    private static SBankSignatureEvent ReadSBankSignatureEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        SBankSignatureEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new SBankSignatureEvent(
            userId, eventId, bits, gameloop, state.ToonHandle ?? string.Empty, state.Signature ?? []);
    }

    private struct SCameraSaveEventReadState : IStructFieldReader
    {
        public int Which;
        public GameEventPoint2D Target;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_which": Which = decoder.ReadInt(fieldTypeId); return true;
                case "m_target":
                    GameEventPoint2DReadState point = default;
                    decoder.ReadStruct(fieldTypeId, ref point);
                    Target = point.ToValue();
                    return true;
                default: return false;
            }
        }
    }

    private static SCameraSaveEvent ReadSCameraSaveEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        SCameraSaveEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new SCameraSaveEvent(userId, eventId, bits, gameloop, state.Which, state.Target.X, state.Target.Y);
    }

    private struct SCameraUpdateEventReadState : IStructFieldReader
    {
        public long? TargetX;
        public long? TargetY;
        public int? Distance;
        public int? Pitch;
        public int? Yaw;
        public string? Reason;
        public bool Follow;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_target":
                    GameEventPoint2DReadState point = default;
                    if (decoder.ReadNullableStruct(fieldTypeId, ref point))
                    {
                        TargetX = point.X;
                        TargetY = point.Y;
                    }
                    return true;
                case "m_distance": Distance = decoder.ReadNullableInt(fieldTypeId); return true;
                case "m_pitch": Pitch = decoder.ReadNullableInt(fieldTypeId); return true;
                case "m_yaw": Yaw = decoder.ReadNullableInt(fieldTypeId); return true;
                case "m_reason": Reason = decoder.ReadNullableString(fieldTypeId); return true;
                case "m_follow": Follow = decoder.ReadBool(fieldTypeId); return true;
                default: return false;
            }
        }
    }

    private static SCameraUpdateEvent ReadSCameraUpdateEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        SCameraUpdateEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new SCameraUpdateEvent(userId, eventId, bits, gameloop, state.Reason, state.Distance,
            state.TargetX, state.TargetY, state.Yaw, state.Pitch, state.Follow);
    }

    private struct TargetPointEventReadState : IStructFieldReader
    {
        public GameEventPoint3D Target;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            if (name != "m_target")
            {
                return false;
            }

            GameEventPoint3DReadState point = default;
            decoder.ReadStruct(fieldTypeId, ref point);
            Target = point.ToValue();
            return true;
        }
    }

    private static SCmdUpdateTargetPointEvent ReadSCmdUpdateTargetPointEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        TargetPointEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new SCmdUpdateTargetPointEvent(
            userId, eventId, bits, gameloop, state.Target.X, state.Target.Y, state.Target.Z);
    }

    private struct SCmdUpdateTargetUnitReadState : IStructFieldReader
    {
        public int SnapshotControlPlayerId;
        public int SnapshotUpkeepPlayerId;
        public int Timer;
        public int TargetUnitFlags;
        public int SnapshotUnitLink;
        public int Tag;
        public GameEventPoint3D SnapshotPoint;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_snapshotControlPlayerId": SnapshotControlPlayerId = decoder.ReadNullableInt(fieldTypeId) ?? 0; return true;
                case "m_snapshotUpkeepPlayerId": SnapshotUpkeepPlayerId = decoder.ReadNullableInt(fieldTypeId) ?? 0; return true;
                case "m_timer": Timer = decoder.ReadInt(fieldTypeId); return true;
                case "m_targetUnitFlags": TargetUnitFlags = decoder.ReadInt(fieldTypeId); return true;
                case "m_snapshotUnitLink": SnapshotUnitLink = decoder.ReadInt(fieldTypeId); return true;
                case "m_tag": Tag = decoder.ReadInt(fieldTypeId); return true;
                case "m_snapshotPoint":
                    GameEventPoint3DReadState point = default;
                    decoder.ReadStruct(fieldTypeId, ref point);
                    SnapshotPoint = point.ToValue();
                    return true;
                default: return false;
            }
        }
    }

    private struct SCmdUpdateTargetUnitEventReadState : IStructFieldReader
    {
        public SCmdUpdateTargetUnitReadState Target;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            if (name != "m_target")
            {
                return false;
            }

            decoder.ReadStruct(fieldTypeId, ref Target);
            return true;
        }
    }

    private static SCmdUpdateTargetUnitEvent ReadSCmdUpdateTargetUnitEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        SCmdUpdateTargetUnitEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        SCmdUpdateTargetUnitReadState target = state.Target;
        return new SCmdUpdateTargetUnitEvent(userId, eventId, bits, gameloop,
            target.SnapshotControlPlayerId, target.SnapshotPoint.X, target.SnapshotPoint.Y,
            target.SnapshotPoint.Z, target.SnapshotUpkeepPlayerId, target.Timer,
            target.TargetUnitFlags, target.SnapshotUnitLink, target.Tag);
    }

    private struct SDecrementGameTimeRemainingEventReadState : IStructFieldReader
    {
        public int Seconds;
        public long? Milliseconds;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_decrementSeconds": Seconds = decoder.ReadInt(fieldTypeId); return true;
                case "m_decrementMs": Milliseconds = decoder.ReadLong(fieldTypeId); return true;
                default: return false;
            }
        }
    }

    private static SDecrementGameTimeRemainingEvent ReadSDecrementGameTimeRemainingEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        SDecrementGameTimeRemainingEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new SDecrementGameTimeRemainingEvent(
            userId, eventId, bits, gameloop, state.Seconds, state.Milliseconds);
    }

    private struct DialogEventDataChoiceHandler : IChoiceHandler
    {
        public DialogControlEventDataKind Kind;
        public bool? Checked;
        public long? ValueChanged;
        public long? SelectionChanged;
        public string? TextChanged;
        public int? MouseButton;

        public bool HandleChoice(TypedProtocolDecoder decoder, string name, int choiceTypeId)
        {
            switch (name)
            {
                case "None":
                    Kind = DialogControlEventDataKind.None;
                    decoder.SkipType(choiceTypeId);
                    return true;
                case "Checked":
                    Kind = DialogControlEventDataKind.Checked;
                    Checked = decoder.ReadBool(choiceTypeId);
                    return true;
                case "ValueChanged":
                    Kind = DialogControlEventDataKind.ValueChanged;
                    ValueChanged = decoder.ReadLong(choiceTypeId);
                    return true;
                case "SelectionChanged":
                    Kind = DialogControlEventDataKind.SelectionChanged;
                    SelectionChanged = decoder.ReadLong(choiceTypeId);
                    return true;
                case "TextChanged":
                    Kind = DialogControlEventDataKind.TextChanged;
                    TextChanged = decoder.ReadString(choiceTypeId);
                    return true;
                case "MouseButton":
                    Kind = DialogControlEventDataKind.MouseButton;
                    MouseButton = decoder.ReadInt(choiceTypeId);
                    return true;
                default: return false;
            }
        }
    }

    private struct STriggerDialogControlEventReadState : IStructFieldReader
    {
        public long ControlId;
        public long EventType;
        public DialogEventDataChoiceHandler EventData;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_controlId": ControlId = decoder.ReadLong(fieldTypeId); return true;
                case "m_eventType": EventType = decoder.ReadLong(fieldTypeId); return true;
                case "m_eventData": decoder.ReadChoice(fieldTypeId, ref EventData); return true;
                default: return false;
            }
        }
    }

    private static STriggerDialogControlEvent ReadSTriggerDialogControlEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        STriggerDialogControlEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new STriggerDialogControlEvent(userId, eventId, bits, gameloop, state.ControlId,
            state.EventData.MouseButton, state.EventData.TextChanged, state.EventType, state.EventData.Kind,
            state.EventData.Checked, state.EventData.ValueChanged, state.EventData.SelectionChanged);
    }

    private struct MouseEventReadState : IStructFieldReader
    {
        public bool Down;
        public int Button;
        public int Flags;
        public long PosUIX;
        public long PosUIY;
        public long? PosWorldX;
        public long? PosWorldY;
        public long? PosWorldZ;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_down": Down = decoder.ReadBool(fieldTypeId); return true;
                case "m_button": Button = decoder.ReadInt(fieldTypeId); return true;
                case "m_flags": Flags = decoder.ReadInt(fieldTypeId); return true;
                case "m_posXUI": PosUIX = decoder.ReadLong(fieldTypeId); return true;
                case "m_posYUI": PosUIY = decoder.ReadLong(fieldTypeId); return true;
                case "m_posXWorld": PosWorldX = decoder.ReadLong(fieldTypeId); return true;
                case "m_posYWorld": PosWorldY = decoder.ReadLong(fieldTypeId); return true;
                case "m_posZWorld": PosWorldZ = decoder.ReadLong(fieldTypeId); return true;
                case "m_posUI":
                    GameEventPoint2DReadState ui = default;
                    decoder.ReadStruct(fieldTypeId, ref ui);
                    PosUIX = ui.X;
                    PosUIY = ui.Y;
                    return true;
                case "m_posWorld":
                    GameEventPoint3DReadState world = default;
                    decoder.ReadStruct(fieldTypeId, ref world);
                    PosWorldX = world.X;
                    PosWorldY = world.Y;
                    PosWorldZ = world.Z;
                    return true;
                default: return false;
            }
        }
    }

    private static STriggerMouseClickedEvent ReadSTriggerMouseClickedEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        MouseEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new STriggerMouseClickedEvent(userId, eventId, bits, gameloop, state.Down, state.Button,
            state.Flags, state.PosUIX, state.PosUIY, state.PosWorldX, state.PosWorldY, state.PosWorldZ);
    }

    private static STriggerMouseMovedEvent ReadSTriggerMouseMovedEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        MouseEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new STriggerMouseMovedEvent(userId, eventId, bits, gameloop, state.Flags,
            state.PosUIX, state.PosUIY, state.PosWorldX, state.PosWorldY, state.PosWorldZ);
    }

    private struct STriggerPingEventReadState : IStructFieldReader
    {
        public GameEventPoint2D Point;
        public int Unit;
        public int UnitLink;
        public int? UnitControlPlayerId;
        public int? UnitUpkeepPlayerId;
        public GameEventPoint3D UnitPosition;
        public bool UnitIsUnderConstruction;
        public bool PingedMinimap;
        public long Option;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_point":
                    GameEventPoint2DReadState point = default;
                    decoder.ReadStruct(fieldTypeId, ref point);
                    Point = point.ToValue();
                    return true;
                case "m_unit": Unit = decoder.ReadInt(fieldTypeId); return true;
                case "m_unitLink": UnitLink = decoder.ReadInt(fieldTypeId); return true;
                case "m_unitControlPlayerId": UnitControlPlayerId = decoder.ReadNullableInt(fieldTypeId); return true;
                case "m_unitUpkeepPlayerId": UnitUpkeepPlayerId = decoder.ReadNullableInt(fieldTypeId); return true;
                case "m_unitPosition":
                    GameEventPoint3DReadState position = default;
                    decoder.ReadStruct(fieldTypeId, ref position);
                    UnitPosition = position.ToValue();
                    return true;
                case "m_unitIsUnderConstruction": UnitIsUnderConstruction = decoder.ReadBool(fieldTypeId); return true;
                case "m_pingedMinimap": PingedMinimap = decoder.ReadBool(fieldTypeId); return true;
                case "m_option": Option = decoder.ReadLong(fieldTypeId); return true;
                default: return false;
            }
        }
    }

    private static STriggerPingEvent ReadSTriggerPingEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        STriggerPingEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new STriggerPingEvent(userId, eventId, bits, gameloop, state.PingedMinimap, state.UnitLink,
            state.UnitIsUnderConstruction, state.Option, state.Unit, state.UnitPosition.X, state.UnitPosition.Y,
            state.UnitPosition.Z, state.UnitControlPlayerId, state.Point.X, state.Point.Y, state.UnitUpkeepPlayerId);
    }

    private struct SUserOptionsEventReadState : IStructFieldReader
    {
        public bool TestCheatsEnabled;
        public bool MultiplayerCheatsEnabled;
        public bool GameFullyDownloaded;
        public string? HotkeyProfile;
        public bool UseGalaxyAsserts;
        public bool DebugPauseEnabled;
        public bool CameraFollow;
        public bool IsMapToMapTransition;
        public int BuildNum;
        public int VersionFlags;
        public bool DevelopmentCheatsEnabled;
        public bool PlatformMac;
        public int BaseBuildNum;
        public bool SyncChecksummingEnabled;
        public bool StartingRally;
        public bool UseAIBeacons;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_testCheatsEnabled": TestCheatsEnabled = decoder.ReadBool(fieldTypeId); return true;
                case "m_multiplayerCheatsEnabled": MultiplayerCheatsEnabled = decoder.ReadBool(fieldTypeId); return true;
                case "m_gameFullyDownloaded": GameFullyDownloaded = decoder.ReadBool(fieldTypeId); return true;
                case "m_hotkeyProfile": HotkeyProfile = decoder.ReadString(fieldTypeId); return true;
                case "m_useGalaxyAsserts": UseGalaxyAsserts = decoder.ReadBool(fieldTypeId); return true;
                case "m_debugPauseEnabled": DebugPauseEnabled = decoder.ReadBool(fieldTypeId); return true;
                case "m_cameraFollow": CameraFollow = decoder.ReadBool(fieldTypeId); return true;
                case "m_isMapToMapTransition": IsMapToMapTransition = decoder.ReadBool(fieldTypeId); return true;
                case "m_buildNum": BuildNum = decoder.ReadInt(fieldTypeId); return true;
                case "m_versionFlags": VersionFlags = decoder.ReadInt(fieldTypeId); return true;
                case "m_developmentCheatsEnabled": DevelopmentCheatsEnabled = decoder.ReadBool(fieldTypeId); return true;
                case "m_platformMac": PlatformMac = decoder.ReadBool(fieldTypeId); return true;
                case "m_baseBuildNum": BaseBuildNum = decoder.ReadInt(fieldTypeId); return true;
                case "m_syncChecksummingEnabled": SyncChecksummingEnabled = decoder.ReadBool(fieldTypeId); return true;
                case "m_startingRally": StartingRally = decoder.ReadBool(fieldTypeId); return true;
                case "m_useAIBeacons": UseAIBeacons = decoder.ReadBool(fieldTypeId); return true;
                default: return false;
            }
        }
    }

    private static SUserOptionsEvent ReadSUserOptionsEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        SUserOptionsEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new SUserOptionsEvent(userId, eventId, bits, gameloop, state.TestCheatsEnabled,
            state.MultiplayerCheatsEnabled, state.GameFullyDownloaded, state.HotkeyProfile ?? string.Empty,
            state.UseGalaxyAsserts, state.DebugPauseEnabled, state.CameraFollow, state.IsMapToMapTransition,
            state.BuildNum, state.VersionFlags, state.DevelopmentCheatsEnabled, state.PlatformMac,
            state.BaseBuildNum, state.SyncChecksummingEnabled, state.StartingRally, state.UseAIBeacons);
    }

    private struct TargetModeEventReadState : IStructFieldReader
    {
        public int AbilLink;
        public int AbilCmdIndex;
        public int State;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_abilLink": AbilLink = decoder.ReadInt(fieldTypeId); return true;
                case "m_abilCmdIndex": AbilCmdIndex = decoder.ReadInt(fieldTypeId); return true;
                case "m_state": State = decoder.ReadInt(fieldTypeId); return true;
                default: return false;
            }
        }
    }

    private static STriggerTargetModeUpdateEvent ReadSTriggerTargetModeUpdateEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        TargetModeEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new STriggerTargetModeUpdateEvent(
            userId, eventId, bits, gameloop, state.AbilCmdIndex, state.AbilLink, state.State);
    }

    private struct SoundLengthQueryEventReadState : IStructFieldReader
    {
        public long SoundHash;
        public int Length;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_soundHash": SoundHash = decoder.ReadLong(fieldTypeId); return true;
                case "m_length": Length = decoder.ReadInt(fieldTypeId); return true;
                default: return false;
            }
        }
    }

    private static STriggerSoundLengthQueryEvent ReadSTriggerSoundLengthQueryEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        SoundLengthQueryEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new STriggerSoundLengthQueryEvent(
            userId, eventId, bits, gameloop, state.SoundHash, state.Length);
    }

    private struct KeyPressedEventReadState : IStructFieldReader
    {
        public int Key;
        public int Flags;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_key": Key = decoder.ReadInt(fieldTypeId); return true;
                case "m_flags": Flags = decoder.ReadInt(fieldTypeId); return true;
                default: return false;
            }
        }
    }

    private static STriggerKeyPressedEvent ReadSTriggerKeyPressedEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        KeyPressedEventReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new STriggerKeyPressedEvent(userId, eventId, bits, gameloop, state.Flags, state.Key);
    }

    private struct LongFieldReadState(string fieldName) : IStructFieldReader
    {
        public long Value;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            if (name != fieldName)
            {
                return false;
            }

            Value = decoder.ReadLong(fieldTypeId);
            return true;
        }
    }

    private struct IntFieldReadState(string fieldName) : IStructFieldReader
    {
        public int Value;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            if (name != fieldName)
            {
                return false;
            }

            Value = decoder.ReadInt(fieldTypeId);
            return true;
        }
    }

    private struct BoolFieldReadState(string fieldName) : IStructFieldReader
    {
        public bool Value;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            if (name != fieldName)
            {
                return false;
            }

            Value = decoder.ReadBool(fieldTypeId);
            return true;
        }
    }

    private struct StringFieldReadState(string fieldName) : IStructFieldReader
    {
        public string? Value;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            if (name != fieldName)
            {
                return false;
            }

            Value = decoder.ReadString(fieldTypeId);
            return true;
        }
    }

    private struct LongPairReadState(string firstName, string secondName) : IStructFieldReader
    {
        public long First;
        public long Second;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            if (name == firstName)
            {
                First = decoder.ReadLong(fieldTypeId);
                return true;
            }

            if (name == secondName)
            {
                Second = decoder.ReadLong(fieldTypeId);
                return true;
            }

            return false;
        }
    }

    private struct StringPairReadState(string firstName, string secondName) : IStructFieldReader
    {
        public string? First;
        public string? Second;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            if (name == firstName)
            {
                First = decoder.ReadString(fieldTypeId);
                return true;
            }

            if (name == secondName)
            {
                Second = decoder.ReadString(fieldTypeId);
                return true;
            }

            return false;
        }
    }

    private struct LongStringReadState(string longName, string stringName) : IStructFieldReader
    {
        public long LongValue;
        public string? StringValue;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            if (name == longName)
            {
                LongValue = decoder.ReadLong(fieldTypeId);
                return true;
            }

            if (name == stringName)
            {
                StringValue = decoder.ReadString(fieldTypeId);
                return true;
            }

            return false;
        }
    }

    private static SAchievementAwardedEvent ReadSAchievementAwardedEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        IntFieldReadState state = new("m_achievementLink");
        decoder.ReadStruct(typeId, ref state);
        return new SAchievementAwardedEvent(userId, eventId, bits, gameloop, state.Value);
    }

    private static STriggerCutsceneBookmarkFiredEvent ReadSTriggerCutsceneBookmarkFiredEvent(
        TypedProtocolDecoder decoder, int typeId, int userId, int eventId, int bits, int gameloop)
    {
        LongStringReadState state = new("m_cutsceneId", "m_bookmarkName");
        decoder.ReadStruct(typeId, ref state);
        return new STriggerCutsceneBookmarkFiredEvent(
            userId, eventId, bits, gameloop, state.LongValue, state.StringValue ?? string.Empty);
    }
}

using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;

internal static partial class Parse
{
    private readonly record struct GameEventHeader(
        int UserId,
        int EventId,
        GameEventType EventType,
        int Bits,
        int Gameloop,
        string EventTypeName);

    public static GameEvents GameEvents(IEnumerable<Dictionary<string, object?>> pydic)
    {
        List<GameEvent> gameevents = [];

        foreach (var ent in pydic)
        {
            if (ent is Dictionary<string, object> gameDicRaw)
            {
                var gameDic = gameDicRaw as Dictionary<string, object>;
                GameEventHeader gameEvent = GetGameEvent(gameDic);

                GameEvent detailEvent = gameEvent.EventType switch
                {
                    GameEventType.SBankFileEvent => GetSBankFileEvent(gameDic, gameEvent),
                    GameEventType.SBankKeyEvent => GetSBankKeyEvent(gameDic, gameEvent),
                    GameEventType.SBankSectionEvent => GetSBankSectionEvent(gameDic, gameEvent),
                    GameEventType.SBankSignatureEvent => GetSBankSignatureEvent(gameDic, gameEvent),
                    GameEventType.SBankValueEvent => GetSBankValueEvent(gameDic, gameEvent),
                    GameEventType.SCameraUpdateEvent => GetSCameraUpdateEvent(gameDic, gameEvent),
                    GameEventType.SCmdEvent => GetSCmdEvent(gameDic, gameEvent),
                    GameEventType.SCmdUpdateTargetPointEvent => GetSCmdUpdateTargetPointEvent(gameDic, gameEvent),
                    GameEventType.SCommandManagerStateEvent => GetSCommandManagerStateEvent(gameDic, gameEvent),
                    GameEventType.SControlGroupUpdateEvent => GetSControlGroupUpdateEvent(gameDic, gameEvent),
                    GameEventType.SGameUserLeaveEvent => GetSGameUserLeaveEvent(gameDic, gameEvent),
                    GameEventType.SSelectionDeltaEvent => GetSSelectionDeltaEvent(gameDic, gameEvent),
                    GameEventType.SSetSyncLoadingTimeEvent => GetSSetSyncLoadingTimeEvent(gameDic, gameEvent),
                    GameEventType.SSetSyncPlayingTimeEvent => GetSSetSyncPlayingTimeEvent(gameDic, gameEvent),
                    GameEventType.STriggerDialogControlEvent => GetSTriggerDialogControlEvent(gameDic, gameEvent),
                    GameEventType.STriggerPingEvent => GetSTriggerPingEvent(gameDic, gameEvent),
                    GameEventType.STriggerSoundLengthSyncEvent => new STriggerSoundLengthSyncEvent(gameEvent.UserId, gameEvent.EventId, gameEvent.Bits, gameEvent.Gameloop),
                    GameEventType.SUserFinishedLoadingSyncEvent => new SUserFinishedLoadingSyncEvent(gameEvent.UserId, gameEvent.EventId, gameEvent.Bits, gameEvent.Gameloop),
                    GameEventType.SUserOptionsEvent => GetSUserOptionsEvent(gameDic, gameEvent),
                    GameEventType.SCmdUpdateTargetUnitEvents => GetSCmdUpdateTargetUnitEvent(gameDic, gameEvent),
                    GameEventType.STriggerKeyPressedEvent => GetSTriggerKeyPressedEvent(gameDic, gameEvent),
                    GameEventType.SUnitClickEvent => GetSUnitClickEvent(gameDic, gameEvent),
                    GameEventType.SDecrementGameTimeRemainingEvent => GetSDecrementGameTimeRemainingEvent(gameDic, gameEvent),
                    GameEventType.STriggerChatMessageEvent => GetSTriggerChatMessageEvent(gameDic, gameEvent),
                    GameEventType.STriggerMouseClickedEvent => GetSTriggerMouseClickedEvent(gameDic, gameEvent),
                    GameEventType.STriggerSoundtrackDoneEvent => GetSTriggerSoundtrackDoneEvent(gameDic, gameEvent),
                    GameEventType.SCameraSaveEvent => GetSCameraSaveEvent(gameDic, gameEvent),
                    GameEventType.STriggerCutsceneBookmarkFiredEvent => GetSTriggerCutsceneBookmarkFiredEvent(gameDic, gameEvent),
                    GameEventType.STriggerCutsceneEndSceneFiredEvent => GetSTriggerCutsceneEndSceneFiredEvent(gameDic, gameEvent),
                    GameEventType.STriggerSoundLengthQueryEvent => GetSTriggerSoundLengthQueryEvent(gameDic, gameEvent),
                    GameEventType.STriggerSoundOffsetEvent => GetSTriggerSoundOffsetEvent(gameDic, gameEvent),
                    GameEventType.STriggerTargetModeUpdateEvent => GetSTriggerTargetModeUpdateEvent(gameDic, gameEvent),
                    GameEventType.STriggerTransmissionCompleteEvent => GetSTriggerTransmissionCompleteEvent(gameDic, gameEvent),
                    GameEventType.SAchievementAwardedEvent => GetSAchievementAwardedEvent(gameDic, gameEvent),
                    GameEventType.STriggerTransmissionOffsetEvent => GetSTriggerTransmissionOffsetEvent(gameDic, gameEvent),
                    GameEventType.STriggerButtonPressedEvent => GetSTriggerButtonPressedEvent(gameDic, gameEvent),
                    GameEventType.STriggerGameMenuItemSelectedEvent => GetSTriggerGameMenuItemSelectedEvent(gameDic, gameEvent),
                    GameEventType.STriggerMouseMovedEvent => GetSTriggerMouseMovedEvent(gameDic, gameEvent),
                    GameEventType.None => throw new NotImplementedException(),
                    _ => GetUnknownEvent(gameEvent)
                };
                gameevents.Add(detailEvent);
            }
        }
        return new GameEvents(gameevents);
    }

    internal static GameEvent GetGameEventTyped(Dictionary<string, object> gameDic)
    {
        GameEventHeader gameEvent = GetGameEvent(gameDic);
        return gameEvent.EventType switch
        {
            GameEventType.SBankFileEvent => GetSBankFileEvent(gameDic, gameEvent),
            GameEventType.SBankKeyEvent => GetSBankKeyEvent(gameDic, gameEvent),
            GameEventType.SBankSectionEvent => GetSBankSectionEvent(gameDic, gameEvent),
            GameEventType.SBankSignatureEvent => GetSBankSignatureEvent(gameDic, gameEvent),
            GameEventType.SBankValueEvent => GetSBankValueEvent(gameDic, gameEvent),
            GameEventType.SCameraUpdateEvent => GetSCameraUpdateEvent(gameDic, gameEvent),
            GameEventType.SCmdEvent => GetSCmdEvent(gameDic, gameEvent),
            GameEventType.SCmdUpdateTargetPointEvent => GetSCmdUpdateTargetPointEvent(gameDic, gameEvent),
            GameEventType.SCommandManagerStateEvent => GetSCommandManagerStateEvent(gameDic, gameEvent),
            GameEventType.SControlGroupUpdateEvent => GetSControlGroupUpdateEvent(gameDic, gameEvent),
            GameEventType.SGameUserLeaveEvent => GetSGameUserLeaveEvent(gameDic, gameEvent),
            GameEventType.SSelectionDeltaEvent => GetSSelectionDeltaEvent(gameDic, gameEvent),
            GameEventType.SSetSyncLoadingTimeEvent => GetSSetSyncLoadingTimeEvent(gameDic, gameEvent),
            GameEventType.SSetSyncPlayingTimeEvent => GetSSetSyncPlayingTimeEvent(gameDic, gameEvent),
            GameEventType.STriggerDialogControlEvent => GetSTriggerDialogControlEvent(gameDic, gameEvent),
            GameEventType.STriggerPingEvent => GetSTriggerPingEvent(gameDic, gameEvent),
            GameEventType.STriggerSoundLengthSyncEvent => new STriggerSoundLengthSyncEvent(gameEvent.UserId, gameEvent.EventId, gameEvent.Bits, gameEvent.Gameloop),
            GameEventType.SUserFinishedLoadingSyncEvent => new SUserFinishedLoadingSyncEvent(gameEvent.UserId, gameEvent.EventId, gameEvent.Bits, gameEvent.Gameloop),
            GameEventType.SUserOptionsEvent => GetSUserOptionsEvent(gameDic, gameEvent),
            GameEventType.SCmdUpdateTargetUnitEvents => GetSCmdUpdateTargetUnitEvent(gameDic, gameEvent),
            GameEventType.STriggerKeyPressedEvent => GetSTriggerKeyPressedEvent(gameDic, gameEvent),
            GameEventType.SUnitClickEvent => GetSUnitClickEvent(gameDic, gameEvent),
            GameEventType.SDecrementGameTimeRemainingEvent => GetSDecrementGameTimeRemainingEvent(gameDic, gameEvent),
            GameEventType.STriggerChatMessageEvent => GetSTriggerChatMessageEvent(gameDic, gameEvent),
            GameEventType.STriggerMouseClickedEvent => GetSTriggerMouseClickedEvent(gameDic, gameEvent),
            GameEventType.STriggerSoundtrackDoneEvent => GetSTriggerSoundtrackDoneEvent(gameDic, gameEvent),
            GameEventType.SCameraSaveEvent => GetSCameraSaveEvent(gameDic, gameEvent),
            GameEventType.STriggerCutsceneBookmarkFiredEvent => GetSTriggerCutsceneBookmarkFiredEvent(gameDic, gameEvent),
            GameEventType.STriggerCutsceneEndSceneFiredEvent => GetSTriggerCutsceneEndSceneFiredEvent(gameDic, gameEvent),
            GameEventType.STriggerSoundLengthQueryEvent => GetSTriggerSoundLengthQueryEvent(gameDic, gameEvent),
            GameEventType.STriggerSoundOffsetEvent => GetSTriggerSoundOffsetEvent(gameDic, gameEvent),
            GameEventType.STriggerTargetModeUpdateEvent => GetSTriggerTargetModeUpdateEvent(gameDic, gameEvent),
            GameEventType.STriggerTransmissionCompleteEvent => GetSTriggerTransmissionCompleteEvent(gameDic, gameEvent),
            GameEventType.SAchievementAwardedEvent => GetSAchievementAwardedEvent(gameDic, gameEvent),
            GameEventType.STriggerTransmissionOffsetEvent => GetSTriggerTransmissionOffsetEvent(gameDic, gameEvent),
            GameEventType.STriggerButtonPressedEvent => GetSTriggerButtonPressedEvent(gameDic, gameEvent),
            GameEventType.STriggerGameMenuItemSelectedEvent => GetSTriggerGameMenuItemSelectedEvent(gameDic, gameEvent),
            GameEventType.STriggerMouseMovedEvent => GetSTriggerMouseMovedEvent(gameDic, gameEvent),
            GameEventType.None => throw new NotImplementedException(),
            _ => GetUnknownEvent(gameEvent)
        };
    }

    private static GameEventHeader GetGameEvent(Dictionary<string, object> pydic)
    {
        int userId = GetUserId(pydic);
        int eventId = GetInt(pydic, "_eventid");
        string type = GetString(pydic, "_event");
        int bits = GetInt(pydic, "_bits");
        int gameloop = GetInt(pydic, "_gameloop");
        return new GameEventHeader(userId, eventId, GetGameEventType(type), bits, gameloop, type);
    }

    private static GameEventType GetGameEventType(string eventTypeName)
    {
        return eventTypeName switch
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
    }

    private static int GetUserId(Dictionary<string, object> pydic)
    {
        if (pydic.ContainsKey("_userid"))
        {
            if (pydic["_userid"] is Dictionary<string, object> userDic)
            {
                return GetInt(userDic, "m_userId");
            }
        }
        return 0;
    }

    private static UnknownGameEvent GetUnknownEvent(GameEventHeader gameEvent)
    {
        return new UnknownGameEvent(gameEvent.UserId, gameEvent.EventId, gameEvent.Bits, gameEvent.Gameloop, gameEvent.EventTypeName);
    }

    private static STriggerDialogControlEvent GetSTriggerDialogControlEvent(Dictionary<string, object> gameDic, GameEventHeader gameEvent)
    {
        long m_controlId = GetBigInt(gameDic, "m_controlId");
        int? mouseButton = GetMouseButton(gameDic);
        string? textChanged = GetTextChanged(gameDic);
        long m_eventType = GetBigInt(gameDic, "m_eventType");
        return new STriggerDialogControlEvent(gameEvent.UserId, gameEvent.EventId, gameEvent.Bits, gameEvent.Gameloop, m_controlId, mouseButton, textChanged, m_eventType);
    }

    private static int? GetMouseButton(Dictionary<string, object> pydic)
    {
        if (pydic.ContainsKey("m_eventData"))
        {
            if (pydic["m_eventData"] is Dictionary<string, object> mouseDic)
            {
                return GetNullableInt(mouseDic, "MouseButton");
            }
        }
        return null;
    }

    private static string? GetTextChanged(Dictionary<string, object> pydic)
    {
        if (pydic.ContainsKey("m_eventData"))
        {
            if (pydic["m_eventData"] is Dictionary<string, object> mouseDic)
            {
                return GetNullableString(mouseDic, "TextChanged");
            }
        }
        return null;
    }

    private static SSetSyncPlayingTimeEvent GetSSetSyncPlayingTimeEvent(Dictionary<string, object> gameDic, GameEventHeader gameEvent)
    {
        int m_syncTime = GetInt(gameDic, "m_syncTime");
        return new SSetSyncPlayingTimeEvent(gameEvent.UserId, gameEvent.EventId, gameEvent.Bits, gameEvent.Gameloop, m_syncTime);
    }

    private static SSetSyncLoadingTimeEvent GetSSetSyncLoadingTimeEvent(Dictionary<string, object> gameDic, GameEventHeader gameEvent)
    {
        int m_syncTime = GetInt(gameDic, "m_syncTime");
        return new SSetSyncLoadingTimeEvent(gameEvent.UserId, gameEvent.EventId, gameEvent.Bits, gameEvent.Gameloop, m_syncTime);
    }

    private static SGameUserLeaveEvent GetSGameUserLeaveEvent(Dictionary<string, object> gameDic, GameEventHeader gameEvent)
    {
        int leaveReason = GetInt(gameDic, "m_leaveReason");
        return new SGameUserLeaveEvent(gameEvent.UserId, gameEvent.EventId, gameEvent.Bits, gameEvent.Gameloop, leaveReason);
    }

    private static SControlGroupUpdateEvent GetSControlGroupUpdateEvent(Dictionary<string, object> gameDic, GameEventHeader gameEvent)
    {
        int controlGroupUpdate = GetInt(gameDic, "m_controlGroupUpdate");
        return new SControlGroupUpdateEvent(gameEvent.UserId, gameEvent.EventId, gameEvent.Bits, gameEvent.Gameloop, controlGroupUpdate);
    }

    private static SCommandManagerStateEvent GetSCommandManagerStateEvent(Dictionary<string, object> gameDic, GameEventHeader gameEvent)
    {
        int state = GetInt(gameDic, "m_state");
        int? sequence = GetNullableInt(gameDic, "m_sequence");
        return new SCommandManagerStateEvent(gameEvent.UserId, gameEvent.EventId, gameEvent.Bits, gameEvent.Gameloop, state, sequence);
    }

    private static SCmdUpdateTargetPointEvent GetSCmdUpdateTargetPointEvent(Dictionary<string, object> gameDic, GameEventHeader gameEvent)
    {
        long x = 0;
        long y = 0;
        long z = 0;
        if (gameDic.TryGetValue("m_target", out object? target))
        {
            if (target is Dictionary<string, object> targetDic)
            {
                x = GetBigInt(targetDic, "x");
                y = GetBigInt(targetDic, "y");
                z = GetBigInt(targetDic, "z");
            }
        }
        return new SCmdUpdateTargetPointEvent(gameEvent.UserId, gameEvent.EventId, gameEvent.Bits, gameEvent.Gameloop, x, y, z);
    }

    private static SBankValueEvent GetSBankValueEvent(Dictionary<string, object> gameDic, GameEventHeader gameEvent)
    {
        string data = GetString(gameDic, "m_data");
        string name = GetString(gameDic, "m_name");
        int type = GetInt(gameDic, "m_type");
        return new SBankValueEvent(gameEvent.UserId, gameEvent.EventId, gameEvent.Bits, gameEvent.Gameloop, name, data, type);
    }

    private static SBankSignatureEvent GetSBankSignatureEvent(Dictionary<string, object> gameDic, GameEventHeader gameEvent)
    {
        string toonHandle = GetString(gameDic, "m_toonHandle");
        var signature = GetIntList(gameDic, "m_signature");
        return new SBankSignatureEvent(gameEvent.UserId, gameEvent.EventId, gameEvent.Bits, gameEvent.Gameloop, toonHandle, signature);
    }

    private static SBankSectionEvent GetSBankSectionEvent(Dictionary<string, object> gameDic, GameEventHeader gameEvent)
    {
        string name = GetString(gameDic, "m_name");
        return new SBankSectionEvent(gameEvent.UserId, gameEvent.EventId, gameEvent.Bits, gameEvent.Gameloop, name);
    }

    private static SBankKeyEvent GetSBankKeyEvent(Dictionary<string, object> pydic, GameEventHeader gameEvent)
    {
        string data = GetString(pydic, "m_data");
        string name = GetString(pydic, "m_name");
        int type = GetInt(pydic, "m_type");
        return new SBankKeyEvent(gameEvent.UserId, gameEvent.EventId, gameEvent.Bits, gameEvent.Gameloop, name, data, type);
    }

    private static SBankFileEvent GetSBankFileEvent(Dictionary<string, object> pydic, GameEventHeader gameEvent)
    {
        string name = GetString(pydic, "m_name");
        return new SBankFileEvent(gameEvent.UserId, gameEvent.EventId, gameEvent.Bits, gameEvent.Gameloop, name);
    }
}

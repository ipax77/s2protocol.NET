using IronPython.Runtime;
using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;internal static partial class Parse
{
    public static GameEvents GameEvents(dynamic pydic)
    {
        List<GameEvent> gameevents = new();

        foreach (var ent in pydic)
        {
            if (ent is PythonDictionary gameDic)
            {
                GameEvent gameEvent = GetGameEvent(gameDic);

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
                    GameEventType.STriggerSoundLengthSyncEvent => new STriggerSoundLengthSyncEvent(gameEvent),
                    GameEventType.SUserFinishedLoadingSyncEvent => new SUserFinishedLoadingSyncEvent(gameEvent),
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
                    _ => GetUnknownEvent(gameDic, gameEvent)
                };
                gameevents.Add(detailEvent);
            }
        }
        return new GameEvents(gameevents);
    }

    private static GameEvent GetGameEvent(PythonDictionary pydic)
    {
        int userId = GetUserId(pydic);
        int eventId = GetInt(pydic, "_eventid");
        string type = GetString(pydic, "_event");
        int bits = GetInt(pydic, "_bits");
        int gameloop = GetInt(pydic, "_gameloop");
        return new GameEvent(userId, eventId, type, bits, gameloop);
    }

    private static int GetUserId(PythonDictionary pydic)
    {
        if (pydic.ContainsKey("_userid"))
        {
            if (pydic["_userid"] is PythonDictionary userDic)
            {
                return GetInt(userDic, "m_userId");
            }
        }
        return 0;
    }

    private static UnknownGameEvent GetUnknownEvent(PythonDictionary pydic, GameEvent gameEvent)
    {
        return new UnknownGameEvent(gameEvent, GetString(pydic, "_event"));
        // return gameEvent;
    }

    private static STriggerDialogControlEvent GetSTriggerDialogControlEvent(PythonDictionary gameDic, GameEvent gameEvent)
    {
        long m_controlId = GetBigInt(gameDic, "m_controlId");
        int? mouseButton = GetMouseButton(gameDic);
        string? textChanged = GetTextChanged(gameDic);
        long m_eventType = GetBigInt(gameDic, "m_eventType");
        return new STriggerDialogControlEvent(gameEvent, m_controlId, mouseButton, textChanged, m_eventType);
    }

    private static int? GetMouseButton(PythonDictionary pydic)
    {
        if (pydic.ContainsKey("m_eventData"))
        {
            if (pydic["m_eventData"] is PythonDictionary mouseDic)
            {
                return GetNullableInt(mouseDic, "MouseButton");
            }
        }
        return null;
    }

    private static string? GetTextChanged(PythonDictionary pydic)
    {
        if (pydic.ContainsKey("m_eventData"))
        {
            if (pydic["m_eventData"] is PythonDictionary mouseDic)
            {
                return GetNullableString(mouseDic, "TextChanged");
            }
        }
        return null;
    }

    private static SSetSyncPlayingTimeEvent GetSSetSyncPlayingTimeEvent(PythonDictionary gameDic, GameEvent gameEvent)
    {
        int m_syncTime = GetInt(gameDic, "m_syncTime");
        return new SSetSyncPlayingTimeEvent(gameEvent, m_syncTime);
    }

    private static SSetSyncLoadingTimeEvent GetSSetSyncLoadingTimeEvent(PythonDictionary gameDic, GameEvent gameEvent)
    {
        int m_syncTime = GetInt(gameDic, "m_syncTime");
        return new SSetSyncLoadingTimeEvent(gameEvent, m_syncTime);
    }

    private static SGameUserLeaveEvent GetSGameUserLeaveEvent(PythonDictionary gameDic, GameEvent gameEvent)
    {
        int leaveReason = GetInt(gameDic, "m_leaveReason");
        return new SGameUserLeaveEvent(gameEvent, leaveReason);
    }

    private static SControlGroupUpdateEvent GetSControlGroupUpdateEvent(PythonDictionary gameDic, GameEvent gameEvent)
    {
        int controlGroupUpdate = GetInt(gameDic, "m_controlGroupUpdate");
        return new SControlGroupUpdateEvent(gameEvent, controlGroupUpdate);
    }

    private static SCommandManagerStateEvent GetSCommandManagerStateEvent(PythonDictionary gameDic, GameEvent gameEvent)
    {
        int state = GetInt(gameDic, "m_state");
        int? sequence = GetNullableInt(gameDic, "m_sequence");
        return new SCommandManagerStateEvent(gameEvent, state, sequence);
    }

    private static SCmdUpdateTargetPointEvent GetSCmdUpdateTargetPointEvent(PythonDictionary gameDic, GameEvent gameEvent)
    {
        long x = 0;
        long y = 0;
        long z = 0;
        if (gameDic.TryGetValue("m_target", out object? target))
        {
            if (target is PythonDictionary targetDic)
            {
                x = GetBigInt(targetDic, "x");
                y = GetBigInt(targetDic, "y");
                z = GetBigInt(targetDic, "z");
            }
        }
        return new SCmdUpdateTargetPointEvent(gameEvent, x, y, z);
    }

    private static SBankValueEvent GetSBankValueEvent(PythonDictionary gameDic, GameEvent gameEvent)
    {
        string data = GetString(gameDic, "m_data");
        string name = GetString(gameDic, "m_name");
        int type = GetInt(gameDic, "m_type");
        return new SBankValueEvent(gameEvent, name, data, type);
    }

    private static SBankSignatureEvent GetSBankSignatureEvent(PythonDictionary gameDic, GameEvent gameEvent)
    {
        string toonHandle = GetString(gameDic, "m_toonHandle");
        var signature = GetIntList(gameDic, "m_signature");
        return new SBankSignatureEvent(gameEvent, toonHandle, signature);
    }

    private static SBankSectionEvent GetSBankSectionEvent(PythonDictionary gameDic, GameEvent gameEvent)
    {
        string name = GetString(gameDic, "m_name");
        return new SBankSectionEvent(gameEvent, name);
    }

    private static SBankKeyEvent GetSBankKeyEvent(PythonDictionary pydic, GameEvent gameEvent)
    {
        string data = GetString(pydic, "m_data");
        string name = GetString(pydic, "m_name");
        int type = GetInt(pydic, "m_type");
        return new SBankKeyEvent(gameEvent, name, data, type);
    }

    private static SBankFileEvent GetSBankFileEvent(PythonDictionary pydic, GameEvent gameEvent)
    {
        string name = GetString(pydic, "m_name");
        return new SBankFileEvent(gameEvent, name);
    }
}

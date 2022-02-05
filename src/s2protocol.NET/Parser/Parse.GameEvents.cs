using IronPython.Runtime;
using s2protocol.NET.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s2protocol.NET.Parser;

internal partial class Parse
{
    public static GameEvents GameEvents(dynamic pydic)
    {
        List<GameEvent> gameevents = new List<GameEvent>();

        foreach (var ent in pydic)
        {
            PythonDictionary? gameDic = ent as PythonDictionary;
            if (gameDic != null)
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
                    GameEventType.STriggerSoundLengthSyncEvent => GetSTriggerSoundLengthSyncEvent(gameDic, gameEvent),
                    GameEventType.SUserFinishedLoadingSyncEvent => GetSUserFinishedLoadingSyncEvent(gameDic, gameEvent),
                    GameEventType.SUserOptionsEvent => GetSUserOptionsEvent(gameDic, gameEvent),
                    _ => GetUnknownEvent(gameDic, gameEvent)
                };
                gameevents.Add(detailEvent);
            }
        }
        return new GameEvents(gameevents);
    }

    private static UnknownGameEvent GetUnknownEvent(PythonDictionary pydic, GameEvent gameEvent)
    {
        ReplayDecoder.logger.DecodeWarning($"Game event type unknown: {GetString(pydic, "_event")}");
        return new UnknownGameEvent(gameEvent, pydic);
    }

    private static SUserOptionsEvent GetSUserOptionsEvent(PythonDictionary gameDic, GameEvent gameEvent)
    {
        bool testCheatsEnabled = GetBool(gameDic, "m_testCheatsEnabled");
        bool multiplayerCheatsEnabled = GetBool(gameDic, "m_multiplayerCheatsEnabled");
        bool gameFullyDownloaded = GetBool(gameDic, "m_gameFullyDownloaded");
        string hotkeyProfile = GetString(gameDic, "m_hotkeyProfile");
        bool useGalaxyAsserts = GetBool(gameDic, "m_useGalaxyAsserts");
        bool debugPauseEnabled = GetBool(gameDic, "m_debugPauseEnabled");
        bool cameraFollow = GetBool(gameDic, "m_cameraFollow");
        bool isMapToMapTransition = GetBool(gameDic, "m_isMapToMapTransition");
        int buildNum = GetInt(gameDic, "m_buildNum");
        int versionFlags = GetInt(gameDic, "m_versionFlags");
        bool developmentCheatsEnabled = GetBool(gameDic, "m_developmentCheatsEnabled");
        bool platformMac = GetBool(gameDic, "m_platformMac");
        int baseBuildNum = GetInt(gameDic, "m_baseBuildNum");
        bool syncChecksummingEnabled = GetBool(gameDic, "m_syncChecksummingEnabled");
        return new SUserOptionsEvent(gameEvent,
                                     testCheatsEnabled,
                                     multiplayerCheatsEnabled,
                                     gameFullyDownloaded,
                                     hotkeyProfile,
                                     useGalaxyAsserts,
                                     debugPauseEnabled,
                                     cameraFollow,
                                     isMapToMapTransition,
                                     buildNum,
                                     versionFlags,
                                     developmentCheatsEnabled,
                                     platformMac,
                                     baseBuildNum,
                                     syncChecksummingEnabled);
    }

    private static SUserFinishedLoadingSyncEvent GetSUserFinishedLoadingSyncEvent(PythonDictionary gameDic, GameEvent gameEvent)
    {
        return new SUserFinishedLoadingSyncEvent(gameEvent);
    }

    private static STriggerSoundLengthSyncEvent GetSTriggerSoundLengthSyncEvent(PythonDictionary gameDic, GameEvent gameEvent)
    {
        return new STriggerSoundLengthSyncEvent(gameEvent);
    }

    private static STriggerPingEvent GetSTriggerPingEvent(PythonDictionary gameDic, GameEvent gameEvent)
    {
        bool pingedMinimap = GetBool(gameDic, "m_pingedMinimap");
        int unitLink = GetInt(gameDic, "m_unitLink");
        bool unitIsUnderConstruction = GetBool(gameDic, "m_unitIsUnderConstruction");
        int option = GetInt(gameDic, "m_option");
        int unit = GetInt(gameDic, "m_unit");
        (int unitX, int unitY, int unitZ) = GetUnitPosition(gameDic);
        int? unitControlPlayerId = GetNullableInt(gameDic, "m_unitControlPlayerId");
        (int pointX, int pointY) = GetPoint(gameDic);
        int? unitUpkeepPlayerId = GetNullableInt(gameDic, "m_unitUpkeepPlayerId");
        return new STriggerPingEvent(gameEvent,
                                     pingedMinimap,
                                     unit,
                                     unitIsUnderConstruction,
                                     option,
                                     unit,
                                     unitX,
                                     unitY,
                                     unitZ,
                                     unitControlPlayerId,
                                     pointX,
                                     pointY,
                                     unitUpkeepPlayerId);
    }

    private static (int, int) GetPoint(PythonDictionary pydic)
    {
        if (pydic.ContainsKey("m_point"))
        {
            PythonDictionary? pointDic = pydic["m_point"] as PythonDictionary;
            if (pointDic != null)
            {
                int x = GetInt(pointDic, "x");
                int y = GetInt(pointDic, "y");
                return (x, y);
            }
        }
        return (0, 0);
    }

    private static (int, int, int) GetUnitPosition(PythonDictionary pydic)
    {
        if (pydic.ContainsKey("m_unitPosition"))
        {
            PythonDictionary? posDic = pydic["m_unitPosition"] as PythonDictionary;
            if (posDic != null)
            {
                int x = GetInt(posDic, "x");
                int y = GetInt(posDic, "y");
                int z = GetInt(posDic, "z");
                return (x, y, z);
            }
        }
        return (0, 0, 0);
    }

    private static STriggerDialogControlEvent GetSTriggerDialogControlEvent(PythonDictionary gameDic, GameEvent gameEvent)
    {
        int m_controlId = GetInt(gameDic, "m_controlId");
        int? mouseButton = GetMouseButton(gameDic);
        string? textChanged = GetTextChanged(gameDic);
        int m_eventType = GetInt(gameDic, "m_eventType");
        return new STriggerDialogControlEvent(gameEvent, m_controlId, mouseButton, textChanged, m_eventType);
    }

    private static int? GetMouseButton(PythonDictionary pydic)
    {
        if (pydic.ContainsKey("m_eventData"))
        {
            PythonDictionary? mouseDic = pydic["m_eventData"] as PythonDictionary;
            if (mouseDic != null)
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
            PythonDictionary? mouseDic = pydic["m_eventData"] as PythonDictionary;
            if (mouseDic != null)
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

    private static SSelectionDeltaEvent GetSSelectionDeltaEvent(PythonDictionary gameDic, GameEvent gameEvent)
    {
        return new SSelectionDeltaEvent(gameEvent, "");
    }

    private static SGameUserLeaveEvent GetSGameUserLeaveEvent(PythonDictionary gameDic, GameEvent gameEvent)
    {
        return new SGameUserLeaveEvent(gameEvent, "");
    }

    private static SControlGroupUpdateEvent GetSControlGroupUpdateEvent(PythonDictionary gameDic, GameEvent gameEvent)
    {
        return new SControlGroupUpdateEvent(gameEvent, "");
    }

    private static SCommandManagerStateEvent GetSCommandManagerStateEvent(PythonDictionary gameDic, GameEvent gameEvent)
    {
        return new SCommandManagerStateEvent(gameEvent, "");
    }

    private static SCmdUpdateTargetPointEvent GetSCmdUpdateTargetPointEvent(PythonDictionary gameDic, GameEvent gameEvent)
    {
        return new SCmdUpdateTargetPointEvent(gameEvent, "");
    }

    private static SCmdEvent GetSCmdEvent(PythonDictionary gameDic, GameEvent gameEvent)
    {
        int? unitGroup = GetNullableInt(gameDic, "m_unitGroup");
        (int abilLink, int abilCmdIndex, string? abilCmdData) = GetAbil(gameDic);
        int cmdFalgs = GetInt(gameDic, "m_cmdFlags");
        int sequence = GetInt(gameDic, "m_sequence");
        int? otherUnit = GetNullableInt(gameDic, "m_otherUnit");
        (int? targetX, int? targetY, int? targetZ) = GetSCmdEventTarget(gameDic);
        return new SCmdEvent(gameEvent, unitGroup, abilLink, abilCmdIndex, abilCmdData, targetX, targetY, targetZ, cmdFalgs, sequence, otherUnit);
    }

    private static (int?, int?, int?) GetSCmdEventTarget(PythonDictionary pydic)
    {
        if (pydic.TryGetValue("m_data", out object? data))
        {
            if (data != null && data as PythonDictionary != null)
            {
                if (pydic.TryGetValue("TargetPoint", out object? target))
                {
                    if (target != null)
                    {
                        PythonDictionary? targetDic = target as PythonDictionary;
                        if (targetDic != null)
                        {
                            int x = GetInt(targetDic, "x");
                            int y = GetInt(targetDic, "y");
                            int z = GetInt(targetDic, "z");
                            return (x, y, z);
                        }
                    }
                }
            }
        }
        return (null, null, null);
    }

    private static (int, int, string?) GetAbil(PythonDictionary pydic)
    {
        if (pydic.ContainsKey("m_abil"))
        {
            PythonDictionary? abilDic = pydic["m_abil"] as PythonDictionary;
            if (abilDic != null)
            {
                int link = GetInt(abilDic, "m_abilLink");
                int cmdIndex = GetInt(abilDic, "m_abilCmdIndex");
                string? cmdData = GetNullableString(abilDic, "m_abilCmdData");
                return (link, cmdIndex, cmdData);
            }
        }
        return (0, 0, null);
    }



    private static SCameraUpdateEvent GetSCameraUpdateEvent(PythonDictionary gameDic, GameEvent gameEvent)
    {
        string? reason = GetNullableString(gameDic, "m_reason");
        int? distance = GetNullableInt(gameDic, "m_distance");
        int? yaw = GetNullableInt(gameDic, "m_yaw");
        int? pitch = GetNullableInt(gameDic, "m_pitch");
        bool follow = GetBool(gameDic, "m_follow");
        (int? targetX, int? targetY) = GetSCameraUpdateEventTarget(gameDic);
        return new SCameraUpdateEvent(gameEvent, reason, distance, targetX, targetY, yaw, pitch, follow);
    }

    private static (int?, int?) GetSCameraUpdateEventTarget(PythonDictionary pydic)
    {
        if (pydic.TryGetValue("m_target", out object? target))
        {
            if (target != null)
            {
                PythonDictionary? targetDic = target as PythonDictionary;
                if (targetDic != null)
                {
                    int x = GetInt(targetDic, "x");
                    int y = GetInt(targetDic, "y");
                    return (x, y);
                }
            }
        }
        return (null, null);
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
            PythonDictionary? userDic = pydic["_userid"] as PythonDictionary;
            if (userDic != null)
            {
                return GetInt(userDic, "m_userId");
            }
        }
        return 0;
    }
}

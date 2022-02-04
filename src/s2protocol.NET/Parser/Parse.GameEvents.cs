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
        List<SBankFileEvent> SBankFileEvents = new();
        List<SBankKeyEvent> SBankKeyEvents = new();
        List<SBankSectionEvent> SBankSectionEvents = new();
        List<SBankSignatureEvent> SBankSignatureEvents = new();
        List<SBankValueEvent> SBankValueEvents = new();
        List<SCameraUpdateEvent> SCameraUpdateEvents = new();
        List<SCmdEvent> SCmdEvents = new();

        foreach (var ent in pydic)
        {
            PythonDictionary? gameDic = ent as PythonDictionary;
            if (gameDic != null)
            {
                GameEvent gameEvent = GetGameEvent(gameDic);
                
                if (gameEvent.EventType == GameEventType.SBankFileEvent)
                {
                    SBankFileEvents.Add(GetSBankFileEvent(gameDic, gameEvent));
                }
                else
                {
                    ReplayDecoder.logger.DecodeWarning($"Game event type unknown: {GetString(gameDic, "_event")}");
                }
            }
        }
        return new GameEvents(
            SBankFileEvents,
            SBankKeyEvents,
            SBankSectionEvents,
            SBankSignatureEvents,
            SBankValueEvents,
            SCameraUpdateEvents,
            SCmdEvents
        );

        //SBankFileEvent = 1,
        //SBankKeyEvent = 2,
        //SBankSectionEvent = 3,
        //SBankSignatureEvent = 4,
        //SBankValueEvent = 5,
        //SCameraUpdateEvent = 6,
        //SCmdEvent = 7,
        // -----------------------------------
        //SCmdUpdateTargetPointEvent = 8,
        //SCommandManagerStateEvent = 9,
        //SControlGroupUpdateEvent = 10,
        //SGameUserLeaveEvent = 11,
        //SSelectionDeltaEvent = 12,
        //SSetSyncLoadingTimeEvent = 13,
        //SSetSyncPlayingTimeEvent = 14,
        //STriggerDialogControlEvent = 15,
        //STriggerPingEvent = 16,
        //STriggerSoundLengthSyncEvent = 17,
        //SUserFinishedLoadingSyncEvent = 18,
        //SUserOptionsEvent = 19
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

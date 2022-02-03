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
    internal static Initdata? InitData(dynamic pydic)
    {
        if (pydic.ContainsKey("m_syncLobbyState"))
        {
            PythonDictionary? initDic = pydic["m_syncLobbyState"] as PythonDictionary;
            if (initDic != null)
            {
                List<UserInitialData> userInitialDatas = GetUserInitialData(initDic);
                LobbyState lobbyState = GetLobbyState(initDic);
                GameDescription gameDescription = GetGameDescription(initDic);
                return new Initdata(userInitialDatas, lobbyState, gameDescription);
            }
        }
        return null;
    }

    private static GameDescription GetGameDescription(PythonDictionary pydic)
    {
        throw new NotImplementedException();
    }

    private static LobbyState GetLobbyState(PythonDictionary pydic)
    {
        throw new NotImplementedException();
    }

    private static List<Slot> GetSlots(PythonDictionary pydic)
    {
        throw new NotImplementedException();
    }

    private static List<UserInitialData> GetUserInitialData(PythonDictionary pydic)
    {
        List<UserInitialData> initDatas = new List<UserInitialData>();
        if (pydic.ContainsKey("m_userInitialData"))
        {
            List? userInitalDatas = pydic["m_userInitialData"] as List;
            
            if (userInitalDatas != null)
            {
                foreach (var userInitalData in userInitalDatas)
                {
                    PythonDictionary? initDic = userInitalData as PythonDictionary;
                    if (initDic != null)
                    {
                        string mount = GetString(initDic, "m_mount");
                        string skin = GetString(initDic, "m_skin");
                        int observe = GetInt(initDic, "m_observe");
                        int? teamPref = GetTeamPreference(initDic);
                        string toonHandle = GetString(initDic, "m_toonHandle");
                        int combinedRaceLevels = GetInt(initDic, "m_combinedRaceLevels");
                        int highestLeague = GetInt(initDic, "m_highestLeague");
                        string clanTag = GetString(initDic, "m_clanTag");
                        bool testMap = GetBool(initDic, "m_testMap");
                        bool testAuto = GetBool(initDic, "m_testAuto");
                        bool examine = GetBool(initDic, "m_examine");
                        int testType = GetInt(initDic, "m_testType");
                        bool customInterface = GetBool(initDic, "m_customInterface");
                        string clanLogo = GetString(initDic, "m_clanLogo");
                        string name = GetString(initDic, "m_name");
                        string? racePreference = GetRacePreference(initDic);
                        int randomSeed = GetInt(initDic, "m_randomSeed");
                        string hero = GetString(initDic, "m_hero");
                        int? scaledRating = GetNullableInt(initDic, "m_scaledRating");
                        UserInitialData initData = new UserInitialData(mount, skin, observe, teamPref, toonHandle, combinedRaceLevels, highestLeague, clanTag, testMap, testAuto, examine, testType, customInterface, clanLogo, name, racePreference, randomSeed, hero, scaledRating);
                        initDatas.Add(initData);
                    }
                }
            }
        }
        return initDatas;
    }

    private static int? GetTeamPreference(PythonDictionary pydic)
    {
        if (pydic.ContainsKey("m_teamPreference"))
        {
            PythonDictionary? teamDic = pydic["m_teamPreference"] as PythonDictionary;
            if (teamDic != null)
            {
                return GetNullableInt(teamDic, "m_team");
            }
        }
        return null;
    }

    private static string? GetRacePreference(PythonDictionary pydic)
    {
        if (pydic.ContainsKey("m_racePreference"))
        {
            PythonDictionary? raceDic = pydic["m_racePreference"] as PythonDictionary;
            if (raceDic != null)
            {
                return GetNullableString(raceDic, "m_race");
            }
        }
        return null;
    }
}

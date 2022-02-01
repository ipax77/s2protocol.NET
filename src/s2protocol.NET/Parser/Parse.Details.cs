using IronPython.Runtime;
using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal partial class Parse
{
    public static Details Datails(PythonDictionary details)
    {
        var campaignIndex = GetInt(details, "m_campaignIndex");
        var defaultDiff = GetInt(details, "m_defaultDifficulty");
        var desc = GetString(details, "m_description");
        var diff = GetString(details, "m_difficulty");
        var disableRec = GetBool(details, "m_disableRecoverGame");
        var speed = GetInt(details, "m_gameSpeed");
        var image = GetString(details, "m_imageFilePath");
        var isBlizzard = GetBool(details, "m_isBlizzardMap");
        var mapName = GetString(details, "m_mapFileName");
        var mini = GetBool(details, "m_miniSave");
        var restart = GetBool(details, "m_restartAsTransitionMap");
        var offset = GetBigInt(details, "m_timeLocalOffset");
        var time = GetBigInt(details, "m_timeUTC");
        var title = GetString(details, "m_title");
        var players = GetDetailsPlayers(details);

        return new Details(campaignIndex, defaultDiff, desc, diff, disableRec, speed, image, isBlizzard, mapName, mini, restart, offset, time, title, players);
    }

    private static List<DetailsPlayer> GetDetailsPlayers(PythonDictionary pydic)
    {
        List<DetailsPlayer> players = new List<DetailsPlayer>();
        if (pydic.ContainsKey("m_playerList"))
        {
            var plDics = pydic["m_playerList"] as ICollection<object>;
            if (plDics != null)
            {
                foreach (PythonDictionary plDic in plDics)
                {
                    if (plDic != null)
                    {
                        var color = GetColor(plDic);
                        var control = GetInt(plDic, "m_control");
                        var handicap = GetInt(plDic, "m_handicap");
                        var hero = GetString(plDic, "m_hero");
                        var name = GetString(plDic, "m_name");
                        var observe = GetInt(plDic, "m_observe");
                        var race = GetString(plDic, "m_race");
                        var result = GetInt(plDic, "m_result");
                        var team = GetInt(plDic, "m_teamId");
                        var toon = GetToon(plDic);
                        var slot = GetInt(plDic, "m_workingSetSlotId");
                        players.Add(new DetailsPlayer(color, control, handicap, hero, name, observe, race, result, team, toon, slot));
                    }
                }
            }
        }

        return players;
    }

    private static Toon GetToon(PythonDictionary pydic)
    {
        if (pydic.ContainsKey("m_toon"))
        {
            var toonDic = pydic["m_toon"] as PythonDictionary;
            if (toonDic != null)
            {
                return new Toon(GetInt(toonDic, "m_id"), GetString(toonDic, "m_programId"), GetInt(toonDic, "m_realm"), GetInt(toonDic, "m_region"));
            }
        }
        return new Toon(0, "", 0, 0);
    }

    private static PlayerColor GetColor(PythonDictionary pydic)
    {
        if (pydic.ContainsKey("m_color"))
        {
            var colDic = pydic["m_color"] as PythonDictionary;
            if (colDic != null)
            {
                return new PlayerColor(GetInt(colDic, "m_a"), GetInt(colDic, "m_b"), GetInt(colDic, "m_g"), GetInt(colDic, "m_r"));
            }
        }
        return new PlayerColor(0, 0, 0, 0);
    }
}

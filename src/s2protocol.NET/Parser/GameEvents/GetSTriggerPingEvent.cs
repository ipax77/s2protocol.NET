using IronPython.Runtime;
using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;

internal partial class Parse
{
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
}

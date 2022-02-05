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
        long option = GetBigInt(gameDic, "m_option");
        int unit = GetInt(gameDic, "m_unit");
        (long unitX, long unitY, long unitZ) = GetUnitPosition(gameDic);
        int? unitControlPlayerId = GetNullableInt(gameDic, "m_unitControlPlayerId");
        (long pointX, long pointY) = GetPoint(gameDic);
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

    private static (long, long) GetPoint(PythonDictionary pydic)
    {
        if (pydic.ContainsKey("m_point"))
        {
            PythonDictionary? pointDic = pydic["m_point"] as PythonDictionary;
            if (pointDic != null)
            {
                long x = GetBigInt(pointDic, "x");
                long y = GetBigInt(pointDic, "y");
                return (x, y);
            }
        }
        return (0, 0);
    }

    private static (long, long, long) GetUnitPosition(PythonDictionary pydic)
    {
        if (pydic.ContainsKey("m_unitPosition"))
        {
            PythonDictionary? posDic = pydic["m_unitPosition"] as PythonDictionary;
            if (posDic != null)
            {
                long x = GetBigInt(posDic, "x");
                long y = GetBigInt(posDic, "y");
                long z = GetBigInt(posDic, "z");
                return (x, y, z);
            }
        }
        return (0, 0, 0);
    }
}

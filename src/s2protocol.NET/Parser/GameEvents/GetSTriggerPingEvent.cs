using s2protocol.NET.Models;

namespace s2protocol.NET.Parser; internal static partial class Parse
{
    private static STriggerPingEvent GetSTriggerPingEvent(Dictionary<string, object> gameDic, GameEvent gameEvent)
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
                                     unitLink,
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

    private static (long, long) GetPoint(Dictionary<string, object> pydic)
    {
        if (pydic.ContainsKey("m_point"))
        {
            if (pydic["m_point"] is Dictionary<string, object> pointDic)
            {
                long x = GetBigInt(pointDic, "x");
                long y = GetBigInt(pointDic, "y");
                return (x, y);
            }
        }
        return (0, 0);
    }

    private static (long, long, long) GetUnitPosition(Dictionary<string, object> pydic)
    {
        if (pydic.ContainsKey("m_unitPosition"))
        {
            if (pydic["m_unitPosition"] is Dictionary<string, object> posDic)
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

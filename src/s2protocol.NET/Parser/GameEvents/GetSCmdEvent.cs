using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;

internal static partial class Parse
{
    private static SCmdEvent GetSCmdEvent(Dictionary<string, object> gameDic, GameEventHeader gameEvent)
    {
        int? unitGroup = GetNullableInt(gameDic, "m_unitGroup");
        (int abilLink, int abilCmdIndex, string? abilCmdData) = GetAbil(gameDic);
        int cmdFalgs = GetInt(gameDic, "m_cmdFlags");
        int sequence = GetInt(gameDic, "m_sequence");
        int? otherUnit = GetNullableInt(gameDic, "m_otherUnit");
        (long? targetX, long? targetY, long? targetZ, int? targetUnitTag) = GetSCmdEventTarget(gameDic);
        return new SCmdEvent(gameEvent.UserId, gameEvent.EventId, gameEvent.Bits, gameEvent.Gameloop, unitGroup, abilLink, abilCmdIndex, abilCmdData, targetX, targetY, targetZ, cmdFalgs, sequence, otherUnit, targetUnitTag);
    }

    private static (long?, long?, long?, int?) GetSCmdEventTarget(Dictionary<string, object> pydic)
    {
        if (pydic.TryGetValue("m_data", out object? data)
            && data is Dictionary<string, object> dataDictionary)
        {
            if (dataDictionary.TryGetValue("TargetPoint", out object? point)
                && point is Dictionary<string, object> pointDictionary)
            {
                return (GetBigInt(pointDictionary, "x"), GetBigInt(pointDictionary, "y"), GetBigInt(pointDictionary, "z"), null);
            }

            if (dataDictionary.TryGetValue("TargetUnit", out object? unit)
                && unit is Dictionary<string, object> unitDictionary)
            {
                int? targetUnitTag = GetNullableInt(unitDictionary, "m_tag");
                if (unitDictionary.TryGetValue("m_snapshotPoint", out object? snapshot)
                    && snapshot is Dictionary<string, object> snapshotDictionary)
                {
                    return (GetBigInt(snapshotDictionary, "x"), GetBigInt(snapshotDictionary, "y"), GetBigInt(snapshotDictionary, "z"), targetUnitTag);
                }

                return (null, null, null, targetUnitTag);
            }
        }

        return (null, null, null, null);
    }

    private static (int, int, string?) GetAbil(Dictionary<string, object> pydic)
    {
        if (pydic.ContainsKey("m_abil"))
        {
            if (pydic["m_abil"] is Dictionary<string, object> abilDic)
            {
                int link = GetInt(abilDic, "m_abilLink");
                int cmdIndex = GetInt(abilDic, "m_abilCmdIndex");
                string? cmdData = GetNullableString(abilDic, "m_abilCmdData");
                return (link, cmdIndex, cmdData);
            }
        }
        return (0, 0, null);
    }
}

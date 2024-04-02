using IronPython.Runtime;
using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;internal static partial class Parse
{
    private static SCmdEvent GetSCmdEvent(PythonDictionary gameDic, GameEvent gameEvent)
    {
        int? unitGroup = GetNullableInt(gameDic, "m_unitGroup");
        (int abilLink, int abilCmdIndex, string? abilCmdData) = GetAbil(gameDic);
        int cmdFalgs = GetInt(gameDic, "m_cmdFlags");
        int sequence = GetInt(gameDic, "m_sequence");
        int? otherUnit = GetNullableInt(gameDic, "m_otherUnit");
        (long? targetX, long? targetY, long? targetZ) = GetSCmdEventTarget(gameDic);
        return new SCmdEvent(gameEvent, unitGroup, abilLink, abilCmdIndex, abilCmdData, targetX, targetY, targetZ, cmdFalgs, sequence, otherUnit);
    }

    private static (long?, long?, long?) GetSCmdEventTarget(PythonDictionary pydic)
    {
        if (pydic.TryGetValue("m_data", out object? data))
        {
            if (data != null)
            {
                if (pydic.TryGetValue("TargetPoint", out object? target))
                {
                    if (target != null)
                    {
                        if (target is PythonDictionary targetDic)
                        {
                            long x = GetBigInt(targetDic, "x");
                            long y = GetBigInt(targetDic, "y");
                            long z = GetBigInt(targetDic, "z");
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
            if (pydic["m_abil"] is PythonDictionary abilDic)
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

using IronPython.Runtime;
using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;

internal partial class Parse
{
    private static SCmdUpdateTargetUnitEvent GetSCmdUpdateTargetUnitEvent(PythonDictionary pydic, GameEvent gameEvent)
    {
        if (pydic.TryGetValue("m_target", out object? target))
        {
            PythonDictionary? targetDic = pydic["m_target"] as PythonDictionary;
            if (targetDic != null)
            {
                int m_snapshotControlPlayerId = GetInt(targetDic, "m_snapshotControlPlayerId");
                (long pointX, long pointY, long pointZ) = GetSnapshotPoint(targetDic);
                int m_snapshotUpkeepPlayerId = GetInt(targetDic, "m_snapshotUpkeepPlayerId");
                int m_timer = GetInt(targetDic, "m_timer");
                int m_targetUnitFlags = GetInt(targetDic, "m_targetUnitFlags");
                int m_snapshotUnitLink = GetInt(targetDic, "m_snapshotUnitLink");
                int m_tag = GetInt(targetDic, "m_tag");
                return new SCmdUpdateTargetUnitEvent(gameEvent,
                                                     m_snapshotControlPlayerId,
                                                     pointX,
                                                     pointY,
                                                     pointZ,
                                                     m_snapshotUpkeepPlayerId,
                                                     m_timer,
                                                     m_targetUnitFlags,
                                                     m_snapshotUnitLink,
                                                     m_tag);
            }
        }
        return new SCmdUpdateTargetUnitEvent(gameEvent,
                                     0,
                                     0,
                                     0,
                                     0,
                                     0,
                                     0,
                                     0,
                                     0,
                                     0);
    }

    private static (long, long, long) GetSnapshotPoint(PythonDictionary pydic)
    {
        if (pydic.TryGetValue("m_snapshotPoint", out object? point))
        {
            PythonDictionary? pointDic = point as PythonDictionary;
            if (pointDic != null)
            {
                return (GetBigInt(pointDic, "x"), GetBigInt(pointDic, "y"), GetBigInt(pointDic, "z"));
            }
        }
        return (0, 0, 0);
    }
}

using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;internal static partial class Parse
{
    private static SCmdUpdateTargetUnitEvent GetSCmdUpdateTargetUnitEvent(Dictionary<string, object> pydic, GameEvent gameEvent)
    {
        if (pydic.TryGetValue("m_target", out object? target))
        {
            if (target is Dictionary<string, object> targetDic)
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

    private static (long, long, long) GetSnapshotPoint(Dictionary<string, object> pydic)
    {
        if (pydic.TryGetValue("m_snapshotPoint", out object? point))
        {
            if (point is Dictionary<string, object> pointDic)
            {
                return (GetBigInt(pointDic, "x"), GetBigInt(pointDic, "y"), GetBigInt(pointDic, "z"));
            }
        }
        return (0, 0, 0);
    }
}

using IronPython.Runtime;
using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal partial class Parse
{
    private static SCameraSaveEvent GetSCameraSaveEvent(PythonDictionary pydic, GameEvent gameEvent)
    {
        int which = GetInt(pydic, "m_which");
        (long targetX, long targetY) = SCameraSaveEventTarget(pydic);
        return new SCameraSaveEvent(gameEvent, which, targetX, targetY);
    }

    private static (long targetX, long targetY) SCameraSaveEventTarget(PythonDictionary pydic)
    {
        if (pydic.TryGetValue("m_target", out object? target))
        {
            if (target != null)
            {
                PythonDictionary? targetDic = target as PythonDictionary;
                if (targetDic != null)
                {
                    return (GetBigInt(targetDic, "x"), GetBigInt(targetDic, "y"));
                }
            }
        }
        return (0, 0);
    }
}

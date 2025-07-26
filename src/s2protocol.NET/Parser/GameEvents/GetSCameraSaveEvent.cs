using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal static partial class Parse
{
    private static SCameraSaveEvent GetSCameraSaveEvent(Dictionary<string, object> pydic, GameEvent gameEvent)
    {
        int which = GetInt(pydic, "m_which");
        (long targetX, long targetY) = SCameraSaveEventTarget(pydic);
        return new SCameraSaveEvent(gameEvent, which, targetX, targetY);
    }

    private static (long targetX, long targetY) SCameraSaveEventTarget(Dictionary<string, object> pydic)
    {
        if (pydic.TryGetValue("m_target", out object? target))
        {
            if (target != null)
            {
                if (target is Dictionary<string, object> targetDic)
                {
                    return (GetBigInt(targetDic, "x"), GetBigInt(targetDic, "y"));
                }
            }
        }
        return (0, 0);
    }
}

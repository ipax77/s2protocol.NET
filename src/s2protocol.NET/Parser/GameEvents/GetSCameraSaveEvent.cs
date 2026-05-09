using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal static partial class Parse
{
    private static SCameraSaveEvent GetSCameraSaveEvent(Dictionary<string, object> pydic, GameEventHeader gameEvent)
    {
        int which = GetInt(pydic, "m_which");
        (long targetX, long targetY) = SCameraSaveEventTarget(pydic);
        return new SCameraSaveEvent(gameEvent.UserId, gameEvent.EventId, gameEvent.Bits, gameEvent.Gameloop, which, targetX, targetY);
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

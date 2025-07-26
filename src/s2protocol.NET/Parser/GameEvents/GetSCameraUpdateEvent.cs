using s2protocol.NET.Models;

namespace s2protocol.NET.Parser; internal static partial class Parse
{
    private static SCameraUpdateEvent GetSCameraUpdateEvent(Dictionary<string, object> gameDic, GameEvent gameEvent)
    {
        string? reason = GetNullableString(gameDic, "m_reason");
        int? distance = GetNullableInt(gameDic, "m_distance");
        int? yaw = GetNullableInt(gameDic, "m_yaw");
        int? pitch = GetNullableInt(gameDic, "m_pitch");
        bool follow = GetBool(gameDic, "m_follow");
        (long? targetX, long? targetY) = GetSCameraUpdateEventTarget(gameDic);
        return new SCameraUpdateEvent(gameEvent, reason, distance, targetX, targetY, yaw, pitch, follow);
    }

    private static (long?, long?) GetSCameraUpdateEventTarget(Dictionary<string, object> pydic)
    {
        if (pydic.TryGetValue("m_target", out object? target))
        {
            if (target != null)
            {
                if (target is Dictionary<string, object> targetDic)
                {
                    long x = GetBigInt(targetDic, "x");
                    long y = GetBigInt(targetDic, "y");
                    return (x, y);
                }
            }
        }
        return (null, null);
    }
}

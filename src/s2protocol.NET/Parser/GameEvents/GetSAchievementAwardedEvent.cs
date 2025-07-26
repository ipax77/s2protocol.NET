using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal static partial class Parse
{
    private static SAchievementAwardedEvent GetSAchievementAwardedEvent(Dictionary<string, object> pydic, GameEvent gameEvent)
    {
        int m_achievementLink = GetInt(pydic, "m_achievementLink");
        return new SAchievementAwardedEvent(gameEvent, m_achievementLink);
    }
}

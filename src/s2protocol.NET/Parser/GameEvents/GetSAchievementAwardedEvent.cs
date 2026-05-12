using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;

internal static partial class Parse
{
    private static SAchievementAwardedEvent GetSAchievementAwardedEvent(Dictionary<string, object> pydic, GameEventHeader gameEvent)
    {
        int m_achievementLink = GetInt(pydic, "m_achievementLink");
        return new SAchievementAwardedEvent(gameEvent.UserId, gameEvent.EventId, gameEvent.Bits, gameEvent.Gameloop, m_achievementLink);
    }
}

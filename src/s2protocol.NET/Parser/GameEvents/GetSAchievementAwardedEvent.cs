using IronPython.Runtime;
using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal partial class Parse
{
    private static SAchievementAwardedEvent GetSAchievementAwardedEvent(PythonDictionary pydic, GameEvent gameEvent)
    {
        int m_achievementLink = GetInt(pydic, "m_achievementLink");
        return new SAchievementAwardedEvent(gameEvent, m_achievementLink);
    }
}

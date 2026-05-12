using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;

internal static partial class Parse
{
    private static SDecrementGameTimeRemainingEvent GetSDecrementGameTimeRemainingEvent(Dictionary<string, object> pydic, GameEventHeader gameEvent)
    {
        int decerementSeconds = GetInt(pydic, "m_decerementSeconds");
        return new SDecrementGameTimeRemainingEvent(gameEvent.UserId, gameEvent.EventId, gameEvent.Bits, gameEvent.Gameloop, decerementSeconds);
    }
}

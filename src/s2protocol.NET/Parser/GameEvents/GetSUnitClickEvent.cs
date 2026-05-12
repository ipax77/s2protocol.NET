using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;

internal static partial class Parse
{
    private static SUnitClickEvent GetSUnitClickEvent(Dictionary<string, object> pydic, GameEventHeader gameEvent)
    {
        int unitTag = GetInt(pydic, "m_unitTag");
        return new SUnitClickEvent(gameEvent.UserId, gameEvent.EventId, gameEvent.Bits, gameEvent.Gameloop, unitTag);
    }
}

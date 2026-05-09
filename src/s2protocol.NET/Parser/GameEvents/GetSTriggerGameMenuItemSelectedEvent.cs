using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal static partial class Parse
{
    private static STriggerGameMenuItemSelectedEvent GetSTriggerGameMenuItemSelectedEvent(Dictionary<string, object> pydic, GameEventHeader gameEvent)
    {
        long m_gameMenuItemIndex = GetBigInt(pydic, "m_gameMenuItemIndex");
        return new STriggerGameMenuItemSelectedEvent(gameEvent.UserId, gameEvent.EventId, gameEvent.Bits, gameEvent.Gameloop, m_gameMenuItemIndex);
    }
}

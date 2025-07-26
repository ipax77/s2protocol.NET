using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal static partial class Parse
{
    private static STriggerGameMenuItemSelectedEvent GetSTriggerGameMenuItemSelectedEvent(Dictionary<string, object> pydic, GameEvent gameEvent)
    {
        long m_gameMenuItemIndex = GetBigInt(pydic, "m_gameMenuItemIndex");
        return new STriggerGameMenuItemSelectedEvent(gameEvent, m_gameMenuItemIndex);
    }
}

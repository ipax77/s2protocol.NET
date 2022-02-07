using IronPython.Runtime;
using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal partial class Parse
{
    private static STriggerGameMenuItemSelectedEvent GetSTriggerGameMenuItemSelectedEvent(PythonDictionary pydic, GameEvent gameEvent)
    {
        long m_gameMenuItemIndex = GetBigInt(pydic, "m_gameMenuItemIndex");
        return new STriggerGameMenuItemSelectedEvent(gameEvent, m_gameMenuItemIndex);
    }
}

using IronPython.Runtime;
using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal partial class Parse
{
    private static STriggerGameMenuItemSelectedEvent GetSTriggerGameMenuItemSelectedEvent(PythonDictionary pydic, GameEvent gameEvent)
    {
        int m_gameMenuItemIndex = GetInt(pydic, "m_gameMenuItemIndex");
        return new STriggerGameMenuItemSelectedEvent(gameEvent, m_gameMenuItemIndex);
    }
}

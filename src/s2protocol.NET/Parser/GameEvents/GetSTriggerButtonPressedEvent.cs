using IronPython.Runtime;
using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal static partial class Parse
{
    private static STriggerButtonPressedEvent GetSTriggerButtonPressedEvent(PythonDictionary pydic, GameEvent gameEvent)
    {
        int button = GetInt(pydic, "m_button");
        return new STriggerButtonPressedEvent(gameEvent, button);
    }
}

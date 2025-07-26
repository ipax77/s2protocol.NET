using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal static partial class Parse
{
    private static STriggerButtonPressedEvent GetSTriggerButtonPressedEvent(Dictionary<string, object> pydic, GameEvent gameEvent)
    {
        int button = GetInt(pydic, "m_button");
        return new STriggerButtonPressedEvent(gameEvent, button);
    }
}

using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal static partial class Parse
{
    private static STriggerKeyPressedEvent GetSTriggerKeyPressedEvent(Dictionary<string, object> pydic, GameEvent gameEvent)
    {
        int flags = GetInt(pydic, "m_flags");
        int key = GetInt(pydic, "m_key");
        return new STriggerKeyPressedEvent(gameEvent, flags, key);
    }
}

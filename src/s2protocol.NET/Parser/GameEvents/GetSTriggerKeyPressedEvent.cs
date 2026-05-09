using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal static partial class Parse
{
    private static STriggerKeyPressedEvent GetSTriggerKeyPressedEvent(Dictionary<string, object> pydic, GameEventHeader gameEvent)
    {
        int flags = GetInt(pydic, "m_flags");
        int key = GetInt(pydic, "m_key");
        return new STriggerKeyPressedEvent(gameEvent.UserId, gameEvent.EventId, gameEvent.Bits, gameEvent.Gameloop, flags, key);
    }
}

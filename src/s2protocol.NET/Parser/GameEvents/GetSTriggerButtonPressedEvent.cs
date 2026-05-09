using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal static partial class Parse
{
    private static STriggerButtonPressedEvent GetSTriggerButtonPressedEvent(Dictionary<string, object> pydic, GameEventHeader gameEvent)
    {
        int button = GetInt(pydic, "m_button");
        return new STriggerButtonPressedEvent(gameEvent.UserId, gameEvent.EventId, gameEvent.Bits, gameEvent.Gameloop, button);
    }
}

using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;

internal static partial class Parse
{
    private static STriggerChatMessageEvent GetSTriggerChatMessageEvent(Dictionary<string, object> pydic, GameEventHeader gameEvent)
    {
        string chatMessage = GetString(pydic, "m_chatMessage");
        return new STriggerChatMessageEvent(gameEvent.UserId, gameEvent.EventId, gameEvent.Bits, gameEvent.Gameloop, chatMessage);
    }
}

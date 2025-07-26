using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal static partial class Parse
{
    private static STriggerChatMessageEvent GetSTriggerChatMessageEvent(Dictionary<string, object> pydic, GameEvent gameEvent)
    {
        string chatMessage = GetString(pydic, "m_chatMessage");
        return new STriggerChatMessageEvent(gameEvent, chatMessage);
    }
}

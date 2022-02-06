using IronPython.Runtime;
using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal partial class Parse
{
    private static STriggerChatMessageEvent GetSTriggerChatMessageEvent(PythonDictionary pydic, GameEvent gameEvent)
    {
        string chatMessage = GetString(pydic, "m_chatMessage");
        return new STriggerChatMessageEvent(gameEvent, chatMessage);
    }
}

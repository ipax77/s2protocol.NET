using IronPython.Runtime;
using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal partial class Parse
{
    public static List<ChatMessageEvent> Messages(dynamic generator)
    {
        List<ChatMessageEvent> messages = new();
        foreach (PythonDictionary pydic in generator)
        {
            var _event = GetString(pydic, "_event");
            if (_event == "NNet.Game.SChatMessage")
            {
                var recipient = GetInt(pydic, "m_recipient");
                var id = GetChatMessageId(pydic);
                var msg = GetString(pydic, "m_string");
                var loop = GetInt(pydic, "_gameloop");
                messages.Add(new ChatMessageEvent(recipient, id, msg, loop));
            }
        }
        return messages;
    }

    private static int GetChatMessageId(PythonDictionary pydic)
    {
        if (pydic.ContainsKey("_userid"))
        {
            if (pydic["_userid"] is PythonDictionary usrdic)
            {
                return GetInt(usrdic, "m_userId");
            }
        }
        return 0;
    }
}

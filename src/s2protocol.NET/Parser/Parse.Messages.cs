using IronPython.Runtime;
using s2protocol.NET.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s2protocol.NET.Parser;
internal partial class Parse
{
    public static List<ChatMessageEvent> ParseMessages(dynamic generator)
    {
        List<ChatMessageEvent> messages = new List<ChatMessageEvent>();
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
            PythonDictionary? usrdic = pydic["_userid"] as PythonDictionary;
            if (usrdic != null)
            {
                return GetInt(usrdic, "m_userId");
            }
        }
        return 0;
    }
}

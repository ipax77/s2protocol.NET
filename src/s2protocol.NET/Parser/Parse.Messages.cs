using IronPython.Runtime;
using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal partial class Parse
{
    public static void SetMessages(dynamic generator, Sc2Replay replay)
    {
        List<ChatMessageEvent> messages = [];
        List<PingMessageEvent> pings = [];

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
            else if (_event == "NNet.Game.SPingMessage")
            {
                var recipient = GetInt(pydic, "m_recipient");
                var id = GetChatMessageId(pydic);
                var loop = GetInt(pydic, "_gameloop");
                (var x, var y) = GetXYCoords(pydic);
                pings.Add(new(recipient, id, x, y, loop));
            }
        }
        replay.ChatMessages = messages;
        replay.PingMessages = pings;
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

    private static (int, int) GetXYCoords(PythonDictionary pydic)
    {
        if (pydic.ContainsKey("m_point"))
        {
            if (pydic["m_point"] is PythonDictionary coorddic)
            {
                var x = GetInt(coorddic, "x");
                var y = GetInt(coorddic, "y");
                return (x, y);
            }
        }
        return (0, 0);
    }
}

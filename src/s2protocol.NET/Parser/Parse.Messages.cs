using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal static partial class Parse
{
    public static void SetMessages(object generator, Sc2Replay replay)
    {
        if (generator is not List<object> genDict)
        {
            throw new ArgumentException("Generator must be a Dictionary<string, object>.", nameof(generator));
        }

        List<ChatMessageEvent> messages = [];
        List<PingMessageEvent> pings = [];

        foreach (var ent in genDict)
        {
            if (ent is not Dictionary<string, object> pydic)
            {
                continue;
            }

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
                pings.Add(new(recipient, id, loop, x, y));
            }
        }
        replay.ChatMessages = messages;
        replay.PingMessages = pings;
    }

    private static int GetChatMessageId(Dictionary<string, object> pydic)
    {
        if (pydic.ContainsKey("_userid"))
        {
            if (pydic["_userid"] is Dictionary<string, object> usrdic)
            {
                return GetInt(usrdic, "m_userId");
            }
        }
        return 0;
    }

    private static (long, long) GetXYCoords(Dictionary<string, object> pydic)
    {
        if (pydic.ContainsKey("m_point"))
        {
            if (pydic["m_point"] is Dictionary<string, object> coorddic)
            {
                var x = GetBigInt(coorddic, "x");
                var y = GetBigInt(coorddic, "y");
                return (x, y);
            }
        }
        return (0, 0);
    }
}

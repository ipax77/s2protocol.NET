using IronPython.Runtime;
using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal partial class Parse
{
    private static SDecrementGameTimeRemainingEvent GetSDecrementGameTimeRemainingEvent(PythonDictionary pydic, GameEvent gameEvent)
    {
        int decerementSeconds = GetInt(pydic, "m_decerementSeconds");
        return new SDecrementGameTimeRemainingEvent(gameEvent, decerementSeconds);
    }
}

using IronPython.Runtime;
using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal static partial class Parse
{
    private static STriggerSoundtrackDoneEvent GetSTriggerSoundtrackDoneEvent(PythonDictionary pydic, GameEvent gameEvent)
    {
        int soundtrack = GetInt(pydic, "m_soundtrack");
        return new STriggerSoundtrackDoneEvent(gameEvent, soundtrack);
    }
}

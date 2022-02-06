using IronPython.Runtime;
using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal partial class Parse
{
    private static STriggerSoundOffsetEvent GetSTriggerSoundOffsetEvent(PythonDictionary pydic, GameEvent gameEvent)
    {
        int m_sound = GetInt(pydic, "m_sound");
        return new STriggerSoundOffsetEvent(gameEvent, m_sound);
    }
}

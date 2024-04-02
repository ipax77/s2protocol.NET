using IronPython.Runtime;
using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal static partial class Parse
{
    private static STriggerSoundLengthQueryEvent GetSTriggerSoundLengthQueryEvent(PythonDictionary pydic, GameEvent gameEvent)
    {
        long m_soundHash = GetBigInt(pydic, "m_soundHash");
        int m_length = GetInt(pydic, "m_length");
        return new STriggerSoundLengthQueryEvent(gameEvent, m_soundHash, m_length);
    }
}

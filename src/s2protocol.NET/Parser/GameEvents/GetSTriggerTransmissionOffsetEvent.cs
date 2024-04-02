using IronPython.Runtime;
using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal static partial class Parse
{
    private static STriggerTransmissionOffsetEvent GetSTriggerTransmissionOffsetEvent(PythonDictionary pydic, GameEvent gameEvent)
    {
        int m_achievementLink = GetInt(pydic, "m_achievementLink");
        return new STriggerTransmissionOffsetEvent(gameEvent, m_achievementLink);
    }
}

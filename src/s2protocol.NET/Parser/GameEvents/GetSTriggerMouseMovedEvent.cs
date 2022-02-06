using IronPython.Runtime;
using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal partial class Parse
{
    private static STriggerMouseMovedEvent GetSTriggerMouseMovedEvent(PythonDictionary pydic, GameEvent gameEvent)
    {
        int m_flags = GetInt(pydic, "m_flags");
        (long x, long y) = GetSTriggerMouseMovedEventPos(pydic);
        return new STriggerMouseMovedEvent(gameEvent, m_flags, x, y);
    }

    private static (long x, long y) GetSTriggerMouseMovedEventPos(PythonDictionary pydic)
    {
        if (pydic.TryGetValue("m_posWorld", out object? pos))
        {
            if (pos != null)
            {
                if (pydic["m_posWorld"] is PythonDictionary posDic)
                {
                    return (GetBigInt(posDic, "x"), GetBigInt(posDic, "y"));
                }
            }
        }
        return (0, 0);
    }
}

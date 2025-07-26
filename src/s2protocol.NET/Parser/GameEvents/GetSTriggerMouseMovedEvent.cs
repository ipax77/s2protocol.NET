using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal static partial class Parse
{
    private static STriggerMouseMovedEvent GetSTriggerMouseMovedEvent(Dictionary<string, object> pydic, GameEvent gameEvent)
    {
        int m_flags = GetInt(pydic, "m_flags");
        (long x, long y) = GetSTriggerMouseMovedEventPos(pydic);
        return new STriggerMouseMovedEvent(gameEvent, m_flags, x, y);
    }

    private static (long x, long y) GetSTriggerMouseMovedEventPos(Dictionary<string, object> pydic)
    {
        if (pydic.TryGetValue("m_posWorld", out object? pos))
        {
            if (pos != null)
            {
                if (pydic["m_posWorld"] is Dictionary<string, object> posDic)
                {
                    return (GetBigInt(posDic, "x"), GetBigInt(posDic, "y"));
                }
            }
        }
        return (0, 0);
    }
}

using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal static partial class Parse
{
    private static STriggerMouseClickedEvent GetSTriggerMouseClickedEvent(Dictionary<string, object> pydic, GameEvent gameEvent)
    {
        bool down = GetBool(pydic, "m_down");
        int button = GetInt(pydic, "m_button");
        int flags = GetInt(pydic, "m_flags");
        (long posX, long posY) = GetPosUI(pydic);
        return new STriggerMouseClickedEvent(gameEvent, down, button, flags, posX, posY);
    }

    private static (long posX, long posY) GetPosUI(Dictionary<string, object> pydic)
    {
        if (pydic.TryGetValue("m_posUI", out object? pos))
        {
            if (pos is Dictionary<string, object> posDic)
            {
                return (GetBigInt(posDic, "x"), GetBigInt(posDic, "y"));
            }
        }
        return (0, 0);
    }
}

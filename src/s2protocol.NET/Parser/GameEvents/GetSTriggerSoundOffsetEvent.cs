using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal static partial class Parse
{
    private static STriggerSoundOffsetEvent GetSTriggerSoundOffsetEvent(Dictionary<string, object> pydic, GameEventHeader gameEvent)
    {
        int m_sound = GetInt(pydic, "m_sound");
        return new STriggerSoundOffsetEvent(gameEvent.UserId, gameEvent.EventId, gameEvent.Bits, gameEvent.Gameloop, m_sound);
    }
}

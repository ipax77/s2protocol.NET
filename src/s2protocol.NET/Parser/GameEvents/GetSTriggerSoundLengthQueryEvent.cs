using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal static partial class Parse
{
    private static STriggerSoundLengthQueryEvent GetSTriggerSoundLengthQueryEvent(Dictionary<string, object> pydic, GameEventHeader gameEvent)
    {
        long m_soundHash = GetBigInt(pydic, "m_soundHash");
        int m_length = GetInt(pydic, "m_length");
        return new STriggerSoundLengthQueryEvent(gameEvent.UserId, gameEvent.EventId, gameEvent.Bits, gameEvent.Gameloop, m_soundHash, m_length);
    }
}

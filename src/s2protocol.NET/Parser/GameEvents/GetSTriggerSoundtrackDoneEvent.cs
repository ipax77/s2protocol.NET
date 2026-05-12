using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;

internal static partial class Parse
{
    private static STriggerSoundtrackDoneEvent GetSTriggerSoundtrackDoneEvent(Dictionary<string, object> pydic, GameEventHeader gameEvent)
    {
        int soundtrack = GetInt(pydic, "m_soundtrack");
        return new STriggerSoundtrackDoneEvent(gameEvent.UserId, gameEvent.EventId, gameEvent.Bits, gameEvent.Gameloop, soundtrack);
    }
}

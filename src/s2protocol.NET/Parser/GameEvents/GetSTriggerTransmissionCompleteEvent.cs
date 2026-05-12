using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;

internal static partial class Parse
{
    private static STriggerTransmissionCompleteEvent GetSTriggerTransmissionCompleteEvent(Dictionary<string, object> pydic, GameEventHeader gameEvent)
    {
        long transmissionId = GetBigInt(pydic, "m_transmissionId");
        return new STriggerTransmissionCompleteEvent(gameEvent.UserId, gameEvent.EventId, gameEvent.Bits, gameEvent.Gameloop, transmissionId);
    }
}

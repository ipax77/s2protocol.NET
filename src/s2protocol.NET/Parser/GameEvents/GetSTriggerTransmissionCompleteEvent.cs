using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal static partial class Parse
{
    private static STriggerTransmissionCompleteEvent GetSTriggerTransmissionCompleteEvent(Dictionary<string, object> pydic, GameEvent gameEvent)
    {
        long transmissionId = GetBigInt(pydic, "m_transmissionId");
        return new STriggerTransmissionCompleteEvent(gameEvent, transmissionId);
    }
}

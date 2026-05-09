namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerTransmissionCompleteEvent</c> STriggerTransmissionCompleteEvent</summary>
///
public sealed class STriggerTransmissionCompleteEvent : GameEvent
{
    /// <summary>Record <c>STriggerTransmissionCompleteEvent</c> constructor</summary>
    ///
    public STriggerTransmissionCompleteEvent(int userId,
        int eventId,
        int bits,
        int gameloop,
        long transmissionId) : base(userId, eventId, GameEventType.STriggerTransmissionCompleteEvent, bits, gameloop)
    {
        TransmissionId = transmissionId;
    }

    /// <summary>Event TransmissionId</summary>
    ///
    public long TransmissionId { get; }
}

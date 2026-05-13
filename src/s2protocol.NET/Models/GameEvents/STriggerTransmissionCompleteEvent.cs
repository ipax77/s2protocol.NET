namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerTransmissionCompleteEvent</c> STriggerTransmissionCompleteEvent</summary>
///
/// <remarks>Record <c>STriggerTransmissionCompleteEvent</c> constructor</remarks>
///
public sealed class STriggerTransmissionCompleteEvent(int userId,
    int eventId,
    int bits,
    int gameloop,
    long transmissionId) : GameEvent(userId, eventId, GameEventType.STriggerTransmissionCompleteEvent, bits, gameloop)
{

    /// <summary>Event TransmissionId</summary>
    ///
    public long TransmissionId { get; } = transmissionId;
}

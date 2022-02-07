using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerTransmissionCompleteEvent</c> STriggerTransmissionCompleteEvent</summary>
///
public record STriggerTransmissionCompleteEvent : GameEvent
{
    /// <summary>Record <c>STriggerTransmissionCompleteEvent</c> constructor</summary>
    ///
    public STriggerTransmissionCompleteEvent(GameEvent gameEvent,
                            long transmissionId) : base(gameEvent)
    {
        TransmissionId = transmissionId;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public STriggerTransmissionCompleteEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Event TransmissionId</summary>
    ///
    public long TransmissionId { get; init; }
}

using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerSoundLengthQueryEvent</c> STriggerSoundLengthQueryEvent</summary>
///
public record STriggerSoundLengthQueryEvent : GameEvent
{
    /// <summary>Record <c>STriggerSoundLengthQueryEvent</c> constructor</summary>
    ///
    public STriggerSoundLengthQueryEvent(GameEvent gameEvent,
                            long soundHash,
                            int length) : base(gameEvent)
    {
        SoundHash = soundHash;
        Length = length;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public STriggerSoundLengthQueryEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Event SoundHash</summary>
    ///
    public long SoundHash { get; init; }
    /// <summary>Event Length</summary>
    ///
    public int Length { get; init; }
}
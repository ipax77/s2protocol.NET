using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerSoundLengthSyncEvent</c> STriggerSoundLengthSyncEvent</summary>
///
public record STriggerSoundLengthSyncEvent : GameEvent
{
    /// <summary>Record <c>STriggerSoundLengthSyncEvent</c> constructor</summary>
    ///
    public STriggerSoundLengthSyncEvent(GameEvent gameEvent) : base(gameEvent)
    {
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public STriggerSoundLengthSyncEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }
}
using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerSoundOffsetEvent</c> STriggerSoundOffsetEvent</summary>
///
public record STriggerSoundOffsetEvent : GameEvent
{
    /// <summary>Record <c>STriggerSoundOffsetEvent</c> constructor</summary>
    ///
    public STriggerSoundOffsetEvent(GameEvent gameEvent,
                            int sound) : base(gameEvent)
    {
        Sound = sound;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public STriggerSoundOffsetEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Event Sound</summary>
    ///
    public int Sound { get; init; }
}
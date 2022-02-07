using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerSoundtrackDoneEvent</c> STriggerSoundtrackDoneEvent</summary>
///
public record STriggerSoundtrackDoneEvent : GameEvent
{
    /// <summary>Record <c>STriggerSoundtrackDoneEvent</c> constructor</summary>
    ///
    public STriggerSoundtrackDoneEvent(GameEvent gameEvent,
                                     int soundtrack) : base(gameEvent)
    {
        Soundtrack = soundtrack;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public STriggerSoundtrackDoneEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Event Down</summary>
    ///
    public int Soundtrack { get; init; }
}
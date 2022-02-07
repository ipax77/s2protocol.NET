using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerCutsceneEndSceneFiredEvent</c> STriggerCutsceneEndSceneFiredEvent</summary>
///
public record STriggerCutsceneEndSceneFiredEvent : GameEvent
{
    /// <summary>Record <c>STriggerCutsceneEndSceneFiredEvent</c> constructor</summary>
    ///
    public STriggerCutsceneEndSceneFiredEvent(GameEvent gameEvent,
                            long cutsceneId) : base(gameEvent)
    {
        CutsceneId = cutsceneId;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public STriggerCutsceneEndSceneFiredEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Event CutsceneId</summary>
    ///
    public long CutsceneId { get; init; }
}
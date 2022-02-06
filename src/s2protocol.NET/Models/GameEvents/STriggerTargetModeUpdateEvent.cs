using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerTargetModeUpdateEvent</c> STriggerTargetModeUpdateEvent</summary>
///
public record STriggerTargetModeUpdateEvent : GameEvent
{
    /// <summary>Record <c>STriggerTargetModeUpdateEvent</c> constructor</summary>
    ///
    public STriggerTargetModeUpdateEvent(GameEvent gameEvent,
                            int abilCmdIndex,
                            int abilLink,
                            int state) : base(gameEvent)
    {
        AbilCmdIndex = abilCmdIndex;
        AbilLink = abilLink;
        State = state;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public STriggerTargetModeUpdateEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Event AbilCmdIndex</summary>
    ///
    public int AbilCmdIndex { get; init; }
    /// <summary>Event AbilLink</summary>
    ///
    public int AbilLink { get; init; }
    /// <summary>Event State</summary>
    ///
    public int State { get; init; }
}
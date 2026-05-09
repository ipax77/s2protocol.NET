namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerTargetModeUpdateEvent</c> STriggerTargetModeUpdateEvent</summary>
///
public sealed class STriggerTargetModeUpdateEvent : GameEvent
{
    /// <summary>Record <c>STriggerTargetModeUpdateEvent</c> constructor</summary>
    ///
    public STriggerTargetModeUpdateEvent(int userId,
        int eventId,
        int bits,
        int gameloop,
        int abilCmdIndex,
                            int abilLink,
                            int state) : base(userId, eventId, GameEventType.STriggerTargetModeUpdateEvent, bits, gameloop)
    {
        AbilCmdIndex = abilCmdIndex;
        AbilLink = abilLink;
        State = state;
    }

    /// <summary>Event AbilCmdIndex</summary>
    ///
    public int AbilCmdIndex { get; }
    /// <summary>Event AbilLink</summary>
    ///
    public int AbilLink { get; }
    /// <summary>Event State</summary>
    ///
    public int State { get; }
}
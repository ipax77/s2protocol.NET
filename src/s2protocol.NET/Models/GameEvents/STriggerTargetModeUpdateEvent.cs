namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerTargetModeUpdateEvent</c> STriggerTargetModeUpdateEvent</summary>
///
/// <remarks>Record <c>STriggerTargetModeUpdateEvent</c> constructor</remarks>
///
public sealed class STriggerTargetModeUpdateEvent(int userId,
    int eventId,
    int bits,
    int gameloop,
    int abilCmdIndex,
                        int abilLink,
                        int state) : GameEvent(userId, eventId, GameEventType.STriggerTargetModeUpdateEvent, bits, gameloop)
{

    /// <summary>Event AbilCmdIndex</summary>
    ///
    public int AbilCmdIndex { get; } = abilCmdIndex;
    /// <summary>Event AbilLink</summary>
    ///
    public int AbilLink { get; } = abilLink;
    /// <summary>Event State</summary>
    ///
    public int State { get; } = state;
}
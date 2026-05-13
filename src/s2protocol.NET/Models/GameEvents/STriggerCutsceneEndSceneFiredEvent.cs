namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerCutsceneEndSceneFiredEvent</c> STriggerCutsceneEndSceneFiredEvent</summary>
///
/// <remarks>Record <c>STriggerCutsceneEndSceneFiredEvent</c> constructor</remarks>
///
public sealed class STriggerCutsceneEndSceneFiredEvent(int userId,
    int eventId,
    int bits,
    int gameloop,
    long cutsceneId) : GameEvent(userId, eventId, GameEventType.STriggerCutsceneEndSceneFiredEvent, bits, gameloop)
{

    /// <summary>Event CutsceneId</summary>
    ///
    public long CutsceneId { get; } = cutsceneId;
}
namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerCutsceneEndSceneFiredEvent</c> STriggerCutsceneEndSceneFiredEvent</summary>
///
public sealed class STriggerCutsceneEndSceneFiredEvent : GameEvent
{
    /// <summary>Record <c>STriggerCutsceneEndSceneFiredEvent</c> constructor</summary>
    ///
    public STriggerCutsceneEndSceneFiredEvent(int userId,
        int eventId,
        int bits,
        int gameloop,
        long cutsceneId) : base(userId, eventId, GameEventType.STriggerCutsceneEndSceneFiredEvent, bits, gameloop)
    {
        CutsceneId = cutsceneId;
    }

    /// <summary>Event CutsceneId</summary>
    ///
    public long CutsceneId { get; }
}
namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerCutsceneBookmarkFiredEvent</c> STriggerCutsceneBookmarkFiredEvent</summary>
///
/// <remarks>Record <c>STriggerCutsceneBookmarkFiredEvent</c> constructor</remarks>
///
public sealed class STriggerCutsceneBookmarkFiredEvent(int userId,
    int eventId,
    int bits,
    int gameloop,
    long cutsceneId,
                        string bookmarkName) : GameEvent(userId, eventId, GameEventType.STriggerCutsceneBookmarkFiredEvent, bits, gameloop)
{

    /// <summary>Event CutsceneId</summary>
    ///
    public long CutsceneId { get; } = cutsceneId;
    /// <summary>Event BookmarkName</summary>
    ///
    public string BookmarkName { get; } = bookmarkName;
}
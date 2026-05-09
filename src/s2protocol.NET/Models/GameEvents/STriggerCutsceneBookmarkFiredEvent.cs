namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerCutsceneBookmarkFiredEvent</c> STriggerCutsceneBookmarkFiredEvent</summary>
///
public sealed class STriggerCutsceneBookmarkFiredEvent : GameEvent
{
    /// <summary>Record <c>STriggerCutsceneBookmarkFiredEvent</c> constructor</summary>
    ///
    public STriggerCutsceneBookmarkFiredEvent(int userId,
        int eventId,
        int bits,
        int gameloop,
        long cutsceneId,
                            string bookmarkName) : base(userId, eventId, GameEventType.STriggerCutsceneBookmarkFiredEvent, bits, gameloop)
    {
        CutsceneId = cutsceneId;
        BookmarkName = bookmarkName;
    }

    /// <summary>Event CutsceneId</summary>
    ///
    public long CutsceneId { get; }
    /// <summary>Event BookmarkName</summary>
    ///
    public string BookmarkName { get; }
}
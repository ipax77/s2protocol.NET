using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerCutsceneBookmarkFiredEvent</c> STriggerCutsceneBookmarkFiredEvent</summary>
///
public record STriggerCutsceneBookmarkFiredEvent : GameEvent
{
    /// <summary>Record <c>STriggerCutsceneBookmarkFiredEvent</c> constructor</summary>
    ///
    public STriggerCutsceneBookmarkFiredEvent(GameEvent gameEvent,
                            long cutsceneId,
                            string bookmarkName) : base(gameEvent)
    {
        CutsceneId = cutsceneId;
        BookmarkName = bookmarkName;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public STriggerCutsceneBookmarkFiredEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Event CutsceneId</summary>
    ///
    public long CutsceneId { get; init; }
    /// <summary>Event BookmarkName</summary>
    ///
    public string BookmarkName { get; init; }
}
namespace s2protocol.NET.Models;
/// <summary>Record <c>Event</c> Event baseclass</summary>
///
/// <remarks>Record <c>GameEvent</c> base constructor</remarks>
///
public abstract class GameEvent(int userId, int eventId, GameEventType eventType, int bits, int gameloop)
{

    /// <summary>Event PlayerId</summary>
    ///
    public int UserId { get; } = userId;

    /// <summary>Event EventId</summary>
    ///
    public int EventId { get; } = eventId;
    /// <summary>Event EventType</summary>
    ///
    public GameEventType EventType { get; } = eventType;
    /// <summary>Event Bits</summary>
    ///
    public int Bits { get; } = bits;
    /// <summary>Event Gameloop</summary>
    ///
    public int Gameloop { get; } = gameloop;
}


/// <summary>Enum <c>EventType</c> Event type</summary>
///
public enum GameEventType
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    None = 0,
    SBankFileEvent = 1,
    SBankKeyEvent = 2,
    SBankSectionEvent = 3,
    SBankSignatureEvent = 4,
    SBankValueEvent = 5,
    SCameraUpdateEvent = 6,
    SCmdEvent = 7,
    SCmdUpdateTargetPointEvent = 8,
    SCommandManagerStateEvent = 9,
    SControlGroupUpdateEvent = 10,
    SGameUserLeaveEvent = 11,
    SSelectionDeltaEvent = 12,
    SSetSyncLoadingTimeEvent = 13,
    SSetSyncPlayingTimeEvent = 14,
    STriggerDialogControlEvent = 15,
    STriggerPingEvent = 16,
    STriggerSoundLengthSyncEvent = 17,
    SUserFinishedLoadingSyncEvent = 18,
    SUserOptionsEvent = 19,
    SCmdUpdateTargetUnitEvents = 20,
    STriggerKeyPressedEvent = 21,
    SUnitClickEvent = 22,
    SDecrementGameTimeRemainingEvent = 23,
    STriggerChatMessageEvent = 24,
    STriggerMouseClickedEvent = 25,
    STriggerSoundtrackDoneEvent = 26,
    SCameraSaveEvent = 27,
    STriggerCutsceneBookmarkFiredEvent = 28,
    STriggerCutsceneEndSceneFiredEvent = 29,
    STriggerSoundLengthQueryEvent = 30,
    STriggerSoundOffsetEvent = 31,
    STriggerTargetModeUpdateEvent = 32,
    STriggerTransmissionCompleteEvent = 33,
    SAchievementAwardedEvent = 34,
    STriggerTransmissionOffsetEvent = 35,
    STriggerButtonPressedEvent = 36,
    STriggerGameMenuItemSelectedEvent = 37,
    STriggerMouseMovedEvent = 38,

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}

namespace s2protocol.NET.Models;
/// <summary>Record <c>Event</c> Event baseclass</summary>
///
public abstract class GameEvent
{
    /// <summary>Record <c>GameEvent</c> base constructor</summary>
    ///
    protected GameEvent(int userId, int eventId, GameEventType eventType, int bits, int gameloop)
    {
        UserId = userId;
        EventId = eventId;
        EventType = eventType;
        Bits = bits;
        Gameloop = gameloop;
    }

    /// <summary>Event PlayerId</summary>
    ///
    public int UserId { get; }

    /// <summary>Event EventId</summary>
    ///
    public int EventId { get; }
    /// <summary>Event EventType</summary>
    ///
    public GameEventType EventType { get; }
    /// <summary>Event Bits</summary>
    ///
    public int Bits { get; }
    /// <summary>Event Gameloop</summary>
    ///
    public int Gameloop { get; }
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
    SHijackReplayGameEvent = 39,
    SGameUserJoinEvent = 40,
    SSaveGameEvent = 41,
    SSaveGameDoneEvent = 42,
    SLoadGameDoneEvent = 43,
    SCommandManagerResetEvent = 44,
    SGameCheatEvent = 45,
    SSelectionSyncCheckEvent = 46,
    SResourceTradeEvent = 47,
    SAICommunicateEvent = 48,
    SSetAbsoluteGameSpeedEvent = 49,
    SAddAbsoluteGameSpeedEvent = 50,
    SBroadcastCheatEvent = 51,
    SAllianceEvent = 52,
    SUnitHighlightEvent = 53,
    STriggerReplySelectedEvent = 54,
    STriggerSkippedEvent = 55,
    STriggerAbortMissionEvent = 56,
    STriggerPurchaseMadeEvent = 57,
    STriggerPurchaseExitEvent = 58,
    STriggerPlanetMissionLaunchedEvent = 59,
    STriggerPlanetPanelCanceledEvent = 60,
    STriggerConversationSkippedEvent = 61,
    STriggerHotkeyPressedEvent = 62,
    STriggerPlanetPanelReplayEvent = 63,
    STriggerPlanetMissionSelectedEvent = 64,
    STriggerMovieFunctionEvent = 65,
    STriggerPlanetPanelBirthCompleteEvent = 66,
    STriggerPlanetPanelDeathCompleteEvent = 67,
    SResourceRequestEvent = 68,
    SResourceRequestFulfillEvent = 69,
    SResourceRequestCancelEvent = 70,
    STriggerResearchPanelExitEvent = 71,
    STriggerResearchPanelPurchaseEvent = 72,
    STriggerResearchPanelSelectionChangedEvent = 73,
    STriggerCommandErrorEvent = 74,
    STriggerMercenaryPanelExitEvent = 75,
    STriggerMercenaryPanelPurchaseEvent = 76,
    STriggerMercenaryPanelSelectionChangedEvent = 77,
    STriggerVictoryPanelExitEvent = 78,
    STriggerBattleReportPanelExitEvent = 79,
    STriggerBattleReportPanelPlayMissionEvent = 80,
    STriggerBattleReportPanelPlaySceneEvent = 81,
    STriggerBattleReportPanelSelectionChangedEvent = 82,
    STriggerVictoryPanelPlayMissionAgainEvent = 83,
    STriggerMovieStartedEvent = 84,
    STriggerMovieFinishedEvent = 85,
    STriggerPortraitLoadedEvent = 86,
    STriggerCustomDialogDismissedEvent = 87,
    STriggerMouseWheelEvent = 88,
    STriggerPurchasePanelSelectedPurchaseItemChangedEvent = 89,
    STriggerPurchasePanelSelectedPurchaseCategoryChangedEvent = 90,
    STriggerGameCreditsFinishedEvent = 91,
    STriggerCutsceneConversationLineEvent = 92,
    STriggerCutsceneConversationLineMissingEvent = 93,
    STriggerAnimLengthQueryByNameEvent = 94,
    STriggerAnimLengthQueryByPropsEvent = 95,
    STriggerAnimOffsetEvent = 96,
    SCatalogModifyEvent = 97,
    SHeroTalentTreeSelectedEvent = 98,
    STriggerProfilerLoggingFinishedEvent = 99,
    SHeroTalentTreeSelectionPanelToggledEvent = 100,
    SPeerSetSyncLoadingTimeEvent = 101,
    SPeerSetSyncPlayingTimeEvent = 102,
    SLagMessageEvent = 103,
    SPlayerLeaveEvent = 104,
    STriggerCameraMoveEvent = 105,

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}

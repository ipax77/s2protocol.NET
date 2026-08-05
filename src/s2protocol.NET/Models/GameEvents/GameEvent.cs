using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>Event</c> Event baseclass</summary>
///
[JsonDerivedType(typeof(SBankFileEvent), nameof(SBankFileEvent))]
[JsonDerivedType(typeof(SBankKeyEvent), nameof(SBankKeyEvent))]
[JsonDerivedType(typeof(SBankSectionEvent), nameof(SBankSectionEvent))]
[JsonDerivedType(typeof(SBankSignatureEvent), nameof(SBankSignatureEvent))]
[JsonDerivedType(typeof(SBankValueEvent), nameof(SBankValueEvent))]
[JsonDerivedType(typeof(SCameraUpdateEvent), nameof(SCameraUpdateEvent))]
[JsonDerivedType(typeof(SCmdEvent), nameof(SCmdEvent))]
[JsonDerivedType(typeof(SCmdUpdateTargetPointEvent), nameof(SCmdUpdateTargetPointEvent))]
[JsonDerivedType(typeof(SCommandManagerStateEvent), nameof(SCommandManagerStateEvent))]
[JsonDerivedType(typeof(SControlGroupUpdateEvent), nameof(SControlGroupUpdateEvent))]
[JsonDerivedType(typeof(SGameUserLeaveEvent), nameof(SGameUserLeaveEvent))]
[JsonDerivedType(typeof(SSelectionDeltaEvent), nameof(SSelectionDeltaEvent))]
[JsonDerivedType(typeof(SSetSyncLoadingTimeEvent), nameof(SSetSyncLoadingTimeEvent))]
[JsonDerivedType(typeof(SSetSyncPlayingTimeEvent), nameof(SSetSyncPlayingTimeEvent))]
[JsonDerivedType(typeof(STriggerDialogControlEvent), nameof(STriggerDialogControlEvent))]
[JsonDerivedType(typeof(STriggerPingEvent), nameof(STriggerPingEvent))]
[JsonDerivedType(typeof(STriggerSoundLengthSyncEvent), nameof(STriggerSoundLengthSyncEvent))]
[JsonDerivedType(typeof(SUserFinishedLoadingSyncEvent), nameof(SUserFinishedLoadingSyncEvent))]
[JsonDerivedType(typeof(SUserOptionsEvent), nameof(SUserOptionsEvent))]
[JsonDerivedType(typeof(SCmdUpdateTargetUnitEvent), nameof(SCmdUpdateTargetUnitEvent))]
[JsonDerivedType(typeof(STriggerKeyPressedEvent), nameof(STriggerKeyPressedEvent))]
[JsonDerivedType(typeof(SUnitClickEvent), nameof(SUnitClickEvent))]
[JsonDerivedType(typeof(SDecrementGameTimeRemainingEvent), nameof(SDecrementGameTimeRemainingEvent))]
[JsonDerivedType(typeof(STriggerChatMessageEvent), nameof(STriggerChatMessageEvent))]
[JsonDerivedType(typeof(STriggerMouseClickedEvent), nameof(STriggerMouseClickedEvent))]
[JsonDerivedType(typeof(STriggerSoundtrackDoneEvent), nameof(STriggerSoundtrackDoneEvent))]
[JsonDerivedType(typeof(SCameraSaveEvent), nameof(SCameraSaveEvent))]
[JsonDerivedType(typeof(STriggerCutsceneBookmarkFiredEvent), nameof(STriggerCutsceneBookmarkFiredEvent))]
[JsonDerivedType(typeof(STriggerCutsceneEndSceneFiredEvent), nameof(STriggerCutsceneEndSceneFiredEvent))]
[JsonDerivedType(typeof(STriggerSoundLengthQueryEvent), nameof(STriggerSoundLengthQueryEvent))]
[JsonDerivedType(typeof(STriggerSoundOffsetEvent), nameof(STriggerSoundOffsetEvent))]
[JsonDerivedType(typeof(STriggerTargetModeUpdateEvent), nameof(STriggerTargetModeUpdateEvent))]
[JsonDerivedType(typeof(STriggerTransmissionCompleteEvent), nameof(STriggerTransmissionCompleteEvent))]
[JsonDerivedType(typeof(SAchievementAwardedEvent), nameof(SAchievementAwardedEvent))]
[JsonDerivedType(typeof(STriggerTransmissionOffsetEvent), nameof(STriggerTransmissionOffsetEvent))]
[JsonDerivedType(typeof(STriggerButtonPressedEvent), nameof(STriggerButtonPressedEvent))]
[JsonDerivedType(typeof(STriggerGameMenuItemSelectedEvent), nameof(STriggerGameMenuItemSelectedEvent))]
[JsonDerivedType(typeof(STriggerMouseMovedEvent), nameof(STriggerMouseMovedEvent))]
[JsonDerivedType(typeof(SHijackReplayGameEvent), nameof(SHijackReplayGameEvent))]
[JsonDerivedType(typeof(SGameUserJoinEvent), nameof(SGameUserJoinEvent))]
[JsonDerivedType(typeof(SSaveGameEvent), nameof(SSaveGameEvent))]
[JsonDerivedType(typeof(SSaveGameDoneEvent), nameof(SSaveGameDoneEvent))]
[JsonDerivedType(typeof(SLoadGameDoneEvent), nameof(SLoadGameDoneEvent))]
[JsonDerivedType(typeof(SCommandManagerResetEvent), nameof(SCommandManagerResetEvent))]
[JsonDerivedType(typeof(SGameCheatEvent), nameof(SGameCheatEvent))]
[JsonDerivedType(typeof(SSelectionSyncCheckEvent), nameof(SSelectionSyncCheckEvent))]
[JsonDerivedType(typeof(SResourceTradeEvent), nameof(SResourceTradeEvent))]
[JsonDerivedType(typeof(SAICommunicateEvent), nameof(SAICommunicateEvent))]
[JsonDerivedType(typeof(SSetAbsoluteGameSpeedEvent), nameof(SSetAbsoluteGameSpeedEvent))]
[JsonDerivedType(typeof(SAddAbsoluteGameSpeedEvent), nameof(SAddAbsoluteGameSpeedEvent))]
[JsonDerivedType(typeof(SBroadcastCheatEvent), nameof(SBroadcastCheatEvent))]
[JsonDerivedType(typeof(SAllianceEvent), nameof(SAllianceEvent))]
[JsonDerivedType(typeof(SUnitHighlightEvent), nameof(SUnitHighlightEvent))]
[JsonDerivedType(typeof(STriggerReplySelectedEvent), nameof(STriggerReplySelectedEvent))]
[JsonDerivedType(typeof(STriggerSkippedEvent), nameof(STriggerSkippedEvent))]
[JsonDerivedType(typeof(STriggerAbortMissionEvent), nameof(STriggerAbortMissionEvent))]
[JsonDerivedType(typeof(STriggerPurchaseMadeEvent), nameof(STriggerPurchaseMadeEvent))]
[JsonDerivedType(typeof(STriggerPurchaseExitEvent), nameof(STriggerPurchaseExitEvent))]
[JsonDerivedType(typeof(STriggerPlanetMissionLaunchedEvent), nameof(STriggerPlanetMissionLaunchedEvent))]
[JsonDerivedType(typeof(STriggerPlanetPanelCanceledEvent), nameof(STriggerPlanetPanelCanceledEvent))]
[JsonDerivedType(typeof(STriggerConversationSkippedEvent), nameof(STriggerConversationSkippedEvent))]
[JsonDerivedType(typeof(STriggerHotkeyPressedEvent), nameof(STriggerHotkeyPressedEvent))]
[JsonDerivedType(typeof(STriggerPlanetPanelReplayEvent), nameof(STriggerPlanetPanelReplayEvent))]
[JsonDerivedType(typeof(STriggerPlanetMissionSelectedEvent), nameof(STriggerPlanetMissionSelectedEvent))]
[JsonDerivedType(typeof(STriggerMovieFunctionEvent), nameof(STriggerMovieFunctionEvent))]
[JsonDerivedType(typeof(STriggerPlanetPanelBirthCompleteEvent), nameof(STriggerPlanetPanelBirthCompleteEvent))]
[JsonDerivedType(typeof(STriggerPlanetPanelDeathCompleteEvent), nameof(STriggerPlanetPanelDeathCompleteEvent))]
[JsonDerivedType(typeof(SResourceRequestEvent), nameof(SResourceRequestEvent))]
[JsonDerivedType(typeof(SResourceRequestFulfillEvent), nameof(SResourceRequestFulfillEvent))]
[JsonDerivedType(typeof(SResourceRequestCancelEvent), nameof(SResourceRequestCancelEvent))]
[JsonDerivedType(typeof(STriggerResearchPanelExitEvent), nameof(STriggerResearchPanelExitEvent))]
[JsonDerivedType(typeof(STriggerResearchPanelPurchaseEvent), nameof(STriggerResearchPanelPurchaseEvent))]
[JsonDerivedType(typeof(STriggerResearchPanelSelectionChangedEvent), nameof(STriggerResearchPanelSelectionChangedEvent))]
[JsonDerivedType(typeof(STriggerCommandErrorEvent), nameof(STriggerCommandErrorEvent))]
[JsonDerivedType(typeof(STriggerMercenaryPanelExitEvent), nameof(STriggerMercenaryPanelExitEvent))]
[JsonDerivedType(typeof(STriggerMercenaryPanelPurchaseEvent), nameof(STriggerMercenaryPanelPurchaseEvent))]
[JsonDerivedType(typeof(STriggerMercenaryPanelSelectionChangedEvent), nameof(STriggerMercenaryPanelSelectionChangedEvent))]
[JsonDerivedType(typeof(STriggerVictoryPanelExitEvent), nameof(STriggerVictoryPanelExitEvent))]
[JsonDerivedType(typeof(STriggerBattleReportPanelExitEvent), nameof(STriggerBattleReportPanelExitEvent))]
[JsonDerivedType(typeof(STriggerBattleReportPanelPlayMissionEvent), nameof(STriggerBattleReportPanelPlayMissionEvent))]
[JsonDerivedType(typeof(STriggerBattleReportPanelPlaySceneEvent), nameof(STriggerBattleReportPanelPlaySceneEvent))]
[JsonDerivedType(typeof(STriggerBattleReportPanelSelectionChangedEvent), nameof(STriggerBattleReportPanelSelectionChangedEvent))]
[JsonDerivedType(typeof(STriggerVictoryPanelPlayMissionAgainEvent), nameof(STriggerVictoryPanelPlayMissionAgainEvent))]
[JsonDerivedType(typeof(STriggerMovieStartedEvent), nameof(STriggerMovieStartedEvent))]
[JsonDerivedType(typeof(STriggerMovieFinishedEvent), nameof(STriggerMovieFinishedEvent))]
[JsonDerivedType(typeof(STriggerPortraitLoadedEvent), nameof(STriggerPortraitLoadedEvent))]
[JsonDerivedType(typeof(STriggerCustomDialogDismissedEvent), nameof(STriggerCustomDialogDismissedEvent))]
[JsonDerivedType(typeof(STriggerMouseWheelEvent), nameof(STriggerMouseWheelEvent))]
[JsonDerivedType(typeof(STriggerPurchasePanelSelectedPurchaseItemChangedEvent), nameof(STriggerPurchasePanelSelectedPurchaseItemChangedEvent))]
[JsonDerivedType(typeof(STriggerPurchasePanelSelectedPurchaseCategoryChangedEvent), nameof(STriggerPurchasePanelSelectedPurchaseCategoryChangedEvent))]
[JsonDerivedType(typeof(STriggerGameCreditsFinishedEvent), nameof(STriggerGameCreditsFinishedEvent))]
[JsonDerivedType(typeof(STriggerCutsceneConversationLineEvent), nameof(STriggerCutsceneConversationLineEvent))]
[JsonDerivedType(typeof(STriggerCutsceneConversationLineMissingEvent), nameof(STriggerCutsceneConversationLineMissingEvent))]
[JsonDerivedType(typeof(STriggerAnimLengthQueryByNameEvent), nameof(STriggerAnimLengthQueryByNameEvent))]
[JsonDerivedType(typeof(STriggerAnimLengthQueryByPropsEvent), nameof(STriggerAnimLengthQueryByPropsEvent))]
[JsonDerivedType(typeof(STriggerAnimOffsetEvent), nameof(STriggerAnimOffsetEvent))]
[JsonDerivedType(typeof(SCatalogModifyEvent), nameof(SCatalogModifyEvent))]
[JsonDerivedType(typeof(SHeroTalentTreeSelectedEvent), nameof(SHeroTalentTreeSelectedEvent))]
[JsonDerivedType(typeof(STriggerProfilerLoggingFinishedEvent), nameof(STriggerProfilerLoggingFinishedEvent))]
[JsonDerivedType(typeof(SHeroTalentTreeSelectionPanelToggledEvent), nameof(SHeroTalentTreeSelectionPanelToggledEvent))]
[JsonDerivedType(typeof(SPeerSetSyncLoadingTimeEvent), nameof(SPeerSetSyncLoadingTimeEvent))]
[JsonDerivedType(typeof(SPeerSetSyncPlayingTimeEvent), nameof(SPeerSetSyncPlayingTimeEvent))]
[JsonDerivedType(typeof(SLagMessageEvent), nameof(SLagMessageEvent))]
[JsonDerivedType(typeof(SPlayerLeaveEvent), nameof(SPlayerLeaveEvent))]
[JsonDerivedType(typeof(STriggerCameraMoveEvent), nameof(STriggerCameraMoveEvent))]
[JsonDerivedType(typeof(UnknownGameEvent), nameof(UnknownGameEvent))]
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
    SCmdUpdateTargetUnitEvent = 20,
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

namespace s2protocol.NET.Models;

#pragma warning disable CS1591

public sealed class SSaveGameEvent(
    int userId, int eventId, int bits, int gameloop,
    string fileName, bool automatic, bool overwrite, string name, string description)
    : GameEvent(userId, eventId, GameEventType.SSaveGameEvent, bits, gameloop)
{
    public string FileName { get; } = fileName;
    public bool Automatic { get; } = automatic;
    public bool Overwrite { get; } = overwrite;
    public string Name { get; } = name;
    public string Description { get; } = description;
}

public sealed class SSaveGameDoneEvent(int userId, int eventId, int bits, int gameloop)
    : GameEvent(userId, eventId, GameEventType.SSaveGameDoneEvent, bits, gameloop);

public sealed class SLoadGameDoneEvent(int userId, int eventId, int bits, int gameloop)
    : GameEvent(userId, eventId, GameEventType.SLoadGameDoneEvent, bits, gameloop);

public sealed class SCommandManagerResetEvent(int userId, int eventId, int bits, int gameloop, long sequence)
    : GameEvent(userId, eventId, GameEventType.SCommandManagerResetEvent, bits, gameloop)
{
    public long Sequence { get; } = sequence;
}

public sealed class SGameCheatEvent(int userId, int eventId, int bits, int gameloop, GameCheatEventData data)
    : GameEvent(userId, eventId, GameEventType.SGameCheatEvent, bits, gameloop)
{
    public GameCheatEventData Data { get; } = data;
}

public sealed class SSelectionSyncCheckEvent(
    int userId, int eventId, int bits, int gameloop,
    int controlGroupId, SelectionSyncCheckEventData selectionSyncData)
    : GameEvent(userId, eventId, GameEventType.SSelectionSyncCheckEvent, bits, gameloop)
{
    public int ControlGroupId { get; } = controlGroupId;
    public SelectionSyncCheckEventData SelectionSyncData { get; } = selectionSyncData;
}

public sealed class SResourceTradeEvent(
    int userId, int eventId, int bits, int gameloop, int recipientId, IReadOnlyList<long> resources)
    : GameEvent(userId, eventId, GameEventType.SResourceTradeEvent, bits, gameloop)
{
    public int RecipientId { get; } = recipientId;
    public IReadOnlyList<long> Resources { get; } = resources;
}

public sealed class SAICommunicateEvent(
    int userId, int eventId, int bits, int gameloop,
    int beacon, int ally, int? autocast, int? flags, int? build,
    long targetUnitTag, int targetUnitSnapshotUnitLink,
    int? targetUnitSnapshotPlayerId, int? targetUnitSnapshotUpkeepPlayerId,
    int? targetUnitSnapshotControlPlayerId, GameEventPoint3D targetPoint)
    : GameEvent(userId, eventId, GameEventType.SAICommunicateEvent, bits, gameloop)
{
    public int Beacon { get; } = beacon;
    public int Ally { get; } = ally;
    public int? Autocast { get; } = autocast;
    public int? Flags { get; } = flags;
    public int? Build { get; } = build;
    public long TargetUnitTag { get; } = targetUnitTag;
    public int TargetUnitSnapshotUnitLink { get; } = targetUnitSnapshotUnitLink;
    public int? TargetUnitSnapshotPlayerId { get; } = targetUnitSnapshotPlayerId;
    public int? TargetUnitSnapshotUpkeepPlayerId { get; } = targetUnitSnapshotUpkeepPlayerId;
    public int? TargetUnitSnapshotControlPlayerId { get; } = targetUnitSnapshotControlPlayerId;
    public GameEventPoint3D TargetPoint { get; } = targetPoint;
}

public sealed class SSetAbsoluteGameSpeedEvent(int userId, int eventId, int bits, int gameloop, int speed)
    : GameEvent(userId, eventId, GameEventType.SSetAbsoluteGameSpeedEvent, bits, gameloop)
{
    public int Speed { get; } = speed;
}

public sealed class SAddAbsoluteGameSpeedEvent(int userId, int eventId, int bits, int gameloop, int delta)
    : GameEvent(userId, eventId, GameEventType.SAddAbsoluteGameSpeedEvent, bits, gameloop)
{
    public int Delta { get; } = delta;
}

public sealed class SBroadcastCheatEvent(int userId, int eventId, int bits, int gameloop, string verb, string arguments)
    : GameEvent(userId, eventId, GameEventType.SBroadcastCheatEvent, bits, gameloop)
{
    public string Verb { get; } = verb;
    public string Arguments { get; } = arguments;
}

public sealed class SAllianceEvent(int userId, int eventId, int bits, int gameloop, long alliance, long control)
    : GameEvent(userId, eventId, GameEventType.SAllianceEvent, bits, gameloop)
{
    public long Alliance { get; } = alliance;
    public long Control { get; } = control;
}

public sealed class SUnitHighlightEvent(int userId, int eventId, int bits, int gameloop, long unitTag, int flags)
    : GameEvent(userId, eventId, GameEventType.SUnitHighlightEvent, bits, gameloop)
{
    public long UnitTag { get; } = unitTag;
    public int Flags { get; } = flags;
}

public sealed class STriggerReplySelectedEvent(
    int userId, int eventId, int bits, int gameloop, long conversationId, long replyId)
    : GameEvent(userId, eventId, GameEventType.STriggerReplySelectedEvent, bits, gameloop)
{
    public long ConversationId { get; } = conversationId;
    public long ReplyId { get; } = replyId;
}

public sealed class STriggerSkippedEvent(int userId, int eventId, int bits, int gameloop)
    : GameEvent(userId, eventId, GameEventType.STriggerSkippedEvent, bits, gameloop);

public sealed class STriggerAbortMissionEvent(int userId, int eventId, int bits, int gameloop)
    : GameEvent(userId, eventId, GameEventType.STriggerAbortMissionEvent, bits, gameloop);

public sealed class STriggerPurchaseMadeEvent(int userId, int eventId, int bits, int gameloop, long purchaseItemId)
    : GameEvent(userId, eventId, GameEventType.STriggerPurchaseMadeEvent, bits, gameloop)
{
    public long PurchaseItemId { get; } = purchaseItemId;
}

public sealed class STriggerPurchaseExitEvent(int userId, int eventId, int bits, int gameloop)
    : GameEvent(userId, eventId, GameEventType.STriggerPurchaseExitEvent, bits, gameloop);

public sealed class STriggerPlanetMissionLaunchedEvent(int userId, int eventId, int bits, int gameloop, long difficultyLevel)
    : GameEvent(userId, eventId, GameEventType.STriggerPlanetMissionLaunchedEvent, bits, gameloop)
{
    public long DifficultyLevel { get; } = difficultyLevel;
}

public sealed class STriggerPlanetPanelCanceledEvent(int userId, int eventId, int bits, int gameloop)
    : GameEvent(userId, eventId, GameEventType.STriggerPlanetPanelCanceledEvent, bits, gameloop);

public sealed class STriggerConversationSkippedEvent(int userId, int eventId, int bits, int gameloop, int skipType)
    : GameEvent(userId, eventId, GameEventType.STriggerConversationSkippedEvent, bits, gameloop)
{
    public int SkipType { get; } = skipType;
}

public sealed class STriggerHotkeyPressedEvent(int userId, int eventId, int bits, int gameloop, long hotkey, bool down)
    : GameEvent(userId, eventId, GameEventType.STriggerHotkeyPressedEvent, bits, gameloop)
{
    public long Hotkey { get; } = hotkey;
    public bool Down { get; } = down;
}

public sealed class STriggerPlanetPanelReplayEvent(int userId, int eventId, int bits, int gameloop)
    : GameEvent(userId, eventId, GameEventType.STriggerPlanetPanelReplayEvent, bits, gameloop);

public sealed class STriggerPlanetMissionSelectedEvent(int userId, int eventId, int bits, int gameloop, long planetId)
    : GameEvent(userId, eventId, GameEventType.STriggerPlanetMissionSelectedEvent, bits, gameloop)
{
    public long PlanetId { get; } = planetId;
}

public sealed class STriggerMovieFunctionEvent(int userId, int eventId, int bits, int gameloop, string functionName)
    : GameEvent(userId, eventId, GameEventType.STriggerMovieFunctionEvent, bits, gameloop)
{
    public string FunctionName { get; } = functionName;
}

public sealed class STriggerPlanetPanelBirthCompleteEvent(int userId, int eventId, int bits, int gameloop)
    : GameEvent(userId, eventId, GameEventType.STriggerPlanetPanelBirthCompleteEvent, bits, gameloop);

public sealed class STriggerPlanetPanelDeathCompleteEvent(int userId, int eventId, int bits, int gameloop)
    : GameEvent(userId, eventId, GameEventType.STriggerPlanetPanelDeathCompleteEvent, bits, gameloop);

public sealed class SResourceRequestEvent(int userId, int eventId, int bits, int gameloop, IReadOnlyList<long> resources)
    : GameEvent(userId, eventId, GameEventType.SResourceRequestEvent, bits, gameloop)
{
    public IReadOnlyList<long> Resources { get; } = resources;
}

public sealed class SResourceRequestFulfillEvent(int userId, int eventId, int bits, int gameloop, long fulfillRequestId)
    : GameEvent(userId, eventId, GameEventType.SResourceRequestFulfillEvent, bits, gameloop)
{
    public long FulfillRequestId { get; } = fulfillRequestId;
}

public sealed class SResourceRequestCancelEvent(int userId, int eventId, int bits, int gameloop, long cancelRequestId)
    : GameEvent(userId, eventId, GameEventType.SResourceRequestCancelEvent, bits, gameloop)
{
    public long CancelRequestId { get; } = cancelRequestId;
}

public sealed class STriggerResearchPanelExitEvent(int userId, int eventId, int bits, int gameloop)
    : GameEvent(userId, eventId, GameEventType.STriggerResearchPanelExitEvent, bits, gameloop);

public sealed class STriggerResearchPanelPurchaseEvent(int userId, int eventId, int bits, int gameloop)
    : GameEvent(userId, eventId, GameEventType.STriggerResearchPanelPurchaseEvent, bits, gameloop);

public sealed class STriggerResearchPanelSelectionChangedEvent(
    int userId, int eventId, int bits, int gameloop, long researchItemId)
    : GameEvent(userId, eventId, GameEventType.STriggerResearchPanelSelectionChangedEvent, bits, gameloop)
{
    public long ResearchItemId { get; } = researchItemId;
}

public sealed class STriggerCommandErrorEvent(
    int userId, int eventId, int bits, int gameloop, long error, GameEventAbility? abil)
    : GameEvent(userId, eventId, GameEventType.STriggerCommandErrorEvent, bits, gameloop)
{
    public long Error { get; } = error;
    public GameEventAbility? Abil { get; } = abil;
}

public sealed class STriggerMercenaryPanelExitEvent(int userId, int eventId, int bits, int gameloop)
    : GameEvent(userId, eventId, GameEventType.STriggerMercenaryPanelExitEvent, bits, gameloop);

public sealed class STriggerMercenaryPanelPurchaseEvent(int userId, int eventId, int bits, int gameloop)
    : GameEvent(userId, eventId, GameEventType.STriggerMercenaryPanelPurchaseEvent, bits, gameloop);

public sealed class STriggerMercenaryPanelSelectionChangedEvent(
    int userId, int eventId, int bits, int gameloop, long mercenaryId)
    : GameEvent(userId, eventId, GameEventType.STriggerMercenaryPanelSelectionChangedEvent, bits, gameloop)
{
    public long MercenaryId { get; } = mercenaryId;
}

public sealed class STriggerVictoryPanelExitEvent(int userId, int eventId, int bits, int gameloop)
    : GameEvent(userId, eventId, GameEventType.STriggerVictoryPanelExitEvent, bits, gameloop);

public sealed class STriggerBattleReportPanelExitEvent(int userId, int eventId, int bits, int gameloop)
    : GameEvent(userId, eventId, GameEventType.STriggerBattleReportPanelExitEvent, bits, gameloop);

public sealed class STriggerBattleReportPanelPlayMissionEvent(
    int userId, int eventId, int bits, int gameloop, long battleReportId, long difficultyLevel)
    : GameEvent(userId, eventId, GameEventType.STriggerBattleReportPanelPlayMissionEvent, bits, gameloop)
{
    public long BattleReportId { get; } = battleReportId;
    public long DifficultyLevel { get; } = difficultyLevel;
}

public sealed class STriggerBattleReportPanelPlaySceneEvent(
    int userId, int eventId, int bits, int gameloop, long battleReportId)
    : GameEvent(userId, eventId, GameEventType.STriggerBattleReportPanelPlaySceneEvent, bits, gameloop)
{
    public long BattleReportId { get; } = battleReportId;
}

public sealed class STriggerBattleReportPanelSelectionChangedEvent(
    int userId, int eventId, int bits, int gameloop, long battleReportId)
    : GameEvent(userId, eventId, GameEventType.STriggerBattleReportPanelSelectionChangedEvent, bits, gameloop)
{
    public long BattleReportId { get; } = battleReportId;
}

public sealed class STriggerVictoryPanelPlayMissionAgainEvent(
    int userId, int eventId, int bits, int gameloop, long difficultyLevel)
    : GameEvent(userId, eventId, GameEventType.STriggerVictoryPanelPlayMissionAgainEvent, bits, gameloop)
{
    public long DifficultyLevel { get; } = difficultyLevel;
}

public sealed class STriggerMovieStartedEvent(int userId, int eventId, int bits, int gameloop)
    : GameEvent(userId, eventId, GameEventType.STriggerMovieStartedEvent, bits, gameloop);

public sealed class STriggerMovieFinishedEvent(int userId, int eventId, int bits, int gameloop)
    : GameEvent(userId, eventId, GameEventType.STriggerMovieFinishedEvent, bits, gameloop);

public sealed class STriggerPortraitLoadedEvent(int userId, int eventId, int bits, int gameloop, long portraitId)
    : GameEvent(userId, eventId, GameEventType.STriggerPortraitLoadedEvent, bits, gameloop)
{
    public long PortraitId { get; } = portraitId;
}

public sealed class STriggerCustomDialogDismissedEvent(int userId, int eventId, int bits, int gameloop, long result)
    : GameEvent(userId, eventId, GameEventType.STriggerCustomDialogDismissedEvent, bits, gameloop)
{
    public long Result { get; } = result;
}

public sealed class STriggerMouseWheelEvent(int userId, int eventId, int bits, int gameloop, int wheelSpin, int flags)
    : GameEvent(userId, eventId, GameEventType.STriggerMouseWheelEvent, bits, gameloop)
{
    public int WheelSpin { get; } = wheelSpin;
    public int Flags { get; } = flags;
}

public sealed class STriggerPurchasePanelSelectedPurchaseItemChangedEvent(
    int userId, int eventId, int bits, int gameloop, long purchaseItemId)
    : GameEvent(userId, eventId, GameEventType.STriggerPurchasePanelSelectedPurchaseItemChangedEvent, bits, gameloop)
{
    public long PurchaseItemId { get; } = purchaseItemId;
}

public sealed class STriggerPurchasePanelSelectedPurchaseCategoryChangedEvent(
    int userId, int eventId, int bits, int gameloop, long purchaseCategoryId)
    : GameEvent(userId, eventId, GameEventType.STriggerPurchasePanelSelectedPurchaseCategoryChangedEvent, bits, gameloop)
{
    public long PurchaseCategoryId { get; } = purchaseCategoryId;
}

public sealed class STriggerGameCreditsFinishedEvent(int userId, int eventId, int bits, int gameloop)
    : GameEvent(userId, eventId, GameEventType.STriggerGameCreditsFinishedEvent, bits, gameloop);

public sealed class STriggerCutsceneConversationLineEvent(
    int userId, int eventId, int bits, int gameloop,
    long cutsceneId, string conversationLine, string altConversationLine)
    : GameEvent(userId, eventId, GameEventType.STriggerCutsceneConversationLineEvent, bits, gameloop)
{
    public long CutsceneId { get; } = cutsceneId;
    public string ConversationLine { get; } = conversationLine;
    public string AltConversationLine { get; } = altConversationLine;
}

public sealed class STriggerCutsceneConversationLineMissingEvent(
    int userId, int eventId, int bits, int gameloop, long cutsceneId, string conversationLine)
    : GameEvent(userId, eventId, GameEventType.STriggerCutsceneConversationLineMissingEvent, bits, gameloop)
{
    public long CutsceneId { get; } = cutsceneId;
    public string ConversationLine { get; } = conversationLine;
}

public sealed class STriggerAnimLengthQueryByNameEvent(
    int userId, int eventId, int bits, int gameloop, int queryId, long lengthMs, long finishGameLoop)
    : GameEvent(userId, eventId, GameEventType.STriggerAnimLengthQueryByNameEvent, bits, gameloop)
{
    public int QueryId { get; } = queryId;
    public long LengthMs { get; } = lengthMs;
    public long FinishGameLoop { get; } = finishGameLoop;
}

public sealed class STriggerAnimLengthQueryByPropsEvent(
    int userId, int eventId, int bits, int gameloop, int queryId, long lengthMs)
    : GameEvent(userId, eventId, GameEventType.STriggerAnimLengthQueryByPropsEvent, bits, gameloop)
{
    public int QueryId { get; } = queryId;
    public long LengthMs { get; } = lengthMs;
}

public sealed class STriggerAnimOffsetEvent(int userId, int eventId, int bits, int gameloop, int animWaitQueryId)
    : GameEvent(userId, eventId, GameEventType.STriggerAnimOffsetEvent, bits, gameloop)
{
    public int AnimWaitQueryId { get; } = animWaitQueryId;
}

public sealed class SCatalogModifyEvent(
    int userId, int eventId, int bits, int gameloop, int catalog, int entry, string field, string value)
    : GameEvent(userId, eventId, GameEventType.SCatalogModifyEvent, bits, gameloop)
{
    public int Catalog { get; } = catalog;
    public int Entry { get; } = entry;
    public string Field { get; } = field;
    public string Value { get; } = value;
}

public sealed class SHeroTalentTreeSelectedEvent(int userId, int eventId, int bits, int gameloop, long index)
    : GameEvent(userId, eventId, GameEventType.SHeroTalentTreeSelectedEvent, bits, gameloop)
{
    public long Index { get; } = index;
}

public sealed class STriggerProfilerLoggingFinishedEvent(int userId, int eventId, int bits, int gameloop)
    : GameEvent(userId, eventId, GameEventType.STriggerProfilerLoggingFinishedEvent, bits, gameloop);

public sealed class SHeroTalentTreeSelectionPanelToggledEvent(
    int userId, int eventId, int bits, int gameloop, bool shown)
    : GameEvent(userId, eventId, GameEventType.SHeroTalentTreeSelectionPanelToggledEvent, bits, gameloop)
{
    public bool Shown { get; } = shown;
}

public sealed class SPeerSetSyncLoadingTimeEvent(int userId, int eventId, int bits, int gameloop, long syncTime)
    : GameEvent(userId, eventId, GameEventType.SPeerSetSyncLoadingTimeEvent, bits, gameloop)
{
    public long SyncTime { get; } = syncTime;
}

public sealed class SPeerSetSyncPlayingTimeEvent(int userId, int eventId, int bits, int gameloop, long syncTime)
    : GameEvent(userId, eventId, GameEventType.SPeerSetSyncPlayingTimeEvent, bits, gameloop)
{
    public long SyncTime { get; } = syncTime;
}

public sealed class SLagMessageEvent(int userId, int eventId, int bits, int gameloop, int laggingPlayerId)
    : GameEvent(userId, eventId, GameEventType.SLagMessageEvent, bits, gameloop)
{
    public int LaggingPlayerId { get; } = laggingPlayerId;
}

public sealed class SPlayerLeaveEvent(int userId, int eventId, int bits, int gameloop)
    : GameEvent(userId, eventId, GameEventType.SPlayerLeaveEvent, bits, gameloop);

public sealed class STriggerCameraMoveEvent(int userId, int eventId, int bits, int gameloop, int reason)
    : GameEvent(userId, eventId, GameEventType.STriggerCameraMoveEvent, bits, gameloop)
{
    public int Reason { get; } = reason;
}

#pragma warning restore CS1591

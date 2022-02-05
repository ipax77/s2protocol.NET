using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>Event</c> Event baseclass</summary>
///
public record GameEvent
{
    /// <summary>Record <c>GameEvent</c> base constructor</summary>
    /// 
    public GameEvent(int userId, int eventId, string eventType, int bits, int gameloop)
    {
        UserId = userId;
        EventId = eventId;
        Bits = bits;
        Gameloop = gameloop;
        EventType = eventType switch
        {
            "NNet.Game.SBankFileEvent" => GameEventType.SBankFileEvent,
            "NNet.Game.SBankKeyEvent" => GameEventType.SBankKeyEvent,
            "NNet.Game.SBankSectionEvent" => GameEventType.SBankSectionEvent,
            "NNet.Game.SBankSignatureEvent" => GameEventType.SBankSignatureEvent,
            "NNet.Game.SBankValueEvent" => GameEventType.SBankValueEvent,
            "NNet.Game.SCameraUpdateEvent" => GameEventType.SCameraUpdateEvent,
            "NNet.Game.SCmdEvent" => GameEventType.SCmdEvent,
            "NNet.Game.SCmdUpdateTargetPointEvent" => GameEventType.SCmdUpdateTargetPointEvent,
            "NNet.Game.SCommandManagerStateEvent" => GameEventType.SCommandManagerStateEvent,
            "NNet.Game.SControlGroupUpdateEvent" => GameEventType.SControlGroupUpdateEvent,
            "NNet.Game.SGameUserLeaveEvent" => GameEventType.SGameUserLeaveEvent,
            "NNet.Game.SSelectionDeltaEvent" => GameEventType.SSelectionDeltaEvent,
            "NNet.Game.SSetSyncLoadingTimeEvent" => GameEventType.SSetSyncLoadingTimeEvent,
            "NNet.Game.SSetSyncPlayingTimeEvent" => GameEventType.SSetSyncPlayingTimeEvent,
            "NNet.Game.STriggerDialogControlEvent" => GameEventType.STriggerDialogControlEvent,
            "NNet.Game.STriggerPingEvent" => GameEventType.STriggerPingEvent,
            "NNet.Game.STriggerSoundLengthSyncEvent" => GameEventType.STriggerSoundLengthSyncEvent,
            "NNet.Game.SUserFinishedLoadingSyncEvent" => GameEventType.SUserFinishedLoadingSyncEvent,
            "NNet.Game.SUserOptionsEvent" => GameEventType.SUserOptionsEvent,
            "NNet.Game.SCmdUpdateTargetUnitEvent" => GameEventType.SCmdUpdateTargetUnitEvents,
            "NNet.Game.STriggerKeyPressedEvent" => GameEventType.STriggerKeyPressedEvent,
            _ => GameEventType.None
        };
    }

    /// <summary>Record <c>GameEvent</c> base clone constructor</summary>
    /// 
    public GameEvent(GameEvent gameEvent)
    {
        if (gameEvent == null)
        {
            throw new ArgumentNullException(nameof(gameEvent));
        }
        UserId = gameEvent.UserId;
        EventId = gameEvent.EventId;
        Bits = gameEvent.Bits;
        Gameloop = gameEvent.Gameloop;
        EventType = gameEvent.EventType;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public GameEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Event PlayerId</summary>
    ///
    public int UserId { get; init; }

    /// <summary>Event EventId</summary>
    ///
    public int EventId { get; init; }
    /// <summary>Event EventType</summary>
    ///
    public GameEventType EventType { get; init; }
    /// <summary>Event Bits</summary>
    ///
    public int Bits { get; init; }
    /// <summary>Event Gameloop</summary>
    ///
    public int Gameloop { get; init; }

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
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}

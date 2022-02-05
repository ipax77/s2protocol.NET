using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;

/// <summary>Record <c>GameEvents</c> GameEvents</summary>
///
public sealed record GameEvents
{
    /// <summary>Record <c>GameEvents</c> constructor</summary>
    ///
    public GameEvents(ICollection<GameEvent> gameEvents)
    {
        BaseGameEvents = gameEvents;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public GameEvents()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>BaseGameEvents</summary>
    ///
    public ICollection<GameEvent> BaseGameEvents { get; init; }
    /// <summary>SBankFileEvent (extract from BaseGameEvents)</summary>
    ///
    public ICollection<SBankFileEvent> SBankFileEvents => BaseGameEvents.OfType<SBankFileEvent>().ToArray();
    /// <summary>SBankKeyEvent (extract from BaseGameEvents)</summary>
    ///
    public ICollection<SBankKeyEvent> SBankKeyEvents => BaseGameEvents.OfType<SBankKeyEvent>().ToArray();
    /// <summary>SBankSectionEvent (extract from BaseGameEvents)</summary>
    ///
    public ICollection<SBankSectionEvent> SBankSectionEvents => BaseGameEvents.OfType<SBankSectionEvent>().ToArray();
    /// <summary>SBankSignatureEvent (extract from BaseGameEvents)</summary>
    ///
    public ICollection<SBankSignatureEvent> SBankSignatureEvents => BaseGameEvents.OfType<SBankSignatureEvent>().ToArray();
    /// <summary>SBankValueEvent (extract from BaseGameEvents)</summary>
    ///
    public ICollection<SBankValueEvent> SBankValueEvents => BaseGameEvents.OfType<SBankValueEvent>().ToArray();
    /// <summary>SCameraUpdateEvent (extract from BaseGameEvents)</summary>
    ///
    public ICollection<SCameraUpdateEvent> SCameraUpdateEvents => BaseGameEvents.OfType<SCameraUpdateEvent>().ToArray();
    /// <summary>SCmdEvent (extract from BaseGameEvents)</summary>
    ///
    public ICollection<SCmdEvent> SCmdEvents => BaseGameEvents.OfType<SCmdEvent>().ToArray();
    /// <summary>SCmdUpdateTargetPointEvents (extract from BaseGameEvents)</summary>
    ///
    public ICollection<SCmdUpdateTargetPointEvent> SCmdUpdateTargetPointEvents => BaseGameEvents.OfType<SCmdUpdateTargetPointEvent>().ToArray();
    /// <summary>SCommandManagerStateEvents (extract from BaseGameEvents)</summary>
    ///
    public ICollection<SCommandManagerStateEvent> SCommandManagerStateEvents => BaseGameEvents.OfType<SCommandManagerStateEvent>().ToArray();
    /// <summary>SControlGroupUpdateEvents (extract from BaseGameEvents)</summary>
    ///
    public ICollection<SControlGroupUpdateEvent> SControlGroupUpdateEvents => BaseGameEvents.OfType<SControlGroupUpdateEvent>().ToArray();
    /// <summary>SGameUserLeaveEvents (extract from BaseGameEvents)</summary>
    ///
    public ICollection<SGameUserLeaveEvent> SGameUserLeaveEvents => BaseGameEvents.OfType<SGameUserLeaveEvent>().ToArray();
    /// <summary>SSelectionDeltaEvents (extract from BaseGameEvents)</summary>
    ///
    public ICollection<SSelectionDeltaEvent> SSelectionDeltaEvents => BaseGameEvents.OfType<SSelectionDeltaEvent>().ToArray();
    /// <summary>SSetSyncLoadingTimeEvents (extract from BaseGameEvents)</summary>
    ///
    public ICollection<SSetSyncLoadingTimeEvent> SSetSyncLoadingTimeEvents => BaseGameEvents.OfType<SSetSyncLoadingTimeEvent>().ToArray();
    /// <summary>SSetSyncPlayingTimeEvents (extract from BaseGameEvents)</summary>
    ///
    public ICollection<SSetSyncPlayingTimeEvent> SSetSyncPlayingTimeEvents => BaseGameEvents.OfType<SSetSyncPlayingTimeEvent>().ToArray();
    /// <summary>STriggerDialogControlEvents (extract from BaseGameEvents)</summary>
    ///
    public ICollection<STriggerDialogControlEvent> STriggerDialogControlEvents => BaseGameEvents.OfType<STriggerDialogControlEvent>().ToArray();
    /// <summary>STriggerPingEvents (extract from BaseGameEvents)</summary>
    ///
    public ICollection<STriggerPingEvent> STriggerPingEvents => BaseGameEvents.OfType<STriggerPingEvent>().ToArray();
    /// <summary>STriggerSoundLengthSyncEvents (extract from BaseGameEvents)</summary>
    ///
    public ICollection<STriggerSoundLengthSyncEvent> STriggerSoundLengthSyncEvents => BaseGameEvents.OfType<STriggerSoundLengthSyncEvent>().ToArray();
    /// <summary>SUserFinishedLoadingSyncEvents (extract from BaseGameEvents)</summary>
    ///
    public ICollection<SUserFinishedLoadingSyncEvent> SUserFinishedLoadingSyncEvents => BaseGameEvents.OfType<SUserFinishedLoadingSyncEvent>().ToArray();
    /// <summary>SUserOptionsEvents (extract from BaseGameEvents)</summary>
    ///
    public ICollection<SUserOptionsEvent> SUserOptionsEvents => BaseGameEvents.OfType<SUserOptionsEvent>().ToArray();
    /// <summary>SCmdUpdateTargetUnitEvents (extract from BaseGameEvents)</summary>
    ///
    public ICollection<SCmdUpdateTargetUnitEvent> SCmdUpdateTargetUnitEvents => BaseGameEvents.OfType<SCmdUpdateTargetUnitEvent>().ToArray();
    /// <summary>UnknownGameEvents with raw PythonDictionary (extract from BaseGameEvents)</summary>
    ///
    public ICollection<UnknownGameEvent> UnknownGameEvents => BaseGameEvents.OfType<UnknownGameEvent>().ToArray();
}

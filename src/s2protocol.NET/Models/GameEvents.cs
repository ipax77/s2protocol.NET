using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace s2protocol.NET.Models;

/// <summary>Record <c>GameEvents</c> GameEvents</summary>
///
public sealed record GameEvents
{
    /// <summary>Record <c>GameEvents</c> constructor</summary>
    ///
    public GameEvents(ICollection<SBankFileEvent> sBankFileEvents,
                      ICollection<SBankKeyEvent> sBankKeyEvents,
                      ICollection<SBankSectionEvent> sBankSectionEvents,
                      ICollection<SBankSignatureEvent> sBankSignatureEvents,
                      ICollection<SBankValueEvent> sBankValueEvents,
                      ICollection<SCameraUpdateEvent> sCameraUpdateEvents,
                      ICollection<SCmdEvent> sCmdEvents)
    {
        SBankFileEvents = sBankFileEvents;
        SBankKeyEvents = sBankKeyEvents;
        SBankSectionEvents = sBankSectionEvents;
        SBankSignatureEvents = sBankSignatureEvents;
        SBankValueEvents = sBankValueEvents;
        SCameraUpdateEvents = sCameraUpdateEvents;
        SCmdEvents = sCmdEvents;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public GameEvents()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Event SBankFileEvents</summary>
    ///
    public ICollection<SBankFileEvent> SBankFileEvents { get; init; }
    /// <summary>Event SBankKeyEvents</summary>
    ///    }
    public ICollection<SBankKeyEvent> SBankKeyEvents { get; init; }
    /// <summary>Event SBankSectionEvents</summary>
    ///
    public ICollection<SBankSectionEvent> SBankSectionEvents { get; init; }
    /// <summary>Event SBankSignatureEvents</summary>
    ///
    public ICollection<SBankSignatureEvent> SBankSignatureEvents { get; init; }
    /// <summary>Event SBankValueEvents</summary>
    ///        init; }
    public ICollection<SBankValueEvent> SBankValueEvents { get; init; }
    /// <summary>Event SCameraUpdateEvents</summary>
    ///
    public ICollection<SCameraUpdateEvent> SCameraUpdateEvents { get; init; }
    /// <summary>Event SCmdEvents</summary>
    /// 
    public ICollection<SCmdEvent> SCmdEvents { get; init; }
    //public ICollection<SCmdUpdateTargetPointEvent> SCmdUpdateTargetPointEvents { get; init; }
    //public ICollection<SCommandManagerStateEvent> SCommandManagerStateEvents { get; init; }
    //public ICollection<SControlGroupUpdateEvent> SControlGroupUpdateEvents { get; init; }
    //public ICollection<SGameUserLeaveEvent> SGameUserLeaveEvents { get; init; }
    //public ICollection<SSelectionDeltaEvent> SSelectionDeltaEvents { get; init; }
    //public ICollection<SSetSyncLoadingTimeEvent> SSetSyncLoadingTimeEvents { get; init; }
    //public ICollection<SSetSyncPlayingTimeEvent> SSetSyncPlayingTimeEvents { get; init; }
    //public ICollection<STriggerDialogControlEvent> STriggerDialogControlEvents { get; init; }
    //public ICollection<STriggerPingEvent> STriggerPingEvents { get; init; }
    //public ICollection<STriggerSoundLengthSyncEvent> STriggerSoundLengthSyncEvents { get; init; }
    //public ICollection<SUserFinishedLoadingSyncEvent> SUserFinishedLoadingSyncEvents { get; init; }
    //public ICollection<SUserOptionsEvent> SUserOptionsEvents { get; init; }
}

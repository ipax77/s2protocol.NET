using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>SBankSignatureEvent</c> SBankSignatureEvent</summary>
///
public record SBankSignatureEvent : GameEvent
{
    /// <summary>Record <c>SBankSignatureEvent</c> constructor</summary>
    ///
    public SBankSignatureEvent(
        GameEvent gameEvent,
        string toonHandle,
        ICollection<int> signature) : base(gameEvent)
    {
        ToonHandle = toonHandle;
        Signature = signature;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SBankSignatureEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Event ToonHandle</summary>
    ///
    public string ToonHandle { get; init; }
    /// <summary>Event Signature</summary>
    ///
    public ICollection<int> Signature { get; init; }
}
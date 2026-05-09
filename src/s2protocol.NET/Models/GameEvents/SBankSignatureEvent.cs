namespace s2protocol.NET.Models;
/// <summary>Record <c>SBankSignatureEvent</c> SBankSignatureEvent</summary>
///
public sealed class SBankSignatureEvent : GameEvent
{
    /// <summary>Record <c>SBankSignatureEvent</c> constructor</summary>
    ///
    public SBankSignatureEvent(int userId,
        int eventId,
        int bits,
        int gameloop,
        string toonHandle,
        ICollection<int> signature) : base(userId, eventId, GameEventType.SBankSignatureEvent, bits, gameloop)
    {
        ToonHandle = toonHandle;
        Signature = signature;
    }

    /// <summary>Event ToonHandle</summary>
    ///
    public string ToonHandle { get; }
    /// <summary>Event Signature</summary>
    ///
    public ICollection<int> Signature { get; }
}
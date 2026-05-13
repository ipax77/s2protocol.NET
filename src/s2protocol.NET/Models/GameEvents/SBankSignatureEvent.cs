namespace s2protocol.NET.Models;
/// <summary>Record <c>SBankSignatureEvent</c> SBankSignatureEvent</summary>
///
/// <remarks>Record <c>SBankSignatureEvent</c> constructor</remarks>
///
public sealed class SBankSignatureEvent(int userId,
    int eventId,
    int bits,
    int gameloop,
    string toonHandle,
    ICollection<int> signature) : GameEvent(userId, eventId, GameEventType.SBankSignatureEvent, bits, gameloop)
{

    /// <summary>Event ToonHandle</summary>
    ///
    public string ToonHandle { get; } = toonHandle;
    /// <summary>Event Signature</summary>
    ///
    public ICollection<int> Signature { get; } = signature;
}
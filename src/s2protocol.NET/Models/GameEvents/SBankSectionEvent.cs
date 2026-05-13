namespace s2protocol.NET.Models;
/// <summary>Record <c>SBankSectionEvent</c> SBankSectionEvent</summary>
///
/// <remarks>Record <c>SBankSectionEvent</c> constructor</remarks>
///
public sealed class SBankSectionEvent(int userId,
    int eventId,
    int bits,
    int gameloop,
    string name) : GameEvent(userId, eventId, GameEventType.SBankSectionEvent, bits, gameloop)
{

    /// <summary>Event Type</summary>
    ///
    public string Name { get; } = name;
}
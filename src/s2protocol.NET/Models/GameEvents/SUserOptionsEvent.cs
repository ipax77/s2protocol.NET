namespace s2protocol.NET.Models;
/// <summary>Record <c>SBankFileEvent</c> SBankFileEvent</summary>
///
/// <remarks>Record <c>SBankFileEvent</c> constructor</remarks>
///
public sealed class SBankFileEvent(int userId,
    int eventId,
    int bits,
    int gameloop,
    string name) : GameEvent(userId, eventId, GameEventType.SBankFileEvent, bits, gameloop)
{

    /// <summary>Event Type</summary>
    ///
    public string Name { get; } = name;
}
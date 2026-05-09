namespace s2protocol.NET.Models;
/// <summary>Record <c>SBankFileEvent</c> SBankFileEvent</summary>
///
public sealed class SBankFileEvent : GameEvent
{
    /// <summary>Record <c>SBankFileEvent</c> constructor</summary>
    ///
    public SBankFileEvent(int userId,
        int eventId,
        int bits,
        int gameloop,
        string name) : base(userId, eventId, GameEventType.SBankFileEvent, bits, gameloop)
    {
        Name = name;
    }

    /// <summary>Event Type</summary>
    ///
    public string Name { get; }
}
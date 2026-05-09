namespace s2protocol.NET.Models;
/// <summary>Record <c>SBankSectionEvent</c> SBankSectionEvent</summary>
///
public sealed class SBankSectionEvent : GameEvent
{
    /// <summary>Record <c>SBankSectionEvent</c> constructor</summary>
    ///
    public SBankSectionEvent(int userId,
        int eventId,
        int bits,
        int gameloop,
        string name) : base(userId, eventId, GameEventType.SBankSectionEvent, bits, gameloop)
    {
        Name = name;
    }

    /// <summary>Event Type</summary>
    ///
    public string Name { get; }
}
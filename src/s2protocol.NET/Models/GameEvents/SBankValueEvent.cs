namespace s2protocol.NET.Models;
/// <summary>Record <c>SBankValueEvent</c> SBankValueEvent</summary>
///
public sealed class SBankValueEvent : GameEvent
{
    /// <summary>Record <c>SBankValueEvent</c> constructor</summary>
    ///
    public SBankValueEvent(int userId,
        int eventId,
        int bits,
        int gameloop,
        string name,
        string data,
        int type) : base(userId, eventId, GameEventType.SBankValueEvent, bits, gameloop)
    {
        Name = name;
        Data = data;
        Type = type;
    }

    /// <summary>Event Type</summary>
    ///
    public string Name { get; }
    /// <summary>Event Data</summary>
    ///
    public string Data { get; }
    /// <summary>Event Type</summary>
    ///
    public int Type { get; }
}
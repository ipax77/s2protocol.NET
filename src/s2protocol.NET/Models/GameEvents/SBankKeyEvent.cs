namespace s2protocol.NET.Models;
/// <summary>Record <c>SBankKeyEvent</c> SBankKeyEvent</summary>
///
public sealed class SBankKeyEvent : GameEvent
{
    /// <summary>Record <c>SBankKeyEvent</c> constructor</summary>
    ///
    public SBankKeyEvent(int userId,
        int eventId,
        int bits,
        int gameloop,
        string name,
        string data,
        int type) : base(userId, eventId, GameEventType.SBankKeyEvent, bits, gameloop)
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
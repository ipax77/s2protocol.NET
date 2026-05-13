namespace s2protocol.NET.Models;
/// <summary>Record <c>SBankKeyEvent</c> SBankKeyEvent</summary>
///
/// <remarks>Record <c>SBankKeyEvent</c> constructor</remarks>
///
public sealed class SBankKeyEvent(int userId,
    int eventId,
    int bits,
    int gameloop,
    string name,
    string data,
    int type) : GameEvent(userId, eventId, GameEventType.SBankKeyEvent, bits, gameloop)
{

    /// <summary>Event Type</summary>
    ///
    public string Name { get; } = name;
    /// <summary>Event Data</summary>
    ///
    public string Data { get; } = data;
    /// <summary>Event Type</summary>
    ///
    public int Type { get; } = type;
}
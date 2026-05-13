namespace s2protocol.NET.Models;
/// <summary>Record <c>SBankValueEvent</c> SBankValueEvent</summary>
///
/// <remarks>Record <c>SBankValueEvent</c> constructor</remarks>
///
public sealed class SBankValueEvent(int userId,
    int eventId,
    int bits,
    int gameloop,
    string name,
    string data,
    int type) : GameEvent(userId, eventId, GameEventType.SBankValueEvent, bits, gameloop)
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
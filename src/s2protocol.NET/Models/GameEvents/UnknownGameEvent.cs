namespace s2protocol.NET.Models;
/// <summary>Record <c>UnknownGameEvent</c> UnknownGameEvent</summary>
///
/// <remarks>Record <c>UnknownGameEvent</c> constructor</remarks>
///
public sealed class UnknownGameEvent(int userId,
    int eventId,
    int bits,
    int gameloop,
    string name) : GameEvent(userId, eventId, GameEventType.None, bits, gameloop)
{

    /// <summary>EventTypeName</summary>
    ///
    public string EventTypeName { get; } = name;
}
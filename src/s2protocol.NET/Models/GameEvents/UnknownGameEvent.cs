namespace s2protocol.NET.Models;
/// <summary>Record <c>UnknownGameEvent</c> UnknownGameEvent</summary>
///
public sealed class UnknownGameEvent : GameEvent
{
    /// <summary>Record <c>UnknownGameEvent</c> constructor</summary>
    ///
    public UnknownGameEvent(int userId,
        int eventId,
        int bits,
        int gameloop,
        string name) : base(userId, eventId, GameEventType.None, bits, gameloop)
    {
        EventTypeName = name;
    }

    /// <summary>EventTypeName</summary>
    ///
    public string EventTypeName { get; }
}
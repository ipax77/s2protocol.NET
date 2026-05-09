namespace s2protocol.NET.Models;
/// <summary>Record <c>SDecrementGameTimeRemainingEvent</c> SDecrementGameTimeRemainingEvent</summary>
///
public sealed class SDecrementGameTimeRemainingEvent : GameEvent
{
    /// <summary>Record <c>SDecrementGameTimeRemainingEvent</c> constructor</summary>
    ///
    public SDecrementGameTimeRemainingEvent(int userId,
        int eventId,
        int bits,
        int gameloop,
        int decrementSeconds) : base(userId, eventId, GameEventType.SDecrementGameTimeRemainingEvent, bits, gameloop)
    {
        DecrementSeconds = decrementSeconds;
    }

    /// <summary>Event DecrementSeconds</summary>
    ///
    public int DecrementSeconds { get; }
}
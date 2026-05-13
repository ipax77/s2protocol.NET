namespace s2protocol.NET.Models;
/// <summary>Record <c>SDecrementGameTimeRemainingEvent</c> SDecrementGameTimeRemainingEvent</summary>
///
/// <remarks>Record <c>SDecrementGameTimeRemainingEvent</c> constructor</remarks>
///
public sealed class SDecrementGameTimeRemainingEvent(int userId,
    int eventId,
    int bits,
    int gameloop,
    int decrementSeconds) : GameEvent(userId, eventId, GameEventType.SDecrementGameTimeRemainingEvent, bits, gameloop)
{

    /// <summary>Event DecrementSeconds</summary>
    ///
    public int DecrementSeconds { get; } = decrementSeconds;
}
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
        int decrementSeconds)
        : this(userId, eventId, bits, gameloop, decrementSeconds, null)
    {
    }

    /// <summary>Creates an event while retaining the original seconds or milliseconds wire field.</summary>
    [System.Text.Json.Serialization.JsonConstructor]
    public SDecrementGameTimeRemainingEvent(
        int userId,
        int eventId,
        int bits,
        int gameloop,
        int decrementSeconds,
        long? decrementMilliseconds) : base(userId, eventId, GameEventType.SDecrementGameTimeRemainingEvent, bits, gameloop)
    {
        DecrementSeconds = decrementSeconds;
        DecrementMilliseconds = decrementMilliseconds;
    }

    /// <summary>Event DecrementSeconds</summary>
    ///
    public int DecrementSeconds { get; }

    /// <summary>The decrement in milliseconds for protocol versions that use <c>m_decrementMs</c>.</summary>
    public long? DecrementMilliseconds { get; }
}

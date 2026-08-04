namespace s2protocol.NET.Models;
/// <summary>Record <c>PingMessageEvent</c> Parsed replay ping messages</summary>
///
/// <remarks>Record <c>PingMessageEvent</c> constructor</remarks>
///
public sealed class PingMessageEvent : MessageEvent
{
    /// <summary>Creates a parsed replay ping message.</summary>
    public PingMessageEvent(int recipient, int userId, int gameloop, long x, long y)
        : this(recipient, userId, eventId: 1, bits: 0, gameloop, x, y)
    {
    }

    internal PingMessageEvent(int recipient, int userId, int eventId, int bits, int gameloop, long x, long y)
        : base(userId, eventId, MessageEventType.PingMessageEvent, bits, gameloop)
    {
        Recipient = recipient;
        X = x;
        Y = y;
    }

    /// <summary>Message Recipient</summary>
    ///
    public int Recipient { get; init; }
    /// <summary>
    /// X coordinate
    /// </summary>
    public long X { get; init; }
    /// <summary>
    /// Y coordinate
    /// </summary>
    public long Y { get; init; }
}

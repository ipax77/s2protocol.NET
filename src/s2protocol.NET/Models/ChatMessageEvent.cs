namespace s2protocol.NET.Models;
/// <summary>Record <c>ChatMessageEvent</c> Parsed replay chat messages</summary>
///
/// <remarks>Record <c>ChatMessageEvent</c> constructor</remarks>
///
public sealed class ChatMessageEvent : MessageEvent
{
    /// <summary>Creates a parsed replay chat message.</summary>
    public ChatMessageEvent(int recipient, int userId, string message, int gameloop)
        : this(recipient, userId, message, eventId: 0, bits: 0, gameloop)
    {
    }

    internal ChatMessageEvent(int recipient, int userId, string message, int eventId, int bits, int gameloop)
        : base(userId, eventId, MessageEventType.ChatMessageEvent, bits, gameloop)
    {
        Recipient = recipient;
        Message = message;
    }

    /// <summary>Message Recipient</summary>
    ///
    public int Recipient { get; init; }
    /// <summary>Message Message</summary>
    ///
    public string Message { get; init; }
}

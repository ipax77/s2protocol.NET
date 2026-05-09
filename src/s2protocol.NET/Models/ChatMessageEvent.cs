namespace s2protocol.NET.Models;
/// <summary>Record <c>ChatMessageEvent</c> Parsed replay chat messages</summary>
///
/// <remarks>Record <c>ChatMessageEvent</c> constructor</remarks>
///
public sealed class ChatMessageEvent(int recipient, int userId, string message, int gameloop)
{
    /// <summary>Message Recipient</summary>
    ///
    public int Recipient { get; init; } = recipient;
    /// <summary>Message UserId</summary>
    ///
    public int UserId { get; init; } = userId;
    /// <summary>Message Message</summary>
    ///
    public string Message { get; init; } = message;
    /// <summary>Message Gameloop</summary>
    ///
    public int Gameloop { get; init; } = gameloop;
}

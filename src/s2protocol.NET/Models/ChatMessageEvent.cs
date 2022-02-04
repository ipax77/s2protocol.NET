using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>ChatMessageEvent</c> Parsed replay chat messages</summary>
///
public sealed record ChatMessageEvent
{
    /// <summary>Record <c>ChatMessageEvent</c> constructor</summary>
    ///
    public ChatMessageEvent(int recipient, int userId, string message, int gameloop)
    {
        Recipient = recipient;
        UserId = userId;
        Message = message;
        Gameloop = gameloop;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public ChatMessageEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Message Recipient</summary>
    ///
    public int Recipient { get; init; }
    /// <summary>Message UserId</summary>
    ///
    public int UserId { get; init; }
    /// <summary>Message Message</summary>
    ///
    public string Message { get; init; }
    /// <summary>Message Gameloop</summary>
    ///
    public int Gameloop { get; init; }
}

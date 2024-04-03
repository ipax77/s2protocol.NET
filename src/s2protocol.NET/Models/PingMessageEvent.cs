using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>PingMessageEvent</c> Parsed replay ping messages</summary>
///
public sealed record PingMessageEvent
{
    /// <summary>Record <c>PingMessageEvent</c> constructor</summary>
    ///
    public PingMessageEvent(int recipient, int userId, int gameloop, long x, long y)
    {
        Recipient = recipient;
        UserId = userId;
        Gameloop = gameloop;
        X = x;
        Y = y;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public PingMessageEvent()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Message Recipient</summary>
    ///
    public int Recipient { get; init; }
    /// <summary>Message UserId</summary>
    ///
    public int UserId { get; init; }
    /// <summary>
    /// X coordinate
    /// </summary>
    public long X { get; init; }
    /// <summary>
    /// Y coordinate
    /// </summary>
    public long Y { get; init; }
    /// <summary>Message Gameloop</summary>
    ///
    public int Gameloop { get; init; }
}

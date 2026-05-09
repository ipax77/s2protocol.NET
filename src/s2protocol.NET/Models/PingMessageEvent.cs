namespace s2protocol.NET.Models;
/// <summary>Record <c>PingMessageEvent</c> Parsed replay ping messages</summary>
///
/// <remarks>Record <c>PingMessageEvent</c> constructor</remarks>
///
public sealed class PingMessageEvent(int recipient, int userId, int gameloop, long x, long y)
{
    /// <summary>Message Recipient</summary>
    ///
    public int Recipient { get; init; } = recipient;
    /// <summary>Message UserId</summary>
    ///
    public int UserId { get; init; } = userId;
    /// <summary>
    /// X coordinate
    /// </summary>
    public long X { get; init; } = x;
    /// <summary>
    /// Y coordinate
    /// </summary>
    public long Y { get; init; } = y;
    /// <summary>Message Gameloop</summary>
    ///
    public int Gameloop { get; init; } = gameloop;
}

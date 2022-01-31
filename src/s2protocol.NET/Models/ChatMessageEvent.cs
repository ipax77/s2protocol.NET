using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

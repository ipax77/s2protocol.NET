namespace s2protocol.NET.Models;

/// <summary>Decoded replay message events.</summary>
public sealed class MessageEvents(
    ICollection<ChatMessageEvent> chatMessages,
    ICollection<PingMessageEvent> pingMessages)
{
    /// <summary>Replay chat messages.</summary>
    public ICollection<ChatMessageEvent> ChatMessages { get; init; } = chatMessages;

    /// <summary>Replay ping messages.</summary>
    public ICollection<PingMessageEvent> PingMessages { get; init; } = pingMessages;
}

using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;

/// <summary>Base class for replay message events.</summary>
[JsonDerivedType(typeof(ChatMessageEvent), nameof(ChatMessageEvent))]
[JsonDerivedType(typeof(PingMessageEvent), nameof(PingMessageEvent))]
[JsonDerivedType(typeof(LoadingProgressMessageEvent), nameof(LoadingProgressMessageEvent))]
[JsonDerivedType(typeof(ServerPingMessageEvent), nameof(ServerPingMessageEvent))]
[JsonDerivedType(typeof(ReconnectNotifyMessageEvent), nameof(ReconnectNotifyMessageEvent))]
[JsonDerivedType(typeof(UnknownMessageEvent), nameof(UnknownMessageEvent))]
public abstract class MessageEvent
{
    /// <summary>Creates a replay message event.</summary>
    protected MessageEvent(
        int userId,
        int eventId,
        MessageEventType eventType,
        int bits,
        int gameloop)
    {
        UserId = userId;
        EventId = eventId;
        EventType = eventType;
        Bits = bits;
        Gameloop = gameloop;
    }

    /// <summary>User that emitted the event.</summary>
    public int UserId { get; init; }

    /// <summary>Protocol-specific message event identifier.</summary>
    public int EventId { get; }

    /// <summary>Strongly typed message event identifier.</summary>
    public MessageEventType EventType { get; }

    /// <summary>Number of encoded bits occupied by the event.</summary>
    public int Bits { get; internal set; }

    /// <summary>Absolute game loop at which the event occurred.</summary>
    public int Gameloop { get; init; }
}

/// <summary>Known replay message event types.</summary>
public enum MessageEventType
{
    /// <summary>An event name not known by this library.</summary>
    None = 0,

    /// <summary>A chat message.</summary>
    ChatMessageEvent = 1,

    /// <summary>A player ping.</summary>
    PingMessageEvent = 2,

    /// <summary>A client loading-progress update.</summary>
    LoadingProgressMessageEvent = 3,

    /// <summary>A server ping.</summary>
    ServerPingMessageEvent = 4,

    /// <summary>A reconnect status notification.</summary>
    ReconnectNotifyMessageEvent = 5,
}

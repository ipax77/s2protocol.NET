namespace s2protocol.NET.Models;

/// <summary>A client loading-progress update.</summary>
public sealed class LoadingProgressMessageEvent(
    int progress,
    int userId,
    int eventId,
    int bits,
    int gameloop)
    : MessageEvent(userId, eventId, MessageEventType.LoadingProgressMessageEvent, bits, gameloop)
{
    /// <summary>Reported loading progress.</summary>
    public int Progress { get; init; } = progress;
}

/// <summary>A payload-free server ping.</summary>
public sealed class ServerPingMessageEvent(
    int userId,
    int eventId,
    int bits,
    int gameloop)
    : MessageEvent(userId, eventId, MessageEventType.ServerPingMessageEvent, bits, gameloop);

/// <summary>A reconnect status notification.</summary>
public sealed class ReconnectNotifyMessageEvent(
    int status,
    int userId,
    int eventId,
    int bits,
    int gameloop)
    : MessageEvent(userId, eventId, MessageEventType.ReconnectNotifyMessageEvent, bits, gameloop)
{
    /// <summary>Protocol reconnect status value.</summary>
    public int Status { get; init; } = status;
}

/// <summary>A message event whose schema name is not known by this library.</summary>
public sealed class UnknownMessageEvent(
    int userId,
    int eventId,
    int bits,
    int gameloop,
    string eventTypeName)
    : MessageEvent(userId, eventId, MessageEventType.None, bits, gameloop)
{
    /// <summary>Full Blizzard schema name.</summary>
    public string EventTypeName { get; } = eventTypeName;
}

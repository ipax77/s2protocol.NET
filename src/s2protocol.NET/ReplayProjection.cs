using s2protocol.NET.Models;

namespace s2protocol.NET;

public sealed record ReplayProjectionProfile
{
    public bool Details { get; init; }
    public bool Initdata { get; init; }
    public bool Metadata { get; init; }
    public bool AttributeEvents { get; init; }
    public MessageEventSelection MessageEvents { get; init; } = MessageEventSelection.Disabled;
    public ReplayEventSelection<TrackerEventType> TrackerEvents { get; init; } = ReplayEventSelection<TrackerEventType>.Disabled;
    public ReplayEventSelection<GameEventType> GameEvents { get; init; } = ReplayEventSelection<GameEventType>.Disabled;
}

public sealed record MessageEventSelection(bool ChatMessages, bool PingMessages)
{
    public static MessageEventSelection Disabled { get; } = new(false, false);
    public static MessageEventSelection All { get; } = new(true, true);
    public static MessageEventSelection ChatOnly { get; } = new(true, false);
    public static MessageEventSelection PingOnly { get; } = new(false, true);
    public bool Enabled => ChatMessages || PingMessages;
}

#pragma warning disable CA1000 // Static factories keep the selection API concise at call sites.
public sealed record ReplayEventSelection<T>
    where T : struct, Enum
{
    private readonly T[] eventTypes;

    private ReplayEventSelection(bool includeAll, T[] eventTypes)
    {
        IncludeAll = includeAll;
        this.eventTypes = eventTypes;
    }

    public static ReplayEventSelection<T> Disabled { get; } = new(false, []);
    public static ReplayEventSelection<T> All { get; } = new(true, []);

    public bool IncludeAll { get; }
    public IReadOnlyCollection<T> EventTypes => eventTypes;
    public bool Enabled => IncludeAll || eventTypes.Length > 0;

    public static ReplayEventSelection<T> Only(params T[] eventTypes)
    {
        ArgumentNullException.ThrowIfNull(eventTypes);
        return eventTypes.Length == 0 ? Disabled : new(false, [.. eventTypes]);
    }

    public bool Includes(T eventType)
    {
        return IncludeAll || Array.IndexOf(eventTypes, eventType) >= 0;
    }
}
#pragma warning restore CA1000

public interface IReplayProjection<TResult>
{
    ReplayProjectionProfile Profile { get; }

    void OnHeader(Header header, string replayPath);
    void OnDetails(Details details);
    void OnInitdata(Initdata initdata);
    void OnMetadata(ReplayMetadata metadata);
    void OnAttributeEvents(AttributeEvents attributeEvents);
    void OnChatMessage(ChatMessageEvent chatMessage);
    void OnPingMessage(PingMessageEvent pingMessage);
    void OnTrackerEvent(TrackerEvent trackerEvent);
    void OnGameEvent(GameEvent gameEvent);
    TResult Complete();
}

using System.Collections;

namespace s2protocol.NET.Models;

/// <summary>Decoded replay message events in stream order.</summary>
public sealed class MessageEvents
{
    private readonly MessageEventStore _events;
    private ICollection<ChatMessageEvent>? _chatMessages;
    private ICollection<PingMessageEvent>? _pingMessages;

    /// <summary>Creates message events from the legacy categorized collections.</summary>
    /// <remarks>The collections are merged with chat messages followed by ping messages.</remarks>
    public MessageEvents(
        ICollection<ChatMessageEvent> chatMessages,
        ICollection<PingMessageEvent> pingMessages)
    {
        ArgumentNullException.ThrowIfNull(chatMessages);
        ArgumentNullException.ThrowIfNull(pingMessages);

        _events = new MessageEventStore(chatMessages.Count + pingMessages.Count);
        _events.AddRange(chatMessages);
        _events.AddRange(pingMessages);
    }

    internal MessageEvents(MessageEventStore events)
    {
        _events = events;
    }

    /// <summary>All message events in encoded stream order.</summary>
    public ICollection<MessageEvent> BaseMessageEvents => _events;

    /// <summary>Replay chat messages.</summary>
    public ICollection<ChatMessageEvent> ChatMessages
    {
        get => _chatMessages ??= _events.CreateView<ChatMessageEvent>(MessageEventType.ChatMessageEvent);
        init
        {
            ArgumentNullException.ThrowIfNull(value);
            _events.Replace(MessageEventType.ChatMessageEvent, value);
            _chatMessages = null;
        }
    }

    /// <summary>Replay ping messages.</summary>
    public ICollection<PingMessageEvent> PingMessages
    {
        get => _pingMessages ??= _events.CreateView<PingMessageEvent>(MessageEventType.PingMessageEvent);
        init
        {
            ArgumentNullException.ThrowIfNull(value);
            _events.Replace(MessageEventType.PingMessageEvent, value);
            _pingMessages = null;
        }
    }
}

internal sealed class MessageEventStore(int capacity = 0) : ICollection<MessageEvent>
{
    private readonly List<MessageEvent> _items = new(capacity);
    private int _chatCount;
    private int _pingCount;
    private int _loadingProgressCount;
    private int _serverPingCount;
    private int _reconnectNotifyCount;

    public int Count => _items.Count;

    public bool IsReadOnly => false;

    internal MessageEvent Last => _items[^1];

    public void Add(MessageEvent item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _items.Add(item);
        ChangeCount(item.EventType, 1);
    }

    internal void AddRange<T>(IEnumerable<T> items)
        where T : MessageEvent
    {
        foreach (T item in items)
        {
            Add(item);
        }
    }

    public void Clear()
    {
        _items.Clear();
        _chatCount = 0;
        _pingCount = 0;
        _loadingProgressCount = 0;
        _serverPingCount = 0;
        _reconnectNotifyCount = 0;
    }

    public bool Contains(MessageEvent item) => _items.Contains(item);

    public void CopyTo(MessageEvent[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

    public bool Remove(MessageEvent item)
    {
        if (!_items.Remove(item))
        {
            return false;
        }

        ChangeCount(item.EventType, -1);
        return true;
    }

    public List<MessageEvent>.Enumerator GetEnumerator() => _items.GetEnumerator();

    IEnumerator<MessageEvent> IEnumerable<MessageEvent>.GetEnumerator() => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

    internal ICollection<T> CreateView<T>(MessageEventType eventType)
        where T : MessageEvent
        => new MessageEventView<T>(this, eventType);

    internal int CountOf(MessageEventType eventType) => eventType switch
    {
        MessageEventType.ChatMessageEvent => _chatCount,
        MessageEventType.PingMessageEvent => _pingCount,
        MessageEventType.LoadingProgressMessageEvent => _loadingProgressCount,
        MessageEventType.ServerPingMessageEvent => _serverPingCount,
        MessageEventType.ReconnectNotifyMessageEvent => _reconnectNotifyCount,
        _ => _items.Count - _chatCount - _pingCount - _loadingProgressCount - _serverPingCount - _reconnectNotifyCount,
    };

    internal void Clear(MessageEventType eventType)
    {
        int destination = 0;
        for (int source = 0; source < _items.Count; source++)
        {
            MessageEvent item = _items[source];
            if (item.EventType != eventType)
            {
                _items[destination++] = item;
            }
        }

        if (destination < _items.Count)
        {
            _items.RemoveRange(destination, _items.Count - destination);
        }

        SetCount(eventType, 0);
    }

    internal void CopyTo<T>(MessageEventType eventType, T[] array, int arrayIndex)
        where T : MessageEvent
    {
        ArgumentNullException.ThrowIfNull(array);
        if (arrayIndex < 0 || arrayIndex > array.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        }

        int count = CountOf(eventType);
        if (array.Length - arrayIndex < count)
        {
            throw new ArgumentException("The destination array is too small.", nameof(array));
        }

        foreach (MessageEvent item in _items)
        {
            if (item.EventType == eventType)
            {
                array[arrayIndex++] = (T)item;
            }
        }
    }

    internal IEnumerable<T> Enumerate<T>(MessageEventType eventType)
        where T : MessageEvent
    {
        foreach (MessageEvent item in _items)
        {
            if (item.EventType == eventType)
            {
                yield return (T)item;
            }
        }
    }

    internal void Replace<T>(MessageEventType eventType, IEnumerable<T> replacements)
        where T : MessageEvent
    {
        T[] snapshot = [.. replacements];
        Clear(eventType);
        AddRange(snapshot);
    }

    private void ChangeCount(MessageEventType eventType, int delta)
    {
        switch (eventType)
        {
            case MessageEventType.ChatMessageEvent: _chatCount += delta; break;
            case MessageEventType.PingMessageEvent: _pingCount += delta; break;
            case MessageEventType.LoadingProgressMessageEvent: _loadingProgressCount += delta; break;
            case MessageEventType.ServerPingMessageEvent: _serverPingCount += delta; break;
            case MessageEventType.ReconnectNotifyMessageEvent: _reconnectNotifyCount += delta; break;
        }
    }

    private void SetCount(MessageEventType eventType, int value)
    {
        switch (eventType)
        {
            case MessageEventType.ChatMessageEvent: _chatCount = value; break;
            case MessageEventType.PingMessageEvent: _pingCount = value; break;
            case MessageEventType.LoadingProgressMessageEvent: _loadingProgressCount = value; break;
            case MessageEventType.ServerPingMessageEvent: _serverPingCount = value; break;
            case MessageEventType.ReconnectNotifyMessageEvent: _reconnectNotifyCount = value; break;
        }
    }
}

internal sealed class MessageEventView<T>(MessageEventStore events, MessageEventType eventType) : ICollection<T>
    where T : MessageEvent
{
    public int Count => events.CountOf(eventType);

    public bool IsReadOnly => false;

    public void Add(T item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (item.EventType != eventType)
        {
            throw new ArgumentException("The event does not match this collection view.", nameof(item));
        }

        events.Add(item);
    }

    public void Clear() => events.Clear(eventType);

    public bool Contains(T item) => item is not null && item.EventType == eventType && events.Contains(item);

    public void CopyTo(T[] array, int arrayIndex) => events.CopyTo(eventType, array, arrayIndex);

    public bool Remove(T item) => item is not null && item.EventType == eventType && events.Remove(item);

    public IEnumerator<T> GetEnumerator() => events.Enumerate<T>(eventType).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

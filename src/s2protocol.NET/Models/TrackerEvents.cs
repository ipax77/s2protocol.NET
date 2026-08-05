using System.Collections;
using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;

/// <summary>Contains strongly typed tracker events in their encoded stream order.</summary>
public sealed class TrackerEvents
{
    private readonly TrackerEventStore _events;
    private ICollection<SPlayerSetupEvent>? _playerSetupEvents;
    private ICollection<SPlayerStatsEvent>? _playerStatsEvents;
    private ICollection<SUnitBornEvent>? _unitBornEvents;
    private ICollection<SUnitDiedEvent>? _unitDiedEvents;
    private ICollection<SUnitOwnerChangeEvent>? _unitOwnerChangeEvents;
    private ICollection<SUnitPositionsEvent>? _unitPositionsEvents;
    private ICollection<SUnitTypeChangeEvent>? _unitTypeChangeEvents;
    private ICollection<SUpgradeEvent>? _upgradeEvents;
    private ICollection<SUnitInitEvent>? _unitInitEvents;
    private ICollection<SUnitDoneEvent>? _unitDoneEvents;

    /// <summary>Creates tracker events from one ordered collection.</summary>
    [JsonConstructor]
    public TrackerEvents(ICollection<TrackerEvent> baseTrackerEvents)
    {
        ArgumentNullException.ThrowIfNull(baseTrackerEvents);
        _events = new TrackerEventStore(baseTrackerEvents.Count);
        _events.AddRange(baseTrackerEvents);
    }

    /// <summary>Creates tracker events from categorized collections.</summary>
    /// <remarks>Events are merged in parameter order because the original stream order is unavailable.</remarks>
    public TrackerEvents(
        SPlayerSetupEvent[] sPlayerSetupEvents,
        SPlayerStatsEvent[] sPlayerStatsEvents,
        SUnitBornEvent[] sUnitBornEvents,
        SUnitDiedEvent[] sUnitDiedEvents,
        SUnitOwnerChangeEvent[] sUnitOwnerChangeEvents,
        SUnitPositionsEvent[] sUnitPositionsEvents,
        SUnitTypeChangeEvent[] sUnitTypeChangeEvents,
        SUpgradeEvent[] sUpgradeEvents,
        SUnitInitEvent[] sUnitInitEvents,
        SUnitDoneEvent[] sUnitDoneEvents)
        : this((ICollection<TrackerEvent>)CreateMergedEvents(
            sPlayerSetupEvents,
            sPlayerStatsEvents,
            sUnitBornEvents,
            sUnitDiedEvents,
            sUnitOwnerChangeEvents,
            sUnitPositionsEvents,
            sUnitTypeChangeEvents,
            sUpgradeEvents,
            sUnitInitEvents,
            sUnitDoneEvents))
    {
    }

    internal TrackerEvents(TrackerEventStore events)
    {
        _events = events;
    }

    /// <summary>All tracker events in encoded stream order.</summary>
    public ICollection<TrackerEvent> BaseTrackerEvents => _events;

    /// <summary>Player setup events.</summary>
    [JsonIgnore]
    public ICollection<SPlayerSetupEvent> SPlayerSetupEvents =>
        _playerSetupEvents ??= _events.CreateView<SPlayerSetupEvent>(TrackerEventType.SPlayerSetupEvent);

    /// <summary>Player statistics events.</summary>
    [JsonIgnore]
    public ICollection<SPlayerStatsEvent> SPlayerStatsEvents =>
        _playerStatsEvents ??= _events.CreateView<SPlayerStatsEvent>(TrackerEventType.SPlayerStatsEvent);

    /// <summary>Events for units created fully constructed.</summary>
    [JsonIgnore]
    public ICollection<SUnitBornEvent> SUnitBornEvents =>
        _unitBornEvents ??= _events.CreateView<SUnitBornEvent>(TrackerEventType.SUnitBornEvent);

    /// <summary>Unit death events.</summary>
    [JsonIgnore]
    public ICollection<SUnitDiedEvent> SUnitDiedEvents =>
        _unitDiedEvents ??= _events.CreateView<SUnitDiedEvent>(TrackerEventType.SUnitDiedEvent);

    /// <summary>Unit owner-change events.</summary>
    [JsonIgnore]
    public ICollection<SUnitOwnerChangeEvent> SUnitOwnerChangeEvents =>
        _unitOwnerChangeEvents ??= _events.CreateView<SUnitOwnerChangeEvent>(TrackerEventType.SUnitOwnerChangeEvent);

    /// <summary>Unit position events.</summary>
    [JsonIgnore]
    public ICollection<SUnitPositionsEvent> SUnitPositionsEvents =>
        _unitPositionsEvents ??= _events.CreateView<SUnitPositionsEvent>(TrackerEventType.SUnitPositionsEvent);

    /// <summary>Unit type-change events.</summary>
    [JsonIgnore]
    public ICollection<SUnitTypeChangeEvent> SUnitTypeChangeEvents =>
        _unitTypeChangeEvents ??= _events.CreateView<SUnitTypeChangeEvent>(TrackerEventType.SUnitTypeChangeEvent);

    /// <summary>Upgrade events.</summary>
    [JsonIgnore]
    public ICollection<SUpgradeEvent> SUpgradeEvents =>
        _upgradeEvents ??= _events.CreateView<SUpgradeEvent>(TrackerEventType.SUpgradeEvent);

    /// <summary>Events for units beginning construction.</summary>
    [JsonIgnore]
    public ICollection<SUnitInitEvent> SUnitInitEvents =>
        _unitInitEvents ??= _events.CreateView<SUnitInitEvent>(TrackerEventType.SUnitInitEvent);

    /// <summary>Unit construction completion events.</summary>
    [JsonIgnore]
    public ICollection<SUnitDoneEvent> SUnitDoneEvents =>
        _unitDoneEvents ??= _events.CreateView<SUnitDoneEvent>(TrackerEventType.SUnitDoneEvent);

    private static List<TrackerEvent> CreateMergedEvents(params IEnumerable<TrackerEvent>[] collections)
    {
        List<TrackerEvent> events = [];
        foreach (IEnumerable<TrackerEvent> collection in collections)
        {
            ArgumentNullException.ThrowIfNull(collection);
            events.AddRange(collection);
        }

        return events;
    }
}

internal sealed class TrackerEventStore(int capacity = 0) : ICollection<TrackerEvent>
{
    private const int EventTypeCount = (int)TrackerEventType.SUnitDoneEvent + 1;
    private readonly List<TrackerEvent> _items = new(capacity);
    private readonly int[] _counts = new int[EventTypeCount];

    public int Count => _items.Count;

    public bool IsReadOnly => false;

    public void Add(TrackerEvent item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _items.Add(item);
        ChangeCount(item.EventType, 1);
    }

    internal void AddRange(IEnumerable<TrackerEvent> items)
    {
        foreach (TrackerEvent item in items)
        {
            Add(item);
        }
    }

    public void Clear()
    {
        _items.Clear();
        Array.Clear(_counts);
    }

    public bool Contains(TrackerEvent item) => _items.Contains(item);

    public void CopyTo(TrackerEvent[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

    public bool Remove(TrackerEvent item)
    {
        if (!_items.Remove(item))
        {
            return false;
        }

        ChangeCount(item.EventType, -1);
        return true;
    }

    public List<TrackerEvent>.Enumerator GetEnumerator() => _items.GetEnumerator();

    IEnumerator<TrackerEvent> IEnumerable<TrackerEvent>.GetEnumerator() => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

    internal ICollection<T> CreateView<T>(TrackerEventType eventType)
        where T : TrackerEvent
        => new TrackerEventView<T>(this, eventType);

    internal int CountOf(TrackerEventType eventType)
        => IsKnown(eventType) ? _counts[(int)eventType] : 0;

    internal IEnumerable<T> Enumerate<T>(TrackerEventType eventType)
        where T : TrackerEvent
    {
        foreach (TrackerEvent item in _items)
        {
            if (item.EventType == eventType)
            {
                yield return (T)item;
            }
        }
    }

    internal void Clear(TrackerEventType eventType)
    {
        int destination = 0;
        for (int source = 0; source < _items.Count; source++)
        {
            TrackerEvent item = _items[source];
            if (item.EventType != eventType)
            {
                _items[destination++] = item;
            }
        }

        if (destination < _items.Count)
        {
            _items.RemoveRange(destination, _items.Count - destination);
        }

        if (IsKnown(eventType))
        {
            _counts[(int)eventType] = 0;
        }
    }

    internal void CopyTo<T>(TrackerEventType eventType, T[] array, int arrayIndex)
        where T : TrackerEvent
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

        foreach (TrackerEvent item in _items)
        {
            if (item.EventType == eventType)
            {
                array[arrayIndex++] = (T)item;
            }
        }
    }

    private void ChangeCount(TrackerEventType eventType, int delta)
    {
        if (IsKnown(eventType))
        {
            _counts[(int)eventType] += delta;
        }
    }

    private static bool IsKnown(TrackerEventType eventType)
        => eventType > TrackerEventType.None && (int)eventType < EventTypeCount;
}

internal sealed class TrackerEventView<T>(TrackerEventStore events, TrackerEventType eventType) : ICollection<T>
    where T : TrackerEvent
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

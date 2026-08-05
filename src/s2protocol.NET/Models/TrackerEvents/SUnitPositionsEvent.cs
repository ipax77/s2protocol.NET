namespace s2protocol.NET.Models;
/// <summary>Record <c>SUnitPositionsEvent</c> SUnitPositionsEvent</summary>
///
public sealed class SUnitPositionsEvent : TrackerEvent
{
    [System.Text.Json.Serialization.JsonConstructor]
    public SUnitPositionsEvent(
        int playerId,
        int eventId,
        int bits,
        int gameloop,
        int firstUnitIndex,
        ICollection<UnitPosition> unitPositions,
        int unitIndex,
        int x,
        int y) : base(playerId, eventId, TrackerEventType.SUnitPositionsEvent, bits, gameloop)
    {
        ArgumentNullException.ThrowIfNull(unitPositions);
        FirstUnitIndex = firstUnitIndex;
        UnitPositions = unitPositions;
        UnitIndex = unitIndex;
        X = x;
        Y = y;
    }

    /// <summary>Record <c>SUnitPositionsEvent</c> constructor</summary>
    /// <comment>Only units that have inflicted or taken damage are mentioned in unit position events, and they occur periodically with a limit of 256 units mentioned per event.</comment>
    /// 
    public SUnitPositionsEvent(int playerId,
                               int eventId,
                               int bits,
                               int gameloop,
                               int firstUnitIndex,
                               int[] items)
        : this(playerId, eventId, bits, gameloop, firstUnitIndex, (IReadOnlyList<int>)items)
    {
    }

    internal SUnitPositionsEvent(int playerId,
                                 int eventId,
                                 int bits,
                                 int gameloop,
                                 int firstUnitIndex,
                                 IReadOnlyList<int> items) : base(playerId, eventId, TrackerEventType.SUnitPositionsEvent, bits, gameloop)
    {
        FirstUnitIndex = firstUnitIndex;

        if (items is null || items.Count < 3 || items.Count % 3 != 0)
        {
            UnitPositions = [];
            UnitIndex = firstUnitIndex;
            X = 0;
            Y = 0;
            return;
        }

        var positions = new UnitPosition[items.Count / 3];

        var unitIndex = firstUnitIndex;
        var lastX = 0;
        var lastY = 0;

        for (var i = 0; i < items.Count; i += 3)
        {
            unitIndex += items[i];

            lastX = items[i + 1] * 4;
            lastY = items[i + 2] * 4;

            positions[i / 3] = new UnitPosition(
                unitIndex,
                lastX,
                lastY);
        }

        UnitPositions = positions;
        UnitIndex = unitIndex;
        X = lastX;
        Y = lastY;
    }

    /// <summary>Event firstUnitIndex</summary>
    ///
    public int FirstUnitIndex { get; init; }
    /// <summary>Event Items</summary>
    ///
    public ICollection<UnitPosition> UnitPositions { get; init; }
    /// <summary>Event UnitIndex</summary>
    ///
    public int UnitIndex { get; init; }
    /// <summary>Event X</summary>
    ///
    public int X { get; init; }
    /// <summary>Event Y</summary>
    ///
    public int Y { get; init; }
}

/// <summary>
/// Position of a unit mentioned by <see cref="SUnitPositionsEvent"/>.
/// </summary>
public sealed class UnitPosition(int unitIndex, int x, int y)
{
    /// <summary>Event UnitIndex</summary>
    ///
    public int UnitIndex { get; } = unitIndex;
    /// <summary>Event X</summary>
    ///
    public int X { get; } = x;
    /// <summary>Event Y</summary>
    ///
    public int Y { get; } = y;
}

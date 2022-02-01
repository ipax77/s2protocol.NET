namespace s2protocol.NET.Models;
/// <summary>Record <c>SUnitPositionsEvent</c> SUnitPositionsEvent</summary>
///
public record SUnitPositionsEvent : TrackerEvent
{
    /// <summary>Record <c>SUnitPositionsEvent</c> constructor</summary>
    /// <comment>Only units that have inflicted or taken damage are mentioned in unit position events, and they occur periodically with a limit of 256 units mentioned per event.</comment>
    /// 
    public SUnitPositionsEvent(
        TrackerEvent trackerEvent,
        int firstUnitIndex,
        int[] items) : base(trackerEvent)
    {
        FirstUnitIndex = firstUnitIndex;
        List<UnitPosition> units = new List<UnitPosition>();
        if (items.Length >= 3 && items.Length % 3 == 0)
        {
            for (int i = 0; i < items.Length; i += 3)
            {
                units.Add(new UnitPosition()
                {
                    UnitIndex = items[i],
                    X = items[i + 1] * 4,
                    Y = items[i + 2] * 4
                });
            }
        }
        UnitPositions = units.ToArray();
    }

    /// <summary>Event firstUnitIndex</summary>
    ///
    public int FirstUnitIndex { get; init; }
    /// <summary>Event Items</summary>
    ///
    public UnitPosition[] UnitPositions { get; init; }
}

/// <summary>Record <c>UnitPosition</c> SUnitPositionsEvent UnitPosition</summary>
///
public record UnitPosition
{
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
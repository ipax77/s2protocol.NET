using System.Text.Json.Serialization;

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
        int unitIndex = FirstUnitIndex;
        List<UnitPosition> units = new List<UnitPosition>();
        if (items != null && items.Length >= 3 && items.Length % 3 == 0)
        {
            for (int i = 0; i < items.Length; i += 3)
            {
                unitIndex += items[i];
                units.Add(new UnitPosition()
                {
                    UnitIndex = items[i],
                    X = items[i + 1] * 4,
                    Y = items[i + 2] * 4
                });
            }
        }
        UnitIndex = unitIndex;
        UnitPositions = units.ToArray();
        if (units.Count != 0)
        {
            X = units.Last().X;
            Y = units.Last().Y;
        }
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SUnitPositionsEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

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

/// <summary>Record <c>UnitPosition</c> SUnitPositionsEvent UnitPosition</summary>
///
public record UnitPosition
{
    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public UnitPosition()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

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
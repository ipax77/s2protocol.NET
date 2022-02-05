using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>SSelectionDeltaEvent</c> SSelectionDeltaEvent</summary>
///
public record SSelectionDeltaEvent : GameEvent
{
    /// <summary>Record <c>SSelectionDeltaEvent</c> constructor</summary>
    ///
    public SSelectionDeltaEvent(
        GameEvent gameEvent,
        SelectionDeltaEventDelta delta,
        int controlGroupId) : base(gameEvent)
    {
        Delta = delta;
        ControlGroupId = controlGroupId;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SSelectionDeltaEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Event Type</summary>
    ///
    public SelectionDeltaEventDelta Delta { get; init; }
    /// <summary>Event ControlGroupId</summary>
    ///
    public int ControlGroupId { get; init; }
}

/// <summary>Record <c>SelectionDeltaEventDelta</c> SelectionDeltaEventDelta</summary>
///
public record SelectionDeltaEventDelta
{
    /// <summary>Record <c>SelectionDeltaEventDelta</c> constructor</summary>
    ///
    public SelectionDeltaEventDelta(ICollection<int> addUnitTags,
                                    ICollection<SelectionDeltaEventDeltaSubGroup> addSubgroups,
                                    ICollection<int> zeroIndices,
                                    int subgroupIndex)
    {
        AddUnitTags = addUnitTags;
        AddSubgroups = addSubgroups;
        ZeroIndices = zeroIndices;
        SubgroupIndex = subgroupIndex;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SelectionDeltaEventDelta()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Event AddUnitTags</summary>
    ///
    public ICollection<int> AddUnitTags { get; init; }
    /// <summary>Event AddSubgroups</summary>
    ///
    public ICollection<SelectionDeltaEventDeltaSubGroup> AddSubgroups { get; init; }
    /// <summary>Event AddSubgroups</summary>
    ///
    public ICollection<int> ZeroIndices { get; init; }
    /// <summary>Event SubgroupIndex</summary>
    ///
    public int SubgroupIndex { get; init; }
}

/// <summary>Record <c>SelectionDeltaEventDeltaSubGroup</c> SelectionDeltaEventDeltaSubGroup</summary>
///
public record SelectionDeltaEventDeltaSubGroup
{
    /// <summary>Record <c>SelectionDeltaEventDeltaSubGroup</c> constructor</summary>
    ///
    public SelectionDeltaEventDeltaSubGroup(int unitLink, int subgroupPriority, int count, int intraSubgroupPriority)
    {
        UnitLink = unitLink;
        SubgroupPriority = subgroupPriority;
        Count = count;
        IntraSubgroupPriority = intraSubgroupPriority;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SelectionDeltaEventDeltaSubGroup()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Event UnitLink</summary>
    ///
    public int UnitLink { get; init; }
    /// <summary>Event SubgroupPriority</summary>
    ///
    public int SubgroupPriority { get; init; }
    /// <summary>Event Count</summary>
    ///
    public int Count { get; init; }
    /// <summary>Event IntraSubgroupPriority</summary>
    ///
    public int IntraSubgroupPriority { get; init; }
}
using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>SCmdEvent</c> SCmdEvent</summary>
///
public record SCmdEvent : GameEvent
{
    /// <summary>Record <c>SCmdEvent</c> constructor</summary>
    ///
    public SCmdEvent(int? unitGroup,
                 int abilLink,
                 int abilCmdIndex,
                 string? abilCmdData,
                 int? targetX,
                 int? targetY,
                 int? targetZ,
                 int cmdFlags,
                 int sequence,
                 int? otherUnit)
    {
        UnitGroup = unitGroup;
        AbilLink = abilLink;
        AbilCmdIndex = abilCmdIndex;
        AbilCmdData = abilCmdData;
        TargetX = targetX;
        TargetY = targetY;
        TargetZ = targetZ;
        CmdFlags = cmdFlags;
        Sequence = sequence;
        OtherUnit = otherUnit;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SCmdEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Event UnitGroup</summary>
    ///
    public int? UnitGroup { get; init; }
    /// <summary>Event AbilLink</summary>
    ///
    public int AbilLink { get; init; }
    /// <summary>Event AbilCmdIndex</summary>
    ///
    public int AbilCmdIndex { get; init; }
    /// <summary>Event AbilCmdData</summary>
    ///
    public string? AbilCmdData { get; init; }
    /// <summary>Event TargetX</summary>
    ///
    public int? TargetX { get; init; }
    /// <summary>Event TargetY</summary>
    ///
    public int? TargetY { get; init; }
    /// <summary>Event TargetZ</summary>
    ///
    public int? TargetZ { get; init; }
    /// <summary>Event CmdFlags</summary>
    ///
    public int CmdFlags { get; init; }
    /// <summary>Event Sequence</summary>
    ///
    public int Sequence { get; init; }
    /// <summary>Event OtherUnit</summary>
    ///
    public int? OtherUnit { get; init; }
}
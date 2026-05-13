namespace s2protocol.NET.Models;
/// <summary>Record <c>SCmdEvent</c> SCmdEvent</summary>
///
/// <remarks>Record <c>SCmdEvent</c> constructor</remarks>
///
public sealed class SCmdEvent(int userId,
    int eventId,
    int bits,
    int gameloop,
    int? unitGroup,
             int abilLink,
             int abilCmdIndex,
             string? abilCmdData,
             long? targetX,
             long? targetY,
             long? targetZ,
             int cmdFlags,
             int sequence,
             int? otherUnit) : GameEvent(userId, eventId, GameEventType.SCmdEvent, bits, gameloop)
{

    /// <summary>Event UnitGroup</summary>
    ///
    public int? UnitGroup { get; } = unitGroup;
    /// <summary>Event AbilLink</summary>
    ///
    public int AbilLink { get; } = abilLink;
    /// <summary>Event AbilCmdIndex</summary>
    ///
    public int AbilCmdIndex { get; } = abilCmdIndex;
    /// <summary>Event AbilCmdData</summary>
    ///
    public string? AbilCmdData { get; } = abilCmdData;
    /// <summary>Event TargetX</summary>
    ///
    public long? TargetX { get; } = targetX;
    /// <summary>Event TargetY</summary>
    ///
    public long? TargetY { get; } = targetY;
    /// <summary>Event TargetZ</summary>
    ///
    public long? TargetZ { get; } = targetZ;
    /// <summary>Event CmdFlags</summary>
    ///
    public int CmdFlags { get; } = cmdFlags;
    /// <summary>Event Sequence</summary>
    ///
    public int Sequence { get; } = sequence;
    /// <summary>Event OtherUnit</summary>
    ///
    public int? OtherUnit { get; } = otherUnit;
}
namespace s2protocol.NET.Models;
/// <summary>Record <c>SCmdEvent</c> SCmdEvent</summary>
///
public sealed class SCmdEvent : GameEvent
{
    /// <summary>Record <c>SCmdEvent</c> constructor</summary>
    ///
    public SCmdEvent(int userId,
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
                 int? otherUnit) : base(userId, eventId, GameEventType.SCmdEvent, bits, gameloop)
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

    /// <summary>Event UnitGroup</summary>
    ///
    public int? UnitGroup { get; }
    /// <summary>Event AbilLink</summary>
    ///
    public int AbilLink { get; }
    /// <summary>Event AbilCmdIndex</summary>
    ///
    public int AbilCmdIndex { get; }
    /// <summary>Event AbilCmdData</summary>
    ///
    public string? AbilCmdData { get; }
    /// <summary>Event TargetX</summary>
    ///
    public long? TargetX { get; }
    /// <summary>Event TargetY</summary>
    ///
    public long? TargetY { get; }
    /// <summary>Event TargetZ</summary>
    ///
    public long? TargetZ { get; }
    /// <summary>Event CmdFlags</summary>
    ///
    public int CmdFlags { get; }
    /// <summary>Event Sequence</summary>
    ///
    public int Sequence { get; }
    /// <summary>Event OtherUnit</summary>
    ///
    public int? OtherUnit { get; }
}
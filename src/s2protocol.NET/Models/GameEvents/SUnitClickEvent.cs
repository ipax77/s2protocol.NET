namespace s2protocol.NET.Models;
/// <summary>Record <c>SUnitClickEvent</c> SUnitClickEvent</summary>
///
/// <remarks>Record <c>SUnitClickEvent</c> constructor</remarks>
///
public sealed class SUnitClickEvent(int userId,
    int eventId,
    int bits,
    int gameloop,
    int unitTag) : GameEvent(userId, eventId, GameEventType.SUnitClickEvent, bits, gameloop)
{

    /// <summary>Event TargetX</summary>
    ///
    public int UnitTag { get; } = unitTag;
}
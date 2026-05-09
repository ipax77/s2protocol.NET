namespace s2protocol.NET.Models;
/// <summary>Record <c>SUnitClickEvent</c> SUnitClickEvent</summary>
///
public sealed class SUnitClickEvent : GameEvent
{
    /// <summary>Record <c>SUnitClickEvent</c> constructor</summary>
    ///
    public SUnitClickEvent(int userId,
        int eventId,
        int bits,
        int gameloop,
        int unitTag) : base(userId, eventId, GameEventType.SUnitClickEvent, bits, gameloop)
    {
        UnitTag = unitTag;
    }

    /// <summary>Event TargetX</summary>
    ///
    public int UnitTag { get; }
}
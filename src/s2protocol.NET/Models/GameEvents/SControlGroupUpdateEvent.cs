namespace s2protocol.NET.Models;
/// <summary>Record <c>SControlGroupUpdateEvent</c> SControlGroupUpdateEvent</summary>
///
public sealed class SControlGroupUpdateEvent : GameEvent
{
    /// <summary>Record <c>SControlGroupUpdateEvent</c> constructor</summary>
    ///
    public SControlGroupUpdateEvent(int userId,
        int eventId,
        int bits,
        int gameloop,
        int controlGroupUpdate) : base(userId, eventId, GameEventType.SControlGroupUpdateEvent, bits, gameloop)
    {
        ControlGroupUpdate = controlGroupUpdate;
    }

    /// <summary>Event ControlGroupUpdate</summary>
    ///
    public int ControlGroupUpdate { get; }
}
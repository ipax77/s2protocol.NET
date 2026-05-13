namespace s2protocol.NET.Models;
/// <summary>Record <c>SControlGroupUpdateEvent</c> SControlGroupUpdateEvent</summary>
///
/// <remarks>Record <c>SControlGroupUpdateEvent</c> constructor</remarks>
///
public sealed class SControlGroupUpdateEvent(int userId,
    int eventId,
    int bits,
    int gameloop,
    int controlGroupUpdate) : GameEvent(userId, eventId, GameEventType.SControlGroupUpdateEvent, bits, gameloop)
{

    /// <summary>Event ControlGroupUpdate</summary>
    ///
    public int ControlGroupUpdate { get; } = controlGroupUpdate;
}
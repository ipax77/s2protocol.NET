namespace s2protocol.NET.Models;
/// <summary>Record <c>SCameraUpdateEvent</c> SCameraUpdateEvent</summary>
///
public sealed class SCameraUpdateEvent : GameEvent
{
    /// <summary>Record <c>SCameraUpdateEvent</c> constructor</summary>
    ///
    public SCameraUpdateEvent(int userId,
        int eventId,
        int bits,
        int gameloop,
        string? reason,
                          int? distance,
                          long? targetX,
                          long? targetY,
                          int? yaw,
                          int? pitch,
                          bool follow) : base(userId, eventId, GameEventType.SCameraUpdateEvent, bits, gameloop)
    {
        Reason = reason;
        Distance = distance;
        TargetX = targetX;
        TargetY = targetY;
        Yaw = yaw;
        Pitch = pitch;
        Follow = follow;
    }

    /// <summary>Event Reason</summary>
    ///
    public string? Reason { get; }
    /// <summary>Event Distance</summary>
    ///
    public int? Distance { get; }
    /// <summary>Event TargetX</summary>
    ///
    public long? TargetX { get; }
    /// <summary>Event TargetY</summary>
    ///
    public long? TargetY { get; }
    /// <summary>Event Yaw</summary>
    ///
    public int? Yaw { get; }
    /// <summary>Event Pitch</summary>
    ///
    public int? Pitch { get; }
    /// <summary>Event Follow</summary>
    ///
    public bool Follow { get; }
}
namespace s2protocol.NET.Models;
/// <summary>Record <c>SCameraUpdateEvent</c> SCameraUpdateEvent</summary>
///
/// <remarks>Record <c>SCameraUpdateEvent</c> constructor</remarks>
///
public sealed class SCameraUpdateEvent(int userId,
    int eventId,
    int bits,
    int gameloop,
    string? reason,
                      int? distance,
                      long? targetX,
                      long? targetY,
                      int? yaw,
                      int? pitch,
                      bool follow) : GameEvent(userId, eventId, GameEventType.SCameraUpdateEvent, bits, gameloop)
{

    /// <summary>Event Reason</summary>
    ///
    public string? Reason { get; } = reason;
    /// <summary>Event Distance</summary>
    ///
    public int? Distance { get; } = distance;
    /// <summary>Event TargetX</summary>
    ///
    public long? TargetX { get; } = targetX;
    /// <summary>Event TargetY</summary>
    ///
    public long? TargetY { get; } = targetY;
    /// <summary>Event Yaw</summary>
    ///
    public int? Yaw { get; } = yaw;
    /// <summary>Event Pitch</summary>
    ///
    public int? Pitch { get; } = pitch;
    /// <summary>Event Follow</summary>
    ///
    public bool Follow { get; } = follow;
}
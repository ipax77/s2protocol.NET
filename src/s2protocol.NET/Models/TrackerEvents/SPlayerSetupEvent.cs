namespace s2protocol.NET.Models;
/// <summary>Record <c>SPlayerSetupEvent</c> SPlayerSetupEvent</summary>
///
public sealed class SPlayerSetupEvent(
    int playerId,
    int eventId,
    int bits,
    int gameloop,
    int type,
    int? userId,
    int slotId) : TrackerEvent(
        playerId,
        eventId,
        TrackerEventType.SPlayerSetupEvent,
        bits,
        gameloop)
{
    public int Type { get; } = type;
    public int? UserId { get; } = userId;
    public int SlotId { get; } = slotId;
}

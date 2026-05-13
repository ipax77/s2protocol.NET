namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerTransmissionOffsetEvent</c> STriggerTransmissionOffsetEvent</summary>
///
/// <remarks>Record <c>STriggerTransmissionOffsetEvent</c> constructor</remarks>
///
public sealed class STriggerTransmissionOffsetEvent(int userId,
    int eventId,
    int bits,
    int gameloop,
    int achievementLink) : GameEvent(userId, eventId, GameEventType.STriggerTransmissionOffsetEvent, bits, gameloop)
{
    /// <summary>Event Flags</summary>
    ///
    public int AchievementLink { get; } = achievementLink;
}
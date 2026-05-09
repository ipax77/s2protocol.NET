namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerTransmissionOffsetEvent</c> STriggerTransmissionOffsetEvent</summary>
///
public sealed class STriggerTransmissionOffsetEvent : GameEvent
{
    /// <summary>Record <c>STriggerTransmissionOffsetEvent</c> constructor</summary>
    ///
    public STriggerTransmissionOffsetEvent(int userId,
        int eventId,
        int bits,
        int gameloop,
        int achievementLink) : base(userId, eventId, GameEventType.STriggerTransmissionOffsetEvent, bits, gameloop)
    {
        AchievementLink = achievementLink;
    }
    /// <summary>Event Flags</summary>
    ///
    public int AchievementLink { get; }
}
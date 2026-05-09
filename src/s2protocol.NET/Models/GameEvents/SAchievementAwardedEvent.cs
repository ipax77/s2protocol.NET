namespace s2protocol.NET.Models;
/// <summary>Record <c>SAchievementAwardedEvent</c> SAchievementAwardedEvent</summary>
///
public sealed class SAchievementAwardedEvent : GameEvent
{
    /// <summary>Record <c>SAchievementAwardedEvent</c> constructor</summary>
    ///
    public SAchievementAwardedEvent(int userId,
        int eventId,
        int bits,
        int gameloop,
        int achievementLink) : base(userId, eventId, GameEventType.SAchievementAwardedEvent, bits, gameloop)
    {
        AchievementLink = achievementLink;
    }
    /// <summary>Event Flags</summary>
    ///
    public int AchievementLink { get; }
}
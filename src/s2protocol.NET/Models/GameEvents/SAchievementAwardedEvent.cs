namespace s2protocol.NET.Models;
/// <summary>Record <c>SAchievementAwardedEvent</c> SAchievementAwardedEvent</summary>
///
/// <remarks>Record <c>SAchievementAwardedEvent</c> constructor</remarks>
///
public sealed class SAchievementAwardedEvent(int userId,
    int eventId,
    int bits,
    int gameloop,
    int achievementLink) : GameEvent(userId, eventId, GameEventType.SAchievementAwardedEvent, bits, gameloop)
{
    /// <summary>Event Flags</summary>
    ///
    public int AchievementLink { get; } = achievementLink;
}
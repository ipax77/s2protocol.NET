namespace s2protocol.NET.Models;

/// <summary>Describes a user joining a game, including replay-hijack metadata.</summary>
public sealed class SGameUserJoinEvent : GameEvent
{
    public SGameUserJoinEvent(
        int userId,
        int eventId,
        int bits,
        int gameloop,
        int observe,
        string name,
        string? toonHandle,
        string? clanTag,
        string? clanLogo,
        bool hijack,
        int? hijackCloneGameUserId)
        : base(userId, eventId, GameEventType.SGameUserJoinEvent, bits, gameloop)
    {
        Observe = observe;
        Name = name;
        ToonHandle = toonHandle;
        ClanTag = clanTag;
        ClanLogo = clanLogo;
        Hijack = hijack;
        HijackCloneGameUserId = hijackCloneGameUserId;
    }

    public int Observe { get; }
    public string Name { get; }
    public string? ToonHandle { get; }
    public string? ClanTag { get; }
    public string? ClanLogo { get; }
    public bool Hijack { get; }
    public int? HijackCloneGameUserId { get; }
}

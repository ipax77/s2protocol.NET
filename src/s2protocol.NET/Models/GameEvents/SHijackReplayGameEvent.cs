namespace s2protocol.NET.Models;

/// <summary>Describes a user participating in a replay hijack.</summary>
public sealed record HijackReplayUserInfo(
    int GameUserId,
    int Observe,
    string Name,
    string? ToonHandle,
    string? ClanTag,
    string? ClanLogo);

/// <summary>Indicates that players took control of a running replay.</summary>
public sealed class SHijackReplayGameEvent : GameEvent
{
    public SHijackReplayGameEvent(
        int userId,
        int eventId,
        int bits,
        int gameloop,
        IReadOnlyList<HijackReplayUserInfo> userInfos,
        int method)
        : base(userId, eventId, GameEventType.SHijackReplayGameEvent, bits, gameloop)
    {
        UserInfos = userInfos;
        Method = method;
    }

    public IReadOnlyList<HijackReplayUserInfo> UserInfos { get; }
    public int Method { get; }
}

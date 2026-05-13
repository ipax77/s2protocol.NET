namespace s2protocol.NET.Models;
/// <summary>Record <c>SCommandManagerStateEvent</c> SCommandManagerStateEvent</summary>
///
/// <remarks>Record <c>SCommandManagerStateEvent</c> constructor</remarks>
///
public sealed class SCommandManagerStateEvent(int userId,
    int eventId,
    int bits,
    int gameloop,
    int state,
                                 int? sequence) : GameEvent(userId, eventId, GameEventType.SCommandManagerStateEvent, bits, gameloop)
{

    /// <summary>Event State</summary>
    ///
    public int State { get; } = state;
    /// <summary>Event Sequence</summary>
    ///
    public int? Sequence { get; } = sequence;
}
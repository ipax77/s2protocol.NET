namespace s2protocol.NET.Models;
/// <summary>Record <c>SCommandManagerStateEvent</c> SCommandManagerStateEvent</summary>
///
public sealed class SCommandManagerStateEvent : GameEvent
{
    /// <summary>Record <c>SCommandManagerStateEvent</c> constructor</summary>
    ///
    public SCommandManagerStateEvent(int userId,
        int eventId,
        int bits,
        int gameloop,
        int state,
                                     int? sequence) : base(userId, eventId, GameEventType.SCommandManagerStateEvent, bits, gameloop)
    {
        State = state;
        Sequence = sequence;
    }

    /// <summary>Event State</summary>
    ///
    public int State { get; }
    /// <summary>Event Sequence</summary>
    ///
    public int? Sequence { get; }
}
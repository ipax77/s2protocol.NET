namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerSoundtrackDoneEvent</c> STriggerSoundtrackDoneEvent</summary>
///
public sealed class STriggerSoundtrackDoneEvent : GameEvent
{
    /// <summary>Record <c>STriggerSoundtrackDoneEvent</c> constructor</summary>
    ///
    public STriggerSoundtrackDoneEvent(int userId,
        int eventId,
        int bits,
        int gameloop,
        int soundtrack) : base(userId, eventId, GameEventType.STriggerSoundtrackDoneEvent, bits, gameloop)
    {
        Soundtrack = soundtrack;
    }

    /// <summary>Event Down</summary>
    ///
    public int Soundtrack { get; }
}
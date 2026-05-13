namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerSoundtrackDoneEvent</c> STriggerSoundtrackDoneEvent</summary>
///
/// <remarks>Record <c>STriggerSoundtrackDoneEvent</c> constructor</remarks>
///
public sealed class STriggerSoundtrackDoneEvent(int userId,
    int eventId,
    int bits,
    int gameloop,
    int soundtrack) : GameEvent(userId, eventId, GameEventType.STriggerSoundtrackDoneEvent, bits, gameloop)
{

    /// <summary>Event Down</summary>
    ///
    public int Soundtrack { get; } = soundtrack;
}
namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerSoundOffsetEvent</c> STriggerSoundOffsetEvent</summary>
///
/// <remarks>Record <c>STriggerSoundOffsetEvent</c> constructor</remarks>
///
public sealed class STriggerSoundOffsetEvent(int userId,
    int eventId,
    int bits,
    int gameloop,
    int sound) : GameEvent(userId, eventId, GameEventType.STriggerSoundOffsetEvent, bits, gameloop)
{

    /// <summary>Event Sound</summary>
    ///
    public int Sound { get; } = sound;
}
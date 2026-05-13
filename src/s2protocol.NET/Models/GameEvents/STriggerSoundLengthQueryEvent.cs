namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerSoundLengthQueryEvent</c> STriggerSoundLengthQueryEvent</summary>
///
/// <remarks>Record <c>STriggerSoundLengthQueryEvent</c> constructor</remarks>
///
public sealed class STriggerSoundLengthQueryEvent(int userId,
    int eventId,
    int bits,
    int gameloop,
    long soundHash,
                        int length) : GameEvent(userId, eventId, GameEventType.STriggerSoundLengthQueryEvent, bits, gameloop)
{

    /// <summary>Event SoundHash</summary>
    ///
    public long SoundHash { get; } = soundHash;
    /// <summary>Event Length</summary>
    ///
    public int Length { get; } = length;
}
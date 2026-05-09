namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerSoundLengthQueryEvent</c> STriggerSoundLengthQueryEvent</summary>
///
public sealed class STriggerSoundLengthQueryEvent : GameEvent
{
    /// <summary>Record <c>STriggerSoundLengthQueryEvent</c> constructor</summary>
    ///
    public STriggerSoundLengthQueryEvent(int userId,
        int eventId,
        int bits,
        int gameloop,
        long soundHash,
                            int length) : base(userId, eventId, GameEventType.STriggerSoundLengthQueryEvent, bits, gameloop)
    {
        SoundHash = soundHash;
        Length = length;
    }

    /// <summary>Event SoundHash</summary>
    ///
    public long SoundHash { get; }
    /// <summary>Event Length</summary>
    ///
    public int Length { get; }
}
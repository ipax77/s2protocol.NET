namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerSoundOffsetEvent</c> STriggerSoundOffsetEvent</summary>
///
public sealed class STriggerSoundOffsetEvent : GameEvent
{
    /// <summary>Record <c>STriggerSoundOffsetEvent</c> constructor</summary>
    ///
    public STriggerSoundOffsetEvent(int userId,
        int eventId,
        int bits,
        int gameloop,
        int sound) : base(userId, eventId, GameEventType.STriggerSoundOffsetEvent, bits, gameloop)
    {
        Sound = sound;
    }

    /// <summary>Event Sound</summary>
    ///
    public int Sound { get; }
}
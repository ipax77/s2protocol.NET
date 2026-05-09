namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerSoundLengthSyncEvent</c> STriggerSoundLengthSyncEvent</summary>
///
public sealed class STriggerSoundLengthSyncEvent : GameEvent
{
    /// <summary>Record <c>STriggerSoundLengthSyncEvent</c> constructor</summary>
    ///
    public STriggerSoundLengthSyncEvent(int userId,
        int eventId,
        int bits,
        int gameloop) : base(userId, eventId, GameEventType.STriggerSoundLengthSyncEvent, bits, gameloop)
    {
    }
}
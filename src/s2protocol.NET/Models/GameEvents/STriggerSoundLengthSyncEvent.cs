namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerSoundLengthSyncEvent</c> STriggerSoundLengthSyncEvent</summary>
///
/// <remarks>Record <c>STriggerSoundLengthSyncEvent</c> constructor</remarks>
///
public sealed class STriggerSoundLengthSyncEvent(int userId,
    int eventId,
    int bits,
    int gameloop) : GameEvent(userId, eventId, GameEventType.STriggerSoundLengthSyncEvent, bits, gameloop)
{
}
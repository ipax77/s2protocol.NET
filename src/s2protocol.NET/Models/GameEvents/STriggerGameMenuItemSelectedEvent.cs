namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerGameMenuItemSelectedEvent</c> STriggerGameMenuItemSelectedEvent</summary>
///
/// <remarks>Record <c>STriggerGameMenuItemSelectedEvent</c> constructor</remarks>
///
public sealed class STriggerGameMenuItemSelectedEvent(int userId,
    int eventId,
    int bits,
    int gameloop,
    long gameMenuItemIndex) : GameEvent(userId, eventId, GameEventType.STriggerGameMenuItemSelectedEvent, bits, gameloop)
{
    /// <summary>Event GameMenuItemIndex</summary>
    ///
    public long GameMenuItemIndex { get; } = gameMenuItemIndex;
}
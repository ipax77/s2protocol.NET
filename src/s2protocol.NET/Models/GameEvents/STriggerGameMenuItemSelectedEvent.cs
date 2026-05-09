namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerGameMenuItemSelectedEvent</c> STriggerGameMenuItemSelectedEvent</summary>
///
public sealed class STriggerGameMenuItemSelectedEvent : GameEvent
{
    /// <summary>Record <c>STriggerGameMenuItemSelectedEvent</c> constructor</summary>
    ///
    public STriggerGameMenuItemSelectedEvent(int userId,
        int eventId,
        int bits,
        int gameloop,
        long gameMenuItemIndex) : base(userId, eventId, GameEventType.STriggerGameMenuItemSelectedEvent, bits, gameloop)
    {
        GameMenuItemIndex = gameMenuItemIndex;
    }
    /// <summary>Event GameMenuItemIndex</summary>
    ///
    public long GameMenuItemIndex { get; }
}
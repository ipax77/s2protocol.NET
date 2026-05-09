namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerChatMessageEvent</c> STriggerChatMessageEvent</summary>
///
public sealed class STriggerChatMessageEvent : GameEvent
{
    /// <summary>Record <c>STriggerChatMessageEvent</c> constructor</summary>
    ///
    public STriggerChatMessageEvent(int userId,
        int eventId,
        int bits,
        int gameloop,
        string chatMessage) : base(userId, eventId, GameEventType.STriggerChatMessageEvent, bits, gameloop)
    {
        ChatMessage = chatMessage;
    }

    /// <summary>Event Type</summary>
    ///
    public string ChatMessage { get; }
}
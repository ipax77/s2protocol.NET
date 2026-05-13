namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerChatMessageEvent</c> STriggerChatMessageEvent</summary>
///
/// <remarks>Record <c>STriggerChatMessageEvent</c> constructor</remarks>
///
public sealed class STriggerChatMessageEvent(int userId,
    int eventId,
    int bits,
    int gameloop,
    string chatMessage) : GameEvent(userId, eventId, GameEventType.STriggerChatMessageEvent, bits, gameloop)
{

    /// <summary>Event Type</summary>
    ///
    public string ChatMessage { get; } = chatMessage;
}
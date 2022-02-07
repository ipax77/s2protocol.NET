using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>STriggerChatMessageEvent</c> STriggerChatMessageEvent</summary>
///
public record STriggerChatMessageEvent : GameEvent
{
    /// <summary>Record <c>STriggerChatMessageEvent</c> constructor</summary>
    ///
    public STriggerChatMessageEvent(
        GameEvent gameEvent,
        string chatMessage) : base(gameEvent)
    {
        ChatMessage = chatMessage;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public STriggerChatMessageEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Event Type</summary>
    ///
    public string ChatMessage { get; init; }
}
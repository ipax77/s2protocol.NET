using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>SPlayerSetupEvent</c> SPlayerSetupEvent</summary>
///
public record SPlayerSetupEvent : TrackerEvent
{
    /// <summary>Record <c>SPlayerSetupEvent</c> constructor</summary>
    ///
    public SPlayerSetupEvent(
        TrackerEvent trackerEvent,
        int type,
        int? userId,
        int slotId) : base(trackerEvent)
    {
        Type = type;
        UserId = userId;
        SlotId = slotId;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SPlayerSetupEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Event Type</summary>
    ///
    public int Type { get; init; }
    /// <summary>Event Gameloop</summary>
    ///
    public int? UserId { get; init; }
    /// <summary>Event SlotId</summary>
    ///
    public int SlotId { get; init; }
}

using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>SUpgradeEvent</c> SUpgradeEvent</summary>
///
public record SUpgradeEvent : TrackerEvent
{
    /// <summary>Record <c>SUpgradeEvent</c> constructor</summary>
    ///
    public SUpgradeEvent(TrackerEvent trackerEvent,
                         int count,
                         string upgradeTypeName) : base(trackerEvent)
    {
        Count = count; ;
        UpgradeTypeName = upgradeTypeName;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SUpgradeEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Event Count</summary>
    ///
    public int Count { get; init; }
    /// <summary>Event UpgradeTypeName</summary>
    ///
    public string UpgradeTypeName { get; init; }
}

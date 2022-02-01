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
    /// <summary>Event Count</summary>
    ///
    public int Count { get; init; }
    /// <summary>Event UpgradeTypeName</summary>
    ///
    public string UpgradeTypeName { get; init; }
}

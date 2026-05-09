namespace s2protocol.NET.Models;
/// <summary>Record <c>SUpgradeEvent</c> SUpgradeEvent</summary>
///
/// <remarks>Record <c>SUpgradeEvent</c> constructor</remarks>
///
public sealed class SUpgradeEvent(int playerId,
                                  int eventId,
                                  int bits,
                                  int gameloop,
                                  int count,
                                  string upgradeTypeName) : TrackerEvent(playerId, eventId, TrackerEventType.SUpgradeEvent, bits, gameloop)
{
    /// <summary>Event Count</summary>
    ///
    public int Count { get; } = count;
    /// <summary>Event UpgradeTypeName</summary>
    ///
    public string UpgradeTypeName { get; } = upgradeTypeName;
}

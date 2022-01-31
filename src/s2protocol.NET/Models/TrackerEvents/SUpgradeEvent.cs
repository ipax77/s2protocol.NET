using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s2protocol.NET.Models;
/// <summary>Record <c>SUpgradeEvent</c> SUpgradeEvent</summary>
///
public record SUpgradeEvent : TrackerEvent
{
    /// <summary>Record <c>SUpgradeEvent</c> constructor</summary>
    ///
    public SUpgradeEvent(int playerId,
                         int eventId,
                         TrackerEventType eventType,
                         int bits,
                         int gameloop,
                         int count,
                         string upgradeTypeName) : base(playerId, eventId, eventType, bits, gameloop)
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

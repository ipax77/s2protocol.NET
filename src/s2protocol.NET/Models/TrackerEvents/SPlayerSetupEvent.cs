using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s2protocol.NET.Models;
/// <summary>Record <c>SPlayerSetupEvent</c> SPlayerSetupEvent</summary>
///
public record SPlayerSetupEvent : TrackerEvent
{
    /// <summary>Record <c>SPlayerSetupEvent</c> constructor</summary>
    ///
    public SPlayerSetupEvent(
        int playerId,
        int eventId,
        TrackerEventType eventType,
        int bits,
        int gameloop,
        int type,
        int userId,
        int slotId) : base(playerId, eventId, eventType, bits, gameloop)
    {
        Type = type;
        UserId = userId;
        SlotId = slotId;
    }

    /// <summary>Event Type</summary>
    ///
    public int Type { get; init; }
    /// <summary>Event Gameloop</summary>
    ///
    public int UserId { get; init; }
    /// <summary>Event SlotId</summary>
    ///
    public int SlotId { get; init; }
}

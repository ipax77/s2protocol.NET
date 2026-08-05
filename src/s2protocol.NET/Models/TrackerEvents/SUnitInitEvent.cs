using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;

/// <summary>A tracker event for a unit beginning construction.</summary>
public sealed class SUnitInitEvent(int playerId,
                                   int eventId,
                                   int bits,
                                   int gameloop,
                                   int unitTagIndex,
                                   int unitTagRecycle,
                                   int controlPlayerId,
                                   int x,
                                   int y,
                                   int upkeepPlayerId,
                                   string unitTypeName) : TrackerEvent(playerId, eventId, TrackerEventType.SUnitInitEvent, bits, gameloop)
{
    public int UnitIndex { get; internal set; }
    public int UnitTagIndex { get; init; } = unitTagIndex;
    public int UnitTagRecycle { get; init; } = unitTagRecycle;
    public int ControlPlayerId { get; init; } = controlPlayerId;
    public int Y { get; init; } = y;
    public int X { get; init; } = x;
    public int UpkeepPlayerId { get; init; } = upkeepPlayerId;
    public string UnitTypeName { get; init; } = unitTypeName;

    [JsonIgnore]
    public SUnitDoneEvent? SUnitDoneEvent { get; internal set; }

    [JsonIgnore]
    public SUnitDiedEvent? SUnitDiedEvent { get; internal set; }
}

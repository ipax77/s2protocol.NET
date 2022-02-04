using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>SBankValueEvent</c> SBankValueEvent</summary>
///
public record SBankValueEvent : GameEvent
{
    /// <summary>Record <c>SBankValueEvent</c> constructor</summary>
    ///
    public SBankValueEvent(
        GameEvent gameEvent,
        string name,
        string data,
        int type) : base(gameEvent)
    {
        Name = name;
        Data = data;
        Type = type;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SBankValueEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Event Type</summary>
    ///
    public string Name { get; init; }
    /// <summary>Event Data</summary>
    ///
    public string Data { get; init; }
    /// <summary>Event Type</summary>
    ///
    public int Type { get; init; }
}
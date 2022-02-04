using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>SBankKeyEvent</c> SBankKeyEvent</summary>
///
public record SBankKeyEvent : GameEvent
{
    /// <summary>Record <c>SBankKeyEvent</c> constructor</summary>
    ///
    public SBankKeyEvent(
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
    public SBankKeyEvent()
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
using IronPython.Runtime;
using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>UnknownGameEvent</c> UnknownGameEvent</summary>
///
public record UnknownGameEvent : GameEvent
{
    /// <summary>Record <c>UnknownGameEvent</c> constructor</summary>
    ///
    public UnknownGameEvent(
        GameEvent gameEvent,
        PythonDictionary data) : base(gameEvent)
    {
        Data = data;
        EventTypeName = Parser.Parse.GetString(data, "_event");
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public UnknownGameEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Event PythonDictionary data</summary>
    ///
    public PythonDictionary Data { get; init; }
    /// <summary>EventTypeName</summary>
    ///
    public string EventTypeName { get; init; }
}
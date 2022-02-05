﻿using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>SSelectionDeltaEvent</c> SSelectionDeltaEvent</summary>
///
public record SSelectionDeltaEvent : GameEvent
{
    /// <summary>Record <c>SSelectionDeltaEvent</c> constructor</summary>
    ///
    public SSelectionDeltaEvent(
        GameEvent gameEvent,
        string name) : base(gameEvent)
    {
        Name = name;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SSelectionDeltaEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Event Type</summary>
    ///
    public string Name { get; init; }
}
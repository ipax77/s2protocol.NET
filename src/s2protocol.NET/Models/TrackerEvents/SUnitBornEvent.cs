﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s2protocol.NET.Models;
/// <summary>Record <c>SUnitBornEvent</c> SUnitBornEvent</summary>
///
public record SUnitBornEvent : TrackerEvent
{
    /// <summary>Record <c>SUnitBornEvent</c> constructor</summary>
    ///
    public SUnitBornEvent(TrackerEvent trackerEvent,
                          int unitTagIndex,
                          int unitTagRecycle,
                          string? creatorAbilityName,
                          int? creatorUnitTagRecycle,
                          int controlPlayerId,
                          int x,
                          int y,
                          int upkeepPlayerId,
                          string unitTypeName,
                          int? creatorUnitTagIndex) : base(trackerEvent)
    {
        UnitTagIndex = unitTagIndex;
        UnitTagRecycle = unitTagRecycle;
        CreatorAbilityName = creatorAbilityName;
        CreatorUnitTagRecycle = creatorUnitTagRecycle;
        ControlPlayerId = controlPlayerId;
        X = x;
        Y = y;
        UpkeepPlayerId = upkeepPlayerId;
        UnitTypeName = unitTypeName;
        CreatorUnitTagIndex = creatorUnitTagIndex;
    }

    /// <summary>Event UnitTagIndex</summary>
    ///
    public int UnitTagIndex { get; init; }
    /// <summary>Event UnitTagRecycle</summary>
    ///
    public int UnitTagRecycle { get; init; }
    /// <summary>Event CreatorAbilityName</summary>
    ///
    public string? CreatorAbilityName { get; init; }
    /// <summary>Event CreatorUnitTagRecycle</summary>
    ///
    public int? CreatorUnitTagRecycle { get; init; }
    /// <summary>Event ControlPlayerId</summary>
    ///
    public int ControlPlayerId { get; init; }
    /// <summary>Event Y</summary>
    ///
    public int Y { get; init; }
    /// <summary>Event X</summary>
    ///
    public int X { get; init; }
    /// <summary>Event UpkeepPlayerId</summary>
    ///
    public int UpkeepPlayerId { get; init; }
    /// <summary>Event UnitTypeName</summary>
    ///
    public string UnitTypeName { get; init; }
    /// <summary>Event CreatorUnitTagIndex</summary>
    ///
    public int? CreatorUnitTagIndex { get; init; }
}

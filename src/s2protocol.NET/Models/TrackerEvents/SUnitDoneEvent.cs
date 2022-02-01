using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s2protocol.NET.Models;
/// <summary>Record <c>SUnitDoneEvent</c> SUnitDoneEvent</summary>
///
public record SUnitDoneEvent : TrackerEvent
{
    /// <summary>Record <c>SUnitDoneEvent</c> constructor</summary>
    ///
    public SUnitDoneEvent(TrackerEvent trackerEvent,
                         int unitTagIndex,
                         int unitTagRecycle) : base(trackerEvent)
    {
        UnitTagIndex = unitTagIndex; ;
        UnitTagRecycle = unitTagRecycle;
    }
    /// <summary>Event UnitTagIndex</summary>
    ///
    public int UnitTagIndex { get; init; }
    /// <summary>Event UnitTagRecycle</summary>
    ///
    public int UnitTagRecycle { get; init; }
}

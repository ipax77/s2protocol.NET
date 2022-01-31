using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s2protocol.NET;

/// <summary>Decodeing Options</summary>
/// 
public record ReplayDecoderOptions
{
    /// <summary>Decode Details (Game time, Player names)</summary>
    /// 
    public bool Details { get; set; } = true;
    /// <summary>Decode Metadata (APM, Race selected/assigned)</summary>
    /// 
    public bool Metadata { get; set; } = true;
    /// <summary>Decode InitData</summary>
    /// 
    public bool InitData { get; set; } = true;
    /// <summary>Decode GameEvents</summary>
    /// 
    public bool GameEvents { get; set; } = true;
    /// <summary>Decode MessageEvents</summary>
    /// 
    public bool MessageEvents { get; set; } = true;
    /// <summary>Decode TrackerEvents</summary>
    /// 
    public bool TrackerEvents { get; set; } = true;
    /// <summary>Parse decoded data</summary>
    /// 
    public bool Parse { get; set; } = true;
}

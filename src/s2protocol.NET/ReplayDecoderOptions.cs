namespace s2protocol.NET;

/// <summary>Decodeing Options</summary>
/// 
public record ReplayDecoderOptions
{
    /// <summary>Decode Initdata (Player infos)</summary>
    /// 
    public bool Initdata { get; set; } = true;
    /// <summary>Decode Details (Game time, Player names)</summary>
    /// 
    public bool Details { get; set; } = true;
    /// <summary>Decode Metadata (APM, Race selected/assigned)</summary>
    /// 
    public bool Metadata { get; set; } = true;
    /// <summary>Decode MessageEvents</summary>
    /// 
    public bool MessageEvents { get; set; } = true;
    /// <summary>Decode TrackerEvents</summary>
    /// 
    public bool TrackerEvents { get; set; } = true;
}

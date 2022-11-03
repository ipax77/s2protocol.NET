
namespace s2protocol.NET;

/// <summary>DecodeParallelResult</summary>
public record DecodeParallelResult
{
    /// <summary>Sc2Replay</summary>
    public Sc2Replay? Sc2Replay { get; init; }
    /// <summary>ReplayPath</summary>
    public string ReplayPath { get; init; } = null!;
    /// <summary>Exception</summary>
    public string? Exception { get; init; } 
}
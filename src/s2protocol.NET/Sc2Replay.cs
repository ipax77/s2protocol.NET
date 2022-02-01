
using s2protocol.NET.Models;

namespace s2protocol.NET;

/// <summary>Record <c>Sc2Replay</c> Starcaft2 replay model</summary>
///
public record Sc2Replay
{
    /// <summary>Replay Header infos</summary>
    ///
    public Header Header { get; init; }

    /// <summary>Replay details infos</summary>
    ///
    public Details? Details { get; internal set; }
    /// <summary>Replay metadata infos</summary>
    ///
    public Metadata? Metadata { get; internal set; }
    /// <summary>Replay chat messages</summary>
    ///
    public ICollection<ChatMessageEvent>? ChatMessages { get; internal set; }
    /// <summary>Replay TrackerEvents</summary>
    ///
    public TrackerEvents? TrackerEvents { get; internal set; }

    /// <summary>Record <c>Sc2Replay</c> constructor</summary>
    ///
    public Sc2Replay(dynamic header)
    {
        Header = Parser.Parse.Header(header);
    }
}
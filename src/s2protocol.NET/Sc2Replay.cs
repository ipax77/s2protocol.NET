
using s2protocol.NET.Models;
using s2protocol.NET.Parser;
using System.Text.Json.Serialization;

namespace s2protocol.NET;

/// <summary>Record <c>Sc2Replay</c> Starcaft2 replay model</summary>
///
public record Sc2Replay : IJsonOnDeserialized
{
    /// <summary>Record <c>Sc2Replay</c> constructor</summary>
    ///
    public Sc2Replay(object header, string fileName)
    {
        FileName = fileName;
        Header = Parser.Parse.Header(header);
    }

    internal Sc2Replay(Header header, string fileName)
    {
        FileName = fileName;
        Header = header;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Sc2Replay()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }
    /// <summary>Replay FileName</summary>
    ///
    public string FileName { get; init; }
    /// <summary>Replay Header infos</summary>
    ///
    public Header Header { get; init; }
    /// <summary>Replay initdata</summary>
    ///
    [JsonInclude]
    public Initdata? Initdata { get; internal set; }

    /// <summary>Replay details infos</summary>
    ///
    [JsonInclude]
    public Details? Details { get; internal set; }
    /// <summary>Replay metadata infos</summary>
    ///
    [JsonInclude]
    public ReplayMetadata? Metadata { get; internal set; }
    /// <summary>Replay message events in stream order.</summary>
    ///
    [JsonInclude]
    public MessageEvents? MessageEvents { get; internal set; }
    /// <summary>Replay chat messages</summary>
    ///
    [JsonIgnore]
    public ICollection<ChatMessageEvent>? ChatMessages => MessageEvents?.ChatMessages;
    /// <summary>Replay ping messages</summary>
    ///
    [JsonIgnore]
    public ICollection<PingMessageEvent>? PingMessages => MessageEvents?.PingMessages;
    /// <summary>Replay TrackerEvents</summary>
    ///
    [JsonInclude]
    public TrackerEvents? TrackerEvents { get; internal set; }
    /// <summary>Replay GameEvents</summary>
    ///
    [JsonInclude]
    public GameEvents? GameEvents { get; internal set; }
    /// <summary>Replay AttributeEvents</summary>
    ///
    [JsonInclude]
    public AttributeEvents? AttributeEvents { get; internal set; }

    void IJsonOnDeserialized.OnDeserialized()
    {
        if (TrackerEvents is null)
        {
            return;
        }

        ReplayDecoder.SetTrackerEventUnitIndexes(TrackerEvents);
        Parse.SetTrackerEventsUnitConnections(TrackerEvents);
    }
}

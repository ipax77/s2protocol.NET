using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;

/// <summary>Record <c>Metadata</c> Parsed replay metadata infos</summary>
///
public sealed record ReplayMetadata
{
    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public ReplayMetadata()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Record <c>Metadata</c> constructor</summary>
    ///
    public ReplayMetadata(
        string baseBuild,
        string dataBuild,
        string dataVersion,
        int duration,
        Version gameVersion,
        bool isNotAvailable,
        string title,
        ICollection<MetadataPlayer> players)
    {
        BaseBuild = baseBuild;
        DataBuild = dataBuild;
        DataVersion = dataVersion;
        Duration = duration;
        GameVersion = gameVersion;
        IsNotAvailable = isNotAvailable;
        Title = title;
        Players = players;
    }

    /// <summary>Replay BaseBuild</summary>
    ///
    public string BaseBuild { get; init; }
    /// <summary>Replay DataBuild</summary>
    ///
    public string DataBuild { get; init; }
    /// <summary>Replay DataVersion</summary>
    ///
    public string DataVersion { get; init; }
    /// <summary>Replay Duration</summary>
    ///
    public int Duration { get; init; }
    /// <summary>Replay GameVersion</summary>
    ///
    public Version GameVersion { get; init; }
    /// <summary>Replay IsNotAvailable</summary>
    ///
    public bool IsNotAvailable { get; init; }
    /// <summary>Replay Title</summary>
    ///
    public string Title { get; init; }
    /// <summary>Replay MetadataPlayers</summary>
    ///
    public ICollection<MetadataPlayer> Players { get; init; }
}

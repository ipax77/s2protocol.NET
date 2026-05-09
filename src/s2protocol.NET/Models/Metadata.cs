namespace s2protocol.NET.Models;

/// <summary>Record <c>Metadata</c> Parsed replay metadata infos</summary>
///
/// <remarks>Record <c>Metadata</c> constructor</remarks>
///
public sealed class ReplayMetadata(
    string baseBuild,
    string dataBuild,
    string dataVersion,
    int duration,
    Version gameVersion,
    bool isNotAvailable,
    string title,
    ICollection<MetadataPlayer> players)
{

    /// <summary>Replay BaseBuild</summary>
    ///
    public string BaseBuild { get; init; } = baseBuild;
    /// <summary>Replay DataBuild</summary>
    ///
    public string DataBuild { get; init; } = dataBuild;
    /// <summary>Replay DataVersion</summary>
    ///
    public string DataVersion { get; init; } = dataVersion;
    /// <summary>Replay Duration</summary>
    ///
    public int Duration { get; init; } = duration;
    /// <summary>Replay GameVersion</summary>
    ///
    public Version GameVersion { get; init; } = gameVersion;
    /// <summary>Replay IsNotAvailable</summary>
    ///
    public bool IsNotAvailable { get; init; } = isNotAvailable;
    /// <summary>Replay Title</summary>
    ///
    public string Title { get; init; } = title;
    /// <summary>Replay MetadataPlayers</summary>
    ///
    public ICollection<MetadataPlayer> Players { get; init; } = players;
}

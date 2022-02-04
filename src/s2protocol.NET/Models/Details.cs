using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>Details</c> Parsed replay detail infos</summary>
///
public sealed record Details
{
    /// <summary>Record <c>Details</c> Constructor</summary>
    ///
    public Details(
        int campaignIndex,
        int defaultDifficulty,
        string description,
        string difficulty,
        bool disableRecoverGame,
        int gameSpeed,
        string imageFilePath,
        bool isBlizzardMap,
        string mapFileName,
        bool miniSave,
        bool restartAsTransitionMap,
        long timeLocalOffset,
        long timeUTC,
        string title,
        ICollection<DetailsPlayer> players)
    {
        CampaignIndex = campaignIndex;
        DefaultDifficulty = defaultDifficulty;
        Description = description;
        Difficulty = difficulty;
        DisableRecoverGame = disableRecoverGame;
        GameSpeed = gameSpeed;
        ImageFilePath = imageFilePath;
        IsBlizzardMap = isBlizzardMap;
        MapFileName = mapFileName;
        MiniSave = miniSave;
        RestartAsTransitionMap = restartAsTransitionMap;
        TimeLocalOffset = timeLocalOffset;
        TimeUTC = timeUTC;
        Title = title;
        Players = players;
        DateTimeUTC = DateTime.FromFileTime(timeUTC);
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Details()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Replay CampaignIndex</summary>
    ///
    public int CampaignIndex { get; init; }
    /// <summary>Replay DefaultDifficulty</summary>
    ///
    public int DefaultDifficulty { get; init; }
    /// <summary>Replay Description</summary>
    ///
    public string Description { get; init; }
    /// <summary>Replay Difficulty</summary>
    ///
    public string Difficulty { get; init; }
    /// <summary>Replay DisableRecoverGame</summary>
    ///
    public bool DisableRecoverGame { get; init; }
    /// <summary>Replay GameSpeed</summary>
    ///
    public int GameSpeed { get; init; }
    /// <summary>Replay ImageFilePath</summary>
    ///
    public string ImageFilePath { get; init; }
    /// <summary>Replay IsBlizzardMap</summary>
    ///
    public bool IsBlizzardMap { get; init; }
    /// <summary>Replay MapFileName</summary>
    ///
    public string MapFileName { get; init; }
    /// <summary>Replay MiniSave</summary>
    ///
    public bool MiniSave { get; init; }
    /// <summary>Replay RestartAsTransitionMap</summary>
    ///
    public bool RestartAsTransitionMap { get; init; }
    /// <summary>Replay TimeLocalOffset</summary>
    ///
    public long TimeLocalOffset { get; init; }
    /// <summary>Replay TimeUTC</summary>
    ///
    public long TimeUTC { get; init; }
    /// <summary>Replay TimeUTC</summary>
    ///
    public DateTime DateTimeUTC { get; init; }
    /// <summary>Replay Title</summary>
    ///
    public string Title { get; init; }
    /// <summary>Replay Players</summary>
    ///
    public ICollection<DetailsPlayer> Players { get; init; }
}

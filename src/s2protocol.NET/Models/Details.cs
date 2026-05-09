namespace s2protocol.NET.Models;
/// <summary>Record <c>Details</c> Parsed replay detail infos</summary>
///
/// <remarks>Record <c>Details</c> Constructor</remarks>
///
public sealed class Details(
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

    /// <summary>Replay CampaignIndex</summary>
    ///
    public int CampaignIndex { get; init; } = campaignIndex;
    /// <summary>Replay DefaultDifficulty</summary>
    ///
    public int DefaultDifficulty { get; init; } = defaultDifficulty;
    /// <summary>Replay Description</summary>
    ///
    public string Description { get; init; } = description;
    /// <summary>Replay Difficulty</summary>
    ///
    public string Difficulty { get; init; } = difficulty;
    /// <summary>Replay DisableRecoverGame</summary>
    ///
    public bool DisableRecoverGame { get; init; } = disableRecoverGame;
    /// <summary>Replay GameSpeed</summary>
    ///
    public int GameSpeed { get; init; } = gameSpeed;
    /// <summary>Replay ImageFilePath</summary>
    ///
    public string ImageFilePath { get; init; } = imageFilePath;
    /// <summary>Replay IsBlizzardMap</summary>
    ///
    public bool IsBlizzardMap { get; init; } = isBlizzardMap;
    /// <summary>Replay MapFileName</summary>
    ///
    public string MapFileName { get; init; } = mapFileName;
    /// <summary>Replay MiniSave</summary>
    ///
    public bool MiniSave { get; init; } = miniSave;
    /// <summary>Replay RestartAsTransitionMap</summary>
    ///
    public bool RestartAsTransitionMap { get; init; } = restartAsTransitionMap;
    /// <summary>Replay TimeLocalOffset</summary>
    ///
    public long TimeLocalOffset { get; init; } = timeLocalOffset;
    /// <summary>Replay TimeUTC</summary>
    ///
    public long TimeUTC { get; init; } = timeUTC;
    /// <summary>Replay TimeUTC</summary>
    ///
    public DateTime DateTimeUTC { get; init; } = DateTime.FromFileTimeUtc(timeUTC);
    /// <summary>Replay Title</summary>
    ///
    public string Title { get; init; } = title;
    /// <summary>Replay Players</summary>
    ///
    public ICollection<DetailsPlayer> Players { get; init; } = players;
}

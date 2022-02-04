using System.Numerics;
using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;

/// <summary>Record <c>Initdata</c> Parsed replay Initdata (m_syncLobbyState)</summary>
///
public sealed record Initdata
{
    /// <summary>Record <c>SyncLobbyState</c> constructor</summary>
    ///
    public Initdata(ICollection<UserInitialData> userInitialData, LobbyState lobbyState, GameDescription gameDescription)
    {
        UserInitialData = userInitialData;
        LobbyState = lobbyState;
        GameDescription = gameDescription;
    }
    /// <summary>InitData SyncLobbyState UserInitialData</summary>
    ///
    public ICollection<UserInitialData> UserInitialData { get; init; }
    /// <summary>InitData SyncLobbyState LobbyState</summary>
    ///
    public LobbyState LobbyState { get; init; }
    /// <summary>InitData SyncLobbyState GameDescription</summary>
    ///
    public GameDescription GameDescription { get; init; }
}

/// <summary>Record <c>UserInitialData</c> Parsed UserInitialData</summary>
///
public sealed record UserInitialData
{
    /// <summary>Record <c>UserInitialData</c> constructor</summary>
    ///
    public UserInitialData(string mount,
                           string skin,
                           int observe,
                           int? teamPreference,
                           string toonHandle,
                           long combinedRaceLevels,
                           int highestLeague,
                           string clanTag,
                           bool testMap,
                           bool testAuto,
                           bool examine,
                           int testType,
                           bool customInterface,
                           string clanLogo,
                           string name,
                           int? racePreference,
                           int randomSeed,
                           string hero,
                           long? scaledRating)
    {
        Mount = mount;
        Skin = skin;
        Observe = observe;
        TeamPreference = teamPreference;
        ToonHandle = toonHandle;
        CombinedRaceLevels = combinedRaceLevels;
        HighestLeague = highestLeague;
        ClanTag = clanTag;
        TestMap = testMap;
        TestAuto = testAuto;
        Examine = examine;
        TestType = testType;
        CustomInterface = customInterface;
        ClanLogo = clanLogo;
        Name = name;
        RacePreference = racePreference;
        RandomSeed = randomSeed;
        Hero = hero;
        ScaledRating = scaledRating;
    }

    /// <summary>InitData Mount</summary>
    ///
    public string Mount { get; init; }
    /// <summary>InitData Skin</summary>
    ///
    public string Skin { get; init; }
    /// <summary>InitData Observe</summary>
    ///
    public int Observe { get; init; }
    /// <summary>InitData TeamPreference</summary>
    ///
    public int? TeamPreference { get; init; }
    /// <summary>InitData ToonHandle</summary>
    ///
    public string ToonHandle { get; init; }
    /// <summary>InitData CombinedRaceLevels</summary>
    ///
    public long CombinedRaceLevels { get; init; }
    /// <summary>InitData HighestLeague</summary>
    ///
    public int HighestLeague { get; init; }
    /// <summary>InitData ClanTag</summary>
    ///
    public string ClanTag { get; init; }
    /// <summary>InitData TestMap</summary>
    ///
    public bool TestMap { get; init; }
    /// <summary>InitData TestAuto</summary>
    ///
    public bool TestAuto { get; init; }
    /// <summary>InitData Examine</summary>
    ///
    public bool Examine { get; init; }
    /// <summary>InitData TestType</summary>
    ///
    public int TestType { get; init; }
    /// <summary>InitData CustomInterface</summary>
    ///
    public bool CustomInterface { get; init; }
    /// <summary>InitData ClanLogo</summary>
    ///
    public string ClanLogo { get; init; }
    /// <summary>InitData Name</summary>
    ///
    public string Name { get; init; }
    /// <summary>InitData RacePreference</summary>
    ///
    public int? RacePreference { get; init; }
    /// <summary>InitData RandomSeed</summary>
    ///
    public int RandomSeed { get; init; }
    /// <summary>InitData Hero</summary>
    ///
    public string Hero { get; init; }
    /// <summary>InitData ScaledRating</summary>
    ///
    public long? ScaledRating { get; init; }
}

/// <summary>Record <c>Slot</c> Parsed UserInitialData Slot</summary>
///
public sealed record Slot
{
    /// <summary>Record <c>Slot</c> constructor</summary>
    ///
    public Slot(int aCEnemyRace,
                string toonHandle,
                ICollection<int> rewardOverrides,
                int? userId,
                string skin,
                ICollection<int> commanderMasteryTalents,
                int aiBuild,
                int teamId,
                ICollection<int> rewards,
                int commanderLevel,
                int logoIndex,
                ICollection<string> artifacts,
                int difficulty,
                int? tandemLeaderId,
                int commanderMasteryLevel,
                int trophyId,
                int brutalPlusDifficulty,
                int? racePref,
                int? tandemId,
                string hero,
                string commander,
                string mount,
                int handicap,
                int observe,
                int aCEnemyWaveType,
                int control,
                ICollection<int> licenses,
                int? colorPref,
                bool hasSilencePenalty,
                int workingSetSlotId,
                ICollection<int> retryMutationIndexes,
                int selectedCommanderPrestige)
    {
        ACEnemyRace = aCEnemyRace;
        ToonHandle = toonHandle;
        RewardOverrides = rewardOverrides;
        UserId = userId;
        Skin = skin;
        CommanderMasteryTalents = commanderMasteryTalents;
        AiBuild = aiBuild;
        TeamId = teamId;
        Rewards = rewards;
        CommanderLevel = commanderLevel;
        LogoIndex = logoIndex;
        Artifacts = artifacts;
        Difficulty = difficulty;
        TandemLeaderId = tandemLeaderId;
        CommanderMasteryLevel = commanderMasteryLevel;
        TrophyId = trophyId;
        BrutalPlusDifficulty = brutalPlusDifficulty;
        RacePref = racePref;
        TandemId = tandemId;
        Hero = hero;
        Commander = commander;
        Mount = mount;
        Handicap = handicap;
        Observe = observe;
        ACEnemyWaveType = aCEnemyWaveType;
        Control = control;
        Licenses = licenses;
        ColorPref = colorPref;
        HasSilencePenalty = hasSilencePenalty;
        WorkingSetSlotId = workingSetSlotId;
        RetryMutationIndexes = retryMutationIndexes;
        SelectedCommanderPrestige = selectedCommanderPrestige;
    }
    /// <summary>InitData Slot ACEnemyRace</summary>
    ///
    public int ACEnemyRace { get; init; }
    /// <summary>InitData Slot ToonHandle</summary>
    ///
    public string ToonHandle { get; init; }
    /// <summary>InitData Slot RewardOverrides</summary>
    ///
    public ICollection<int> RewardOverrides { get; init; }
    /// <summary>InitData Slot UserId</summary>
    ///
    public int? UserId { get; init; }
    /// <summary>InitData Slot Skin</summary>
    ///
    public string Skin { get; init; }
    /// <summary>InitData Slot CommanderMasteryTalents</summary>
    ///
    public ICollection<int> CommanderMasteryTalents { get; init; }
    /// <summary>InitData Slot AiBuild</summary>
    ///
    public int AiBuild { get; init; }
    /// <summary>InitData Slot TeamId</summary>
    ///
    public int TeamId { get; init; }
    /// <summary>InitData Slot Rewards</summary>
    ///
    public ICollection<int> Rewards { get; init; }
    /// <summary>InitData Slot CommanderLevel</summary>
    ///
    public int CommanderLevel { get; init; }
    /// <summary>InitData Slot LogoIndex</summary>
    ///
    public int LogoIndex { get; init; }
    /// <summary>InitData Slot Artifacts</summary>
    ///
    public ICollection<string> Artifacts { get; init; }
    /// <summary>InitData Slot Difficulty</summary>
    ///
    public int Difficulty { get; init; }
    /// <summary>InitData Slot TandemLeaderId</summary>
    ///
    public int? TandemLeaderId { get; init; }
    /// <summary>InitData Slot CommanderMasteryLevel</summary>
    ///
    public int CommanderMasteryLevel { get; init; }
    /// <summary>InitData Slot TrophyId</summary>
    ///
    public int TrophyId { get; init; }
    /// <summary>InitData Slot BrutalPlusDifficulty</summary>
    ///
    public int BrutalPlusDifficulty { get; init; }
    /// <summary>InitData Slot RacePref</summary>
    ///
    public int? RacePref { get; init; }
    /// <summary>InitData Slot TandemId</summary>
    ///
    public int? TandemId { get; init; }
    /// <summary>InitData Slot Hero</summary>
    ///
    public string Hero { get; init; }
    /// <summary>InitData Slot Commander</summary>
    ///
    public string Commander { get; init; }
    /// <summary>InitData Slot Mount</summary>
    ///
    public string Mount { get; init; }
    /// <summary>InitData Slot Handicap</summary>
    ///
    public int Handicap { get; init; }
    /// <summary>InitData Slot Observe</summary>
    ///
    public int Observe { get; init; }
    /// <summary>InitData Slot ACEnemyWaveType</summary>
    ///
    public int ACEnemyWaveType { get; init; }
    /// <summary>InitData Slot Control</summary>
    ///
    public int Control { get; init; }
    /// <summary>InitData Slot Licenses</summary>
    ///
    public ICollection<int> Licenses { get; init; }
    /// <summary>InitData Slot ColorPref</summary>
    ///
    public int? ColorPref { get; init; }
    /// <summary>InitData Slot HasSilencePenalty</summary>
    ///
    public bool HasSilencePenalty { get; init; }
    /// <summary>InitData Slot WorkingSetSlotId</summary>
    ///
    public int WorkingSetSlotId { get; init; }
    /// <summary>InitData Slot RetryMutationIndexes</summary>
    ///
    public ICollection<int> RetryMutationIndexes { get; init; }
    /// <summary>InitData Slot SelectedCommanderPrestige</summary>
    ///
    public int SelectedCommanderPrestige { get; init; }
}

/// <summary>Record <c>LobbyState</c> Parsed UserInitialData LobbyState</summary>
///
public sealed record LobbyState
{
    /// <summary>Record <c>LobbyState</c> constructor</summary>
    ///
    public LobbyState(int maxUsers,
                      ICollection<Slot> slots,
                      int defaultDifficulty,
                      bool isSinglePlayer,
                      int phase,
                      int? hostUserId,
                      int maxObservers,
                      int defaultAIBuild,
                      int pickedMapTag,
                      long randomSeed,
                      int gameDuration)
    {
        MaxUsers = maxUsers;
        Slots = slots;
        DefaultDifficulty = defaultDifficulty;
        IsSinglePlayer = isSinglePlayer;
        Phase = phase;
        HostUserId = hostUserId;
        MaxObservers = maxObservers;
        DefaultAIBuild = defaultAIBuild;
        PickedMapTag = pickedMapTag;
        RandomSeed = randomSeed;
        GameDuration = gameDuration;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public LobbyState()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>InitData LobbyState MaxUsers</summary>
    ///
    public int MaxUsers { get; init; }
    /// <summary>InitData LobbyState Slots</summary>
    ///
    public ICollection<Slot> Slots { get; init; }
    /// <summary>InitData LobbyState DefaultDifficulty</summary>
    ///
    public int DefaultDifficulty { get; init; }
    /// <summary>InitData LobbyState IsSinglePlayer</summary>
    ///
    public bool IsSinglePlayer { get; init; }
    /// <summary>InitData LobbyState Phase</summary>
    ///
    public int Phase { get; init; }
    /// <summary>InitData LobbyState HostUserId</summary>
    ///
    public int? HostUserId { get; init; }
    /// <summary>InitData LobbyState MaxObservers</summary>
    ///
    public int MaxObservers { get; init; }
    /// <summary>InitData LobbyState DefaultAIBuild</summary>
    ///
    public int DefaultAIBuild { get; init; }
    /// <summary>InitData LobbyState PickedMapTag</summary>
    ///
    public int PickedMapTag { get; init; }
    /// <summary>InitData LobbyState RandomSeed</summary>
    ///
    public long RandomSeed { get; init; }
    /// <summary>InitData LobbyState GameDuration</summary>
    ///
    public int GameDuration { get; init; }
}

/// <summary>Record <c>GameOptions</c> Parsed UserInitialData GameOptions</summary>
///
public sealed record GameOptions
{
    /// <summary>Record <c>GameOptions</c> constructor</summary>
    ///
    public GameOptions(bool competitive,
                       bool practice,
                       bool lockTeams,
                       bool amm,
                       bool battleNet,
                       int fog,
                       bool noVictoryOrDefeat,
                       bool heroDuplicatesAllowed,
                       int userDifficulty,
                       bool advancedSharedControl,
                       bool cooperative,
                       long clientDebugFlags,
                       int observers,
                       bool teamsTogether,
                       bool randomRaces,
                       bool buildCoachEnabled)
    {
        Competitive = competitive;
        Practice = practice;
        LockTeams = lockTeams;
        Amm = amm;
        BattleNet = battleNet;
        Fog = fog;
        NoVictoryOrDefeat = noVictoryOrDefeat;
        HeroDuplicatesAllowed = heroDuplicatesAllowed;
        UserDifficulty = userDifficulty;
        AdvancedSharedControl = advancedSharedControl;
        Cooperative = cooperative;
        ClientDebugFlags = clientDebugFlags;
        Observers = observers;
        TeamsTogether = teamsTogether;
        RandomRaces = randomRaces;
        BuildCoachEnabled = buildCoachEnabled;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public GameOptions()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>InitData GameOptions Competitive</summary>
    ///
    public bool Competitive { get; init; }
    /// <summary>InitData GameOptions Practice</summary>
    ///
    public bool Practice { get; init; }
    /// <summary>InitData GameOptions LockTeams</summary>
    ///
    public bool LockTeams { get; init; }
    /// <summary>InitData GameOptions Amm</summary>
    ///
    public bool Amm { get; init; }
    /// <summary>InitData GameOptions BattleNet</summary>
    ///
    public bool BattleNet { get; init; }
    /// <summary>InitData GameOptions Fog</summary>
    ///
    public int Fog { get; init; }
    /// <summary>InitData GameOptions NoVictoryOrDefeat</summary>
    ///
    public bool NoVictoryOrDefeat { get; init; }
    /// <summary>InitData GameOptions HeroDuplicatesAllowed</summary>
    ///
    public bool HeroDuplicatesAllowed { get; init; }
    /// <summary>InitData GameOptions UserDifficulty</summary>
    ///
    public int UserDifficulty { get; init; }
    /// <summary>InitData GameOptions AdvancedSharedControl</summary>
    ///
    public bool AdvancedSharedControl { get; init; }
    /// <summary>InitData GameOptions Cooperative</summary>
    ///
    public bool Cooperative { get; init; }
    /// <summary>InitData GameOptions ClientDebugFlags</summary>
    ///
    public long ClientDebugFlags { get; init; }
    /// <summary>InitData GameOptions Observers</summary>
    ///
    public int Observers { get; init; }
    /// <summary>InitData GameOptions TeamsTogether</summary>
    ///
    public bool TeamsTogether { get; init; }
    /// <summary>InitData GameOptions RandomRaces</summary>
    ///
    public bool RandomRaces { get; init; }
    /// <summary>InitData GameOptions BuildCoachEnabled</summary>
    ///
    public bool BuildCoachEnabled { get; init; }
}

/// <summary>Record <c>SlotDescription</c> Parsed UserInitialData SlotDescription</summary>
///
public sealed record SlotDescription
{
    /// <summary>Record <c>SlotDescription</c> constructor</summary>
    ///
    public SlotDescription(KeyValuePair<int, BigInteger> allowedRaces,
                           KeyValuePair<int, BigInteger> allowedColors,
                           KeyValuePair<int, BigInteger> allowedAIBuilds,
                           KeyValuePair<int, BigInteger> allowedDifficulty,
                           KeyValuePair<int, BigInteger> allowedObserveTypes,
                           KeyValuePair<int, BigInteger> allowedControls)
    {
        AllowedRaces = allowedRaces;
        AllowedColors = allowedColors;
        AllowedAIBuilds = allowedAIBuilds;
        AllowedDifficulty = allowedDifficulty;
        AllowedObserveTypes = allowedObserveTypes;
        AllowedControls = allowedControls;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SlotDescription()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>InitData SlotDescription AllowedRaces</summary>
    ///
    public KeyValuePair<int, BigInteger> AllowedRaces { get; init; }
    /// <summary>InitData SlotDescription AllowedColors</summary>
    ///
    public KeyValuePair<int, BigInteger> AllowedColors { get; init; }
    /// <summary>InitData SlotDescription AllowedAIBuilds</summary>
    ///
    public KeyValuePair<int, BigInteger> AllowedAIBuilds { get; init; }
    /// <summary>InitData SlotDescription AllowedDifficulty</summary>
    ///
    public KeyValuePair<int, BigInteger> AllowedDifficulty { get; init; }
    /// <summary>InitData SlotDescription AllowedObserveTypes</summary>
    ///
    public KeyValuePair<int, BigInteger> AllowedObserveTypes { get; init; }
    /// <summary>InitData SlotDescription AllowedControls</summary>
    ///
    public KeyValuePair<int, BigInteger> AllowedControls { get; init; }
}

/// <summary>Record <c>GameDescription</c> Parsed UserInitialData GameDescription</summary>
///
public sealed record GameDescription
{
    /// <summary>Record <c>GameDescription</c> constructor</summary>
    ///
    public GameDescription(int maxRaces,
                           int maxTeams,
                           bool hasExtensionMod,
                           int maxColors,
                           bool isBlizzardMap,
                           GameOptions gameOptions,
                           int defaultDifficulty,
                           bool isCoopMode,
                           string mapFileName,
                           int defaultAIBuild,
                           int gameType,
                           bool hasNonBlizzardExtensionMod,
                           long randomValue,
                           int maxObservers,
                           bool isRealtimeMode,
                           int maxUsers,
                           long modFileSyncChecksum,
                           long mapFileSyncChecksum,
                           int maxPlayers,
                           ICollection<string> cacheHandles,
                           int gameSpeed,
                           int maxControls,
                           string gameCacheName,
                           string mapAuthorName,
                           ICollection<SlotDescription> slotDescriptions,
                           int mapSizeY,
                           int mapSizeX,
                           bool isPremadeFFA)
    {
        MaxRaces = maxRaces;
        MaxTeams = maxTeams;
        HasExtensionMod = hasExtensionMod;
        MaxColors = maxColors;
        IsBlizzardMap = isBlizzardMap;
        GameOptions = gameOptions;
        DefaultDifficulty = defaultDifficulty;
        IsCoopMode = isCoopMode;
        MapFileName = mapFileName;
        DefaultAIBuild = defaultAIBuild;
        GameType = gameType;
        HasNonBlizzardExtensionMod = hasNonBlizzardExtensionMod;
        RandomValue = randomValue;
        MaxObservers = maxObservers;
        IsRealtimeMode = isRealtimeMode;
        MaxUsers = maxUsers;
        ModFileSyncChecksum = modFileSyncChecksum;
        MapFileSyncChecksum = mapFileSyncChecksum;
        MaxPlayers = maxPlayers;
        CacheHandles = cacheHandles;
        GameSpeed = gameSpeed;
        MaxControls = maxControls;
        GameCacheName = gameCacheName;
        MapAuthorName = mapAuthorName;
        SlotDescriptions = slotDescriptions;
        MapSizeY = mapSizeY;
        MapSizeX = mapSizeX;
        IsPremadeFFA = isPremadeFFA;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public GameDescription()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>InitData GameDescription MaxRaces</summary>
    ///
    public int MaxRaces { get; init; }
    /// <summary>InitData GameDescription MaxTeams</summary>
    ///
    public int MaxTeams { get; init; }
    /// <summary>InitData GameDescription HasExtensionMod</summary>
    ///
    public bool HasExtensionMod { get; init; }
    /// <summary>InitData GameDescription MaxColors</summary>
    ///
    public int MaxColors { get; init; }
    /// <summary>InitData GameDescription IsBlizzardMap</summary>
    ///
    public bool IsBlizzardMap { get; init; }
    /// <summary>InitData GameDescription GameOptions</summary>
    ///
    public GameOptions GameOptions { get; init; }
    /// <summary>InitData GameDescription DefaultDifficulty</summary>
    ///
    public int DefaultDifficulty { get; init; }
    /// <summary>InitData GameDescription IsCoopMode</summary>
    ///
    public bool IsCoopMode { get; init; }
    /// <summary>InitData GameDescription MapFileName</summary>
    ///
    public string MapFileName { get; init; }
    /// <summary>InitData GameDescription DefaultAIBuild</summary>
    ///
    public int DefaultAIBuild { get; init; }
    /// <summary>InitData GameDescription GameType</summary>
    ///
    public int GameType { get; init; }
    /// <summary>InitData GameDescription HasNonBlizzardExtensionMod</summary>
    ///
    public bool HasNonBlizzardExtensionMod { get; init; }
    /// <summary>InitData GameDescription RandomValue</summary>
    ///
    public long RandomValue { get; init; }
    /// <summary>InitData GameDescription MaxObservers</summary>
    ///
    public int MaxObservers { get; init; }
    /// <summary>InitData GameDescription IsRealtimeMode</summary>
    ///
    public bool IsRealtimeMode { get; init; }
    /// <summary>InitData GameDescription MaxUsers</summary>
    ///
    public int MaxUsers { get; init; }
    /// <summary>InitData GameDescription ModFileSyncChecksum</summary>
    ///
    public long ModFileSyncChecksum { get; init; }
    /// <summary>InitData GameDescription MapFileSyncChecksum</summary>
    ///
    public long MapFileSyncChecksum { get; init; }
    /// <summary>InitData GameDescription MaxPlayers</summary>
    ///
    public int MaxPlayers { get; init; }
    /// <summary>InitData GameDescription CacheHandles</summary>
    ///
    public ICollection<string> CacheHandles { get; init; }
    /// <summary>InitData GameDescription GameSpeed</summary>
    ///
    public int GameSpeed { get; init; }
    /// <summary>InitData GameDescription MaxControls</summary>
    ///
    public int MaxControls { get; init; }
    /// <summary>InitData GameDescription GameCacheName</summary>
    ///
    public string GameCacheName { get; init; }
    /// <summary>InitData GameDescription MapAuthorName</summary>
    ///
    public string MapAuthorName { get; init; }
    /// <summary>InitData GameDescription SlotDescriptions</summary>
    ///
    public ICollection<SlotDescription> SlotDescriptions { get; init; }
    /// <summary>InitData GameDescription MapSizeY</summary>
    ///
    public int MapSizeY { get; init; }
    /// <summary>InitData GameDescription MapSizeX</summary>
    ///
    public int MapSizeX { get; init; }
    /// <summary>InitData GameDescription IsPremadeFFA</summary>
    ///
    public bool IsPremadeFFA { get; init; }
}


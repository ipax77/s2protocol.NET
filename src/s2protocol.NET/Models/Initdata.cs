using System.Numerics;

namespace s2protocol.NET.Models;

/// <summary>Record <c>Initdata</c> Parsed replay Initdata (m_syncLobbyState)</summary>
///
/// <remarks>Record <c>SyncLobbyState</c> constructor</remarks>
///
public sealed class Initdata(ICollection<UserInitialData> userInitialData, LobbyState lobbyState, GameDescription gameDescription)
{
    /// <summary>InitData SyncLobbyState UserInitialData</summary>
    ///
    public ICollection<UserInitialData> UserInitialData { get; init; } = userInitialData;
    /// <summary>InitData SyncLobbyState LobbyState</summary>
    ///
    public LobbyState LobbyState { get; init; } = lobbyState;
    /// <summary>InitData SyncLobbyState GameDescription</summary>
    ///
    public GameDescription GameDescription { get; init; } = gameDescription;
}

/// <summary>Record <c>UserInitialData</c> Parsed UserInitialData</summary>
///
/// <remarks>Record <c>UserInitialData</c> constructor</remarks>
///
public sealed class UserInitialData(string mount,
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

    /// <summary>InitData Mount</summary>
    ///
    public string Mount { get; init; } = mount;
    /// <summary>InitData Skin</summary>
    ///
    public string Skin { get; init; } = skin;
    /// <summary>InitData Observe</summary>
    ///
    public int Observe { get; init; } = observe;
    /// <summary>InitData TeamPreference</summary>
    ///
    public int? TeamPreference { get; init; } = teamPreference;
    /// <summary>InitData ToonHandle</summary>
    ///
    public string ToonHandle { get; init; } = toonHandle;
    /// <summary>InitData CombinedRaceLevels</summary>
    ///
    public long CombinedRaceLevels { get; init; } = combinedRaceLevels;
    /// <summary>InitData HighestLeague</summary>
    ///
    public int HighestLeague { get; init; } = highestLeague;
    /// <summary>InitData ClanTag</summary>
    ///
    public string ClanTag { get; init; } = clanTag;
    /// <summary>InitData TestMap</summary>
    ///
    public bool TestMap { get; init; } = testMap;
    /// <summary>InitData TestAuto</summary>
    ///
    public bool TestAuto { get; init; } = testAuto;
    /// <summary>InitData Examine</summary>
    ///
    public bool Examine { get; init; } = examine;
    /// <summary>InitData TestType</summary>
    ///
    public int TestType { get; init; } = testType;
    /// <summary>InitData CustomInterface</summary>
    ///
    public bool CustomInterface { get; init; } = customInterface;
    /// <summary>InitData ClanLogo</summary>
    ///
    public string ClanLogo { get; init; } = clanLogo;
    /// <summary>InitData Name</summary>
    ///
    public string Name { get; init; } = name;
    /// <summary>InitData RacePreference</summary>
    ///
    public int? RacePreference { get; init; } = racePreference;
    /// <summary>InitData RandomSeed</summary>
    ///
    public int RandomSeed { get; init; } = randomSeed;
    /// <summary>InitData Hero</summary>
    ///
    public string Hero { get; init; } = hero;
    /// <summary>InitData ScaledRating</summary>
    ///
    public long? ScaledRating { get; init; } = scaledRating;
}

/// <summary>Record <c>Slot</c> Parsed UserInitialData Slot</summary>
///
/// <remarks>Record <c>Slot</c> constructor</remarks>
///
public sealed class Slot(int aCEnemyRace,
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
    /// <summary>InitData Slot ACEnemyRace</summary>
    ///
    public int ACEnemyRace { get; init; } = aCEnemyRace;
    /// <summary>InitData Slot ToonHandle</summary>
    ///
    public string ToonHandle { get; init; } = toonHandle;
    /// <summary>InitData Slot RewardOverrides</summary>
    ///
    public ICollection<int> RewardOverrides { get; init; } = rewardOverrides;
    /// <summary>InitData Slot UserId</summary>
    ///
    public int? UserId { get; init; } = userId;
    /// <summary>InitData Slot Skin</summary>
    ///
    public string Skin { get; init; } = skin;
    /// <summary>InitData Slot CommanderMasteryTalents</summary>
    ///
    public ICollection<int> CommanderMasteryTalents { get; init; } = commanderMasteryTalents;
    /// <summary>InitData Slot AiBuild</summary>
    ///
    public int AiBuild { get; init; } = aiBuild;
    /// <summary>InitData Slot TeamId</summary>
    ///
    public int TeamId { get; init; } = teamId;
    /// <summary>InitData Slot Rewards</summary>
    ///
    public ICollection<int> Rewards { get; init; } = rewards;
    /// <summary>InitData Slot CommanderLevel</summary>
    ///
    public int CommanderLevel { get; init; } = commanderLevel;
    /// <summary>InitData Slot LogoIndex</summary>
    ///
    public int LogoIndex { get; init; } = logoIndex;
    /// <summary>InitData Slot Artifacts</summary>
    ///
    public ICollection<string> Artifacts { get; init; } = artifacts;
    /// <summary>InitData Slot Difficulty</summary>
    ///
    public int Difficulty { get; init; } = difficulty;
    /// <summary>InitData Slot TandemLeaderId</summary>
    ///
    public int? TandemLeaderId { get; init; } = tandemLeaderId;
    /// <summary>InitData Slot CommanderMasteryLevel</summary>
    ///
    public int CommanderMasteryLevel { get; init; } = commanderMasteryLevel;
    /// <summary>InitData Slot TrophyId</summary>
    ///
    public int TrophyId { get; init; } = trophyId;
    /// <summary>InitData Slot BrutalPlusDifficulty</summary>
    ///
    public int BrutalPlusDifficulty { get; init; } = brutalPlusDifficulty;
    /// <summary>InitData Slot RacePref</summary>
    ///
    public int? RacePref { get; init; } = racePref;
    /// <summary>InitData Slot TandemId</summary>
    ///
    public int? TandemId { get; init; } = tandemId;
    /// <summary>InitData Slot Hero</summary>
    ///
    public string Hero { get; init; } = hero;
    /// <summary>InitData Slot Commander</summary>
    ///
    public string Commander { get; init; } = commander;
    /// <summary>InitData Slot Mount</summary>
    ///
    public string Mount { get; init; } = mount;
    /// <summary>InitData Slot Handicap</summary>
    ///
    public int Handicap { get; init; } = handicap;
    /// <summary>InitData Slot Observe</summary>
    ///
    public int Observe { get; init; } = observe;
    /// <summary>InitData Slot ACEnemyWaveType</summary>
    ///
    public int ACEnemyWaveType { get; init; } = aCEnemyWaveType;
    /// <summary>InitData Slot Control</summary>
    ///
    public int Control { get; init; } = control;
    /// <summary>InitData Slot Licenses</summary>
    ///
    public ICollection<int> Licenses { get; init; } = licenses;
    /// <summary>InitData Slot ColorPref</summary>
    ///
    public int? ColorPref { get; init; } = colorPref;
    /// <summary>InitData Slot HasSilencePenalty</summary>
    ///
    public bool HasSilencePenalty { get; init; } = hasSilencePenalty;
    /// <summary>InitData Slot WorkingSetSlotId</summary>
    ///
    public int WorkingSetSlotId { get; init; } = workingSetSlotId;
    /// <summary>InitData Slot RetryMutationIndexes</summary>
    ///
    public ICollection<int> RetryMutationIndexes { get; init; } = retryMutationIndexes;
    /// <summary>InitData Slot SelectedCommanderPrestige</summary>
    ///
    public int SelectedCommanderPrestige { get; init; } = selectedCommanderPrestige;
}

/// <summary>Record <c>LobbyState</c> Parsed UserInitialData LobbyState</summary>
///
/// <remarks>Record <c>LobbyState</c> constructor</remarks>
///
public sealed class LobbyState(int maxUsers,
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
    /// <summary>InitData LobbyState MaxUsers</summary>
    ///
    public int MaxUsers { get; init; } = maxUsers;
    /// <summary>InitData LobbyState Slots</summary>
    ///
    public ICollection<Slot> Slots { get; init; } = slots;
    /// <summary>InitData LobbyState DefaultDifficulty</summary>
    ///
    public int DefaultDifficulty { get; init; } = defaultDifficulty;
    /// <summary>InitData LobbyState IsSinglePlayer</summary>
    ///
    public bool IsSinglePlayer { get; init; } = isSinglePlayer;
    /// <summary>InitData LobbyState Phase</summary>
    ///
    public int Phase { get; init; } = phase;
    /// <summary>InitData LobbyState HostUserId</summary>
    ///
    public int? HostUserId { get; init; } = hostUserId;
    /// <summary>InitData LobbyState MaxObservers</summary>
    ///
    public int MaxObservers { get; init; } = maxObservers;
    /// <summary>InitData LobbyState DefaultAIBuild</summary>
    ///
    public int DefaultAIBuild { get; init; } = defaultAIBuild;
    /// <summary>InitData LobbyState PickedMapTag</summary>
    ///
    public int PickedMapTag { get; init; } = pickedMapTag;
    /// <summary>InitData LobbyState RandomSeed</summary>
    ///
    public long RandomSeed { get; init; } = randomSeed;
    /// <summary>InitData LobbyState GameDuration</summary>
    ///
    public int GameDuration { get; init; } = gameDuration;
}

/// <summary>Record <c>GameOptions</c> Parsed UserInitialData GameOptions</summary>
///
/// <remarks>Record <c>GameOptions</c> constructor</remarks>
///
public sealed class GameOptions(bool competitive,
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
    /// <summary>InitData GameOptions Competitive</summary>
    ///
    public bool Competitive { get; init; } = competitive;
    /// <summary>InitData GameOptions Practice</summary>
    ///
    public bool Practice { get; init; } = practice;
    /// <summary>InitData GameOptions LockTeams</summary>
    ///
    public bool LockTeams { get; init; } = lockTeams;
    /// <summary>InitData GameOptions Amm</summary>
    ///
    public bool Amm { get; init; } = amm;
    /// <summary>InitData GameOptions BattleNet</summary>
    ///
    public bool BattleNet { get; init; } = battleNet;
    /// <summary>InitData GameOptions Fog</summary>
    ///
    public int Fog { get; init; } = fog;
    /// <summary>InitData GameOptions NoVictoryOrDefeat</summary>
    ///
    public bool NoVictoryOrDefeat { get; init; } = noVictoryOrDefeat;
    /// <summary>InitData GameOptions HeroDuplicatesAllowed</summary>
    ///
    public bool HeroDuplicatesAllowed { get; init; } = heroDuplicatesAllowed;
    /// <summary>InitData GameOptions UserDifficulty</summary>
    ///
    public int UserDifficulty { get; init; } = userDifficulty;
    /// <summary>InitData GameOptions AdvancedSharedControl</summary>
    ///
    public bool AdvancedSharedControl { get; init; } = advancedSharedControl;
    /// <summary>InitData GameOptions Cooperative</summary>
    ///
    public bool Cooperative { get; init; } = cooperative;
    /// <summary>InitData GameOptions ClientDebugFlags</summary>
    ///
    public long ClientDebugFlags { get; init; } = clientDebugFlags;
    /// <summary>InitData GameOptions Observers</summary>
    ///
    public int Observers { get; init; } = observers;
    /// <summary>InitData GameOptions TeamsTogether</summary>
    ///
    public bool TeamsTogether { get; init; } = teamsTogether;
    /// <summary>InitData GameOptions RandomRaces</summary>
    ///
    public bool RandomRaces { get; init; } = randomRaces;
    /// <summary>InitData GameOptions BuildCoachEnabled</summary>
    ///
    public bool BuildCoachEnabled { get; init; } = buildCoachEnabled;
}

/// <summary>Record <c>SlotDescription</c> Parsed UserInitialData SlotDescription</summary>
///
/// <remarks>Record <c>SlotDescription</c> constructor</remarks>
///
public sealed class SlotDescription(KeyValuePair<int, BigInteger> allowedRaces,
                       KeyValuePair<int, BigInteger> allowedColors,
                       KeyValuePair<int, BigInteger> allowedAIBuilds,
                       KeyValuePair<int, BigInteger> allowedDifficulty,
                       KeyValuePair<int, BigInteger> allowedObserveTypes,
                       KeyValuePair<int, BigInteger> allowedControls)
{

    /// <summary>InitData SlotDescription AllowedRaces</summary>
    ///
    public KeyValuePair<int, BigInteger> AllowedRaces { get; init; } = allowedRaces;
    /// <summary>InitData SlotDescription AllowedColors</summary>
    ///
    public KeyValuePair<int, BigInteger> AllowedColors { get; init; } = allowedColors;
    /// <summary>InitData SlotDescription AllowedAIBuilds</summary>
    ///
    public KeyValuePair<int, BigInteger> AllowedAIBuilds { get; init; } = allowedAIBuilds;
    /// <summary>InitData SlotDescription AllowedDifficulty</summary>
    ///
    public KeyValuePair<int, BigInteger> AllowedDifficulty { get; init; } = allowedDifficulty;
    /// <summary>InitData SlotDescription AllowedObserveTypes</summary>
    ///
    public KeyValuePair<int, BigInteger> AllowedObserveTypes { get; init; } = allowedObserveTypes;
    /// <summary>InitData SlotDescription AllowedControls</summary>
    ///
    public KeyValuePair<int, BigInteger> AllowedControls { get; init; } = allowedControls;
}

/// <summary>Record <c>GameDescription</c> Parsed UserInitialData GameDescription</summary>
///
/// <remarks>Record <c>GameDescription</c> constructor</remarks>
///
public sealed class GameDescription(int maxRaces,
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

    /// <summary>InitData GameDescription MaxRaces</summary>
    ///
    public int MaxRaces { get; init; } = maxRaces;
    /// <summary>InitData GameDescription MaxTeams</summary>
    ///
    public int MaxTeams { get; init; } = maxTeams;
    /// <summary>InitData GameDescription HasExtensionMod</summary>
    ///
    public bool HasExtensionMod { get; init; } = hasExtensionMod;
    /// <summary>InitData GameDescription MaxColors</summary>
    ///
    public int MaxColors { get; init; } = maxColors;
    /// <summary>InitData GameDescription IsBlizzardMap</summary>
    ///
    public bool IsBlizzardMap { get; init; } = isBlizzardMap;
    /// <summary>InitData GameDescription GameOptions</summary>
    ///
    public GameOptions GameOptions { get; init; } = gameOptions;
    /// <summary>InitData GameDescription DefaultDifficulty</summary>
    ///
    public int DefaultDifficulty { get; init; } = defaultDifficulty;
    /// <summary>InitData GameDescription IsCoopMode</summary>
    ///
    public bool IsCoopMode { get; init; } = isCoopMode;
    /// <summary>InitData GameDescription MapFileName</summary>
    ///
    public string MapFileName { get; init; } = mapFileName;
    /// <summary>InitData GameDescription DefaultAIBuild</summary>
    ///
    public int DefaultAIBuild { get; init; } = defaultAIBuild;
    /// <summary>InitData GameDescription GameType</summary>
    ///
    public int GameType { get; init; } = gameType;
    /// <summary>InitData GameDescription HasNonBlizzardExtensionMod</summary>
    ///
    public bool HasNonBlizzardExtensionMod { get; init; } = hasNonBlizzardExtensionMod;
    /// <summary>InitData GameDescription RandomValue</summary>
    ///
    public long RandomValue { get; init; } = randomValue;
    /// <summary>InitData GameDescription MaxObservers</summary>
    ///
    public int MaxObservers { get; init; } = maxObservers;
    /// <summary>InitData GameDescription IsRealtimeMode</summary>
    ///
    public bool IsRealtimeMode { get; init; } = isRealtimeMode;
    /// <summary>InitData GameDescription MaxUsers</summary>
    ///
    public int MaxUsers { get; init; } = maxUsers;
    /// <summary>InitData GameDescription ModFileSyncChecksum</summary>
    ///
    public long ModFileSyncChecksum { get; init; } = modFileSyncChecksum;
    /// <summary>InitData GameDescription MapFileSyncChecksum</summary>
    ///
    public long MapFileSyncChecksum { get; init; } = mapFileSyncChecksum;
    /// <summary>InitData GameDescription MaxPlayers</summary>
    ///
    public int MaxPlayers { get; init; } = maxPlayers;
    /// <summary>InitData GameDescription CacheHandles</summary>
    ///
    public ICollection<string> CacheHandles { get; init; } = cacheHandles;
    /// <summary>InitData GameDescription GameSpeed</summary>
    ///
    public int GameSpeed { get; init; } = gameSpeed;
    /// <summary>InitData GameDescription MaxControls</summary>
    ///
    public int MaxControls { get; init; } = maxControls;
    /// <summary>InitData GameDescription GameCacheName</summary>
    ///
    public string GameCacheName { get; init; } = gameCacheName;
    /// <summary>InitData GameDescription MapAuthorName</summary>
    ///
    public string MapAuthorName { get; init; } = mapAuthorName;
    /// <summary>InitData GameDescription SlotDescriptions</summary>
    ///
    public ICollection<SlotDescription> SlotDescriptions { get; init; } = slotDescriptions;
    /// <summary>InitData GameDescription MapSizeY</summary>
    ///
    public int MapSizeY { get; init; } = mapSizeY;
    /// <summary>InitData GameDescription MapSizeX</summary>
    ///
    public int MapSizeX { get; init; } = mapSizeX;
    /// <summary>InitData GameDescription IsPremadeFFA</summary>
    ///
    public bool IsPremadeFFA { get; init; } = isPremadeFFA;
}


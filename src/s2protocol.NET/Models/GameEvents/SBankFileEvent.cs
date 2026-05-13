namespace s2protocol.NET.Models;
/// <summary>Record <c>SUserOptionsEvent</c> SUserOptionsEvent</summary>
///
/// <remarks>Record <c>SUserOptionsEvent</c> constructor</remarks>
///
public sealed class SUserOptionsEvent(int userId,
    int eventId,
    int bits,
    int gameloop,
    bool testCheatsEnabled,
                         bool multiplayerCheatsEnabled,
                         bool gameFullyDownloaded,
                         string hotkeyProfile,
                         bool useGalaxyAsserts,
                         bool debugPauseEnabled,
                         bool cameraFollow,
                         bool isMapToMapTransition,
                         int buildNum,
                         int versionFlags,
                         bool developmentCheatsEnabled,
                         bool platformMac,
                         int baseBuildNum,
                         bool syncChecksummingEnabled) : GameEvent(userId, eventId, GameEventType.SUserOptionsEvent, bits, gameloop)
{



    /// <summary>Event TestCheatsEnabled</summary>
    ///
    public bool TestCheatsEnabled { get; } = testCheatsEnabled;
    /// <summary>Event MultiplayerCheatsEnabled</summary>
    ///
    public bool MultiplayerCheatsEnabled { get; } = multiplayerCheatsEnabled;
    /// <summary>Event GameFullyDownloaded</summary>
    ///
    public bool GameFullyDownloaded { get; } = gameFullyDownloaded;
    /// <summary>Event HotkeyProfile</summary>
    ///
    public string HotkeyProfile { get; } = hotkeyProfile;
    /// <summary>Event UseGalaxyAsserts</summary>
    ///
    public bool UseGalaxyAsserts { get; } = useGalaxyAsserts;
    /// <summary>Event DebugPauseEnabled</summary>
    ///
    public bool DebugPauseEnabled { get; } = debugPauseEnabled;
    /// <summary>Event CameraFollow</summary>
    ///
    public bool CameraFollow { get; } = cameraFollow;
    /// <summary>Event IsMapToMapTransition</summary>
    ///
    public bool IsMapToMapTransition { get; } = isMapToMapTransition;
    /// <summary>Event BuildNum</summary>
    ///
    public int BuildNum { get; } = buildNum;
    /// <summary>Event VersionFlags</summary>
    ///
    public int VersionFlags { get; } = versionFlags;
    /// <summary>Event DevelopmentCheatsEnabled</summary>
    ///
    public bool DevelopmentCheatsEnabled { get; } = developmentCheatsEnabled;
    /// <summary>Event PlatformMac</summary>
    ///
    public bool PlatformMac { get; } = platformMac;
    /// <summary>Event BaseBuildNum</summary>
    ///
    public int BaseBuildNum { get; } = baseBuildNum;
    /// <summary>Event SyncChecksummingEnabled</summary>
    ///
    public bool SyncChecksummingEnabled { get; } = syncChecksummingEnabled;

}
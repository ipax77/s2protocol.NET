namespace s2protocol.NET.Models;
/// <summary>Record <c>SUserOptionsEvent</c> SUserOptionsEvent</summary>
///
public sealed class SUserOptionsEvent : GameEvent
{
    /// <summary>Record <c>SUserOptionsEvent</c> constructor</summary>
    ///
    public SUserOptionsEvent(int userId,
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
                             bool syncChecksummingEnabled) : base(userId, eventId, GameEventType.SUserOptionsEvent, bits, gameloop)
    {
        TestCheatsEnabled = testCheatsEnabled;
        MultiplayerCheatsEnabled = multiplayerCheatsEnabled;
        GameFullyDownloaded = gameFullyDownloaded;
        HotkeyProfile = hotkeyProfile;
        UseGalaxyAsserts = useGalaxyAsserts;
        DebugPauseEnabled = debugPauseEnabled;
        CameraFollow = cameraFollow;
        IsMapToMapTransition = isMapToMapTransition;
        BuildNum = buildNum;
        VersionFlags = versionFlags;
        DevelopmentCheatsEnabled = developmentCheatsEnabled;
        PlatformMac = platformMac;
        BaseBuildNum = baseBuildNum;
        SyncChecksummingEnabled = syncChecksummingEnabled;
    }



    /// <summary>Event TestCheatsEnabled</summary>
    ///
    public bool TestCheatsEnabled { get; }
    /// <summary>Event MultiplayerCheatsEnabled</summary>
    ///
    public bool MultiplayerCheatsEnabled { get; }
    /// <summary>Event GameFullyDownloaded</summary>
    ///
    public bool GameFullyDownloaded { get; }
    /// <summary>Event HotkeyProfile</summary>
    ///
    public string HotkeyProfile { get; }
    /// <summary>Event UseGalaxyAsserts</summary>
    ///
    public bool UseGalaxyAsserts { get; }
    /// <summary>Event DebugPauseEnabled</summary>
    ///
    public bool DebugPauseEnabled { get; }
    /// <summary>Event CameraFollow</summary>
    ///
    public bool CameraFollow { get; }
    /// <summary>Event IsMapToMapTransition</summary>
    ///
    public bool IsMapToMapTransition { get; }
    /// <summary>Event BuildNum</summary>
    ///
    public int BuildNum { get; }
    /// <summary>Event VersionFlags</summary>
    ///
    public int VersionFlags { get; }
    /// <summary>Event DevelopmentCheatsEnabled</summary>
    ///
    public bool DevelopmentCheatsEnabled { get; }
    /// <summary>Event PlatformMac</summary>
    ///
    public bool PlatformMac { get; }
    /// <summary>Event BaseBuildNum</summary>
    ///
    public int BaseBuildNum { get; }
    /// <summary>Event SyncChecksummingEnabled</summary>
    ///
    public bool SyncChecksummingEnabled { get; }

}
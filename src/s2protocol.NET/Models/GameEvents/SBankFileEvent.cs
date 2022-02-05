using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>SUserOptionsEvent</c> SUserOptionsEvent</summary>
///
public record SUserOptionsEvent : GameEvent
{
    /// <summary>Record <c>SUserOptionsEvent</c> constructor</summary>
    ///
    public SUserOptionsEvent(GameEvent gameEvent,
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
                             bool syncChecksummingEnabled) : base(gameEvent)
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

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SUserOptionsEvent()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        
    }



    /// <summary>Event TestCheatsEnabled</summary>
    ///
    public bool TestCheatsEnabled { get; init; }
    /// <summary>Event MultiplayerCheatsEnabled</summary>
    ///
    public bool MultiplayerCheatsEnabled { get; init; }
    /// <summary>Event GameFullyDownloaded</summary>
    ///
    public bool GameFullyDownloaded { get; init; }
    /// <summary>Event HotkeyProfile</summary>
    ///
    public string HotkeyProfile { get; init; }
    /// <summary>Event UseGalaxyAsserts</summary>
    ///
    public bool UseGalaxyAsserts { get; init; }
    /// <summary>Event DebugPauseEnabled</summary>
    ///
    public bool DebugPauseEnabled { get; init; }
    /// <summary>Event CameraFollow</summary>
    ///
    public bool CameraFollow { get; init; }
    /// <summary>Event IsMapToMapTransition</summary>
    ///
    public bool IsMapToMapTransition { get; init; }
    /// <summary>Event BuildNum</summary>
    ///
    public int BuildNum { get; init; }
    /// <summary>Event VersionFlags</summary>
    ///
    public int VersionFlags { get; init; }
    /// <summary>Event DevelopmentCheatsEnabled</summary>
    ///
    public bool DevelopmentCheatsEnabled { get; init; }
    /// <summary>Event PlatformMac</summary>
    ///
    public bool PlatformMac { get; init; }
    /// <summary>Event BaseBuildNum</summary>
    ///
    public int BaseBuildNum { get; init; }
    /// <summary>Event SyncChecksummingEnabled</summary>
    ///
    public bool SyncChecksummingEnabled { get; init; }

}
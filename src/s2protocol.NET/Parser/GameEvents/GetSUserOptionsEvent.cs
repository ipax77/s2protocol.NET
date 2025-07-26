using s2protocol.NET.Models;

namespace s2protocol.NET.Parser; internal static partial class Parse
{
    private static SUserOptionsEvent GetSUserOptionsEvent(Dictionary<string, object> gameDic, GameEvent gameEvent)
    {
        bool testCheatsEnabled = GetBool(gameDic, "m_testCheatsEnabled");
        bool multiplayerCheatsEnabled = GetBool(gameDic, "m_multiplayerCheatsEnabled");
        bool gameFullyDownloaded = GetBool(gameDic, "m_gameFullyDownloaded");
        string hotkeyProfile = GetString(gameDic, "m_hotkeyProfile");
        bool useGalaxyAsserts = GetBool(gameDic, "m_useGalaxyAsserts");
        bool debugPauseEnabled = GetBool(gameDic, "m_debugPauseEnabled");
        bool cameraFollow = GetBool(gameDic, "m_cameraFollow");
        bool isMapToMapTransition = GetBool(gameDic, "m_isMapToMapTransition");
        int buildNum = GetInt(gameDic, "m_buildNum");
        int versionFlags = GetInt(gameDic, "m_versionFlags");
        bool developmentCheatsEnabled = GetBool(gameDic, "m_developmentCheatsEnabled");
        bool platformMac = GetBool(gameDic, "m_platformMac");
        int baseBuildNum = GetInt(gameDic, "m_baseBuildNum");
        bool syncChecksummingEnabled = GetBool(gameDic, "m_syncChecksummingEnabled");
        return new SUserOptionsEvent(gameEvent,
                                     testCheatsEnabled,
                                     multiplayerCheatsEnabled,
                                     gameFullyDownloaded,
                                     hotkeyProfile,
                                     useGalaxyAsserts,
                                     debugPauseEnabled,
                                     cameraFollow,
                                     isMapToMapTransition,
                                     buildNum,
                                     versionFlags,
                                     developmentCheatsEnabled,
                                     platformMac,
                                     baseBuildNum,
                                     syncChecksummingEnabled);
    }
}

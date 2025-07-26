using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal static partial class Parse
{
    private static bool DoGetRootKey;

    internal static Header Header(object header)
    {
        DoGetRootKey = false;
        if (header is not Dictionary<string, object> dic)
        {
            throw new DecodeException("Header is not a Dictionary<string, object>");
        }
        var useScaledTime = GetBool(dic, "m_useScaledTime");
        var signature = GetString(dic, "m_signature");
        (var version, int flags, int build, int basebuild) = GetVersion(dic);
        var elapsed = GetInt(dic, "m_elapsedGameLoops");
        var protocol = GetInt(dic, "m_dataBuildNum");
        var type = GetInt(dic, "m_type");
        var rootKey = GetRootKey(dic);
        return new Header(protocol, elapsed, useScaledTime, version, signature, rootKey, "", type, flags, build, basebuild);
    }

    private static (Version, int, int, int) GetVersion(Dictionary<string, object> pydic)
    {
        var version = new Version();
        int flags = 0;
        int build = 0;
        int basebuild = 0;
        if (pydic.ContainsKey("m_version"))
        {
            if (pydic["m_version"] is Dictionary<string, object> versionDic)
            {
                version = new Version(GetInt(versionDic, "m_major"), GetInt(versionDic, "m_minor"), GetInt(versionDic, "m_revision"));
                flags = GetInt(versionDic, "m_flags");
                build = GetInt(versionDic, "m_build");
                basebuild = GetInt(versionDic, "m_baseBuild");
            }
        }
        return (version, flags, build, basebuild);
    }

    private static string GetRootKey(Dictionary<string, object> pydic)
    {
        if (!DoGetRootKey)
            return "";
        else
            if (pydic.ContainsKey("m_ngdpRootKey"))
        {
            if (pydic["m_ngdpRootKey"] is Dictionary<string, object> rootDic)
            {
                return GetAsciiString(rootDic, "m_data");
            }
        }
        return "";
    }
}

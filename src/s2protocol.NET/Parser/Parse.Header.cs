using IronPython.Runtime;
using s2protocol.NET.Models;

namespace s2protocol.NET.Parser;
internal partial class Parse
{
    private static bool DoGetRootKey;

    internal static Header Header(PythonDictionary header)
    {
        DoGetRootKey = false;

        var useScaledTime = GetBool(header, "m_useScaledTime");
        var signature = GetString(header, "m_signature");
        (var version, int flags, int build, int basebuild) = GetVersion(header);
        var elapsed = GetInt(header, "m_elapsedGameLoops");
        var protocol = GetInt(header, "m_dataBuildNum");
        var type = GetInt(header, "m_type");
        var rootKey = GetRootKey(header);
        return new Header(protocol, elapsed, useScaledTime, version, signature, rootKey, "", type, flags, build, basebuild);
    }

    private static (Version, int, int, int) GetVersion(PythonDictionary pydic)
    {
        var version = new Version();
        int flags = 0;
        int build = 0;
        int basebuild = 0;
        if (pydic.ContainsKey("m_version"))
        {
            if (pydic["m_version"] is PythonDictionary versionDic)
            {
                version = new Version(GetInt(versionDic, "m_major"), GetInt(versionDic, "m_minor"), GetInt(versionDic, "m_revision"));
                flags = GetInt(versionDic, "m_flags");
                build = GetInt(versionDic, "m_build");
                basebuild = GetInt(versionDic, "m_baseBuild");
            }
        }
        return (version, flags, build, basebuild);
    }

    private static string GetRootKey(PythonDictionary pydic)
    {
        if (!DoGetRootKey)
            return "";
        else
            if (pydic.ContainsKey("m_ngdpRootKey"))
        {
            if (pydic["m_ngdpRootKey"] is PythonDictionary rootDic)
            {
                return GetAsciiString(rootDic, "m_data");
            }
        }
        return "";
    }
}

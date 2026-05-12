using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace s2protocol.NET.S2Protocol;

/// <summary>
/// TypeInfoLoader - loading s2protocol versions
/// </summary>
public static partial class TypeInfoLoader
{
    private static readonly Dictionary<int, string> _protocolResourceMap = new();
    private static int[] _protocolVersions = [];
    private static readonly Dictionary<int, S2ProtocolVersion> _typeInfoCache = new();
    private static readonly object _initLock = new();
    private static readonly object _typeInfoCacheLock = new();
    private static int _latestProtocolVersion;
    private static volatile bool _initialized;

    private static void Initialize()
    {
        if (_initialized)
            return;

        lock (_initLock)
        {
            if (_initialized)
                return;

            var assembly = typeof(TypeInfoLoader).Assembly;
            var resourceNames = assembly.GetManifestResourceNames();

            if (resourceNames.Length == 0)
            {
                throw new DecodeException("No embedded resource files found.");
            }

            foreach (var name in resourceNames)
            {
                if (name.StartsWith("s2protocol.NET.Resources.versions.protocol", StringComparison.Ordinal)
                    && name.EndsWith(".json", StringComparison.Ordinal))
                {
                    var version = ExtractVersionNumber(name);
                    if (version > 0 && !_protocolResourceMap.ContainsKey(version))
                    {
                        _protocolResourceMap[version] = name;
                    }
                }
            }

            _protocolVersions = [.. _protocolResourceMap.Keys.Order()];
            _latestProtocolVersion = _protocolVersions.Length > 0 ? _protocolVersions[^1] : 0;

            _initialized = true;
        }
    }

    private static int ExtractVersionNumber(string resourceName)
    {
        var match = ProtocolRegex().Match(resourceName);
        return match.Success ? int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture) : 0;
    }

    private static S2ProtocolVersion LoadResourceContent(string resourceName)
    {
        var assembly = typeof(TypeInfoLoader).Assembly;
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new DecodeException("resource stream was null.");
        }

        var compactVersion = JsonSerializer.Deserialize(
            stream,
            S2ProtocolTypeInfoJsonSerializerContext.Default.CompactProtocolVersion);

        if (compactVersion is null)
        {
            throw new DecodeException($"Protocol resource '{resourceName}' could not be deserialized.");
        }

        return compactVersion.ToProtocolVersion();
    }

    /// <summary>
    /// GetLatestVersion
    /// </summary>
    /// <returns></returns>
    /// <exception cref="DecodeException"></exception>
    public static S2ProtocolVersion GetLatestVersion()
    {
        Initialize();

        if (_latestProtocolVersion == 0)
        {
            throw new DecodeException("No protocol versions found.");
        }

        return LoadTypeInfos(_latestProtocolVersion);
    }

    /// <summary>
    /// Load latest appropriate version
    /// </summary>
    /// <param name="protocolVersion"></param>
    /// <returns></returns>
    public static S2ProtocolVersion LoadTypeInfos(int protocolVersion)
    {
        var usedVersion = ResolveProtocolVersion(protocolVersion);

        lock (_typeInfoCacheLock)
        {
            if (_typeInfoCache.TryGetValue(usedVersion, out var cachedVersion))
            {
                return cachedVersion;
            }
        }

        var resourceName = _protocolResourceMap[usedVersion];
        var version = LoadResourceContent(resourceName);

        foreach (var typeInfo in version.TypeInfos)
        {
            typeInfo.BuildDecoderMetadata();
        }

        lock (_typeInfoCacheLock)
        {
            if (_typeInfoCache.TryGetValue(usedVersion, out var cachedVersion))
            {
                return cachedVersion;
            }

            _typeInfoCache[usedVersion] = version;
        }

        return version;
    }

    private static int ResolveProtocolVersion(int protocolVersion)
    {
        Initialize();

        var index = Array.BinarySearch(_protocolVersions, protocolVersion);
        if (index < 0)
        {
            index = ~index - 1;
        }

        if (index < 0)
        {
            throw new DecodeException("No protocol resource found.");
        }

        return _protocolVersions[index];
    }

    [GeneratedRegex(@"protocol(\d+)\.json")]
    private static partial Regex ProtocolRegex();
}

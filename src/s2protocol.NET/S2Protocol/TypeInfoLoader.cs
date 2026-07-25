using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace s2protocol.NET.S2Protocol;

/// <summary>
/// TypeInfoLoader - loading s2protocol versions
/// </summary>
public static partial class TypeInfoLoader
{
    private const string ResourcePrefix = "s2protocol.NET.Resources.versions.";
    private const string ManifestResourceName = ResourcePrefix + "protocol-versions.json";

    private static readonly Dictionary<int, string> _protocolResourceMap = new();
    private static readonly Dictionary<int, int> _protocolPayloadMap = new();
    private static int[] _protocolVersions = [];
    private static readonly Dictionary<int, S2ProtocolVersion> _typeInfoCache = new();
    private static readonly Dictionary<int, S2ProtocolVersion> _canonicalTypeInfoCache = new();
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

            var manifest = LoadManifest(assembly);
            foreach (var name in resourceNames)
            {
                if (name.StartsWith(ResourcePrefix + "protocol", StringComparison.Ordinal)
                    && name.EndsWith(".json", StringComparison.Ordinal))
                {
                    var version = ExtractVersionNumber(name);
                    if (version > 0 && !_protocolResourceMap.ContainsKey(version))
                    {
                        _protocolResourceMap[version] = name;
                    }
                }
            }

            foreach (var (version, canonicalVersion) in manifest.Versions)
            {
                if (version <= 0 || canonicalVersion <= 0)
                {
                    throw new DecodeException("Protocol version manifest contains an invalid version.");
                }

                if (!_protocolResourceMap.ContainsKey(canonicalVersion))
                {
                    throw new DecodeException(
                        $"Protocol version {version} references missing payload {canonicalVersion}.");
                }

                _protocolPayloadMap.Add(version, canonicalVersion);
            }

            _protocolVersions = [.. _protocolPayloadMap.Keys.Order()];
            _latestProtocolVersion = _protocolVersions.Length > 0 ? _protocolVersions[^1] : 0;

            _initialized = true;
        }
    }

    private static ProtocolVersionManifest LoadManifest(Assembly assembly)
    {
        using var stream = assembly.GetManifestResourceStream(ManifestResourceName);
        if (stream is null)
        {
            throw new DecodeException("Protocol version manifest was not found.");
        }

        var manifest = JsonSerializer.Deserialize(
            stream,
            S2ProtocolTypeInfoJsonSerializerContext.Default.ProtocolVersionManifest);

        if (manifest is null || manifest.Versions.Count == 0)
        {
            throw new DecodeException("Protocol version manifest is empty or invalid.");
        }

        return manifest;
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

        var canonicalVersionNumber = _protocolPayloadMap[usedVersion];
        lock (_typeInfoCacheLock)
        {
            if (_canonicalTypeInfoCache.TryGetValue(canonicalVersionNumber, out var cachedCanonicalVersion))
            {
                return CacheVersion(usedVersion, canonicalVersionNumber, cachedCanonicalVersion);
            }
        }

        var resourceName = _protocolResourceMap[canonicalVersionNumber];
        var canonicalVersion = LoadResourceContent(resourceName);
        foreach (var typeInfo in canonicalVersion.TypeInfos)
        {
            typeInfo.BuildDecoderMetadata();
        }

        lock (_typeInfoCacheLock)
        {
            if (_typeInfoCache.TryGetValue(usedVersion, out var cachedVersion))
            {
                return cachedVersion;
            }

            if (!_canonicalTypeInfoCache.TryGetValue(canonicalVersionNumber, out var cachedCanonicalVersion))
            {
                cachedCanonicalVersion = canonicalVersion;
                _canonicalTypeInfoCache.Add(canonicalVersionNumber, cachedCanonicalVersion);
            }

            return CacheVersion(usedVersion, canonicalVersionNumber, cachedCanonicalVersion);
        }
    }

    private static S2ProtocolVersion CacheVersion(
        int usedVersion,
        int canonicalVersionNumber,
        S2ProtocolVersion canonicalVersion)
    {
        var version = usedVersion == canonicalVersionNumber
            ? canonicalVersion
            : canonicalVersion with { Version = usedVersion };
        _typeInfoCache.Add(usedVersion, version);
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

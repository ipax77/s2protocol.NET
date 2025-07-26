using System.Globalization;
using System.Text.RegularExpressions;

namespace s2protocol.NET.S2Protocol;

/// <summary>
/// TypeInfoLoader - loading s2protocol versions
/// </summary>
public static class TypeInfoLoader
{
    private static readonly Dictionary<int, string> _protocolResourceMap = new();
    private static readonly Dictionary<string, string[]> _resourceContents = new();
    private static readonly object _initLock = new();
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
                    && name.EndsWith(".py", StringComparison.Ordinal))
                {
                    var version = ExtractVersionNumber(name);
                    if (version > 0 && !_protocolResourceMap.ContainsKey(version))
                    {
                        _protocolResourceMap[version] = name;
                    }
                }
            }

            _initialized = true;
        }
    }

    private static string[] GetPythonVersionLines(int version, out int usedVersion)
    {
        Initialize();

        var matchingVersion = _protocolResourceMap.Keys
            .Where(v => v <= version)
            .OrderByDescending(v => v)
            .FirstOrDefault();

        if (matchingVersion == 0)
        {
            throw new DecodeException("No python protocol found.");
        }

        usedVersion = matchingVersion;

        var resourceName = _protocolResourceMap[matchingVersion];

        lock (_resourceContents)
        {
            if (!_resourceContents.TryGetValue(resourceName, out var lines))
            {
                lines = LoadResourceContent(resourceName);
                _resourceContents[resourceName] = lines;
            }

            return lines;
        }
    }

    private static int ExtractVersionNumber(string resourceName)
    {
        var match = Regex.Match(resourceName, @"protocol(\d+)\.py");
        return match.Success ? int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture) : 0;
    }

    private static string[] LoadResourceContent(string resourceName)
    {
        var assembly = typeof(TypeInfoLoader).Assembly;
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new DecodeException("resource stream was null.");
        }

        using var reader = new StreamReader(stream);
        string content = reader.ReadToEnd();
        return content.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);
    }

    /// <summary>
    /// GetLatestVersion
    /// </summary>
    /// <returns></returns>
    /// <exception cref="DecodeException"></exception>
    public static S2ProtocolVersion GetLatestVersion()
    {
        Initialize();

        if (_protocolResourceMap.Count == 0)
        {
            throw new DecodeException("No protocol versions found.");
        }

        var latestVersion = _protocolResourceMap.Keys.Max();
        return LoadTypeInfos(latestVersion);
    }

    /// <summary>
    /// Load latest appropriate version
    /// </summary>
    /// <param name="protocolVersion"></param>
    /// <returns></returns>
    public static S2ProtocolVersion LoadTypeInfos(int protocolVersion)
    {
        var usedVersion = protocolVersion;
        var lines = GetPythonVersionLines(protocolVersion, out usedVersion);
        S2ProtocolVersion version = new()
        {
            Version = usedVersion
        };
        bool inTypeInfos = false;
        bool inGameEventTypes = false;
        bool inMessageEventTypes = false;
        bool inTrackerEventTypes = false;
        int currentTypeId = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (line.StartsWith("typeinfos", StringComparison.Ordinal))
            {
                inTypeInfos = true;
                continue;
            }
            else if (line.StartsWith("game_event_types", StringComparison.Ordinal))
            {
                inGameEventTypes = true;
                ParseEventTypeBlock(lines[i..], version.GameEvents);
                continue;
            }
            else if (line.StartsWith("message_event_types", StringComparison.Ordinal))
            {
                inMessageEventTypes = true;
                ParseEventTypeBlock(lines[i..], version.MessageEvents);
                continue;
            }
            else if (line.StartsWith("tracker_event_types", StringComparison.Ordinal))
            {
                inTrackerEventTypes = true;
                ParseEventTypeBlock(lines[i..], version.TrackerEvents);
                continue;
            }

            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                continue;
            if (line.StartsWith(']'))
            {
                inTypeInfos = false;
            }

            if (line.StartsWith('}'))
            {
                inGameEventTypes = false;
                inMessageEventTypes = false;
                inTrackerEventTypes = false;
            }

            if (inTypeInfos)
            {

                var match = Regex.Match(line, @"\('(\w+)',\s*\[(.+)\]\)");
                string typeName = string.Empty;
                string content = string.Empty;
                if (!match.Success)
                {
                    if (line.Contains("_bool", StringComparison.Ordinal))
                    {
                        // Special case for _bool, which has no content
                        typeName = "_bool";
                        content = string.Empty;
                    }
                    else if (line.Contains("_null", StringComparison.Ordinal))
                    {
                        typeName = "_null";
                    }
                    else if (line.Contains("_fourcc", StringComparison.Ordinal))
                    {
                        typeName = "_fourcc";
                    }
                    else
                    {
                        Console.WriteLine($"[WARN] Skipping unsupported typeinfo line: {line}");
                        continue;
                    }
                }
                else
                {
                    typeName = match.Groups[1].Value;
                    content = match.Groups[2].Value.Trim();
                }

                var s2TypeInfo = new S2TypeInfo(typeName, currentTypeId++);
                if (typeName is "_int" or "_blob" or "_bitarray")
                {
                    // Try to extract bounds: (0, 7)
                    var boundsMatch = Regex.Match(content, @"\((-?\d+),\s*(-?\d+)\)");
                    if (boundsMatch.Success)
                    {
                        long a = long.Parse(boundsMatch.Groups[1].Value, CultureInfo.InvariantCulture);
                        long b = long.Parse(boundsMatch.Groups[2].Value, CultureInfo.InvariantCulture);

                        s2TypeInfo.Elements.Add(new S2TypeInfoTypeElement { Bounds = new(a, b) });
                    }
                }
                else if (typeName == "_choice")
                {
                    var tupleDictMatch = Regex.Match(content, @"\((-?\d+),\s*(-?\d+)\),\s*\{(.+)\}");
                    if (tupleDictMatch.Success)
                    {
                        long a = long.Parse(tupleDictMatch.Groups[1].Value, CultureInfo.InvariantCulture);
                        long b = long.Parse(tupleDictMatch.Groups[2].Value, CultureInfo.InvariantCulture);
                        string dictRaw = tupleDictMatch.Groups[3].Value;

                        s2TypeInfo.Elements.Add(new S2TypeInfoTypeElement { Bounds = new(a, b) });

                        var dict = new Dictionary<int, S2ChoiceElement>();

                        var entryMatches = Regex.Matches(dictRaw, @"(-?\d+):\s*\('([^']+)',\s*(-?\d+)\)");
                        foreach (Match em in entryMatches)
                        {
                            int key = int.Parse(em.Groups[1].Value, CultureInfo.InvariantCulture);
                            string type = em.Groups[2].Value;
                            int number = int.Parse(em.Groups[3].Value, CultureInfo.InvariantCulture);
                            dict[key] = new S2ChoiceElement { TypeName = type, Number = number };
                        }

                        s2TypeInfo.Elements.Add(new DsTypeInfoChoiceElemet { Elements = dict });
                    }
                }
                else if (typeName == "_struct")
                {
                    // Match nested list of tuples like: [('m_flags', 10, 0), ...]
                    var innerListMatch = Regex.Match(content, @"\[\((.*?)\)\]");
                    var mElement = new S2TypeInfoMElement();

                    var fieldMatches = Regex.Matches(content, @"\('([^']+)',\s*(-?\d+),\s*(-?\d+)\)");
                    foreach (Match m in fieldMatches)
                    {
                        string typeField = m.Groups[1].Value;
                        long typeId = long.Parse(m.Groups[2].Value, CultureInfo.InvariantCulture);
                        long tag = long.Parse(m.Groups[3].Value, CultureInfo.InvariantCulture);

                        mElement.Elements.Add(new S2MElement
                        {
                            TypeName = typeField,
                            Bounds = new(typeId, tag)
                        });
                    }

                    s2TypeInfo.Elements.Add(mElement);
                }
                else if (typeName == "_array")
                {
                    // Catch both bounds and typeId
                    var argsMatch = Regex.Match(content, @"\((-?\d+),\s*(-?\d+)\),\s*(-?\d+)");
                    if (argsMatch.Success)
                    {
                        long a = long.Parse(argsMatch.Groups[1].Value, CultureInfo.InvariantCulture);
                        long b = long.Parse(argsMatch.Groups[2].Value, CultureInfo.InvariantCulture);
                        long innerTypeId = long.Parse(argsMatch.Groups[3].Value, CultureInfo.InvariantCulture);

                        s2TypeInfo.Elements.Add(new S2TypeInfoTypeElement { Bounds = new(a, b) });
                        s2TypeInfo.Elements.Add(new S2TypeInfoTypeElement { Bounds = new(innerTypeId, -1) });
                    }
                }
                else if (typeName == "_optional")
                {
                    //     ('_optional',[10]),  #25
                    var optionalMatch = Regex.Match(content, @"(\d+)");
                    if (optionalMatch.Success)
                    {
                        long innerTypeId = long.Parse(optionalMatch.Groups[1].Value, CultureInfo.InvariantCulture);
                        s2TypeInfo.Elements.Add(new S2TypeInfoTypeElement { Bounds = new(innerTypeId, -1) });
                    }
                }
                else if (typeName == "_bool" || typeName == "_null" || typeName == "_fourcc")
                {
                    // No arguments
                }
                else
                {
                    Console.WriteLine($"[WARN] Skipping unsupported typeinfo line: {line}");
                }
                version.TypeInfos.Add(s2TypeInfo);
            }
            else if (!inGameEventTypes && !inMessageEventTypes && !inTrackerEventTypes)
            {
                var idMatch = Regex.Match(line.Trim(), @"(\w+)\s*=\s*(-?\d+)");
                if (idMatch.Success)
                {
                    string key = idMatch.Groups[1].Value;
                    int value = int.Parse(idMatch.Groups[2].Value, CultureInfo.InvariantCulture);

                    switch (key)
                    {
                        case "game_eventid_typeid":
                            version.GameEventIdTypeId = value;
                            break;
                        case "message_eventid_typeid":
                            version.MessageEventIdTypeId = value;
                            break;
                        case "tracker_eventid_typeid":
                            version.TrackerEventIdTypeId = value;
                            break;
                        case "svaruint32_typeid":
                            version.SVarUint32TypeId = value;
                            break;
                        case "replay_userid_typeid":
                            version.ReplayUserIdTypeId = value;
                            break;
                        case "replay_header_typeid":
                            version.ReplayHeaderTypeId = value;
                            break;
                        case "game_details_typeid":
                            version.GameDetailsTypeId = value;
                            break;
                        case "replay_initdata_typeid":
                            version.ReplayInitDataTypeId = value;
                            break;
                    }
                }
            }

        }
        return version;
    }

    private static void ParseEventTypeBlock(IEnumerable<string> lines, Dictionary<int, S2EventType> targetDict)
    {
        foreach (var line in lines)
        {
            if (line.Trim().StartsWith('}'))
                break;

            var match = Regex.Match(line.Trim(), @"(-?\d+):\s*\((-?\d+),\s*'([^']+)'\)");
            if (match.Success)
            {
                int eventId = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                int typeId = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
                string name = match.Groups[3].Value;
                targetDict[eventId] = new S2EventType(typeId, name);
            }
        }
    }
}
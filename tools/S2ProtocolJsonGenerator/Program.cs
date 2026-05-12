using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

var repoRoot = args.Length > 0
    ? Path.GetFullPath(args[0])
    : FindRepoRoot(AppContext.BaseDirectory);

var versionsDirectory = Path.Combine(repoRoot, "src", "s2protocol.NET", "Resources", "versions");
if (!Directory.Exists(versionsDirectory))
{
    throw new DirectoryNotFoundException($"Protocol versions directory not found: {versionsDirectory}");
}

var assembly = Assembly.GetExecutingAssembly();
var inputResources = assembly
    .GetManifestResourceNames()
    .Where(name => name.StartsWith("S2ProtocolJsonGenerator.Resources.versions.protocol", StringComparison.Ordinal)
        && name.EndsWith(".py", StringComparison.Ordinal))
    .Order(StringComparer.Ordinal)
    .ToArray();

foreach (var resourceName in inputResources)
{
    using var resourceStream = assembly.GetManifestResourceStream(resourceName)
        ?? throw new InvalidOperationException($"Protocol resource not found: {resourceName}");
    var compactVersion = ProtocolPythonParser.Parse(resourceName, resourceStream);
    var outputFile = Path.Combine(versionsDirectory, $"protocol{compactVersion.Version}.json");
    using var output = File.Create(outputFile);
    JsonSerializer.Serialize(output, compactVersion, JsonContext.Default.CompactProtocolVersion);
    output.WriteByte((byte)'\n');
}

Console.WriteLine(
    string.Create(
        CultureInfo.InvariantCulture,
        $"Generated {inputResources.Length} compact protocol JSON files in {versionsDirectory}."));

static string FindRepoRoot(string startDirectory)
{
    var directory = new DirectoryInfo(startDirectory);
    while (directory is not null)
    {
        if (File.Exists(Path.Combine(directory.FullName, "s2protocol.NET.sln")))
        {
            return directory.FullName;
        }

        directory = directory.Parent;
    }

    throw new InvalidOperationException("Could not locate repository root.");
}

internal static partial class ProtocolPythonParser
{
    public static CompactProtocolVersion Parse(string resourceName, Stream stream)
    {
        var versionMatch = ProtocolFileRegex().Match(resourceName);
        if (!versionMatch.Success)
        {
            throw new InvalidOperationException($"Could not extract protocol version from {resourceName}.");
        }

        CompactProtocolVersion version = new()
        {
            Version = int.Parse(versionMatch.Groups[1].Value, CultureInfo.InvariantCulture)
        };

        bool inTypeInfos = false;
        bool inGameEventTypes = false;
        bool inMessageEventTypes = false;
        bool inTrackerEventTypes = false;

        using var reader = new StreamReader(stream);
        while (reader.ReadLine() is { } rawLine)
        {
            var line = rawLine.Trim();
            if (line.Length == 0 || line.StartsWith('#'))
            {
                continue;
            }

            if (line.StartsWith("typeinfos", StringComparison.Ordinal))
            {
                inTypeInfos = true;
                continue;
            }

            if (line.StartsWith("game_event_types", StringComparison.Ordinal))
            {
                inGameEventTypes = true;
                continue;
            }

            if (line.StartsWith("message_event_types", StringComparison.Ordinal))
            {
                inMessageEventTypes = true;
                continue;
            }

            if (line.StartsWith("tracker_event_types", StringComparison.Ordinal))
            {
                inTrackerEventTypes = true;
                continue;
            }

            if (line.StartsWith(']'))
            {
                inTypeInfos = false;
                continue;
            }

            if (line.StartsWith('}'))
            {
                inGameEventTypes = false;
                inMessageEventTypes = false;
                inTrackerEventTypes = false;
                continue;
            }

            if (inTypeInfos)
            {
                version.TypeInfos.Add(ParseTypeInfo(line));
                continue;
            }

            if (inGameEventTypes)
            {
                ParseEventType(line, version.GameEvents);
                continue;
            }

            if (inMessageEventTypes)
            {
                ParseEventType(line, version.MessageEvents);
                continue;
            }

            if (inTrackerEventTypes)
            {
                ParseEventType(line, version.TrackerEvents);
                continue;
            }

            ParseRootTypeId(line, version);
        }

        return version;
    }

    private static CompactTypeInfo ParseTypeInfo(string line)
    {
        var match = TypeInfoRegex().Match(line);
        if (!match.Success)
        {
            throw new InvalidOperationException($"Unsupported typeinfo line: {line}");
        }

        var typeName = match.Groups["type"].Value;
        var content = match.Groups["content"].Value;

        CompactTypeInfo typeInfo = new()
        {
            TypeName = typeName
        };

        switch (typeName)
        {
            case "_int":
            case "_blob":
            case "_bitarray":
                typeInfo.Bounds = ParseBounds(content);
                break;

            case "_choice":
                typeInfo.Bounds = ParseBounds(content);
                typeInfo.Choices = ParseChoices(content);
                break;

            case "_struct":
                typeInfo.Fields = ParseFields(content);
                break;

            case "_array":
                typeInfo.Bounds = ParseBounds(content);
                typeInfo.TypeId = ParseArrayTypeId(content);
                break;

            case "_optional":
                typeInfo.TypeId = ParseOptionalTypeId(content);
                break;

            case "_bool":
            case "_null":
            case "_fourcc":
            case "_real32":
            case "_real64":
                break;

            default:
                throw new InvalidOperationException($"Unsupported typeinfo '{typeName}' in line: {line}");
        }

        return typeInfo;
    }

    private static long[] ParseBounds(string content)
    {
        var match = BoundsRegex().Match(content);
        if (!match.Success)
        {
            throw new InvalidOperationException($"Could not parse bounds from: {content}");
        }

        return
        [
            long.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture),
            long.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture)
        ];
    }

    private static int ParseArrayTypeId(string content)
    {
        var match = ArrayRegex().Match(content);
        if (!match.Success)
        {
            throw new InvalidOperationException($"Could not parse array type id from: {content}");
        }

        return int.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);
    }

    private static int ParseOptionalTypeId(string content)
    {
        var match = OptionalRegex().Match(content);
        if (!match.Success)
        {
            throw new InvalidOperationException($"Could not parse optional type id from: {content}");
        }

        return int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
    }

    private static List<CompactField> ParseFields(string content)
    {
        var fields = new List<CompactField>();
        var matches = FieldRegex().Matches(content);
        foreach (Match match in matches)
        {
            fields.Add(new CompactField
            {
                Name = match.Groups[1].Value,
                TypeId = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture),
                Tag = long.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture)
            });
        }

        return fields;
    }

    private static Dictionary<int, CompactChoice> ParseChoices(string content)
    {
        var choices = new Dictionary<int, CompactChoice>();
        var matches = ChoiceEntryRegex().Matches(content);
        foreach (Match match in matches)
        {
            choices[int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture)] = new CompactChoice
            {
                Name = match.Groups[2].Value,
                TypeId = int.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture)
            };
        }

        return choices;
    }

    private static void ParseEventType(string line, Dictionary<int, CompactEventType> target)
    {
        var match = EventTypeRegex().Match(line);
        if (!match.Success)
        {
            return;
        }

        target[int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture)] = new CompactEventType
        {
            TypeId = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture),
            Name = match.Groups[3].Value
        };
    }

    private static void ParseRootTypeId(string line, CompactProtocolVersion version)
    {
        var match = RootTypeIdRegex().Match(line);
        if (!match.Success)
        {
            return;
        }

        var value = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
        switch (match.Groups[1].Value)
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

    [GeneratedRegex(@"protocol(\d+)\.py")]
    private static partial Regex ProtocolFileRegex();

    [GeneratedRegex(@"^\('(?<type>_\w+)',\s*\[(?<content>.*)\]\),\s*#\d+")]
    private static partial Regex TypeInfoRegex();

    [GeneratedRegex(@"\((-?\d+),\s*(-?\d+)\)")]
    private static partial Regex BoundsRegex();

    [GeneratedRegex(@"\((-?\d+),\s*(-?\d+)\),\s*(-?\d+)")]
    private static partial Regex ArrayRegex();

    [GeneratedRegex(@"(-?\d+)")]
    private static partial Regex OptionalRegex();

    [GeneratedRegex(@"\('([^']+)',\s*(-?\d+),\s*(-?\d+)\)")]
    private static partial Regex FieldRegex();

    [GeneratedRegex(@"(-?\d+):\s*\('([^']+)',\s*(-?\d+)\)")]
    private static partial Regex ChoiceEntryRegex();

    [GeneratedRegex(@"(-?\d+):\s*\((-?\d+),\s*'([^']+)'\)")]
    private static partial Regex EventTypeRegex();

    [GeneratedRegex(@"(\w+)\s*=\s*(-?\d+)")]
    private static partial Regex RootTypeIdRegex();
}

internal sealed record CompactProtocolVersion
{
    [JsonPropertyName("v")]
    public int Version { get; init; }
    [JsonPropertyName("t")]
    public List<CompactTypeInfo> TypeInfos { get; init; } = [];
    [JsonPropertyName("ge")]
    public Dictionary<int, CompactEventType> GameEvents { get; init; } = [];
    [JsonPropertyName("me")]
    public Dictionary<int, CompactEventType> MessageEvents { get; init; } = [];
    [JsonPropertyName("te")]
    public Dictionary<int, CompactEventType> TrackerEvents { get; init; } = [];
    [JsonPropertyName("gei")]
    public int? GameEventIdTypeId { get; set; }
    [JsonPropertyName("mei")]
    public int? MessageEventIdTypeId { get; set; }
    [JsonPropertyName("tei")]
    public int? TrackerEventIdTypeId { get; set; }
    [JsonPropertyName("sv")]
    public int? SVarUint32TypeId { get; set; }
    [JsonPropertyName("ru")]
    public int? ReplayUserIdTypeId { get; set; }
    [JsonPropertyName("rh")]
    public int? ReplayHeaderTypeId { get; set; }
    [JsonPropertyName("gd")]
    public int? GameDetailsTypeId { get; set; }
    [JsonPropertyName("ri")]
    public int? ReplayInitDataTypeId { get; set; }
}

internal sealed record CompactTypeInfo
{
    [JsonPropertyName("n")]
    public string TypeName { get; init; } = string.Empty;
    [JsonPropertyName("b")]
    public long[]? Bounds { get; set; }
    [JsonPropertyName("i")]
    public int? TypeId { get; set; }
    [JsonPropertyName("f")]
    public List<CompactField>? Fields { get; set; }
    [JsonPropertyName("c")]
    public Dictionary<int, CompactChoice>? Choices { get; set; }
}

internal sealed record CompactField
{
    [JsonPropertyName("n")]
    public string Name { get; init; } = string.Empty;
    [JsonPropertyName("i")]
    public int TypeId { get; init; }
    [JsonPropertyName("g")]
    public long Tag { get; init; }
}

internal sealed record CompactChoice
{
    [JsonPropertyName("n")]
    public string Name { get; init; } = string.Empty;
    [JsonPropertyName("i")]
    public int TypeId { get; init; }
}

internal sealed record CompactEventType
{
    [JsonPropertyName("i")]
    public int TypeId { get; init; }
    [JsonPropertyName("n")]
    public string Name { get; init; } = string.Empty;
}

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = false)]
[JsonSerializable(typeof(CompactProtocolVersion))]
internal sealed partial class JsonContext : JsonSerializerContext;

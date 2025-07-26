namespace s2protocol.NET.S2Protocol.Models;

internal sealed class ReplayHeader
{
    public string Signature { get; set; } = "";
    public ReplayVersion Version { get; set; } = new();
    public int Type { get; set; }
    public long ElapsedGameLoops { get; set; }
    public bool UseScaledTime { get; set; }
    public BinaryData NgdpRootKey { get; set; } = new();
    public int DataBuildNum { get; set; }
    public BinaryData ReplayCompatibilityHash { get; set; } = new();
    public bool NgdpRootKeyIsDevData { get; set; }
}

internal sealed class ReplayVersion
{
    public int Flags { get; set; }
    public int Major { get; set; }
    public int Minor { get; set; }
    public int Revision { get; set; }
    public int Build { get; set; }
    public int BaseBuild { get; set; }
}

internal sealed class BinaryData
{
    public byte[] Data { get; set; } = Array.Empty<byte>();

    public override string ToString()
    {
        try
        {
            var str = System.Text.Encoding.UTF8.GetString(Data);
            return str.All(c => c >= 0x20 && c <= 0x7E) ? str : Convert.ToBase64String(Data);
        }
        catch (System.Text.DecoderFallbackException)
        {
            return Convert.ToBase64String(Data);
        }
    }
}

internal sealed class ReplayDetails
{
    public List<Player> PlayerList { get; set; } = [];
    public string Title { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public Thumbnail Thumbnail { get; set; } = new();
    public bool IsBlizzardMap { get; set; }
    public long TimeUTC { get; set; }
    public long TimeLocalOffset { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ImageFilePath { get; set; } = string.Empty;
    public string MapFileName { get; set; } = string.Empty;
    public List<string> CacheHandles { get; set; } = [];
    public bool MiniSave { get; set; }
    public int GameSpeed { get; set; }
    public int DefaultDifficulty { get; set; }
    public object? ModPaths { get; set; }
    public int CampaignIndex { get; set; }
    public bool RestartAsTransitionMap { get; set; }
    public bool DisableRecoverGame { get; set; }
    public DateTime GameTimeUtc { get { return DateTime.FromFileTimeUtc(TimeUTC); } }
}

internal sealed class Player
{
    public string Name { get; set; } = string.Empty;
    public Toon Toon { get; set; } = new();
    public string Race { get; set; } = string.Empty;
    public Color Color { get; set; } = new();
    public int Control { get; set; }
    public int TeamId { get; set; }
    public int Handicap { get; set; }
    public int Observe { get; set; }
    public int Result { get; set; }
    public int WorkingSetSlotId { get; set; }
    public string Hero { get; set; } = string.Empty;
}

internal sealed class Toon
{
    public int Region { get; set; }
    public string ProgramId { get; set; } = string.Empty;
    public int Realm { get; set; }
    public long Id { get; set; }
}

internal sealed class Color
{
    public byte A { get; set; }
    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }
}

internal sealed class Thumbnail
{
    public string File { get; set; } = string.Empty;
}

internal static class S2ModelMapper
{
    public static ReplayHeader? MapToReplayHeader(object? header)
    {
        if (header is not Dictionary<string, object?> dict) return null;

        var versionDict = TryGetDict(dict, "m_version");
        var ngdpRootKeyDict = TryGetDict(dict, "m_ngdpRootKey");
        var replayCompatHashDict = TryGetDict(dict, "m_replayCompatibilityHash");

        return new ReplayHeader
        {
            Signature = DecodeString(TryGet<byte[]>(dict, "m_signature")),
            Version = versionDict != null ? MapToReplayVersion(versionDict) : new ReplayVersion(),
            Type = (int)TryGet<long>(dict, "m_type", 0),
            ElapsedGameLoops = TryGet(dict, "m_elapsedGameLoops", 0L),
            UseScaledTime = TryGet(dict, "m_useScaledTime", false),
            NgdpRootKey = new BinaryData { Data = TryGet(ngdpRootKeyDict ?? new(), "m_data", Array.Empty<byte>()) ?? [] },
            DataBuildNum = TryGet(dict, "m_dataBuildNum", 0),
            ReplayCompatibilityHash = new BinaryData { Data = TryGet(replayCompatHashDict ?? new(), "m_data", Array.Empty<byte>()) ?? [] },
            NgdpRootKeyIsDevData = TryGet(dict, "m_ngdpRootKeyIsDevData", false)
        };
    }

    private static ReplayVersion MapToReplayVersion(Dictionary<string, object?> dict)
    {
        return new ReplayVersion
        {
            Flags = (int)TryGet<long>(dict, "m_flags", 0),
            Major = (int)TryGet<long>(dict, "m_major", 0),
            Minor = (int)TryGet<long>(dict, "m_minor", 0),
            Revision = (int)TryGet<long>(dict, "m_revision", 0),
            Build = (int)TryGet<long>(dict, "m_build", 0),
            BaseBuild = (int)TryGet<long>(dict, "m_baseBuild", 0)
        };
    }

    public static ReplayDetails? MapToReplayDetails(object? header)
    {
        if (header is not Dictionary<string, object?> dict) return null;

        return new ReplayDetails
        {
            PlayerList = TryGetList(dict, "m_playerList").Select(p =>
                p is Dictionary<string, object?> pd ? MapToPlayer(pd) : new Player()).ToList(),

            Title = DecodeString(TryGet<byte[]>(dict, "m_title")),
            Difficulty = DecodeString(TryGet<byte[]>(dict, "m_difficulty")),
            Thumbnail = MapToThumbnail(TryGetDict(dict, "m_thumbnail") ?? new()),
            IsBlizzardMap = TryGet(dict, "m_isBlizzardMap", false),
            TimeUTC = TryGet(dict, "m_timeUTC", 0L),
            TimeLocalOffset = TryGet(dict, "m_timeLocalOffset", 0L),
            Description = DecodeString(TryGet<byte[]>(dict, "m_description")),
            ImageFilePath = DecodeString(TryGet<byte[]>(dict, "m_imageFilePath")),
            MapFileName = DecodeString(TryGet<byte[]>(dict, "m_mapFileName")),
            CacheHandles = TryGetList(dict, "m_cacheHandles").Select(h => DecodeCacheHandle(h as byte[])).ToList(),
            MiniSave = TryGet(dict, "m_miniSave", false),
            GameSpeed = (int)TryGet<long>(dict, "m_gameSpeed", 0),
            DefaultDifficulty = (int)TryGet<long>(dict, "m_defaultDifficulty", 0),
            ModPaths = dict.TryGetValue("m_modPaths", out object? value) ? value : null,
            CampaignIndex = (int)TryGet<long>(dict, "m_campaignIndex", 0),
            RestartAsTransitionMap = TryGet(dict, "m_restartAsTransitionMap", false),
            DisableRecoverGame = TryGet(dict, "m_disableRecoverGame", false)
        };
    }

    private static Thumbnail MapToThumbnail(Dictionary<string, object?> dictionary)
    {
        return new Thumbnail
        {
            File = DecodeString((byte[]?)dictionary["m_file"])
        };
    }

    private static Player MapToPlayer(Dictionary<string, object?> dict)
    {
        return new Player
        {
            Name = DecodeString(TryGet<byte[]>(dict, "m_name")),
            Toon = MapToToon(TryGetDict(dict, "m_toon") ?? new()),
            Race = DecodeString(TryGet<byte[]>(dict, "m_race")),
            Color = MapToColor(TryGetDict(dict, "m_color") ?? new()),
            Control = (int)TryGet<long>(dict, "m_control", 0),
            TeamId = (int)TryGet<long>(dict, "m_teamId", 0),
            Handicap = (int)TryGet<long>(dict, "m_handicap", 0),
            Observe = (int)TryGet<long>(dict, "m_observe", 0),
            Result = (int)TryGet<long>(dict, "m_result", 0),
            WorkingSetSlotId = (int)TryGet<long>(dict, "m_workingSetSlotId", 0),
            Hero = DecodeString(TryGet<byte[]>(dict, "m_hero"))
        };
    }

    private static Color MapToColor(Dictionary<string, object?> dictionary)
    {
        return new Color
        {
            A = (byte)TryGet<long>(dictionary, "m_a", 0),
            R = (byte)TryGet<long>(dictionary, "m_r", 0),
            G = (byte)TryGet<long>(dictionary, "m_g", 0),
            B = (byte)TryGet<long>(dictionary, "m_b", 0),
        };
    }

    private static Toon MapToToon(Dictionary<string, object?> dictionary)
    {
        return new Toon
        {
            Region = (int)TryGet<long>(dictionary, "m_region", 0),
            ProgramId = DecodeString(TryGet<byte[]>(dictionary, "m_programId")),
            Realm = (int)TryGet<long>(dictionary, "m_realm", 0),
            Id = TryGet<long>(dictionary, "m_id", 0)
        };
    }

    private static string DecodeString(byte[]? bytes)
    {
        if (bytes == null) return string.Empty;

        try
        {
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
        catch (System.Text.DecoderFallbackException)
        {
            return Convert.ToBase64String(bytes);
        }
    }

    private static string DecodeCacheHandle(byte[]? bytes)
    {
        if (bytes == null || bytes.Length <= 8)
            return string.Empty;

        // Verify the "s2ma" signature
        if (bytes[0] != (byte)'s' || bytes[1] != (byte)'2' || bytes[2] != (byte)'m' || bytes[3] != (byte)'a')
            return string.Empty;

        // Skip 8 bytes: "s2ma" (4) + metadata (3) + extra byte (1)
        var hashBytes = bytes[8..];

#pragma warning disable CA1308 // Normalize strings to uppercase
        var hashHex = BitConverter.ToString(hashBytes)
            .Replace("-", "", StringComparison.Ordinal)
            .ToLowerInvariant();
#pragma warning restore CA1308 // Normalize strings to uppercase

        return $"http://eu.depot.battle.net:1119/{hashHex}.s2ma";
    }

    private static int TryGetInt(Dictionary<string, object?> dict, string key, int defaultValue = 0)
    {
        if (dict.TryGetValue(key, out var value))
        {
            return value switch
            {
                int i => i,
                long l when l >= int.MinValue && l <= int.MaxValue => (int)l,
                _ => defaultValue,
            };
        }
        return defaultValue;
    }

    private static T? TryGet<T>(Dictionary<string, object?> dict, string key, T? defaultValue = default)
    {
        if (dict.TryGetValue(key, out var value) && value is T t)
            return t;
        return defaultValue;
    }

    private static Dictionary<string, object?>? TryGetDict(Dictionary<string, object?> dict, string key)
    {
        return TryGet<Dictionary<string, object?>>(dict, key);
    }

    private static List<object?> TryGetList(Dictionary<string, object?> dict, string key)
    {
        return TryGet<List<object?>>(dict, key, new List<object?>()) ?? [];
    }
}
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using s2protocol.NET.Mpq;
using s2protocol.NET.S2Protocol;

namespace s2protocol.NET;

/// <summary>
/// ReplayRawDecoder - return raw objects
/// </summary>
public static class ReplayRawDecoder
{
    private static readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        WriteIndented = true,
        Converters = {
            new Utf8ByteArrayConverter(),
            new CacheHandleListConverter(),
        }
    };

    /// <summary>
    /// Decode
    /// </summary>
    /// <param name="replayPath"></param>
    public static void Decode(string replayPath)
    {
        using var mpqArchive = new MPQArchive(replayPath);
        DecodeJob(mpqArchive);
    }

    /// <summary>
    /// Decode
    /// </summary>
    /// <param name="fileStream"></param>
    public static void Decode(FileStream fileStream)
    {
        using var mpqArchive = new MPQArchive(fileStream);
        DecodeJob(mpqArchive);
    }

    private static void DecodeJob(MPQArchive mpqArchive)
    {
        var headerContent = mpqArchive.GetUserDataHeaderContent();
        ArgumentNullException.ThrowIfNull(headerContent);

        var latestVersion = TypeInfoLoader.GetLatestVersion();
        var header = latestVersion.DecodeReplayHeader(headerContent);
        ArgumentNullException.ThrowIfNull(header);
        if (header is not Dictionary<string, object> headerDict
            || !headerDict.TryGetValue("m_version", out object? value)
            || value is not Dictionary<string, object> headerVersionDict
            || !headerVersionDict.TryGetValue("m_baseBuild", out object? baseBuildValue)
            || baseBuildValue is not long baseBuild)
        {
            throw new DecodeException("Header is not as expected.");
        }
        var s2protocol = TypeInfoLoader.LoadTypeInfos((int)baseBuild);
        ArgumentNullException.ThrowIfNull(s2protocol, nameof(s2protocol));

        var headerRaw = latestVersion.DecodeReplayHeader(headerContent);

        var initContent = mpqArchive.ReadFile("replay.initData");
        ArgumentNullException.ThrowIfNull(initContent);
        var initDataRaw = s2protocol.DecodeReplayInitDataRaw(initContent);
        var json = JsonSerializer.Serialize(initDataRaw, jsonSerializerOptions);
        Console.WriteLine(json);
    }

    internal static string ExtractCacheHandleUrl(byte[] handleData, string region = "eu")
    {
        const string prefix = "http://{0}.depot.battle.net:1119/{1}.s2ma";

        // Verify "s2ma" magic header
        if (handleData.Length < 36 || Encoding.ASCII.GetString(handleData, 0, 4) != "s2ma")
            throw new InvalidDataException("Invalid s2ma cache handle");

        // SHA256 starts at byte 8 typically (after "s2ma" + 4 bytes padding)
        // But exact offset may vary depending on s2protocol version.
        int hashOffset = 8;

        if (handleData.Length < hashOffset + 32)
            throw new InvalidDataException("Handle data too short for SHA256");

        byte[] hash = new byte[32];
        Array.Copy(handleData, hashOffset, hash, 0, 32);

#pragma warning disable CA1308 // Normalize strings to uppercase
        string hexHash = BitConverter.ToString(hash).Replace("-", "", StringComparison.Ordinal).ToLowerInvariant();
#pragma warning restore CA1308 // Normalize strings to uppercase
        return string.Format(CultureInfo.InvariantCulture, prefix, region, hexHash);
    }
}

internal class Utf8ByteArrayConverter : JsonConverter<byte[]>
{
    public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Not needed for your case
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
    {
        // Interpret as UTF-8 string and escape non-printable characters
        var str = Encoding.UTF8.GetString(value);

        // Escape using JSON-compatible method
        var escaped = JsonEncodedText.Encode(str);
        writer.WriteStringValue(escaped);
    }
}

internal class CacheHandleListConverter : JsonConverter<List<byte[]>>
{
    public override List<byte[]> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => throw new NotImplementedException();

    public override void Write(Utf8JsonWriter writer, List<byte[]> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var handle in value)
        {
            string url = ReplayRawDecoder.ExtractCacheHandleUrl(handle);
            writer.WriteStringValue(url);
        }
        writer.WriteEndArray();
    }
}


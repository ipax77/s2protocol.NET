using System.CommandLine;
using System.Globalization;
using System.Text.Json;
using s2protocol.NET;
using s2protocol.NET.Mpq;
using s2protocol.NET.S2Protocol;

namespace s2cli;

sealed class Program
{
    private const string VersionManifestResource = "s2protocol.NET.Resources.versions.protocol-versions.json";

    static int Main(string[] args) => Run(args, Console.Out, Console.Error);

    internal static int Run(string[] args, TextWriter output, TextWriter error)
    {
        var replayArgument = new Option<FileInfo>("--replay", "-r")
        {
            Description = "Path to .SC2Replay file",
        };

        var gameEvents = new Option<bool>("--gameevents", "-ge") { Description = "Print game events" };
        var messageEvents = new Option<bool>("--messageevents", "-me") { Description = "Print message events" };
        var trackerEvents = new Option<bool>("--trackerevents", "-te") { Description = "Print tracker events" };
        var attributeEvents = new Option<bool>("--attributeevents", "-at") { Description = "Print attributes events" };
        var header = new Option<bool>("--header") { Description = "Print protocol header" };
        var metadata = new Option<bool>("--metadata", "-md") { Description = "Print game metadata" };
        var details = new Option<bool>("--details", "-d") { Description = "Print protocol details" };
        var detailsBackup = new Option<bool>("--details_backup", "-db") { Description = "Print anonymized details" };
        var initdata = new Option<bool>("--initdata", "-id") { Description = "Print protocol initdata" };
        var all = new Option<bool>("--all", "-a") { Description = "Print all data" };
        var versions = new Option<bool>("--versions") { Description = "Show all protocol versions" };
        var ndjson = new Option<bool>("--ndjson", "-nd") { Description = "Print output as NDJSON" };

        var rootCommand = new RootCommand("SC2 replay decoder using s2protocol.NET")
        {
            replayArgument,
            gameEvents,
            messageEvents,
            trackerEvents,
            attributeEvents,
            header,
            metadata,
            details,
            detailsBackup,
            initdata,
            all,
            versions,
            ndjson,
        };

        rootCommand.SetAction(parseResult =>
        {
            try
            {
                return Decode(
                    parseResult.GetValue(replayArgument),
                    header: parseResult.GetValue(header),
                    initData: parseResult.GetValue(initdata),
                    trackerEvents: parseResult.GetValue(trackerEvents),
                    messageEvents: parseResult.GetValue(messageEvents),
                    gameEvents: parseResult.GetValue(gameEvents),
                    attributeEvents: parseResult.GetValue(attributeEvents),
                    metadata: parseResult.GetValue(metadata),
                    details: parseResult.GetValue(details),
                    detailsBackup: parseResult.GetValue(detailsBackup),
                    versions: parseResult.GetValue(versions),
                    all: parseResult.GetValue(all),
                    ndjson: parseResult.GetValue(ndjson),
                    output,
                    error);
            }
            catch (Exception ex) when (ex is DecodeException or ArgumentException or IOException)
            {
                error.WriteLine($"Failed decoding replay: {ex.Message}");
                return 1;
            }
        });

        return rootCommand.Parse(args).Invoke();
    }

    private static readonly JsonSerializerOptions JsonSerializerOptionsBase = new()
    {
        Converters = { new Utf8ByteArrayConverter() },
    };

    private static int Decode(
        FileInfo? file,
        bool header,
        bool initData,
        bool trackerEvents,
        bool messageEvents,
        bool gameEvents,
        bool attributeEvents,
        bool metadata,
        bool details,
        bool detailsBackup,
        bool versions,
        bool all,
        bool ndjson,
        TextWriter output,
        TextWriter error)
    {
        var jsonOptions = new JsonSerializerOptions(JsonSerializerOptionsBase)
        {
            WriteIndented = !ndjson,
        };

        if (versions)
        {
            output.WriteLine(JsonSerializer.Serialize(GetAvailableVersions(), jsonOptions)
                .Replace("\"", "'", StringComparison.Ordinal));
            return 0;
        }

        if (file == null || !file.Exists)
        {
            error.WriteLine($"❌ Error: File not found at '{file?.FullName}'");
            return 1;
        }

        using var archive = new MPQArchive(file.FullName);
        var protocol = TypeInfoLoader.GetLatestVersion();
        var headerContent = archive.GetUserDataHeaderContent();
        var headerTyped = protocol.DecodeReplayHeader(headerContent);
        var replayProtocol = TypeInfoLoader.LoadTypeInfos(headerTyped.BaseBuild);

        if (header || all)
        {
            WriteJson(output, headerTyped, jsonOptions);
        }

        if (metadata || all)
        {
            ReadOnlyMemory<byte> content = ReadRequiredFile(archive, "replay.gamemetadata.json", "metadata");
            using JsonDocument document = JsonDocument.Parse(content);
            WriteJson(output, document.RootElement, jsonOptions);
        }

        if (details || all)
        {
            ReadOnlyMemory<byte> content = ReadRequiredFile(archive, "replay.details", "details");
            WriteJson(output, replayProtocol.DecodeReplayDetails(content), jsonOptions);
        }

        if (detailsBackup || all)
        {
            ReadOnlyMemory<byte> content = ReadRequiredFile(archive, "replay.details.backup", "backup details");
            WriteJson(output, replayProtocol.DecodeReplayDetails(content), jsonOptions);
        }

        if (initData || all)
        {
            ReadOnlyMemory<byte> content = ReadRequiredFile(archive, "replay.initData", "init data");
            WriteJson(output, replayProtocol.DecodeReplayInitData(content), jsonOptions);
        }

        if (gameEvents || all)
        {
            ReadOnlyMemory<byte> content = ReadRequiredFile(archive, "replay.game.events", "game events");
            WriteJson(output, replayProtocol.DecodeReplayGameEvents(content), jsonOptions);
        }

        if (messageEvents || all)
        {
            ReadOnlyMemory<byte> content = ReadRequiredFile(archive, "replay.message.events", "message events");
            WriteJson(output, replayProtocol.DecodeReplayMessageEvents(content), jsonOptions);
        }

        if (trackerEvents || all)
        {
            ReadOnlyMemory<byte> content = ReadRequiredFile(archive, "replay.tracker.events", "tracker events");
            WriteJson(output, replayProtocol.DecodeReplayTrackerEvents(content), jsonOptions);
        }

        if (attributeEvents || all)
        {
            ReadOnlyMemory<byte> content = ReadRequiredFile(archive, "replay.attributes.events", "attribute events");
            WriteJson(output, S2ProtocolVersion.DecodeReplayAttributeEvents(content), jsonOptions);
        }

        return 0;
    }

    private static void WriteJson<T>(TextWriter output, T value, JsonSerializerOptions options)
        => output.WriteLine(JsonSerializer.Serialize(value, options));

    private static ReadOnlyMemory<byte> ReadRequiredFile(MPQArchive archive, string fileName, string description)
        => archive.ReadFile(fileName) ?? throw new DecodeException($"No {description} found in replay.");

    private static List<string> GetAvailableVersions()
    {
        using Stream? stream = typeof(TypeInfoLoader).Assembly.GetManifestResourceStream(VersionManifestResource);
        if (stream is null)
        {
            throw new DecodeException("Protocol version manifest was not found.");
        }

        using JsonDocument document = JsonDocument.Parse(stream);
        if (!document.RootElement.TryGetProperty("v", out JsonElement versions) ||
            versions.ValueKind != JsonValueKind.Object)
        {
            throw new DecodeException("Protocol version manifest is invalid.");
        }

        return versions.EnumerateObject()
            .Select(property => int.Parse(property.Name, CultureInfo.InvariantCulture))
            .Order()
            .Select(version => version.ToString(CultureInfo.InvariantCulture))
            .ToList();
    }
}

using System.CommandLine;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using s2protocol.NET;
using s2protocol.NET.Mpq;
using s2protocol.NET.S2Protocol;

namespace s2cli;

sealed class Program
{
    static int Main(string[] args)
    {
        var replayArgument = new Option<FileInfo>("--replay", "-r")
        {
            Description = "Path to .SC2Replay file",
            Required = true,
        };

        var gameEvents = new Option<bool>("--gameevents", "-ge")
        {
            Description = "Print game events"
        };
        var messageEvents = new Option<bool>("--messageevents", "-me")
        {
            Description = "Print message events"
        };
        var trackerEvents = new Option<bool>("--trackerevents", "-te")
        {
            Description = "Print tracker events"
        };
        var attributeEvents = new Option<bool>("--attributeevents", "-at")
        {
            Description = "Print attributes events"
        };
        var header = new Option<bool>("--header")
        {
            Description = "Print protocol header"
        };
        var metadata = new Option<bool>("--metadata", "-md")
        {
            Description = "Print game metadata"
        };
        var details = new Option<bool>("--details", "-d")
        {
            Description = "Print protocol details"
        };
        var detailsBackup = new Option<bool>("--details_backup", "-db")
        {
            Description = "Print anonymized details"
        };
        var initdata = new Option<bool>("--initdata", "-id")
        {
            Description = "Print protocol initdata"
        };

        var all = new Option<bool>("--all", "-a")
        {
            Description = "Print all data"
        };
        var versions = new Option<bool>("--versions")
        {
            Description = "Show all protocol versions"
        };
        var ndjson = new Option<bool>("--ndjson", "-nd")
        {
            Description = "Print output as NDJSON"
        };

        var rootCommand = new RootCommand("SC2 replay decoder using s2protocol.NET")
        {
            replayArgument,
            gameEvents, messageEvents, trackerEvents, attributeEvents,
            header, metadata, details, detailsBackup, initdata,
            all, versions, ndjson
        };

        rootCommand.SetAction(parseResult =>
        {
            try
            {
                return Decode(parseResult.GetValue(replayArgument),
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
                              ndjson: parseResult.GetValue(ndjson));
            }
            catch (DecodeException ex)
            {
                Console.Error.WriteLine($"Failed decoding replay: {ex.Message}");
                return 1;
            }
            catch (ArgumentNullException ex)
            {
                Console.Error.WriteLine($"Failed decoding replay: {ex.Message}");
                return 1;
            }
        });
        return rootCommand.Parse(args).Invoke();
    }

    static readonly JsonSerializerOptions jsonSerializerOptionsBase = new()
    {
        Converters = {
            new Utf8ByteArrayConverter()
        }
    };

    static int Decode(FileInfo? file,
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
                      bool ndjson)
    {
        if (file == null || !file.Exists)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"❌ Error: File not found at '{file?.FullName}'");
            Console.ResetColor();
            return 1;
        }

        using var mpqArchive = new MPQArchive(file.FullName);
        var protocol = TypeInfoLoader.GetLatestVersion();
        var headerContent = mpqArchive.GetUserDataHeaderContent();
        ArgumentNullException.ThrowIfNull(headerContent);
        var headerRaw = protocol.DecodeReplayHeader(headerContent);
        if (headerRaw is not Dictionary<string, object> headerDict
            || !headerDict.TryGetValue("m_version", out object? value)
            || value is not Dictionary<string, object> headerVersionDict
            || !headerVersionDict.TryGetValue("m_baseBuild", out object? baseBuildValue)
            || baseBuildValue is not long baseBuild)
        {
            throw new DecodeException("Header is not as expected.");
        }
        var s2protocol = TypeInfoLoader.LoadTypeInfos((int)baseBuild);
        ArgumentNullException.ThrowIfNull(s2protocol, nameof(s2protocol));

        var jsonSerializerOptions = new JsonSerializerOptions(jsonSerializerOptionsBase)
        {
            WriteIndented = !ndjson
        };

        StringBuilder sb = new();

        if (header)
        {
            sb.AppendLine(JsonSerializer.Serialize(headerRaw, jsonSerializerOptions));
        }

        if (metadata || all)
        {
            var metaContent = mpqArchive.ReadFile("replay.gamemetadata.json");
            ArgumentNullException.ThrowIfNull(metaContent, "No metadata found in replay.");
            var metaString = Encoding.UTF8.GetString(metaContent);
            using var doc = JsonDocument.Parse(metaString);
            var jsonElement = doc.RootElement.Clone();
            sb.AppendLine(JsonSerializer.Serialize(jsonElement, jsonSerializerOptions));
        }

        if (details || all)
        {
            var detailsContent = mpqArchive.ReadFile("replay.details");
            ArgumentNullException.ThrowIfNull(detailsContent, "No details found in replay.");
            var detailsRaw = s2protocol.DecodeReplayDetails(detailsContent);
            sb.AppendLine(JsonSerializer.Serialize(detailsRaw, jsonSerializerOptions));
        }

        if (detailsBackup || all)
        {
            var detailsContent = mpqArchive.ReadFile("replay.details.backup");
            ArgumentNullException.ThrowIfNull(detailsContent, "No details found in replay.");
            var detailsRaw = s2protocol.DecodeReplayDetails(detailsContent);
            sb.AppendLine(JsonSerializer.Serialize(detailsRaw, jsonSerializerOptions));
        }

        if (initData || all)
        {
            var initDataContent = mpqArchive.ReadFile("replay.initData");
            ArgumentNullException.ThrowIfNull(initDataContent, "No init data found in replay.");
            var initDataRaw = s2protocol.DecodeReplayInitDataRaw(initDataContent);
            ArgumentNullException.ThrowIfNull(initDataRaw, "Failed decoding initData.");
            sb.AppendLine(JsonSerializer.Serialize(initDataRaw, jsonSerializerOptions));
        }

        if (gameEvents || all)
        {
            var gameContent = mpqArchive.ReadFile("replay.game.events");
            ArgumentNullException.ThrowIfNull(gameContent, "No gameEvents found in replay.");
            foreach (var gameRaw in s2protocol.DecodeReplayGameEvents(gameContent))
            {
                sb.AppendLine(JsonSerializer.Serialize(gameRaw, jsonSerializerOptions));
            }
        }

        if (messageEvents || all)
        {
            var messageContent = mpqArchive.ReadFile("replay.message.events");
            ArgumentNullException.ThrowIfNull(messageContent, "No message events found in replay.");
            foreach (var messageRaw in s2protocol.DecodeReplayMessageEvents(messageContent))
            {
                sb.AppendLine(JsonSerializer.Serialize(messageRaw, jsonSerializerOptions));
            }
        }

        if (trackerEvents || all)
        {
            var trackerContent = mpqArchive.ReadFile("replay.tracker.events");
            ArgumentNullException.ThrowIfNull(trackerContent, "No tracker events found in replay.");
            foreach (var trackerRaw in s2protocol.DecodeReplayTrackerEvents(trackerContent))
            {
                sb.AppendLine(JsonSerializer.Serialize(trackerRaw, jsonSerializerOptions));
            }
        }

        if (attributeEvents || all)
        {
            var attributeContent = mpqArchive.ReadFile("replay.attributes.events");
            ArgumentNullException.ThrowIfNull(attributeContent, "No attributeEvents found in replay.");
            var attributesRaw = S2ProtocolVersion.DecodeReplayAttributeEventsRaw(attributeContent);
            sb.AppendLine(JsonSerializer.Serialize(attributesRaw, jsonSerializerOptions));
        }


        if (versions)
        {
            var assembly = typeof(TypeInfoLoader).Assembly;
            var resourceNames = assembly.GetManifestResourceNames();

            if (resourceNames.Length == 0)
            {
                throw new DecodeException("No embedded resource files found.");
            }

            List<string> availableVersions = [];
            foreach (var name in resourceNames)
            {
                if (name.StartsWith("s2protocol.NET.Resources.versions.protocol", StringComparison.Ordinal)
                    && name.EndsWith(".py", StringComparison.Ordinal))
                {
                    var match = Regex.Match(name, @"protocol(\d+)\.py");
                    if (match.Success)
                    {
                        availableVersions.Add(match.Groups[1].Value);
                    }
                }
            }
            sb.AppendLine(JsonSerializer.Serialize(availableVersions, jsonSerializerOptions)
                .Replace("\"", "'", StringComparison.Ordinal)); // s2_cli compatibility
        }

        Console.WriteLine(sb.ToString());

        return 0;
    }
}

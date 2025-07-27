using System.CommandLine;

namespace s2cli;

class Program
{
    static int Main(string[] args)
    {
        var replayArgument = new Option<FileInfo>("--replay")
        {
            Description = "Path to .SC2Replay file",
            Required = true,
        };

        var gameEvents = new Option<bool>("--gameevents")
        {
            Description = "Print game events"
        };
        var messageEvents = new Option<bool>("--messageevents")
        {
            Description = "Print message events"
        };
        var trackerEvents = new Option<bool>("--trackerevents")
        {
            Description = "Print tracker events"
        };
        var attributeEvents = new Option<bool>("--attributeevents")
        {
            Description = "Print attributes events"
        };
        var attributeParse = new Option<bool>("--attributeparse")
        {
            Description = "Parse attributes events"
        };

        var header = new Option<bool>("--header")
        {
            Description = "Print protocol header"
        };
        var metadata = new Option<bool>("--metadata")
        {
            Description = "Print game metadata"
        };
        var details = new Option<bool>("--details")
        {
            Description = "Print protocol details"
        };
        var detailsBackup = new Option<bool>("--details_backup")
        {
            Description = "Print anonymized details"
        };
        var initdata = new Option<bool>("--initdata")
        {
            Description = "Print protocol initdata"
        };

        var all = new Option<bool>("--all")
        {
            Description = "Print all data"
        };
        var quiet = new Option<bool>("--quiet")
        {
            Description = "Disable printing"
        };
        var stats = new Option<bool>("--stats")
        {
            Description = "Print stats"
        };

        var diff = new Option<string>("--diff")
        {
            Description = "Diff two protocols (provide file path)"
        };

        var versions = new Option<bool>("--versions")
        {
            Description = "Show all protocol versions"
        };
        var types = new Option<bool>("--types")
        {
            Description = "Show type information in event output"
        };
        var json = new Option<bool>("--json")
        {
            Description = "Print output as JSON"
        };
        var ndjson = new Option<bool>("--ndjson")
        {
            Description = "Print output as NDJSON"
        };
        var profile = new Option<bool>("--profile")
        {
            Description = "Whether to profile or not"
        };

        var rootCommand = new RootCommand("SC2 replay decoder using s2protocol.NET")
        {
            replayArgument,
            gameEvents, messageEvents, trackerEvents, attributeEvents, attributeParse,
            header, metadata, details, detailsBackup, initdata,
            all, quiet, stats,
            diff, versions, types,
            json, ndjson, profile
        };

        rootCommand.SetAction(parseResult =>
        {
            return ReadFile(parseResult.GetValue(replayArgument));
        });
        return rootCommand.Parse(args).Invoke();
    }

    static int ReadFile(FileInfo? file)
    {
        if (file == null || !file.Exists)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"❌ Error: File not found at '{file?.FullName}'");
            Console.ResetColor();
            return 1;
        }
        foreach (string line in File.ReadLines(file.FullName))
        {
            Console.WriteLine(line);
        }
        return 0;
    }
}

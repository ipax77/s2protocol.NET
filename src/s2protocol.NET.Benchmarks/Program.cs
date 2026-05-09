using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using s2protocol.NET;

namespace s2protocol.NET.Benchmarks;

public static class Program
{
    public static void Main(string[] args)
    {
        ReplayBenchmarkData.PrintReplayInventory();

        if (args.Length == 0)
        {
            BenchmarkRunner.Run<SingleReplayDecodeBenchmarks>();
            BenchmarkRunner.Run<ParallelReplayDecodeBenchmarks>();
            return;
        }

        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}

[MemoryDiagnoser]
public class SingleReplayDecodeBenchmarks
{
    private byte[] replayBytes = [];
    private string replayPath = string.Empty;

    public static IEnumerable<string> DecodeModes => ReplayDecodeMode.Names;

    public static IEnumerable<string> DecodeSources => ["FilePath", "MemoryStream"];

    [ParamsSource(nameof(DecodeModes))]
    public string DecodeMode { get; set; } = ReplayDecodeMode.DirectStrikeCore;

    [ParamsSource(nameof(DecodeSources))]
    public string DecodeSource { get; set; } = "FilePath";

    [GlobalSetup]
    public void Setup()
    {
        replayPath = ReplayBenchmarkData.GetSingleReplayPath();
        replayBytes = File.ReadAllBytes(replayPath);
    }

    [Benchmark]
    public async Task<ReplayDecodeSummary> DecodeSingleReplay()
    {
        ReplayDecoderOptions options = ReplayDecodeMode.CreateOptions(DecodeMode);

        using ReplayDecoder replayDecoder = new();
        Sc2Replay replay;
        if (string.Equals(DecodeSource, "MemoryStream", StringComparison.Ordinal))
        {
            using MemoryStream stream = new(replayBytes, writable: false);
            replay = await replayDecoder.DecodeAsync(stream, options, CancellationToken.None).ConfigureAwait(false)
                ?? throw new InvalidOperationException($"Could not decode replay '{replayPath}'.");
        }
        else
        {
            replay = await replayDecoder.DecodeAsync(replayPath, options, CancellationToken.None).ConfigureAwait(false)
                ?? throw new InvalidOperationException($"Could not decode replay '{replayPath}'.");
        }

        return ReplayDecodeSummary.From(replay);
    }
}

[MemoryDiagnoser]
public class ParallelReplayDecodeBenchmarks
{
    private string[] replayPaths = [];

    [Params(1, 2, 4)]
    public int Threads { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        replayPaths = ReplayBenchmarkData.GetReplayPaths();
    }

    [Benchmark]
    public async Task<ReplayBatchDecodeSummary> DecodeCorpusParallel()
    {
        ReplayDecoderOptions options = ReplayDecodeMode.CreateOptions(ReplayDecodeMode.DirectStrikeCore);
        using ReplayDecoder replayDecoder = new();

        int decoded = 0;
        int errors = 0;
        int trackerEvents = 0;
        await foreach (DecodeParallelResult result in replayDecoder.DecodeParallelWithErrorReport(
                           replayPaths,
                           Threads,
                           options,
                           CancellationToken.None).ConfigureAwait(false))
        {
            if (result.Sc2Replay is null)
            {
                errors++;
                continue;
            }

            decoded++;
            trackerEvents += result.Sc2Replay.TrackerEvents?.SUnitBornEvents.Count ?? 0;
        }

        if (errors > 0)
        {
            throw new InvalidOperationException($"Failed to decode {errors} of {replayPaths.Length} replay(s).");
        }

        return new ReplayBatchDecodeSummary(decoded, errors, trackerEvents);
    }
}

public readonly record struct ReplayDecodeSummary(
    int BaseBuild,
    int PlayerCount,
    int TrackerBornEvents,
    int TrackerStatsEvents,
    int GameEvents,
    int ChatMessages)
{
    public static ReplayDecodeSummary From(Sc2Replay replay)
    {
        return new ReplayDecodeSummary(
            replay.Header.BaseBuild,
            replay.Details?.Players.Count ?? 0,
            replay.TrackerEvents?.SUnitBornEvents.Count ?? 0,
            replay.TrackerEvents?.SPlayerStatsEvents.Count ?? 0,
            replay.GameEvents?.BaseGameEvents.Count ?? 0,
            replay.ChatMessages?.Count ?? 0);
    }
}

public readonly record struct ReplayBatchDecodeSummary(int Decoded, int Errors, int TrackerBornEvents);

internal static class ReplayDecodeMode
{
    public const string HeaderOnly = nameof(HeaderOnly);
    public const string DirectStrikeCore = nameof(DirectStrikeCore);
    public const string Full = nameof(Full);

    public static readonly string[] Names = [HeaderOnly, DirectStrikeCore, Full];

    public static ReplayDecoderOptions CreateOptions(string mode)
    {
        return mode switch
        {
            HeaderOnly => new ReplayDecoderOptions
            {
                Initdata = false,
                Details = false,
                Metadata = false,
                GameEvents = false,
                MessageEvents = false,
                TrackerEvents = false,
                AttributeEvents = false,
            },
            DirectStrikeCore => new ReplayDecoderOptions
            {
                Details = true,
                Initdata = true,
                Metadata = true,
                GameEvents = false,
                MessageEvents = false,
                TrackerEvents = true,
                AttributeEvents = false,
            },
            Full => new ReplayDecoderOptions(),
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unknown replay decode mode."),
        };
    }
}

internal static class ReplayBenchmarkData
{
    private const string ReplaySearchPattern = "*.SC2Replay";
    private const string ReplayDirectoryEnvironmentVariable = "S2PROTOCOL_BENCHMARK_REPLAY_DIR";

    public static string GetSingleReplayPath()
    {
        return GetReplayPaths()
            .OrderByDescending(FileLength)
            .First();
    }

    public static string[] GetReplayPaths()
    {
        string replayDirectory = ResolveReplayDirectory();
        string[] replayPaths = Directory.GetFiles(replayDirectory, ReplaySearchPattern, SearchOption.TopDirectoryOnly)
            .Where(path => !path.Contains("Error", StringComparison.OrdinalIgnoreCase))
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (replayPaths.Length == 0)
        {
            throw new InvalidOperationException($"No {ReplaySearchPattern} files found in '{replayDirectory}'.");
        }

        return replayPaths;
    }

    public static void PrintReplayInventory()
    {
        string replayDirectory = ResolveReplayDirectory();
        string[] replayPaths = GetReplayPaths();

        Console.WriteLine("Replay benchmark corpus:");
        Console.WriteLine("  Directory: {0}", replayDirectory);
        foreach (string replayPath in replayPaths)
        {
            FileInfo replayFile = new(replayPath);
            Console.WriteLine("  {0} ({1:N0} bytes)", replayFile.Name, replayFile.Length);
        }

        Console.WriteLine();
    }

    private static string ResolveReplayDirectory()
    {
        string? configuredReplayDirectory = Environment.GetEnvironmentVariable(ReplayDirectoryEnvironmentVariable);
        if (IsReplayDirectory(configuredReplayDirectory))
        {
            return Path.GetFullPath(configuredReplayDirectory!);
        }

        foreach (string baseDirectory in EnumerateBaseDirectories())
        {
            foreach (string candidate in EnumerateReplayDirectoryCandidates(baseDirectory))
            {
                if (IsReplayDirectory(candidate))
                {
                    return Path.GetFullPath(candidate);
                }
            }
        }

        throw new DirectoryNotFoundException(
            $"Could not find a replay corpus. Set {ReplayDirectoryEnvironmentVariable} to a directory containing {ReplaySearchPattern} files.");
    }

    private static IEnumerable<string> EnumerateBaseDirectories()
    {
        HashSet<string> seenDirectories = new(StringComparer.OrdinalIgnoreCase);
        string[] seedDirectories =
        [
            AppContext.BaseDirectory,
            Directory.GetCurrentDirectory(),
        ];

        foreach (string seedDirectory in seedDirectories)
        {
            DirectoryInfo? current = new(seedDirectory);
            while (current is not null && seenDirectories.Add(current.FullName))
            {
                yield return current.FullName;
                current = current.Parent;
            }
        }
    }

    private static IEnumerable<string> EnumerateReplayDirectoryCandidates(string baseDirectory)
    {
        yield return Path.Combine(baseDirectory, "testdata");
        yield return Path.Combine(baseDirectory, "replays");
        yield return Path.Combine(baseDirectory, "Sc2DirectStrike", "src", "Sc2DirectStrike.Tests", "testdata");
        yield return Path.Combine(baseDirectory, "s2protocol.NET.tests", "replays");
    }

    private static bool IsReplayDirectory(string? replayDirectory)
    {
        return !string.IsNullOrWhiteSpace(replayDirectory)
            && Directory.Exists(replayDirectory)
            && Directory.EnumerateFiles(replayDirectory, ReplaySearchPattern, SearchOption.TopDirectoryOnly).Any();
    }

    private static long FileLength(string path)
    {
        return new FileInfo(path).Length;
    }
}

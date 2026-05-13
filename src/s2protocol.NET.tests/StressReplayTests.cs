using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace s2protocol.NET.tests;

[TestClass]
public sealed class StressReplayTests
{
    private const string RunStressTestsEnvironmentVariable = "S2PROTOCOL_RUN_STRESS_TESTS";

    private static readonly string[] StressReplayNames =
    [
        "Direct Strike (10060).SC2Replay",
        "Direct Strike (10096).SC2Replay",
        "Direct Strike (10124).SC2Replay",
        "Direct Strike (10143).SC2Replay",
        "Direct Strike TE (1904).SC2Replay",
    ];

    [TestMethod]
    [StressReplayEnabled]
    [DataRow("Direct Strike (10060).SC2Replay")]
    [DataRow("Direct Strike (10096).SC2Replay")]
    [DataRow("Direct Strike (10124).SC2Replay")]
    [DataRow("Direct Strike (10143).SC2Replay")]
    [DataRow("Direct Strike TE (1904).SC2Replay")]
    public async Task DirectStrikeStressReplayCanDecode(string replayName)
    {
        string? replayDirectory = StressReplayData.TryResolveReplayDirectory();
        if (replayDirectory is null)
        {
            return;
        }

        string replayPath = Path.Combine(replayDirectory, replayName);
        if (!File.Exists(replayPath))
        {
            return;
        }

        using ReplayDecoder replayDecoder = new();
        Sc2Replay replay = await replayDecoder.DecodeAsync(replayPath, CreateDirectStrikeCoreOptions(), CancellationToken.None)
            .ConfigureAwait(false)
            ?? throw new InvalidOperationException($"Could not decode replay '{replayPath}'.");

        var details = replay.Details ?? throw new AssertFailedException("Expected replay details.");
        Assert.IsTrue(details.Title.StartsWith("Direct Strike", StringComparison.OrdinalIgnoreCase));

        var trackerEvents = replay.TrackerEvents ?? throw new AssertFailedException("Expected tracker events.");
        Assert.IsTrue(trackerEvents.SUnitBornEvents.Count > 0, "Expected unit born tracker events.");
        Assert.IsTrue(trackerEvents.SPlayerStatsEvents.Count > 0, "Expected player stats tracker events.");
    }

    [TestMethod]
    [StressReplayEnabled]
    public async Task DirectStrikeCorpusCanDecodeInParallel()
    {
        string? replayDirectory = StressReplayData.TryResolveReplayDirectory();
        if (replayDirectory is null)
        {
            return;
        }

        string[] replayPaths = Directory.GetFiles(replayDirectory, "*.SC2Replay", SearchOption.TopDirectoryOnly)
            .Where(path => StressReplayNames.Contains(Path.GetFileName(path), StringComparer.OrdinalIgnoreCase))
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (replayPaths.Length == 0)
        {
            return;
        }

        using ReplayDecoder replayDecoder = new();

        int decoded = 0;
        List<string> errors = [];
#pragma warning disable CS0618 // Stress test keeps legacy helper coverage while it remains public.
        await foreach (DecodeParallelResult result in replayDecoder.DecodeParallelWithErrorReport(
                           replayPaths,
                           2,
                           CreateDirectStrikeCoreOptions(),
                           CancellationToken.None).ConfigureAwait(false))
#pragma warning restore CS0618 // Stress test keeps legacy helper coverage while it remains public.
        {
            if (result.Sc2Replay is null)
            {
                errors.Add($"{Path.GetFileName(result.ReplayPath)}: {result.Exception}");
                continue;
            }

            decoded++;
        }

        Assert.AreEqual(0, errors.Count);
        Assert.AreEqual(replayPaths.Length, decoded);
    }

    private static ReplayDecoderOptions CreateDirectStrikeCoreOptions()
    {
        return new ReplayDecoderOptions
        {
            Details = true,
            Initdata = true,
            Metadata = true,
            GameEvents = false,
            MessageEvents = false,
            TrackerEvents = true,
            AttributeEvents = false,
        };
    }

    [AttributeUsage(AttributeTargets.Method)]
    private sealed class StressReplayEnabledAttribute : ConditionBaseAttribute
    {
        public StressReplayEnabledAttribute()
            : base(ConditionMode.Include)
        {
            IgnoreMessage = $"Set {RunStressTestsEnvironmentVariable}=1 to run DirectStrike stress replay tests.";
        }

        public override bool IsConditionMet => IsStressTestEnabled();

        public override string GroupName => nameof(StressReplayEnabledAttribute);
    }

    private static bool IsStressTestEnabled()
    {
        return string.Equals(
            Environment.GetEnvironmentVariable(RunStressTestsEnvironmentVariable),
            "1",
            StringComparison.OrdinalIgnoreCase);
    }
}

internal static class StressReplayData
{
    private const string ReplayDirectoryEnvironmentVariable = "S2PROTOCOL_BENCHMARK_REPLAY_DIR";

    public static string? TryResolveReplayDirectory()
    {
        string? configuredReplayDirectory = Environment.GetEnvironmentVariable(ReplayDirectoryEnvironmentVariable);
        if (IsReplayDirectory(configuredReplayDirectory))
        {
            return Path.GetFullPath(configuredReplayDirectory!);
        }

        foreach (string baseDirectory in EnumerateBaseDirectories())
        {
            string candidate = Path.Combine(baseDirectory, "Sc2DirectStrike", "src", "Sc2DirectStrike.Tests", "testdata");
            if (IsReplayDirectory(candidate))
            {
                return Path.GetFullPath(candidate);
            }
        }

        return null;
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

    private static bool IsReplayDirectory(string? replayDirectory)
    {
        return !string.IsNullOrWhiteSpace(replayDirectory)
            && Directory.Exists(replayDirectory)
            && Directory.EnumerateFiles(replayDirectory, "*.SC2Replay", SearchOption.TopDirectoryOnly).Any();
    }
}

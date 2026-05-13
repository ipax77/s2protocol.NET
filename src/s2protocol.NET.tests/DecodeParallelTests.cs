using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace s2protocol.NET.tests;

[TestClass]
public class DecodeParallelTests
{
    public static readonly string? assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    [TestMethod]
    public async Task DecodeParallelTest()
    {
        Assert.IsTrue(assemblyPath != null, "Could not get ExecutionAssembly path");
        if (assemblyPath == null)
        {
            return;
        }
        using ReplayDecoder decoder = new();
        ReplayDecoderOptions options = new()
        {
            Initdata = false,
            Details = false,
            Metadata = false,
            MessageEvents = false,
            TrackerEvents = false,
            GameEvents = false,
            AttributeEvents = false,
        };

        var replays = Directory.GetFiles(Path.Combine(assemblyPath, "replays"), "*.SC2Replay", SearchOption.TopDirectoryOnly);
        replays = replays.Where(x => !x.Contains("Error", System.StringComparison.OrdinalIgnoreCase)).ToArray();

        CancellationTokenSource cts = new();

        int decoded = 0;
#pragma warning disable CS0618 // Legacy helper is intentionally covered by compatibility tests.
        await foreach (var sc2rep in decoder.DecodeParallel(replays, 2, options, cts.Token))
#pragma warning restore CS0618 // Legacy helper is intentionally covered by compatibility tests.
        {
            if (sc2rep != null)
            {
                decoded++;
            }
        }
        Assert.AreEqual(replays.Length, decoded);

        await cts.CancelAsync();
        cts.Dispose();
        decoder.Dispose();
    }

    [TestMethod]
    public async Task DecodeParallelErrorTest()
    {
        Assert.IsTrue(assemblyPath != null, "Could not get ExecutionAssembly path");
        if (assemblyPath == null)
        {
            return;
        }
        using ReplayDecoder decoder = new();
        ReplayDecoderOptions options = new()
        {
            Initdata = false,
            Details = false,
            Metadata = false,
            MessageEvents = false,
            TrackerEvents = true,
            GameEvents = false,
            AttributeEvents = false,
        };

        var replays = Directory.GetFiles(Path.Combine(assemblyPath, "replays"), "*.SC2Replay", SearchOption.TopDirectoryOnly);

        CancellationTokenSource cts = new();

        int decoded = 0;
#pragma warning disable CS0618 // Legacy helper is intentionally covered by compatibility tests.
        await foreach (var sc2rep in decoder.DecodeParallel(replays, 2, options, cts.Token))
#pragma warning restore CS0618 // Legacy helper is intentionally covered by compatibility tests.
        {
            decoded++;
        }
        Assert.AreEqual(replays.Length - 3, decoded);

        await cts.CancelAsync();
        cts.Dispose();
        decoder.Dispose();
    }

    [TestMethod]
    public async Task DecodeParallelWithResultTest()
    {
        Assert.IsTrue(assemblyPath != null, "Could not get ExecutionAssembly path");
        if (assemblyPath == null)
        {
            return;
        }
        using ReplayDecoder decoder = new();
        ReplayDecoderOptions options = new()
        {
            Initdata = false,
            Details = false,
            Metadata = false,
            MessageEvents = false,
            TrackerEvents = true,
            GameEvents = false,
            AttributeEvents = false,
        };

        var replays = Directory.GetFiles(Path.Combine(assemblyPath, "replays"), "*.SC2Replay", SearchOption.TopDirectoryOnly);

        CancellationTokenSource cts = new();

        int decoded = 0;
        int errors = 0;
        List<DecodeParallelResult> results = [];
#pragma warning disable CS0618 // Legacy helper is intentionally covered by compatibility tests.
        await foreach (var decodeResult in decoder.DecodeParallelWithErrorReport(replays, 2, options, cts.Token))
#pragma warning restore CS0618 // Legacy helper is intentionally covered by compatibility tests.
        {
            if (decodeResult.Sc2Replay == null)
            {
                errors++;
            }
            else
            {
                decoded++;
            }
            results.Add(decodeResult);
        }
        Assert.AreEqual(replays.Length - errors, decoded);
        Assert.AreEqual(replays.Length, decoded + errors);

        await cts.CancelAsync();
        cts.Dispose();
        decoder.Dispose();
    }

    [TestMethod]
    public async Task DecodeParallelWithResultSlowConsumerTest()
    {
        Assert.IsTrue(assemblyPath != null, "Could not get ExecutionAssembly path");
        if (assemblyPath == null)
        {
            return;
        }
        using ReplayDecoder decoder = new();
        ReplayDecoderOptions options = new()
        {
            Initdata = false,
            Details = false,
            Metadata = false,
            MessageEvents = false,
            TrackerEvents = true,
            GameEvents = false,
            AttributeEvents = false,
        };

        var replays = Directory.GetFiles(Path.Combine(assemblyPath, "replays"), "*.SC2Replay", SearchOption.TopDirectoryOnly);

        CancellationTokenSource cts = new();

        int decoded = 0;
        int errors = 0;
#pragma warning disable CS0618 // Legacy helper is intentionally covered by compatibility tests.
        await foreach (var decodeResult in decoder.DecodeParallelWithErrorReport(replays, 2, options, cts.Token))
#pragma warning restore CS0618 // Legacy helper is intentionally covered by compatibility tests.
        {
            await Task.Delay(1, cts.Token);

            if (decodeResult.Sc2Replay == null)
            {
                errors++;
            }
            else
            {
                decoded++;
            }
        }
        Assert.AreEqual(replays.Length, decoded + errors);

        await cts.CancelAsync();
        cts.Dispose();
        decoder.Dispose();
    }
}

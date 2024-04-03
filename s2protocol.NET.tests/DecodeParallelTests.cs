using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace s2protocol.NET.tests;

public class DecodeParallelTests
{
    public static readonly string? assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    [Fact]
    public async Task DecodeParallelTest()
    {
        Assert.True(assemblyPath != null, "Could not get ExecutionAssembly path");
        if (assemblyPath == null)
        {
            return;
        }
        using ReplayDecoder decoder = new(assemblyPath);
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
        await foreach (var sc2rep in decoder.DecodeParallel(replays, 2, options, cts.Token))
        {
            if (sc2rep != null)
            {
                decoded++;
            }
        }
        Assert.Equal(replays.Length, decoded);

        await cts.CancelAsync();
        cts.Dispose();
        decoder.Dispose();
    }

    [Fact]
    public async Task DecodeParallelErrorTest()
    {
        Assert.True(assemblyPath != null, "Could not get ExecutionAssembly path");
        if (assemblyPath == null)
        {
            return;
        }
        using ReplayDecoder decoder = new(assemblyPath);
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
        int errorReplays = replays.Where(x => x.Contains("Error", System.StringComparison.OrdinalIgnoreCase)).Count();

        CancellationTokenSource cts = new();

        int decoded = 0;
        await foreach (var sc2rep in decoder.DecodeParallel(replays, 2, options, cts.Token))
        {
            if (sc2rep != null)
            {
                decoded++;
            }
        }
        Assert.Equal(replays.Length - errorReplays, decoded);

        await cts.CancelAsync();
        cts.Dispose();
        decoder.Dispose();
    }

    [Fact]
    public async Task DecodeParallelWithResultTest()
    {
        Assert.True(assemblyPath != null, "Could not get ExecutionAssembly path");
        if (assemblyPath == null)
        {
            return;
        }
        using ReplayDecoder decoder = new(assemblyPath);
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
        int errorReplays = replays.Where(x => x.Contains("Error", System.StringComparison.OrdinalIgnoreCase)).Count();

        CancellationTokenSource cts = new();

        int decoded = 0;
        int errors = 0;
        await foreach (var decodeResult in decoder.DecodeParallelWithErrorReport(replays, 2, options, cts.Token))
        {
            if (decodeResult.Sc2Replay == null)
            {
                errors++;
            }
            else
            {
                decoded++;
            }
        }
        Assert.Equal(replays.Length - errorReplays, decoded);
        Assert.Equal(errorReplays, errors);

        await cts.CancelAsync();
        cts.Dispose();
        decoder.Dispose();
    }
}

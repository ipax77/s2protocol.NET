using s2protocol.NET.Models;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace s2protocol.NET;

public sealed partial class ReplayDecoder
{
    /// <summary>Decode Starcraft2 replays
    /// Replays replays will be skipped
    /// </summary>
    /// <param name="replayPaths">The paths to the Starcraft2 replays</param>
    /// /// <param name="threads">Number of parallelism</param>
    /// <param name="options">Optional decoding options</param>
    /// <param name="token">Optional CancellationToken</param>
    [Obsolete("Use DecodeAsync and caller-owned parallelism or pipelines for large replay sets.")]
    public async IAsyncEnumerable<Sc2Replay> DecodeParallel(ICollection<string> replayPaths,
                                                            int threads,
                                                            ReplayDecoderOptions? options = null,
                                                            [EnumeratorCancellation] CancellationToken token = default)
    {
        Channel<Sc2Replay> replayChannel = CreateBoundedReplayChannel<Sc2Replay>(threads);

        _ = Produce(replayChannel, replayPaths, threads, options, token);

        while (await replayChannel.Reader.WaitToReadAsync(token).ConfigureAwait(false))
        {
            if (replayChannel.Reader.TryRead(out var replay))
            {
                yield return replay;
            }
        }
    }

    /// <summary>Decode Starcraft2 replays and report potential errors
    /// </summary>
    /// <param name="replayPaths">The paths to the Starcraft2 replays</param>
    /// /// <param name="threads">Number of parallelism</param>
    /// <param name="options">Optional decoding options</param>
    /// <param name="token">Optional CancellationToken</param>
    [Obsolete("Use DecodeAsync and caller-owned parallelism or pipelines for large replay sets.")]
    public async IAsyncEnumerable<DecodeParallelResult> DecodeParallelWithErrorReport(ICollection<string> replayPaths,
                                                                                      int threads,
                                                                                      ReplayDecoderOptions? options = null,
                                                                                      [EnumeratorCancellation] CancellationToken token = default)
    {
        Channel<DecodeParallelResult> replayResultChannel = CreateBoundedReplayChannel<DecodeParallelResult>(threads);

        _ = ProduceResults(replayResultChannel, replayPaths, threads, options, token);

        while (await replayResultChannel.Reader.WaitToReadAsync(token).ConfigureAwait(false))
        {
            if (replayResultChannel.Reader.TryRead(out var replayResult))
            {
                yield return replayResult;
            }
        }
    }

    private static Channel<T> CreateBoundedReplayChannel<T>(int threads)
    {
        return Channel.CreateBounded<T>(
            new BoundedChannelOptions(Math.Max(1, threads))
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            }
        );
    }

    private async Task Produce(Channel<Sc2Replay> channel,
                               ICollection<string> replayPaths,
                               int threads,
                               ReplayDecoderOptions? options,
                               CancellationToken token)
    {
        ParallelOptions parallelOptions = new()
        {
            CancellationToken = token,
            MaxDegreeOfParallelism = threads
        };

        try
        {
            await Parallel.ForEachAsync(replayPaths, parallelOptions, async (replayPath, token) =>
            {
#pragma warning disable CA1031 // Do not catch general exception types - finish the loop event if some replays fail.
                try
                {
                    var replay = await DecodeAsync(replayPath, options, token).ConfigureAwait(false);

                    if (replay != null)
                    {
                        await channel.Writer.WriteAsync(replay, token).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException) { }
                catch (Exception)
                {

                }
#pragma warning restore CA1031 // Do not catch general exception types
            }).ConfigureAwait(false);
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            throw new DecodeException($"failed decoding replays: {ex.Message}");
        }
        finally
        {
            channel.Writer.Complete();
        }
    }

    private async Task ProduceResults(Channel<DecodeParallelResult> channel,
                                      ICollection<string> replayPaths,
                                      int threads,
                                      ReplayDecoderOptions? options,
                                      CancellationToken token)
    {
        ParallelOptions parallelOptions = new()
        {
            CancellationToken = token,
            MaxDegreeOfParallelism = threads
        };

        try
        {
            await Parallel.ForEachAsync(replayPaths, parallelOptions, async (replayPath, token) =>
            {
#pragma warning disable CA1031 // Do not catch general exception types - finish the loop event if some replays fail.
                try
                {
                    var replay = await DecodeAsync(replayPath, options, token).ConfigureAwait(false);
                    ArgumentNullException.ThrowIfNull(replay, nameof(replay));

                    await channel.Writer.WriteAsync(new DecodeParallelResult()
                    {
                        Sc2Replay = replay,
                        ReplayPath = replayPath
                    }, token).ConfigureAwait(false);
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    try
                    {
                        await channel.Writer.WriteAsync(new DecodeParallelResult()
                        {
                            ReplayPath = replayPath,
                            Exception = ex.Message
                        }, token).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException) { }
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }).ConfigureAwait(false);
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            throw new DecodeException($"failed decoding replays: {ex.Message}");
        }
        finally
        {
            channel.Writer.Complete();
        }
    }
}

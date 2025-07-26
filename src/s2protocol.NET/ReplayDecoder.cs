using s2protocol.NET.Models;
using s2protocol.NET.Mpq;
using s2protocol.NET.Parser;
using s2protocol.NET.S2Protocol;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace s2protocol.NET;

/// <summary>
/// Provides functionality to decode Starcraft II replay files, supporting parallel processing and optional error
/// reporting.
/// </summary>
/// <remarks>The <see cref="ReplayDecoder"/> class is designed to process Starcraft II replay files efficiently,
/// with support for parallel decoding and customizable decoding options. It also provides methods to handle decoding
/// errors and report them alongside the decoded results.</remarks>
public sealed class ReplayDecoder : IDisposable
{
    /// <summary>Creates the decoder</summary>
    public ReplayDecoder()
    {
    }

    /// <summary>Decode Starcraft2 replays
    /// Replays replays will be skipped
    /// </summary>
    /// <param name="replayPaths">The paths to the Starcraft2 replays</param>
    /// /// <param name="threads">Number of parallelism</param>
    /// <param name="options">Optional decoding options</param>
    /// <param name="token">Optional CancellationToken</param>
    public async IAsyncEnumerable<Sc2Replay> DecodeParallel(ICollection<string> replayPaths, int threads, ReplayDecoderOptions? options = null, [EnumeratorCancellation] CancellationToken token = default)
    {
        Channel<Sc2Replay> replayChannel = Channel.CreateUnbounded<Sc2Replay>();

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
    public async IAsyncEnumerable<DecodeParallelResult> DecodeParallelWithErrorReport(ICollection<string> replayPaths, int threads, ReplayDecoderOptions? options = null, [EnumeratorCancellation] CancellationToken token = default)
    {
        Channel<DecodeParallelResult> replayResultChannel = Channel.CreateUnbounded<DecodeParallelResult>(
            new UnboundedChannelOptions()
            {
                SingleReader = true,
                SingleWriter = false
            }
        );

        _ = ProduceResults(replayResultChannel, replayPaths, threads, options, token);

        while (await replayResultChannel.Reader.WaitToReadAsync(token).ConfigureAwait(false))
        {
            if (replayResultChannel.Reader.TryRead(out var replayResult))
            {
                yield return replayResult;
            }
        }
    }

    private async Task Produce(Channel<Sc2Replay> channel, ICollection<string> replayPaths, int threads, ReplayDecoderOptions? options, CancellationToken token)
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
                        channel.Writer.TryWrite(replay);
                    }
                    else
                    {
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

    private async Task ProduceResults(Channel<DecodeParallelResult> channel, ICollection<string> replayPaths, int threads, ReplayDecoderOptions? options, CancellationToken token)
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

                    if (!channel.Writer.TryWrite(new DecodeParallelResult()
                    {
                        Sc2Replay = replay,
                        ReplayPath = replayPath
                    })
                    )
                    {
                        channel.Writer.TryWrite(new DecodeParallelResult()
                        {
                            ReplayPath = replayPath,
                            Exception = "Failed writing to channel."
                        });
                    }
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    channel.Writer.TryWrite(new DecodeParallelResult()
                    {
                        ReplayPath = replayPath,
                        Exception = ex.Message
                    });
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

    /// <summary>Decode Starcraft2 replay</summary>
    /// <param name="replayPath">The path to the Starcraft2 replay</param>
    /// <param name="options">Optional decoding options</param>
    /// <param name="token">Optional CancellationToken</param>
#pragma warning disable CA1822 // Mark members as static
    public async Task<Sc2Replay?> DecodeAsync(string replayPath, ReplayDecoderOptions? options = null, CancellationToken token = default)
#pragma warning restore CA1822 // Mark members as static
    {
        if (!File.Exists(replayPath))
        {
            throw new ArgumentNullException(nameof(replayPath), "Replay not found.");
        }

        if (options == null)
        {
            options = new ReplayDecoderOptions();
        }

        try
        {
            using var MPQArchive = new MPQArchive(replayPath);

            var headerContent = MPQArchive.GetUserDataHeaderContent();
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
            ArgumentNullException.ThrowIfNull(headerRaw, nameof(headerRaw));
            Sc2Replay replay = new(headerRaw, replayPath);

            if (options.Initdata)
            {
                var init = await GetInitDataAsync(MPQArchive, s2protocol, token).ConfigureAwait(false);
                ArgumentNullException.ThrowIfNull((object?)init, nameof(init));

                replay.Initdata = Parse.InitData(init);
            }

            if (options.Details)
            {
                var details = await GetDetailsAsync(MPQArchive, s2protocol, token).ConfigureAwait(false);
                ArgumentNullException.ThrowIfNull((object?)details, nameof(details));
                if (details is not Dictionary<string, object> detailsDict)
                {
                    throw new DecodeException("Details is not a Dictionary<string, object>");
                }
                replay.Details = Parse.Datails(detailsDict);
            }

            if (options.Metadata)
            {
                var metadata = await GetMetadataAsync(MPQArchive, token).ConfigureAwait(false);
                ArgumentNullException.ThrowIfNull((object?)metadata, nameof(metadata));

                replay.Metadata = metadata;
            }

            if (options.MessageEvents)
            {
                var messages = await GetMessagesAsync(MPQArchive, s2protocol, token).ConfigureAwait(false);
                ArgumentNullException.ThrowIfNull((object?)messages, nameof(messages));
                Parse.SetMessages(messages, replay);
            }

            if (options.TrackerEvents)
            {
                var trackerEvents = await GetTrackereventsAsync(MPQArchive, s2protocol, token).ConfigureAwait(false);
                ArgumentNullException.ThrowIfNull((object?)trackerEvents, nameof(trackerEvents));

                replay.TrackerEvents = Parse.Tracker(trackerEvents);

                if (replay.TrackerEvents != null)
                {
                    replay.TrackerEvents.SUnitBornEvents.ToList().ForEach(f => f.UnitIndex = GetUnitIndex(f.UnitTagIndex, f.UnitTagRecycle));
                    replay.TrackerEvents.SUnitInitEvents.ToList().ForEach(f => f.UnitIndex = GetUnitIndex(f.UnitTagIndex, f.UnitTagRecycle));
                    replay.TrackerEvents.SUnitDiedEvents.ToList().ForEach(f => f.UnitIndex = GetUnitIndex(f.UnitTagIndex, f.UnitTagRecycle));
                    replay.TrackerEvents.SUnitDoneEvents.ToList().ForEach(f => f.UnitIndex = GetUnitIndex(f.UnitTagIndex, f.UnitTagRecycle));
                    replay.TrackerEvents.SUnitOwnerChangeEvents.ToList().ForEach(f => f.UnitIndex = GetUnitIndex(f.UnitTagIndex, f.UnitTagRecycle));
                    Parse.SetTrackerEventsUnitConnections(replay.TrackerEvents);
                }
            }

            if (options.GameEvents)
            {
                // await SetGameEventsAsync(MPQArchive, s2protocol, replay, token).ConfigureAwait(false);
                var gameEventsRaw = await GetGameEventsAsync(MPQArchive, s2protocol, token).ConfigureAwait(false);
                ArgumentNullException.ThrowIfNull((object?)gameEventsRaw, nameof(gameEventsRaw));
                var gameEvents = Parse.GameEvents(gameEventsRaw);
            }

            if (options.AttributeEvents)
            {
                var attributeEvents = await GetAttributeEventsAsync(MPQArchive, token).ConfigureAwait(false);
                ArgumentNullException.ThrowIfNull((object?)attributeEvents, nameof(attributeEvents));

                replay.AttributeEvents = Parse.GetAttributeEvents(attributeEvents);
            }



            return replay;
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception ex)
        {
            throw new DecodeException(ex.Message);
        }
    }

    private static int GetUnitIndex(int unitTagIndex, int unitTagRecyle)
    {
        // todo: can be BitInterger
        var unitTag = S2ProtocolVersion.UnitTag(unitTagIndex, unitTagRecyle);
        if (unitTag is int intUnitTag)
        {
            return intUnitTag;
        }
        else
        {
            return 0;
        }
    }

    private static async Task<Dictionary<string, object>?> GetAttributeEventsAsync(MPQArchive archive, CancellationToken token)
    {
        return await Task.Run(() =>
        {
            var game_enc = archive.ReadFile("replay.attributes.events");
            if (game_enc != null)
            {
                return S2ProtocolVersion.DecodeReplayAttributeEventsRaw(game_enc);
            }
            return null;
        }, token).ConfigureAwait(false);
    }

    private static async Task<List<Dictionary<string, object?>>?> GetGameEventsAsync(MPQArchive archive, S2ProtocolVersion protocol, CancellationToken token)
    {
        return await Task.Run(() =>
        {
            var game_enc = archive.ReadFile("replay.game.events");
            if (game_enc != null)
            {
                List<Dictionary<string, object?>> gameEvents = [];
                foreach (var gameEvent in protocol.DecodeReplayGameEvents(game_enc))
                {
                    gameEvents.Add(gameEvent);
                }
                return gameEvents;
            }
            return null;
        }, token).ConfigureAwait(false);
    }

    private static async Task<object?> GetInitDataAsync(MPQArchive archive, S2ProtocolVersion protocol, CancellationToken token)
    {
        return await Task.Run(() =>
        {
            var init_enc = archive.ReadFile("replay.initData");
            if (init_enc != null)
            {
                return protocol.DecodeReplayInitDataRaw(init_enc);
            }
            return null;
        }, token).ConfigureAwait(false);
    }

    private static async Task<List<Dictionary<string, object?>>?> GetTrackereventsAsync(MPQArchive archive, S2ProtocolVersion protocol, CancellationToken token)
    {
        return await Task.Run(() =>
        {
            var tracker_dec = archive.ReadFile("replay.tracker.events");
            if (tracker_dec != null)
            {
                List<Dictionary<string, object?>> trackerEvents = [];
                foreach (var trackerEvent in protocol.DecodeReplayTrackerEvents(tracker_dec))
                {
                    trackerEvents.Add(trackerEvent);
                }
                return trackerEvents;
            }
            return null;
        }, token).ConfigureAwait(false);
    }

    private static async Task<List<object>?> GetMessagesAsync(MPQArchive archive, S2ProtocolVersion protocol, CancellationToken token)
    {
        return await Task.Run(() =>
        {
            List<object> messageEvents = [];
            var msg_enc = archive.ReadFile("replay.message.events");
            if (msg_enc != null)
            {
                foreach (var messageEvent in protocol.DecodeReplayMessageEvents(msg_enc))
                {
                    messageEvents.Add(messageEvent);
                }
                return messageEvents;
            }
            return null;
        }, token).ConfigureAwait(false);
    }

    private static async Task<ReplayMetadata?> GetMetadataAsync(MPQArchive archive, CancellationToken token)
    {
        return await Task.Run(() =>
        {
            var meta_bytes = archive.ReadFile("replay.gamemetadata.json");
            if (meta_bytes != null)
            {
                var meta_string = Encoding.UTF8.GetString(meta_bytes.ToArray());
                if (meta_string != null)
                {
                    var data = JsonSerializer.Deserialize<ReplayMetadata>(meta_string);
                    return data;
                }
            }
            return null;
        }, token).ConfigureAwait(false);
    }

    private static async Task<object?> GetDetailsAsync(MPQArchive archive, S2ProtocolVersion protocol, CancellationToken token)
    {
        return await Task.Run(() =>
        {
            var details_enc = archive.ReadFile("replay.details");
            if (details_enc != null)
            {
                return protocol.DecodeReplayDetails(details_enc);
            }
            return null;
        }, token).ConfigureAwait(false);
    }

    /// <summary>Shutting down Python engine and call GC.Collect()</summary>
    ///
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        GC.Collect();
    }
}

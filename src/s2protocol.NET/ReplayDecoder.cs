using IronPython.Runtime;
using Microsoft.Scripting.Hosting;
using s2protocol.NET.Models;
using s2protocol.NET.Parser;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using System.Text.RegularExpressions;

namespace s2protocol.NET;

/// <summary>Class <c>ReplayDecoder</c> Starcaft2 replay decoding</summary>
///
public sealed class ReplayDecoder : IDisposable
{
    private readonly ScriptScope scriptScope;
    private dynamic? versions;
    private readonly List<int> intVersions = new();
    private readonly SemaphoreSlim semaphoreSlim = new(1, 1);
    private readonly Regex versionRx = new(@"^protocol(\d+)");


    /// <summary>Creates the decoder</summary>
    /// <param name="appPath">The path to the executing assembly</param>
    /// <example>Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location</example>
    public ReplayDecoder(string appPath)
    {
        scriptScope = LoadEngine(appPath);
    }

    private ScriptScope LoadEngine(string appPath)
    {

        if (!Directory.Exists(appPath))
        {
            throw new ArgumentNullException(nameof(appPath), "Could not find python libraries.");
        }

        try
        {
            ScriptEngine engine = IronPython.Hosting.Python.CreateEngine();
            var paths = engine.GetSearchPaths();
            paths.Add(Path.Combine(appPath, "Lib"));
            paths.Add(Path.Combine(appPath, "libs2"));
            engine.SetSearchPaths(paths);
            var scope = engine.CreateScope();
            // engine.ExecuteFile(appPath + "/libs2/mpyq.py", scope);
            engine.Execute("from mpyq import MPQArchive", scope);
            engine.Execute("import s2protocol", scope);
            engine.Execute("from s2protocol import versions", scope);
            versions = scope.GetVariable("versions");
            List pyVersions = versions.list_all();
            foreach (string v in pyVersions.OrderBy(o => o).Cast<string>())
            {
                var match = versionRx.Match(v);
                if (match.Success)
                {
                    intVersions.Add(int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture));
                }
            }
            return scope;
        }
        catch (Exception ex)
        {
            throw new EngineException(ex.Message);
        }
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
                try
                {
                    var replay = await DecodeAsync(replayPath, options, token).ConfigureAwait(false);
                    ArgumentNullException.ThrowIfNull((object?)replay, nameof(replay));

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
    public async Task<Sc2Replay?> DecodeAsync(string replayPath, ReplayDecoderOptions? options = null, CancellationToken token = default)
    {
        if (!File.Exists(replayPath))
        {
            throw new ArgumentNullException(nameof(replayPath), "Replay not found.");
        }

        if (versions == null)
        {
            throw new DecodeException("No Versions found.");
        }

        if (options == null)
        {
            options = new ReplayDecoderOptions();
        }

        dynamic? archive = null;
        try
        {
            var MPQArchive = scriptScope.GetVariable("MPQArchive");
            archive = scriptScope.Engine.Operations.CreateInstance(MPQArchive, replayPath, false);
            ArgumentNullException.ThrowIfNull((object?)archive, nameof(archive));

            var header = await GetHeaderAsync(archive, versions, token);
            ArgumentNullException.ThrowIfNull((object?)header, nameof(header));

            int baseBuild = header["m_version"]["m_baseBuild"];

            if (!intVersions.Contains(baseBuild))
            {
                int replBuild = baseBuild;
                baseBuild = intVersions.LastOrDefault(f => f < baseBuild);
                if (baseBuild == 0)
                {
                    baseBuild = intVersions.Last();
                }
            }

            await semaphoreSlim.WaitAsync(token).ConfigureAwait(false);
            dynamic? protocol;
            try
            {
                protocol = versions.build(baseBuild);
            }
            finally
            {
                semaphoreSlim.Release();
            }
            ArgumentNullException.ThrowIfNull((object?)protocol, nameof(protocol));

            Sc2Replay replay = new(header, replayPath);

            if (options.Initdata)
            {
                var init = await GetInitdataAsync(archive, protocol, token);
                ArgumentNullException.ThrowIfNull((object?)init, nameof(init));

                replay.Initdata = Parse.InitData(init);
            }

            if (options.Details)
            {
                var details = await GetDetailsAsync(archive, protocol, token);
                ArgumentNullException.ThrowIfNull((object?)details, nameof(details));

                replay.Details = Parse.Datails(details);
            }

            if (options.Metadata)
            {
                var metadata = await GetMetadataAsync(archive, token);
                ArgumentNullException.ThrowIfNull((object?)metadata, nameof(metadata));

                replay.Metadata = metadata;
            }

            if (options.MessageEvents)
            {
                var messages = await GetMessagesAsync(archive, protocol, token);
                ArgumentNullException.ThrowIfNull((object?)messages, nameof(messages));

                Parse.SetMessages(messages, replay);
            }

            if (options.TrackerEvents)
            {
                var trackerEvents = await GetTrackereventsAsync(archive, protocol, token);
                ArgumentNullException.ThrowIfNull((object?)trackerEvents, nameof(trackerEvents));

                replay.TrackerEvents = Parse.Tracker(trackerEvents);

                if (replay.TrackerEvents != null)
                {
                    replay.TrackerEvents.SUnitBornEvents.ToList().ForEach(f => f.UnitIndex = GetUnitIndex(protocol, f.UnitTagIndex, f.UnitTagRecycle));
                    replay.TrackerEvents.SUnitInitEvents.ToList().ForEach(f => f.UnitIndex = GetUnitIndex(protocol, f.UnitTagIndex, f.UnitTagRecycle));
                    replay.TrackerEvents.SUnitDiedEvents.ToList().ForEach(f => f.UnitIndex = GetUnitIndex(protocol, f.UnitTagIndex, f.UnitTagRecycle));
                    replay.TrackerEvents.SUnitDoneEvents.ToList().ForEach(f => f.UnitIndex = GetUnitIndex(protocol, f.UnitTagIndex, f.UnitTagRecycle));
                    replay.TrackerEvents.SUnitOwnerChangeEvents.ToList().ForEach(f => f.UnitIndex = GetUnitIndex(protocol, f.UnitTagIndex, f.UnitTagRecycle));
                    Parse.SetTrackerEventsUnitConnections(replay.TrackerEvents);
                }
            }

            if (options.GameEvents)
            {
                var gameEvents = await GetGameEventsAsync(archive, protocol, token);
                ArgumentNullException.ThrowIfNull((object?)gameEvents, nameof(gameEvents));

                replay.GameEvents = Parse.GameEvents(gameEvents);
            }

            if (options.AttributeEvents)
            {
                var attributeEvents = await GetAttributeEventsAsync(archive, protocol, token);
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
        finally
        {
            if (archive is not null)
            {
                archive.file.close();
                archive.file = null;
                archive = null;
            }
        }
    }

    private static int GetUnitIndex(dynamic protocol, int unitTagIndex, int unitTagRecyle)
    {
        // todo: can be BitInterger
        var unitTag = protocol.unit_tag(unitTagIndex, unitTagRecyle);
        if (unitTag is int intUnitTag)
        {
            return intUnitTag;
        }
        else
        {
            return 0;
        }
    }

    private static async Task<dynamic?> GetAttributeEventsAsync(dynamic archive, dynamic protocol, CancellationToken token)
    {
        return await Task.Run(() =>
        {
            var game_enc = archive.read_file("replay.attributes.events");
            if (game_enc != null)
            {
                return protocol.decode_replay_attributes_events(game_enc);
            }
            return null;
        }, token).ConfigureAwait(false);
    }

    private static async Task<dynamic?> GetGameEventsAsync(dynamic archive, dynamic protocol, CancellationToken token)
    {
        return await Task.Run(() =>
        {
            var game_enc = archive.read_file("replay.game.events");
            if (game_enc != null)
            {
                return protocol.decode_replay_game_events(game_enc);
            }
            return null;
        }, token).ConfigureAwait(false);
    }

    private static async Task<dynamic?> GetInitdataAsync(dynamic archive, dynamic protocol, CancellationToken token)
    {
        return await Task.Run(() =>
        {
            var init_enc = archive.read_file("replay.initData");
            if (init_enc != null)
            {
                return protocol.decode_replay_initdata(init_enc);
            }
            return null;
        }, token).ConfigureAwait(false);
    }

    private static async Task<dynamic?> GetTrackereventsAsync(dynamic archive, dynamic protocol, CancellationToken token)
    {
        return await Task.Run(() =>
        {
            var tracker_dec = archive.read_file("replay.tracker.events");
            if (tracker_dec != null)
            {
                return protocol.decode_replay_tracker_events(tracker_dec);
            }
            return null;
        }, token).ConfigureAwait(false);
    }

    private static async Task<dynamic?> GetMessagesAsync(dynamic archive, dynamic protocol, CancellationToken token)
    {
        return await Task.Run(() =>
        {
            var msg_enc = archive.read_file("replay.message.events");
            if (msg_enc != null)
            {
                return protocol.decode_replay_message_events(msg_enc);
            }
            return null;
        }, token).ConfigureAwait(false);
    }

    private static async Task<Metadata?> GetMetadataAsync(dynamic archive, CancellationToken token)
    {
        return await Task.Run(() =>
        {
            Bytes? meta_bytes = archive.read_file("replay.gamemetadata.json");
            if (meta_bytes != null)
            {
                var meta_string = Encoding.UTF8.GetString(meta_bytes.ToArray());
                if (meta_string != null)
                {
                    var data = JsonSerializer.Deserialize<Metadata>(meta_string);
                    return data;
                }
            }
            return null;
        }, token).ConfigureAwait(false);
    }

    private static async Task<dynamic?> GetDetailsAsync(dynamic archive, dynamic protocol, CancellationToken token)
    {
        return await Task.Run(() =>
        {
            var details_enc = archive.read_file("replay.details");
            if (details_enc != null)
            {
                return protocol.decode_replay_details(details_enc);
            }
            return null;
        }, token).ConfigureAwait(false);
    }

    private async Task<dynamic?> GetHeaderAsync(dynamic archive, dynamic versions, CancellationToken token)
    {
        return await Task.Run(async () =>
        {
            var contents = archive.header["user_data_header"]["content"];
            await semaphoreSlim.WaitAsync(token).ConfigureAwait(false);
            try
            {
                return versions.latest().decode_replay_header(contents);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }, token).ConfigureAwait(false);
    }

    /// <summary>Shutting down Python engine and call GC.Collect()</summary>
    ///
    public void Dispose()
    {
        scriptScope.Engine.Runtime.Shutdown();
        semaphoreSlim.Dispose();
        GC.SuppressFinalize(this);
        GC.Collect();
    }
}

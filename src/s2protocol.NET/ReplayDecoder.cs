﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using IronPython.Runtime;
using Microsoft.Scripting.Hosting;
using s2protocol.NET.Parser;
using System.Text.Json;
using s2protocol.NET.Models;

namespace s2protocol.NET;

/// <summary>Class <c>ReplayDecoder</c> Starcaft2 replay decoding</summary>
///
public sealed class ReplayDecoder : IDisposable
{
    private readonly ScriptScope scriptScope;
    dynamic? versions;
    List<int> intVersions = new List<int>();
    private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

    /// <summary>Creates the decoder</summary>
    /// <param name="appPath">The path to the executing assembly
    /// <example>Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location</example>
    /// </param>
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
            paths.Add(Path.Combine(appPath, "pylib"));
            engine.SetSearchPaths(paths);
            var scope = engine.CreateScope();
            engine.ExecuteFile(appPath + "/pylib/mpyq.py", scope);
            engine.Execute("import s2protocol", scope);
            engine.Execute("from s2protocol import versions", scope);
            versions = scope.GetVariable("versions");
            List pyVersions = versions.list_all();
            foreach (string v in pyVersions)
            {
                intVersions.Add(int.Parse(v.Substring(8, 5), CultureInfo.InvariantCulture));
            }

            return scope;
        }
        catch (Exception ex)
        {
            throw new EngineException(ex.Message);
        }
    }

    /// <summary>Decode Starcraft2 replay</summary>
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

    private async Task Produce(Channel<Sc2Replay> channel, ICollection<string> replayPaths, int threads, ReplayDecoderOptions? options, CancellationToken token)
    {
        ParallelOptions parallelOptions = new ParallelOptions()
        {
            CancellationToken = token,
            MaxDegreeOfParallelism = threads
        };

        try
        {
            await Parallel.ForEachAsync(replayPaths, parallelOptions, async (replayPath, token) =>
            {
                var replay = await DecodeAsync(replayPath, options, token).ConfigureAwait(false);
                if (replay != null)
                {
                    channel.Writer.TryWrite(replay);
                }
            }).ConfigureAwait(false);
        }
        catch (OperationCanceledException) { }
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
            throw new ArgumentNullException(nameof(versions), "No Versions found");
        }

        if (options == null)
        {
            options = new ReplayDecoderOptions();
        }

        dynamic MPQArchive = scriptScope.GetVariable("MPQArchive");
        var archive = MPQArchive(replayPath);

        var header = await GetHeaderAsync(archive, versions, token);
        if (header == null)
        {
            if (token.IsCancellationRequested)
                return null;
            throw new DecodeException($"could not get replay header {replayPath}");
        }

        int baseBuild = header["m_version"]["m_baseBuild"];

        if (!intVersions.Contains(baseBuild))
        {
            int replBuild = baseBuild;
            baseBuild = intVersions.FirstOrDefault(f => f > baseBuild);
            if (baseBuild == 0)
            {
                baseBuild = intVersions.Last();
            }
            Console.WriteLine($"fixed protocol from {replBuild} to {baseBuild}");
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
        if (protocol == null)
        {
            if (token.IsCancellationRequested)
                return null;
            throw new DecodeException($"could not get replay protocol {replayPath} {baseBuild}");
        }

        Sc2Replay replay = new Sc2Replay(header);

        if (options.Details)
        {
            var details = await GetDetailsAsync(archive, protocol, token);

            if (details == null)
            {
                if (token.IsCancellationRequested)
                    return null;
                throw new DecodeException($"could not get replay details {replayPath}");
            }
            replay.Details = Parse.Datails(details);
        }

        if (options.Metadata)
        {
            var metadata = await GetMetadataAsync(archive, protocol, token);
            if (metadata == null)
            {
                if (token.IsCancellationRequested)
                    return null;
                throw new DecodeException($"could not get replay metadata {replayPath}");
            }
            replay.Metadata = metadata;
        }

        if (options.MessageEvents)
        {
            var messages = await GetMessagesAsync(archive, protocol, token);
            if (messages == null)
            {
                if (token.IsCancellationRequested)
                    return null;
                throw new DecodeException($"could not get replay messages {replayPath}");
            }
            replay.ChatMessages = Parse.Messages(messages);
        }

        if (options.TrackerEvents)
        {
            var trackerEvents = await GetTrackereventsAsync(archive, protocol, token);
            if (trackerEvents == null)
            {
                if (token.IsCancellationRequested)
                    return null;
                throw new DecodeException($"could not get replay TrackerEvents {replayPath}");
            }
            replay.TrackerEvents = Parse.Tracker(trackerEvents);
        }

        return replay;
    }

    private static async Task<dynamic?> GetTrackereventsAsync(dynamic archive, dynamic protocol, CancellationToken token)
    {
        try
        {
            return await Task.Run(() =>
            {
                var treacker_enc = archive.read_file("replay.tracker.events");
                if (treacker_enc != null)
                {
                    return protocol.decode_replay_tracker_events(treacker_enc);
                }
                return null;
            }, token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) { }
        return null;
    }

    private static async Task<dynamic?> GetMessagesAsync(dynamic archive, dynamic protocol, CancellationToken token)
    {
        try
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
        catch (OperationCanceledException) { }
        return null;
    }

    private static async Task<Metadata?> GetMetadataAsync(dynamic archive, dynamic protocol, CancellationToken token)
    {
        try
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
        catch (OperationCanceledException) { }
        return null;
    }

    private static async Task<dynamic?> GetDetailsAsync(dynamic archive, dynamic protocol, CancellationToken token)
    {
        try
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
        catch (OperationCanceledException) { }
        return null;
    }

    private async Task<dynamic?> GetHeaderAsync(dynamic archive, dynamic versions, CancellationToken token)
    {
        try
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
        catch (OperationCanceledException) { }
        return null;
    }



    /// <summary>Shutting down Python engine</summary>
    ///
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        semaphoreSlim.Dispose();
        scriptScope.Engine.Runtime.Shutdown();
    }
}

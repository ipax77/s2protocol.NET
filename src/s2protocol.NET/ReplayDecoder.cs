using s2protocol.NET.Models;
using s2protocol.NET.Mpq;
using s2protocol.NET.Parser;
using s2protocol.NET.S2Protocol;
using System.Text.Json;

namespace s2protocol.NET;

/// <summary>
/// Provides functionality to decode Starcraft II replay files.
/// </summary>
/// <remarks>The <see cref="ReplayDecoder"/> class is designed to decode individual Starcraft II replay files with
/// customizable decoding options. For large replay sets, prefer <see cref="DecodeAsync(string, ReplayDecoderOptions?, CancellationToken)"/>
/// with caller-owned parallelism so the caller can control buffering, persistence, and memory use.</remarks>
public sealed partial class ReplayDecoder : IDisposable
{
    /// <summary>Creates the decoder</summary>
    public ReplayDecoder()
    {
    }

    /// <summary>Decode Starcraft2 replay</summary>
    /// <param name="stream">The stream of the Starcraft2 replay</param>
    /// <param name="options">Optional decoding options</param>
    /// <param name="token">Optional CancellationToken</param>
    public async Task<Sc2Replay?> DecodeAsync(Stream stream,
                                              ReplayDecoderOptions? options = null,
                                              CancellationToken token = default)
    {
        MPQArchive archive = new(stream, false);
        var replay = await DecodeAsync(archive, string.Empty, options, token)
            .ConfigureAwait(false);
        archive.Dispose();
        return replay;
    }

    /// <summary>Decode Starcraft2 replay</summary>
    /// <param name="replayPath">The path to the Starcraft2 replay</param>
    /// <param name="options">Optional decoding options</param>
    /// <param name="token">Optional CancellationToken</param>
    public async Task<Sc2Replay?> DecodeAsync(string replayPath,
                                              ReplayDecoderOptions? options = null,
                                              CancellationToken token = default)
    {
        if (!File.Exists(replayPath))
        {
            throw new ArgumentNullException(nameof(replayPath), "Replay not found.");
        }
        MPQArchive archive = new(replayPath, false);
        var replay = await DecodeAsync(archive, replayPath, options, token)
            .ConfigureAwait(false);
        archive.Dispose();
        return replay;
    }

#pragma warning disable CA1822 // Mark members as static
    private async Task<Sc2Replay?> DecodeAsync(MPQArchive MPQArchive,
                                               string replayPath,
                                               ReplayDecoderOptions? options = null,
                                               CancellationToken token = default)
#pragma warning restore CA1822 // Mark members as static
    {
        if (options == null)
        {
            options = new ReplayDecoderOptions();
        }

        try
        {
            var headerContent = MPQArchive.GetUserDataHeaderContent();
            ArgumentNullException.ThrowIfNull(headerContent);

            var latestVersion = TypeInfoLoader.GetLatestVersion();
            var header = latestVersion.DecodeReplayHeader(headerContent);
            var s2protocol = TypeInfoLoader.LoadTypeInfos(header.BaseBuild);
            ArgumentNullException.ThrowIfNull(s2protocol, nameof(s2protocol));

            Sc2Replay replay = new(header, replayPath);

            if (options.Initdata)
            {
                var init = await GetInitDataAsync(MPQArchive, s2protocol, token).ConfigureAwait(false);
                ArgumentNullException.ThrowIfNull(init, nameof(init));

                replay.Initdata = init;
            }

            if (options.Details)
            {
                var details = await GetDetailsAsync(MPQArchive, s2protocol, token).ConfigureAwait(false);
                ArgumentNullException.ThrowIfNull(details, nameof(details));
                replay.Details = details;
            }

            if (options.Metadata)
            {
                var metadata = await GetMetadataAsync(MPQArchive, token).ConfigureAwait(false);
                ArgumentNullException.ThrowIfNull(metadata, nameof(metadata));

                replay.Metadata = metadata;
            }

            if (options.MessageEvents)
            {
                var messages = await GetMessagesAsync(MPQArchive, s2protocol, token).ConfigureAwait(false);
                ArgumentNullException.ThrowIfNull(messages, nameof(messages));
                replay.ChatMessages = messages.ChatMessages;
                replay.PingMessages = messages.PingMessages;
            }

            if (options.TrackerEvents)
            {
                var trackerEvents = await GetTrackereventsAsync(MPQArchive, s2protocol, token).ConfigureAwait(false);
                ArgumentNullException.ThrowIfNull(trackerEvents, nameof(trackerEvents));

                replay.TrackerEvents = trackerEvents;

                if (replay.TrackerEvents != null)
                {
                    SetTrackerEventUnitIndexes(replay.TrackerEvents);
                    Parse.SetTrackerEventsUnitConnections(replay.TrackerEvents);
                }
            }

            if (options.GameEvents)
            {
                var gameEventsRaw = await GetGameEventsAsync(MPQArchive, s2protocol, token).ConfigureAwait(false);
                ArgumentNullException.ThrowIfNull(gameEventsRaw, nameof(gameEventsRaw));
                replay.GameEvents = gameEventsRaw;
            }

            if (options.AttributeEvents)
            {
                var attributeEvents = await GetAttributeEventsAsync(MPQArchive, token).ConfigureAwait(false);
                ArgumentNullException.ThrowIfNull(attributeEvents, nameof(attributeEvents));

                replay.AttributeEvents = attributeEvents;
            }



            return replay;
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception ex)
        {
            throw new DecodeException(ex.Message);
        }
    }

    private static void SetTrackerEventUnitIndexes(TrackerEvents trackerEvents)
    {
        foreach (var e in trackerEvents.SUnitBornEvents)
        {
            e.UnitIndex = GetUnitIndex(e.UnitTagIndex, e.UnitTagRecycle);
        }

        foreach (var e in trackerEvents.SUnitInitEvents)
        {
            e.UnitIndex = GetUnitIndex(e.UnitTagIndex, e.UnitTagRecycle);
        }

        foreach (var e in trackerEvents.SUnitDiedEvents)
        {
            e.UnitIndex = GetUnitIndex(e.UnitTagIndex, e.UnitTagRecycle);
        }

        foreach (var e in trackerEvents.SUnitDoneEvents)
        {
            e.UnitIndex = GetUnitIndex(e.UnitTagIndex, e.UnitTagRecycle);
        }

        foreach (var e in trackerEvents.SUnitOwnerChangeEvents)
        {
            e.UnitIndex = GetUnitIndex(e.UnitTagIndex, e.UnitTagRecycle);
        }
    }

    private static int GetUnitIndex(int unitTagIndex, int unitTagRecyle)
    {
        // todo: can be BitInterger
        var unitTag = S2ProtocolVersion.UnitTag(unitTagIndex, unitTagRecyle);
        return unitTag is int intUnitTag ? intUnitTag : 0;
    }

    private static async Task<AttributeEvents?> GetAttributeEventsAsync(MPQArchive archive, CancellationToken token)
    {
        var game_enc = await archive.ReadFileAsync("replay.attributes.events", false, token).ConfigureAwait(false);
        return game_enc.HasValue ? S2ProtocolVersion.DecodeReplayAttributeEvents(game_enc.Value) : null;
    }

    private static async Task<GameEvents?> GetGameEventsAsync(MPQArchive archive, S2ProtocolVersion protocol, CancellationToken token)
    {
        var game_enc = await archive.ReadFileAsync("replay.game.events", false, token).ConfigureAwait(false);
        return game_enc.HasValue ? protocol.DecodeReplayGameEvents(game_enc.Value) : null;
    }

    private static async Task<Initdata?> GetInitDataAsync(MPQArchive archive, S2ProtocolVersion protocol, CancellationToken token)
    {
        var init_enc = await archive.ReadFileAsync("replay.initData", false, token).ConfigureAwait(false);
        return init_enc.HasValue ? protocol.DecodeReplayInitData(init_enc.Value) : null;
    }

    private static async Task<TrackerEvents?> GetTrackereventsAsync(MPQArchive archive, S2ProtocolVersion protocol, CancellationToken token)
    {
        var tracker_dec = await archive.ReadFileAsync("replay.tracker.events", false, token).ConfigureAwait(false);
        return tracker_dec.HasValue ? protocol.DecodeReplayTrackerEvents(tracker_dec.Value) : null;
    }

    private static async Task<MessageEvents?> GetMessagesAsync(MPQArchive archive, S2ProtocolVersion protocol, CancellationToken token)
    {
        var msg_enc = await archive.ReadFileAsync("replay.message.events", false, token).ConfigureAwait(false);
        return msg_enc.HasValue ? protocol.DecodeReplayMessageEvents(msg_enc.Value) : null;
    }

    private static async Task<ReplayMetadata?> GetMetadataAsync(MPQArchive archive, CancellationToken token)
    {
        var meta_bytes = await archive.ReadFileAsync("replay.gamemetadata.json", false, token).ConfigureAwait(false);
        return meta_bytes.HasValue
            ? JsonSerializer.Deserialize(meta_bytes.Value.Span, S2ProtocolJsonSerializerContext.Default.ReplayMetadata)
            : null;
    }

    private static async Task<Details?> GetDetailsAsync(MPQArchive archive, S2ProtocolVersion protocol, CancellationToken token)
    {
        var details_enc = await archive.ReadFileAsync("replay.details", false, token).ConfigureAwait(false);
        return details_enc.HasValue ? protocol.DecodeReplayDetails(details_enc.Value) : null;
    }

    /// <summary>Suppress finalization for compatibility with <see cref="IDisposable"/>.</summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}

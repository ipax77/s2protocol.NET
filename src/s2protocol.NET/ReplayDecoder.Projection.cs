using s2protocol.NET.Models;
using s2protocol.NET.Mpq;
using s2protocol.NET.S2Protocol;

namespace s2protocol.NET;

public sealed partial class ReplayDecoder
{
#pragma warning disable CA1822 // The projection overloads are part of the instance decoder API.
    public async Task<TResult> DecodeAsync<TResult>(
        Stream stream,
        IReplayProjection<TResult> projection,
        CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(projection);

        using MPQArchive archive = new(stream, false);
        return await DecodeProjectionAsync(archive, string.Empty, projection, token).ConfigureAwait(false);
    }

    public async Task<TResult> DecodeAsync<TResult>(
        string replayPath,
        IReplayProjection<TResult> projection,
        CancellationToken token = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(replayPath);
        ArgumentNullException.ThrowIfNull(projection);

        if (!File.Exists(replayPath))
        {
            throw new ArgumentNullException(nameof(replayPath), "Replay not found.");
        }

        using MPQArchive archive = new(replayPath, false);
        return await DecodeProjectionAsync(archive, replayPath, projection, token).ConfigureAwait(false);
    }
#pragma warning restore CA1822

    private static async Task<TResult> DecodeProjectionAsync<TResult>(
        MPQArchive archive,
        string replayPath,
        IReplayProjection<TResult> projection,
        CancellationToken token)
    {
        var headerContent = archive.GetUserDataHeaderContent();
        ArgumentNullException.ThrowIfNull(headerContent);

        var latestVersion = TypeInfoLoader.GetLatestVersion();
        Header header = latestVersion.DecodeReplayHeader(headerContent);
        var protocol = TypeInfoLoader.LoadTypeInfos(header.BaseBuild);
        ArgumentNullException.ThrowIfNull(protocol, nameof(protocol));

        ReplayProjectionProfile profile = projection.Profile;
        projection.OnHeader(header, replayPath);

        if (profile.Initdata)
        {
            var init = await GetInitDataAsync(archive, protocol, token).ConfigureAwait(false);
            ArgumentNullException.ThrowIfNull(init, nameof(init));
            projection.OnInitdata(init);
        }

        if (profile.Details)
        {
            var details = await GetDetailsAsync(archive, protocol, token).ConfigureAwait(false);
            ArgumentNullException.ThrowIfNull(details, nameof(details));
            projection.OnDetails(details);
        }

        if (profile.Metadata)
        {
            var metadata = await GetMetadataAsync(archive, token).ConfigureAwait(false);
            ArgumentNullException.ThrowIfNull(metadata, nameof(metadata));
            projection.OnMetadata(metadata);
        }

        if (profile.MessageEvents.Enabled)
        {
            var messageBytes = await archive.ReadFileAsync("replay.message.events", false, token).ConfigureAwait(false);
            if (messageBytes.HasValue)
            {
                protocol.DecodeReplayMessageEvents(
                    messageBytes.Value,
                    profile.MessageEvents,
                    projection.OnChatMessage,
                    projection.OnPingMessage);
            }
        }

        if (profile.TrackerEvents.Enabled)
        {
            var trackerBytes = await archive.ReadFileAsync("replay.tracker.events", false, token).ConfigureAwait(false);
            if (trackerBytes.HasValue)
            {
                protocol.DecodeReplayTrackerEvents(trackerBytes.Value, profile.TrackerEvents, projection.OnTrackerEvent);
            }
        }

        if (profile.GameEvents.Enabled)
        {
            var gameBytes = await archive.ReadFileAsync("replay.game.events", false, token).ConfigureAwait(false);
            if (gameBytes.HasValue)
            {
                protocol.DecodeReplayGameEvents(gameBytes.Value, profile.GameEvents, projection.OnGameEvent);
            }
        }

        if (profile.AttributeEvents)
        {
            var attributeEvents = await GetAttributeEventsAsync(archive, token).ConfigureAwait(false);
            ArgumentNullException.ThrowIfNull(attributeEvents, nameof(attributeEvents));
            projection.OnAttributeEvents(attributeEvents);
        }

        return projection.Complete();
    }
}

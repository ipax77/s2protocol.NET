using s2protocol.NET.Models;
using s2protocol.NET.S2Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace s2protocol.NET.tests;

public partial class MessageEventCoverageTests
{
    private static readonly IReadOnlyDictionary<string, Type> ExpectedModels =
        new Dictionary<string, Type>(StringComparer.Ordinal)
        {
            ["NNet.Game.SChatMessage"] = typeof(ChatMessageEvent),
            ["NNet.Game.SPingMessage"] = typeof(PingMessageEvent),
            ["NNet.Game.SLoadingProgressMessage"] = typeof(LoadingProgressMessageEvent),
            ["NNet.Game.SServerPingMessage"] = typeof(ServerPingMessageEvent),
            ["NNet.Game.SReconnectNotifyMessage"] = typeof(ReconnectNotifyMessageEvent),
        };

    [Fact]
    public void EveryBundledMessageEventDecodesToItsPublicConcreteModel()
    {
        HashSet<string> union = new(StringComparer.Ordinal);
        int[] versions = CanonicalProtocolVersions();
        Assert.Equal(23, versions.Length);

        foreach (int version in versions)
        {
            S2ProtocolVersion protocol = TypeInfoLoader.LoadTypeInfos(version);
            foreach ((int eventId, S2EventType eventType) in protocol.MessageEvents)
            {
                union.Add(eventType.Name);
                byte[] encoded = BitPackedTestEncoder.EncodeMessageEvent(protocol, eventId);
                MessageEvent decoded = Assert.Single(
                    protocol.DecodeReplayMessageEvents(encoded).BaseMessageEvents);

                Assert.IsNotType<UnknownMessageEvent>(decoded);
                Assert.IsType(ExpectedModels[eventType.Name], decoded);
                Assert.NotEqual(MessageEventType.None, decoded.EventType);
                Assert.Equal(eventId, decoded.EventId);
                Assert.Equal(encoded.Length * 8, decoded.Bits);
            }
        }

        Assert.Equal(5, union.Count);
        Assert.Equal(ExpectedModels.Keys.Order(), union.Order());
    }

    [Fact]
    public void EarliestLoadingProgressDecodesSignedValueAndMetadata()
    {
        S2ProtocolVersion protocol = TypeInfoLoader.LoadTypeInfos(15405);
        KeyValuePair<int, S2EventType> eventType = Find(protocol, "NNet.Game.SLoadingProgressMessage");
        byte[] encoded = BitPackedTestEncoder.EncodeMessageEvent(
            protocol,
            eventType.Key,
            new Dictionary<string, object?> { ["m_progress"] = -123 },
            gameloopDelta: 17,
            userId: 3);

        LoadingProgressMessageEvent decoded = Assert.IsType<LoadingProgressMessageEvent>(
            Assert.Single(protocol.DecodeReplayMessageEvents(encoded).BaseMessageEvents));

        Assert.Equal(-123, decoded.Progress);
        Assert.Equal(3, decoded.UserId);
        Assert.Equal(17, decoded.Gameloop);
        Assert.Equal(eventType.Key, decoded.EventId);
        Assert.Equal(encoded.Length * 8, decoded.Bits);
    }

    [Fact]
    public void CurrentProtocolPreservesMixedMessageStreamOrderAndPayloads()
    {
        S2ProtocolVersion protocol = TypeInfoLoader.GetLatestVersion();
        TestMessageEvent[] input =
        [
            Create(protocol, "NNet.Game.SChatMessage", 1, 1,
                new Dictionary<string, object?> { ["m_recipient"] = 2, ["m_string"] = "hello" }),
            Create(protocol, "NNet.Game.SPingMessage", 2, 2,
                new Dictionary<string, object?> { ["m_recipient"] = 1, ["m_point.x"] = 11L, ["m_point.y"] = 12L }),
            Create(protocol, "NNet.Game.SLoadingProgressMessage", 3, 3,
                new Dictionary<string, object?> { ["m_progress"] = 99 }),
            Create(protocol, "NNet.Game.SServerPingMessage", 4, 4),
            Create(protocol, "NNet.Game.SReconnectNotifyMessage", 5, 5,
                new Dictionary<string, object?> { ["m_status"] = 2 }),
        ];

        byte[] encoded = BitPackedTestEncoder.EncodeMessageEvents(protocol, input);
        MessageEvents decoded = protocol.DecodeReplayMessageEvents(encoded);
        MessageEvent[] events = [.. decoded.BaseMessageEvents];

        Assert.Collection(events,
            item =>
            {
                ChatMessageEvent chat = Assert.IsType<ChatMessageEvent>(item);
                Assert.Equal("hello", chat.Message);
                Assert.Equal(2, chat.Recipient);
                Assert.Equal(1, chat.Gameloop);
            },
            item =>
            {
                PingMessageEvent ping = Assert.IsType<PingMessageEvent>(item);
                Assert.Equal(11, ping.X);
                Assert.Equal(12, ping.Y);
                Assert.Equal(3, ping.Gameloop);
            },
            item =>
            {
                LoadingProgressMessageEvent progress = Assert.IsType<LoadingProgressMessageEvent>(item);
                Assert.Equal(99, progress.Progress);
                Assert.Equal(6, progress.Gameloop);
            },
            item =>
            {
                Assert.IsType<ServerPingMessageEvent>(item);
                Assert.Equal(10, item.Gameloop);
            },
            item =>
            {
                ReconnectNotifyMessageEvent reconnect = Assert.IsType<ReconnectNotifyMessageEvent>(item);
                Assert.Equal(2, reconnect.Status);
                Assert.Equal(15, reconnect.Gameloop);
            });

        Assert.Single(decoded.ChatMessages);
        Assert.Single(decoded.PingMessages);
        Assert.All(events, item => Assert.True(item.Bits > 0));
    }

    [Fact]
    public void UnknownSchemaNameProducesMetadataPlaceholder()
    {
        S2ProtocolVersion canonical = TypeInfoLoader.GetLatestVersion();
        S2ProtocolVersion protocol = canonical with
        {
            MessageEvents = new Dictionary<int, S2EventType>(canonical.MessageEvents),
        };
        KeyValuePair<int, S2EventType> serverPing = Find(protocol, "NNet.Game.SServerPingMessage");
        protocol.MessageEvents[serverPing.Key] = serverPing.Value with { Name = "NNet.Game.SFutureMessage" };

        byte[] encoded = BitPackedTestEncoder.EncodeMessageEvent(
            protocol, serverPing.Key, gameloopDelta: 8, userId: 2);
        UnknownMessageEvent decoded = Assert.IsType<UnknownMessageEvent>(
            Assert.Single(protocol.DecodeReplayMessageEvents(encoded).BaseMessageEvents));

        Assert.Equal("NNet.Game.SFutureMessage", decoded.EventTypeName);
        Assert.Equal(MessageEventType.None, decoded.EventType);
        Assert.Equal(serverPing.Key, decoded.EventId);
        Assert.Equal(2, decoded.UserId);
        Assert.Equal(8, decoded.Gameloop);
        Assert.Equal(encoded.Length * 8, decoded.Bits);
    }

    [Fact]
    public void LegacyViewsStaySynchronizedAndMutable()
    {
        MessageEvents events = new([], []);
        ChatMessageEvent first = new(1, 2, "first", 3);
        PingMessageEvent ping = new(1, 2, 3, 4, 5);

        events.ChatMessages.Add(first);
        events.BaseMessageEvents.Add(ping);

        Assert.Equal(2, events.BaseMessageEvents.Count);
        Assert.Single(events.ChatMessages);
        Assert.Single(events.PingMessages);
        Assert.Same(first, events.BaseMessageEvents.First());

        Assert.True(events.ChatMessages.Remove(first));
        Assert.Empty(events.ChatMessages);
        Assert.Single(events.BaseMessageEvents);

        events.PingMessages.Clear();
        Assert.Empty(events.PingMessages);
        Assert.Empty(events.BaseMessageEvents);
    }

    [Fact]
    public void UnifiedCollectionSerializesConcretePayloadsAndLegacyProperties()
    {
        MessageEvents events = new([], []);
        events.BaseMessageEvents.Add(new LoadingProgressMessageEvent(42, 1, 2, 24, 3));
        events.ChatMessages.Add(new ChatMessageEvent(1, 2, "serialized", 3));

        string json = JsonSerializer.Serialize(events);

        Assert.Contains("BaseMessageEvents", json, StringComparison.Ordinal);
        Assert.Contains("LoadingProgressMessageEvent", json, StringComparison.Ordinal);
        Assert.Contains("\"Progress\":42", json, StringComparison.Ordinal);
        Assert.Contains("ChatMessages", json, StringComparison.Ordinal);
        Assert.Contains("serialized", json, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ReplayDecoderExposesUnifiedAndLegacyViewsAsync()
    {
        string replayPath = Path.Combine(AppContext.BaseDirectory, "replays", "test2.SC2Replay");
        using ReplayDecoder decoder = new();
        ReplayDecoderOptions options = new()
        {
            Initdata = false,
            Details = false,
            Metadata = false,
            MessageEvents = true,
            TrackerEvents = false,
            GameEvents = false,
            AttributeEvents = false,
        };

        Sc2Replay replay = Assert.IsType<Sc2Replay>(await decoder.DecodeAsync(replayPath, options));
        MessageEvents messages = Assert.IsType<MessageEvents>(replay.MessageEvents);

        Assert.Same(messages.ChatMessages, replay.ChatMessages);
        Assert.Same(messages.PingMessages, replay.PingMessages);
        Assert.Equal(messages.BaseMessageEvents.Count,
            messages.ChatMessages.Count + messages.PingMessages.Count
            + messages.BaseMessageEvents.Count(item => item is not ChatMessageEvent and not PingMessageEvent));
    }

    private static TestMessageEvent Create(
        S2ProtocolVersion protocol,
        string name,
        int gameloopDelta,
        int userId,
        IReadOnlyDictionary<string, object?>? values = null)
        => new(Find(protocol, name).Key, gameloopDelta, userId, values);

    private static KeyValuePair<int, S2EventType> Find(S2ProtocolVersion protocol, string name)
        => protocol.MessageEvents.Single(item => string.Equals(item.Value.Name, name, StringComparison.Ordinal));

    private static int[] CanonicalProtocolVersions()
        => typeof(TypeInfoLoader).Assembly.GetManifestResourceNames()
            .Select(name => ProtocolResourceRegex().Match(name))
            .Where(match => match.Success)
            .Select(match => int.Parse(match.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture))
            .Order()
            .ToArray();

    [GeneratedRegex(@"\.protocol(\d+)\.json$", RegexOptions.CultureInvariant)]
    private static partial Regex ProtocolResourceRegex();
}

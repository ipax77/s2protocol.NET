using s2protocol.NET.Models;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;

namespace s2protocol.NET.tests;

public sealed class JsonContractTests
{
    private static readonly string ReplayDirectory = Path.Combine(
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            ?? throw new InvalidOperationException("Could not resolve the test assembly path."),
        "replays");

    [Fact]
    public async Task CompleteReplayRoundTripsWithCanonicalSourceGeneratedContract()
    {
        ReplayDecoder decoder = new();
        Sc2Replay replay = await decoder.DecodeAsync(Path.Combine(ReplayDirectory, "test11.SC2Replay"));

        string json = JsonSerializer.Serialize(replay, TestJsonSerializerContext.Default.Sc2Replay);
        Sc2Replay roundTripped = Assert.IsType<Sc2Replay>(
            JsonSerializer.Deserialize(json, TestJsonSerializerContext.Default.Sc2Replay));
        string secondJson = JsonSerializer.Serialize(roundTripped, TestJsonSerializerContext.Default.Sc2Replay);

        Assert.True(JsonNode.DeepEquals(JsonNode.Parse(json), JsonNode.Parse(secondJson)), FirstDifference(json, secondJson));
        Assert.Contains("\"$type\":", json, StringComparison.Ordinal);
        Assert.Contains("\"BaseGameEvents\":", json, StringComparison.Ordinal);
        Assert.Contains("\"BaseMessageEvents\":", json, StringComparison.Ordinal);
        Assert.Contains("\"BaseTrackerEvents\":", json, StringComparison.Ordinal);
        Assert.DoesNotContain("\"ChatMessages\":", json, StringComparison.Ordinal);
        Assert.DoesNotContain("\"PingMessages\":", json, StringComparison.Ordinal);
        Assert.DoesNotContain("\"SUnitBornEvents\":", json, StringComparison.Ordinal);

        Assert.Equal(replay.GameEvents?.BaseGameEvents.Count, roundTripped.GameEvents?.BaseGameEvents.Count);
        Assert.Equal(replay.MessageEvents?.BaseMessageEvents.Count, roundTripped.MessageEvents?.BaseMessageEvents.Count);
        Assert.Equal(replay.TrackerEvents?.BaseTrackerEvents.Count, roundTripped.TrackerEvents?.BaseTrackerEvents.Count);

        if (replay.TrackerEvents?.SUnitBornEvents.Any(item => item.SUnitDiedEvent is not null) == true)
        {
            Assert.Contains(roundTripped.TrackerEvents!.SUnitBornEvents, item => item.SUnitDiedEvent is not null);
        }
    }

    [Fact]
    public async Task CompleteReplayAlsoRoundTripsWithReflectionContract()
    {
        ReplayDecoder decoder = new();
        Sc2Replay replay = await decoder.DecodeAsync(Path.Combine(ReplayDirectory, "test11.SC2Replay"));

        string json = JsonSerializer.Serialize(replay);
        Sc2Replay roundTripped = Assert.IsType<Sc2Replay>(JsonSerializer.Deserialize<Sc2Replay>(json));

        string secondJson = JsonSerializer.Serialize(roundTripped);
        Assert.True(JsonNode.DeepEquals(JsonNode.Parse(json), JsonNode.Parse(secondJson)), FirstDifference(json, secondJson));
    }

    [Fact]
    public void UnknownJsonDiscriminatorFailsInsteadOfDroppingData()
    {
        TrackerEvent trackerEvent = new UnknownTrackerEvent(
            1,
            2,
            TrackerEventType.None,
            3,
            4,
            "NNet.Replay.Tracker.SFutureEvent");
        string json = JsonSerializer.Serialize(trackerEvent);
        string invalid = json.Replace(nameof(UnknownTrackerEvent), "FutureTrackerEvent", StringComparison.Ordinal);

        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<TrackerEvent>(invalid));
    }

    [Fact]
    public void KnownUnknownReplayModelsRoundTripWithTheirProtocolNames()
    {
        GameEvent game = new UnknownGameEvent(1, 2, 3, 4, "NNet.Game.SFutureEvent");
        MessageEvent message = new UnknownMessageEvent(5, 6, 7, 8, "NNet.Game.SFutureMessage");
        TrackerEvent tracker = new UnknownTrackerEvent(
            9,
            10,
            TrackerEventType.None,
            11,
            12,
            "NNet.Replay.Tracker.SFutureEvent");

        UnknownGameEvent gameCopy = Assert.IsType<UnknownGameEvent>(
            JsonSerializer.Deserialize<GameEvent>(JsonSerializer.Serialize(game)));
        UnknownMessageEvent messageCopy = Assert.IsType<UnknownMessageEvent>(
            JsonSerializer.Deserialize<MessageEvent>(JsonSerializer.Serialize(message)));
        UnknownTrackerEvent trackerCopy = Assert.IsType<UnknownTrackerEvent>(
            JsonSerializer.Deserialize<TrackerEvent>(JsonSerializer.Serialize(tracker)));

        Assert.Equal("NNet.Game.SFutureEvent", gameCopy.EventTypeName);
        Assert.Equal("NNet.Game.SFutureMessage", messageCopy.EventTypeName);
        Assert.Equal("NNet.Replay.Tracker.SFutureEvent", trackerCopy.EventTypeName);
        Assert.Equal((1, 2, 3, 4), (gameCopy.UserId, gameCopy.EventId, gameCopy.Bits, gameCopy.Gameloop));
        Assert.Equal((5, 6, 7, 8), (messageCopy.UserId, messageCopy.EventId, messageCopy.Bits, messageCopy.Gameloop));
        Assert.Equal((9, 10, 11, 12), (trackerCopy.PlayerId, trackerCopy.EventId, trackerCopy.Bits, trackerCopy.Gameloop));
    }

    private static string FirstDifference(string left, string right)
    {
        int length = Math.Min(left.Length, right.Length);
        int index = 0;
        while (index < length && left[index] == right[index])
        {
            index++;
        }

        int start = Math.Max(0, index - 120);
        int leftLength = Math.Min(240, left.Length - start);
        int rightLength = Math.Min(240, right.Length - start);
        return $"First JSON difference at {index}. Original: {left.Substring(start, leftLength)} Round-tripped: {right.Substring(start, rightLength)}";
    }
}

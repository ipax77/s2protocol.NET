using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using s2protocol.NET.Models;

namespace s2protocol.NET.tests;

[TestClass]
public sealed class ProjectionTests
{
    private static readonly string? AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    [TestMethod]
    public async Task ProjectionCountsMatchDecodedReplay()
    {
        string replayPath = GetReplayPath("test2.SC2Replay");
        using ReplayDecoder decoder = new();
        Sc2Replay replay = await decoder.DecodeAsync(replayPath, new ReplayDecoderOptions
        {
            Details = true,
            Initdata = true,
            Metadata = true,
            MessageEvents = true,
            TrackerEvents = true,
            GameEvents = false,
            AttributeEvents = false,
        }, CancellationToken.None) ?? throw new AssertFailedException("Sc2Replay was null.");

        CountProjection projection = new(new ReplayProjectionProfile
        {
            Details = true,
            Initdata = true,
            Metadata = true,
            MessageEvents = MessageEventSelection.All,
            TrackerEvents = ReplayEventSelection<TrackerEventType>.All,
        });

        ProjectionCounts counts = await decoder.DecodeAsync(replayPath, projection, CancellationToken.None);

        Assert.AreEqual(replay.Header.BaseBuild, counts.BaseBuild);
        Assert.AreEqual(1, counts.Details);
        Assert.AreEqual(1, counts.Initdata);
        Assert.AreEqual(1, counts.Metadata);
        Assert.AreEqual(replay.ChatMessages?.Count ?? 0, counts.ChatMessages);
        Assert.AreEqual(replay.PingMessages?.Count ?? 0, counts.PingMessages);
        Assert.AreEqual(replay.TrackerEvents?.SPlayerSetupEvents.Count ?? 0, counts.GetTrackerCount(TrackerEventType.SPlayerSetupEvent));
        Assert.AreEqual(replay.TrackerEvents?.SPlayerStatsEvents.Count ?? 0, counts.GetTrackerCount(TrackerEventType.SPlayerStatsEvent));
        Assert.AreEqual(replay.TrackerEvents?.SUnitBornEvents.Count ?? 0, counts.GetTrackerCount(TrackerEventType.SUnitBornEvent));
        Assert.AreEqual(replay.TrackerEvents?.SUnitDiedEvents.Count ?? 0, counts.GetTrackerCount(TrackerEventType.SUnitDiedEvent));
        Assert.AreEqual(replay.TrackerEvents?.SUnitOwnerChangeEvents.Count ?? 0, counts.GetTrackerCount(TrackerEventType.SUnitOwnerChangeEvent));
        Assert.AreEqual(replay.TrackerEvents?.SUnitPositionsEvents.Count ?? 0, counts.GetTrackerCount(TrackerEventType.SUnitPositionsEvent));
        Assert.AreEqual(replay.TrackerEvents?.SUnitTypeChangeEvents.Count ?? 0, counts.GetTrackerCount(TrackerEventType.SUnitTypeChangeEvent));
        Assert.AreEqual(replay.TrackerEvents?.SUpgradeEvents.Count ?? 0, counts.GetTrackerCount(TrackerEventType.SUpgradeEvent));
        Assert.AreEqual(replay.TrackerEvents?.SUnitInitEvents.Count ?? 0, counts.GetTrackerCount(TrackerEventType.SUnitInitEvent));
        Assert.AreEqual(replay.TrackerEvents?.SUnitDoneEvents.Count ?? 0, counts.GetTrackerCount(TrackerEventType.SUnitDoneEvent));
    }

    [TestMethod]
    public async Task ProjectionSkipsUnselectedTrackerEvents()
    {
        string replayPath = GetReplayPath("test2.SC2Replay");
        using ReplayDecoder decoder = new();
        Sc2Replay replay = await decoder.DecodeAsync(replayPath, new ReplayDecoderOptions
        {
            Details = false,
            Initdata = false,
            Metadata = false,
            MessageEvents = false,
            TrackerEvents = true,
            GameEvents = false,
            AttributeEvents = false,
        }, CancellationToken.None) ?? throw new AssertFailedException("Sc2Replay was null.");

        CountProjection projection = new(new ReplayProjectionProfile
        {
            TrackerEvents = ReplayEventSelection<TrackerEventType>.Only(
                TrackerEventType.SPlayerSetupEvent,
                TrackerEventType.SPlayerStatsEvent,
                TrackerEventType.SUnitBornEvent,
                TrackerEventType.SUnitDiedEvent,
                TrackerEventType.SUnitOwnerChangeEvent,
                TrackerEventType.SUnitTypeChangeEvent,
                TrackerEventType.SUpgradeEvent),
        });

        ProjectionCounts counts = await decoder.DecodeAsync(replayPath, projection, CancellationToken.None);
        int expectedSelectedCount =
            (replay.TrackerEvents?.SPlayerSetupEvents.Count ?? 0)
            + (replay.TrackerEvents?.SPlayerStatsEvents.Count ?? 0)
            + (replay.TrackerEvents?.SUnitBornEvents.Count ?? 0)
            + (replay.TrackerEvents?.SUnitDiedEvents.Count ?? 0)
            + (replay.TrackerEvents?.SUnitOwnerChangeEvents.Count ?? 0)
            + (replay.TrackerEvents?.SUnitTypeChangeEvents.Count ?? 0)
            + (replay.TrackerEvents?.SUpgradeEvents.Count ?? 0);

        Assert.AreEqual(expectedSelectedCount, counts.TotalTrackerEvents);
        Assert.AreEqual(0, counts.GetTrackerCount(TrackerEventType.SUnitPositionsEvent));
        Assert.AreEqual(0, counts.GetTrackerCount(TrackerEventType.SUnitInitEvent));
        Assert.AreEqual(0, counts.GetTrackerCount(TrackerEventType.SUnitDoneEvent));
    }

    private static string GetReplayPath(string replayFile)
    {
        Assert.IsNotNull(AssemblyPath, "Could not get ExecutionAssembly path.");
        return Path.Combine(AssemblyPath, "replays", replayFile);
    }

    private sealed class CountProjection(ReplayProjectionProfile profile) : IReplayProjection<ProjectionCounts>
    {
        private readonly ProjectionCounts counts = new();

        public ReplayProjectionProfile Profile { get; } = profile;

        public ProjectionCounts Complete()
        {
            return counts;
        }

        public void OnAttributeEvents(AttributeEvents attributeEvents)
        {
            counts.AttributeEvents++;
        }

        public void OnChatMessage(ChatMessageEvent chatMessage)
        {
            counts.ChatMessages++;
        }

        public void OnDetails(Details details)
        {
            counts.Details++;
        }

        public void OnGameEvent(GameEvent gameEvent)
        {
            counts.GameEvents++;
        }

        public void OnHeader(Header header, string replayPath)
        {
            counts.BaseBuild = header.BaseBuild;
        }

        public void OnInitdata(Initdata initdata)
        {
            counts.Initdata++;
        }

        public void OnMetadata(ReplayMetadata metadata)
        {
            counts.Metadata++;
        }

        public void OnPingMessage(PingMessageEvent pingMessage)
        {
            counts.PingMessages++;
        }

        public void OnTrackerEvent(TrackerEvent trackerEvent)
        {
            counts.TotalTrackerEvents++;
            counts.TrackerEvents[trackerEvent.EventType] = counts.GetTrackerCount(trackerEvent.EventType) + 1;
        }
    }

    private sealed class ProjectionCounts
    {
        public int BaseBuild { get; set; }
        public int Details { get; set; }
        public int Initdata { get; set; }
        public int Metadata { get; set; }
        public int AttributeEvents { get; set; }
        public int ChatMessages { get; set; }
        public int PingMessages { get; set; }
        public int GameEvents { get; set; }
        public int TotalTrackerEvents { get; set; }
        public Dictionary<TrackerEventType, int> TrackerEvents { get; } = [];

        public int GetTrackerCount(TrackerEventType eventType)
        {
            return TrackerEvents.GetValueOrDefault(eventType);
        }
    }
}

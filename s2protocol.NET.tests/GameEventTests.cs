using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using s2protocol.NET.Models;
using Xunit;

namespace s2protocol.NET.tests;

public sealed class GameEventTests
{
    private static readonly string AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
        ?? throw new InvalidOperationException("Could not resolve the test assembly path.");

    [Fact]
    public async Task CommandsAreStronglyTypedAsync()
    {
        using ReplayDecoder decoder = new();
        Sc2Replay replay = Assert.IsType<Sc2Replay>(await decoder.DecodeAsync(
            Path.Combine(AssemblyPath, "replays", "test6.SC2Replay"),
            new ReplayDecoderOptions
            {
                Initdata = false,
                Details = false,
                Metadata = false,
                MessageEvents = false,
                TrackerEvents = false,
                GameEvents = true,
                AttributeEvents = false,
            }));

        GameEvents gameEvents = Assert.IsType<GameEvents>(replay.GameEvents);
        SCmdEvent firstCommand = gameEvents.BaseGameEvents.OfType<SCmdEvent>().First();
        Assert.Equal(63, firstCommand.Gameloop);
        Assert.Equal(1, firstCommand.UserId);
        Assert.Equal(1113, firstCommand.AbilLink);
        Assert.Equal(0, firstCommand.AbilCmdIndex);
        Assert.Equal(448, firstCommand.CmdFlags);
        Assert.Null(firstCommand.TargetX);
        Assert.Null(firstCommand.TargetY);
        Assert.Null(firstCommand.TargetZ);
    }

    [Fact]
    public async Task UnitTargetCommandsExposeTargetUnitTagAsync()
    {
        using ReplayDecoder decoder = new();
        Sc2Replay replay = Assert.IsType<Sc2Replay>(await decoder.DecodeAsync(
            Path.Combine(AssemblyPath, "replays", "karax-target.SC2Replay"),
            new ReplayDecoderOptions
            {
                Initdata = false,
                Details = false,
                Metadata = false,
                MessageEvents = false,
                TrackerEvents = false,
                GameEvents = true,
                AttributeEvents = false,
            }));

        GameEvents gameEvents = Assert.IsType<GameEvents>(replay.GameEvents);
        int[] targetTags = gameEvents.BaseGameEvents
            .OfType<SCmdEvent>()
            .Where(command => command is { AbilLink: 2014, AbilCmdIndex: 0 })
            .Select(command => Assert.IsType<int>(command.TargetUnitTag))
            .ToArray();

        Assert.Equal(14, targetTags.Length);
        Assert.Contains(62390277, targetTags);
        Assert.Contains(77856771, targetTags);
        Assert.Contains(27000851, targetTags);
        Assert.Contains(77594634, targetTags);
        Assert.Contains(62652429, targetTags);
    }
}

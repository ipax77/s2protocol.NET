using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace s2protocol.NET.tests;

public sealed class ReplayDecoderDisposalTests
{
    private static readonly ReplayDecoderOptions HeaderOnly = new()
    {
        Initdata = false,
        Details = false,
        Metadata = false,
        GameEvents = false,
        MessageEvents = false,
        TrackerEvents = false,
        AttributeEvents = false,
    };

    [Fact]
    public async Task PathDecodeReleasesFileOnSuccessAsync()
    {
        string replayPath = GetReplayPath();
        using ReplayDecoder decoder = new();

        await decoder.DecodeAsync(replayPath, HeaderOnly);

        using FileStream exclusive = File.Open(replayPath, FileMode.Open, FileAccess.Read, FileShare.None);
        Assert.True(exclusive.CanRead);
    }

    [Fact]
    public async Task PathDecodeReleasesFileOnFailureAsync()
    {
        string replayPath = CreateTemporaryReplay(new byte[] { 0x4D, 0x50, 0x51, 0x1A });
        try
        {
            using ReplayDecoder decoder = new();

            DecodeException exception = await Assert.ThrowsAsync<DecodeException>(
                () => decoder.DecodeAsync(replayPath, HeaderOnly));

            Assert.NotNull(exception.InnerException);
            using FileStream exclusive = File.Open(replayPath, FileMode.Open, FileAccess.Read, FileShare.None);
            Assert.True(exclusive.CanRead);
        }
        finally
        {
            File.Delete(replayPath);
        }
    }

    [Fact]
    public async Task PathDecodeReleasesFileOnCancellationAsync()
    {
        string replayPath = GetReplayPath();
        using ReplayDecoder decoder = new();
        using CancellationTokenSource cancellation = new();
        await cancellation.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => decoder.DecodeAsync(replayPath, HeaderOnly, cancellation.Token));

        using FileStream exclusive = File.Open(replayPath, FileMode.Open, FileAccess.Read, FileShare.None);
        Assert.True(exclusive.CanRead);
    }

    [Fact]
    public async Task StreamDecodeLeavesCallerStreamOpenOnSuccessAsync()
    {
        await using FileStream replay = File.OpenRead(GetReplayPath());
        using MemoryStream stream = new();
        await replay.CopyToAsync(stream);
        stream.Position = 0;
        using ReplayDecoder decoder = new();

        await decoder.DecodeAsync(stream, HeaderOnly);

        Assert.True(stream.CanRead);
    }

    [Fact]
    public async Task StreamDecodeLeavesCallerStreamOpenAndPreservesFailureAsync()
    {
        using MemoryStream stream = new(new byte[] { 0x4D, 0x50, 0x51, 0x1A });
        using ReplayDecoder decoder = new();

        DecodeException exception = await Assert.ThrowsAsync<DecodeException>(
            () => decoder.DecodeAsync(stream, HeaderOnly));

        Assert.NotNull(exception.InnerException);
        Assert.True(stream.CanRead);
    }

    [Fact]
    public async Task StreamDecodeLeavesCallerStreamOpenOnCancellationAsync()
    {
        await using FileStream replay = File.OpenRead(GetReplayPath());
        using MemoryStream stream = new();
        await replay.CopyToAsync(stream);
        stream.Position = 0;
        using ReplayDecoder decoder = new();
        using CancellationTokenSource cancellation = new();
        await cancellation.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => decoder.DecodeAsync(stream, HeaderOnly, cancellation.Token));

        Assert.True(stream.CanRead);
    }

    [Fact]
    public async Task MissingReplayThrowsFileNotFoundExceptionAsync()
    {
        using ReplayDecoder decoder = new();
        string replayPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.SC2Replay");

        FileNotFoundException exception = await Assert.ThrowsAsync<FileNotFoundException>(
            () => decoder.DecodeAsync(replayPath, HeaderOnly));

        Assert.Equal(replayPath, exception.FileName);
    }

    private static string GetReplayPath()
    {
        string? assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        Assert.NotNull(assemblyPath);
        return Path.Combine(assemblyPath, "replays", "test1.SC2Replay");
    }

    private static string CreateTemporaryReplay(byte[] contents)
    {
        string replayPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.SC2Replay");
        File.WriteAllBytes(replayPath, contents);
        return replayPath;
    }
}

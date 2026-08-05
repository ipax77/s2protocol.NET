using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using s2protocol.NET.Mpq;
using Xunit;

namespace s2protocol.NET.tests;

public sealed class MPQArchiveSecurityTests
{
    [Fact]
    public void TruncatedArchiveIsRejected()
    {
        using MemoryStream stream = new(new byte[] { 0x4D, 0x50, 0x51, 0x1A });

        Assert.Throws<InvalidDataException>(() => new MPQArchive(stream, false));
    }

    [Fact]
    public void OversizedTableDeclarationIsRejectedBeforeAllocation()
    {
        byte[] archive = CreateSimpleArchive();
        BinaryPrimitives.WriteUInt32LittleEndian(archive.AsSpan(24), uint.MaxValue);
        using MemoryStream stream = new(archive);

        Assert.Throws<InvalidDataException>(() => new MPQArchive(stream, false));
    }

    [Fact]
    public void TableOutsideArchiveIsRejected()
    {
        byte[] archive = CreateSimpleArchive();
        BinaryPrimitives.WriteUInt32LittleEndian(archive.AsSpan(16), checked((uint)archive.Length));
        using MemoryStream stream = new(archive);

        Assert.Throws<InvalidDataException>(() => new MPQArchive(stream, false));
    }

    [Fact]
    public void InvalidHashBlockIndexIsRejected()
    {
        MpqTestArchiveBuilder builder = new();
        builder.AddStored("bad.bin", new byte[] { 1 }, 1, hashBlockIndex: 7);
        using MemoryStream stream = new(builder.Build());

        Assert.Throws<InvalidDataException>(() => new MPQArchive(stream, false));
    }

    [Fact]
    public void OversizedFileDeclarationIsRejectedBeforeAllocation()
    {
        MpqTestArchiveBuilder builder = new();
        builder.AddStored("large.bin", new byte[] { 1 }, uint.MaxValue);
        using MemoryStream stream = new(builder.Build());

        Assert.Throws<InvalidDataException>(() => new MPQArchive(stream, false));
    }

    [Fact]
    public void InvalidSectorOffsetsAreRejected()
    {
        MpqTestArchiveBuilder builder = new();
        builder.AddMultiSector("multi.bin", CreatePayload(1200));
        byte[] archive = builder.Build();
        BinaryPrimitives.WriteInt32LittleEndian(archive.AsSpan(36), 0);
        using MPQArchive mpq = new(new MemoryStream(archive), false);

        Assert.Throws<InvalidDataException>(() => mpq.ReadFile("multi.bin"));
    }

    [Fact]
    public async Task ZlibAndBzip2DecodeSynchronouslyAndAsynchronouslyAsync()
    {
        byte[] payload = CreatePayload(4096);
        MpqTestArchiveBuilder builder = new();
        builder.AddSingleUnit("zlib.bin", payload, 2);
        builder.AddSingleUnit("bzip.bin", payload, 16);
        using MPQArchive archive = new(new MemoryStream(builder.Build()), false);

#pragma warning disable CA1849 // Both synchronous and asynchronous public paths are intentionally covered.
        Assert.Equal(payload, archive.ReadFile("zlib.bin", true)?.ToArray());
        Assert.Equal(payload, archive.ReadFile("bzip.bin", true)?.ToArray());
#pragma warning restore CA1849
        Assert.Equal(payload, (await archive.ReadFileAsync("zlib.bin", true))?.ToArray());
        Assert.Equal(payload, (await archive.ReadFileAsync("bzip.bin", true))?.ToArray());
    }

    [Fact]
    public async Task MultiSectorFileDecodesSynchronouslyAndAsynchronouslyAsync()
    {
        byte[] payload = CreatePayload(1500);
        MpqTestArchiveBuilder builder = new();
        builder.AddMultiSector("multi.bin", payload);
        using MPQArchive archive = new(new MemoryStream(builder.Build()), false);

#pragma warning disable CA1849 // Both synchronous and asynchronous public paths are intentionally covered.
        Assert.Equal(payload, archive.ReadFile("multi.bin")?.ToArray());
#pragma warning restore CA1849
        Assert.Equal(payload, (await archive.ReadFileAsync("multi.bin"))?.ToArray());
    }

    [Fact]
    public async Task AsyncDecompressionRejectsShortOutputAsync()
    {
        byte[] payload = CreatePayload(1024);
        MpqTestArchiveBuilder builder = new();
        builder.AddStored("short.bin", CompressAsZlib(payload), checked((uint)payload.Length + 1), 0x81000200);
        using MPQArchive archive = new(new MemoryStream(builder.Build()), false);

        await Assert.ThrowsAsync<InvalidDataException>(async () =>
            await archive.ReadFileAsync("short.bin", true));
    }

    [Fact]
    public async Task MissingEntriesReturnNullSynchronouslyAndAsynchronouslyAsync()
    {
        using MPQArchive archive = CreateArchive();

#pragma warning disable CA1849 // Both public paths are intentionally covered.
        Assert.Null(archive.ReadFile("missing.bin"));
#pragma warning restore CA1849
        Assert.Null(await archive.ReadFileAsync("missing.bin"));
    }

    [Fact]
    public async Task AsyncReadHonorsPreCanceledTokenAsync()
    {
        using MPQArchive archive = CreateArchive();
        using CancellationTokenSource cancellation = new();
        await cancellation.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            await archive.ReadFileAsync("safe.bin", cancellationToken: cancellation.Token));
    }

    [Fact]
    public void CorrectlyNamedExtractionWritesTheRequestedEntry()
    {
        string fileName = $"s2protocol-extract-{Guid.NewGuid():N}.bin";
        string outputPath = Path.Combine(Environment.CurrentDirectory, fileName);
        byte[] payload = [1, 2, 3, 4];
        MpqTestArchiveBuilder builder = new();
        builder.AddSingleUnit(fileName, payload);
        using MPQArchive archive = new(new MemoryStream(builder.Build()), false);

        try
        {
            archive.ExtractFilesToDisk(fileName);
            Assert.Equal(payload, File.ReadAllBytes(outputPath));
        }
        finally
        {
            File.Delete(outputPath);
        }
    }

    [Theory]
    [InlineData("../escape.bin")]
    [InlineData("folder/../../escape.bin")]
    public void ExtractionRejectsTraversal(string archivePath)
    {
        using MPQArchive archive = CreateArchive();

        Assert.Throws<InvalidDataException>(() => archive.ExtractFilesToDisk(archivePath));
    }

    [Fact]
    public void ExtractionRejectsRootedPath()
    {
        using MPQArchive archive = CreateArchive();
        string rootedPath = Path.Combine(Path.GetPathRoot(Environment.CurrentDirectory)!, "escape.bin");

        Assert.Throws<InvalidDataException>(() => archive.ExtractFilesToDisk(rootedPath));
    }

    private static MPQArchive CreateArchive()
        => new(new MemoryStream(CreateSimpleArchive()), false);

    private static byte[] CreateSimpleArchive()
    {
        MpqTestArchiveBuilder builder = new();
        builder.AddSingleUnit("safe.bin", new byte[] { 1, 2, 3 });
        return builder.Build();
    }

    private static byte[] CreatePayload(int length)
    {
        byte[] pattern = Encoding.ASCII.GetBytes("StarCraft II replay payload for MPQ compression tests. ");
        byte[] result = new byte[length];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = pattern[i % pattern.Length];
        }

        return result;
    }

    private static byte[] CompressAsZlib(byte[] payload)
    {
        MpqTestArchiveBuilder builder = new();
        builder.AddSingleUnit("payload.bin", payload, 2);
        byte[] completeArchive = builder.Build();
        int hashOffset = checked((int)BinaryPrimitives.ReadUInt32LittleEndian(completeArchive.AsSpan(16)));
        return completeArchive.AsSpan(32, hashOffset - 32).ToArray();
    }
}

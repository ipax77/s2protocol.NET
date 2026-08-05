using System.Buffers;
using System.Buffers.Binary;
using System.IO.Compression;
using System.Text;

namespace s2protocol.NET.Mpq;

public sealed partial class MPQArchive
{
    /// <summary>Reads a file from the archive.</summary>
    public ReadOnlyMemory<byte>? ReadFile(
        string filename,
        bool forceDecompress = false,
        CancellationToken cancellationToken = default)
    {
        MPQBlockTableEntry? block = GetBlockEntry(filename);
        if (!block.HasValue)
        {
            return null;
        }

        cancellationToken.ThrowIfCancellationRequested();
        MPQBlockTableEntry blockEntry = block.Value;
        int rawLength = checked((int)blockEntry.CompressedSize);
        bool retainRawData = ShouldRetainRawData(blockEntry, forceDecompress);
        byte[] rawData = ReadRawData(blockEntry, rentBuffer: !retainRawData);
        try
        {
            return DecodeBlock(rawData, rawLength, blockEntry, forceDecompress, cancellationToken);
        }
        finally
        {
            if (!retainRawData && rawData.Length != 0)
            {
                ArrayPool<byte>.Shared.Return(rawData);
            }
        }
    }

    /// <summary>Asynchronously reads a file from the archive.</summary>
    public async Task<ReadOnlyMemory<byte>?> ReadFileAsync(
        string filename,
        bool forceDecompress = false,
        CancellationToken cancellationToken = default)
    {
        MPQBlockTableEntry? block = GetBlockEntry(filename);
        if (!block.HasValue)
        {
            return null;
        }

        cancellationToken.ThrowIfCancellationRequested();
        MPQBlockTableEntry blockEntry = block.Value;
        int rawLength = checked((int)blockEntry.CompressedSize);
        bool retainRawData = ShouldRetainRawData(blockEntry, forceDecompress);
        byte[] rawData = await ReadRawDataAsync(blockEntry, !retainRawData, cancellationToken).ConfigureAwait(false);
        try
        {
            return await DecodeBlockAsync(rawData, rawLength, blockEntry, forceDecompress, cancellationToken)
                .ConfigureAwait(false);
        }
        finally
        {
            if (!retainRawData && rawData.Length != 0)
            {
                ArrayPool<byte>.Shared.Return(rawData);
            }
        }
    }

    private MPQBlockTableEntry? GetBlockEntry(string filename)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filename);
        MPQHashTableEntry? hashEntry = GetHashTableEntry(filename);
        if (!hashEntry.HasValue)
        {
            return null;
        }

        uint blockIndex = hashEntry.Value.BlockIndex;
        if (blockIndex >= _blockTable.Length)
        {
            throw new InvalidDataException($"MPQ hash entry references invalid block index {blockIndex}.");
        }

        MPQBlockTableEntry blockEntry = _blockTable[blockIndex];
        if ((blockEntry.Flags & MpqFileExists) == 0)
        {
            return null;
        }

        if ((blockEntry.Flags & MpqFileEncrypted) != 0)
        {
            throw new NotSupportedException("Encrypted files are not supported yet.");
        }

        return blockEntry;
    }

    private static bool ShouldRetainRawData(MPQBlockTableEntry blockEntry, bool forceDecompress)
        => (blockEntry.Flags & MpqFileSingleUnit) != 0 &&
           ((blockEntry.Flags & MpqFileCompress) == 0 ||
            (!forceDecompress && blockEntry.CompressedSize >= blockEntry.FileSize));

    private byte[] ReadRawData(MPQBlockTableEntry blockEntry, bool rentBuffer)
    {
        int compressedSize = checked((int)blockEntry.CompressedSize);
        if (compressedSize == 0)
        {
            return [];
        }

        ValidateArchiveRange(blockEntry.FileOffset, blockEntry.CompressedSize, "file data");
        _reader.BaseStream.Position = checked(_headerOffset + blockEntry.FileOffset);
        byte[] rawData = rentBuffer ? ArrayPool<byte>.Shared.Rent(compressedSize) : new byte[compressedSize];
        try
        {
            _reader.BaseStream.ReadExactly(rawData.AsSpan(0, compressedSize));
            return rawData;
        }
        catch
        {
            if (rentBuffer)
            {
                ArrayPool<byte>.Shared.Return(rawData);
            }

            throw;
        }
    }

    private async Task<byte[]> ReadRawDataAsync(
        MPQBlockTableEntry blockEntry,
        bool rentBuffer,
        CancellationToken cancellationToken)
    {
        int compressedSize = checked((int)blockEntry.CompressedSize);
        if (compressedSize == 0)
        {
            return [];
        }

        ValidateArchiveRange(blockEntry.FileOffset, blockEntry.CompressedSize, "file data");
        _reader.BaseStream.Position = checked(_headerOffset + blockEntry.FileOffset);
        byte[] rawData = rentBuffer ? ArrayPool<byte>.Shared.Rent(compressedSize) : new byte[compressedSize];
        try
        {
            await _reader.BaseStream.ReadExactlyAsync(rawData.AsMemory(0, compressedSize), cancellationToken)
                .ConfigureAwait(false);
            return rawData;
        }
        catch
        {
            if (rentBuffer)
            {
                ArrayPool<byte>.Shared.Return(rawData);
            }

            throw;
        }
    }

    private ReadOnlyMemory<byte> DecodeBlock(
        byte[] rawData,
        int rawLength,
        MPQBlockTableEntry blockEntry,
        bool forceDecompress,
        CancellationToken cancellationToken)
    {
        int fileSize = checked((int)blockEntry.FileSize);
        if (fileSize == 0)
        {
            if (rawLength != 0)
            {
                throw new InvalidDataException("Empty MPQ file has non-empty archived data.");
            }

            return ReadOnlyMemory<byte>.Empty;
        }

        if (rawLength == 0)
        {
            throw new InvalidDataException("MPQ file data is empty.");
        }

        if ((blockEntry.Flags & MpqFileSingleUnit) != 0)
        {
            if ((blockEntry.Flags & MpqFileCompress) != 0 &&
                (forceDecompress || blockEntry.CompressedSize < blockEntry.FileSize))
            {
                byte[] result = new byte[fileSize];
                Decompress(rawData, 0, rawLength, result, 0, fileSize);
                return result;
            }

            if (rawLength != fileSize)
            {
                throw new InvalidDataException(
                    $"MPQ file length mismatch. Expected {fileSize} bytes, got {rawLength} bytes.");
            }

            return rawData.AsMemory(0, rawLength);
        }

        SectorLayout layout = ReadSectorLayout(rawData, rawLength, blockEntry);
        byte[] output = new byte[fileSize];
        int bytesCopied = 0;

        for (int i = 0; i < layout.SectorCount; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            int start = layout.Positions[i];
            int length = layout.Positions[i + 1] - start;
            int expectedLength = Math.Min(layout.SectorSize, fileSize - bytesCopied);

            CopyOrDecompressSector(rawData, start, length, output, bytesCopied, expectedLength, blockEntry.Flags);
            bytesCopied += expectedLength;
        }

        return output;
    }

    private async Task<ReadOnlyMemory<byte>> DecodeBlockAsync(
        byte[] rawData,
        int rawLength,
        MPQBlockTableEntry blockEntry,
        bool forceDecompress,
        CancellationToken cancellationToken)
    {
        int fileSize = checked((int)blockEntry.FileSize);
        if (fileSize == 0)
        {
            if (rawLength != 0)
            {
                throw new InvalidDataException("Empty MPQ file has non-empty archived data.");
            }

            return ReadOnlyMemory<byte>.Empty;
        }

        if (rawLength == 0)
        {
            throw new InvalidDataException("MPQ file data is empty.");
        }

        if ((blockEntry.Flags & MpqFileSingleUnit) != 0)
        {
            if ((blockEntry.Flags & MpqFileCompress) != 0 &&
                (forceDecompress || blockEntry.CompressedSize < blockEntry.FileSize))
            {
                byte[] result = new byte[fileSize];
                await DecompressAsync(rawData, 0, rawLength, result, 0, fileSize, cancellationToken)
                    .ConfigureAwait(false);
                return result;
            }

            if (rawLength != fileSize)
            {
                throw new InvalidDataException(
                    $"MPQ file length mismatch. Expected {fileSize} bytes, got {rawLength} bytes.");
            }

            return rawData.AsMemory(0, rawLength);
        }

        SectorLayout layout = ReadSectorLayout(rawData, rawLength, blockEntry);
        byte[] output = new byte[fileSize];
        int bytesCopied = 0;

        for (int i = 0; i < layout.SectorCount; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            int start = layout.Positions[i];
            int length = layout.Positions[i + 1] - start;
            int expectedLength = Math.Min(layout.SectorSize, fileSize - bytesCopied);

            if ((blockEntry.Flags & MpqFileCompress) != 0 && length < expectedLength)
            {
                await DecompressAsync(rawData, start, length, output, bytesCopied, expectedLength, cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                CopyUncompressedSector(rawData, start, length, output, bytesCopied, expectedLength);
            }

            bytesCopied += expectedLength;
        }

        return output;
    }

    private SectorLayout ReadSectorLayout(byte[] rawData, int rawLength, MPQBlockTableEntry blockEntry)
    {
        int fileSize = checked((int)blockEntry.FileSize);
        int sectorSize = checked(512 << _header.SectorSizeShift);
        int sectorCount = checked((fileSize + sectorSize - 1) / sectorSize);
        bool hasSectorChecksum = (blockEntry.Flags & MpqFileSectorCrc) != 0;
        int tableEntries = checked(sectorCount + 1 + (hasSectorChecksum ? 1 : 0));
        int tableSize = checked(tableEntries * sizeof(int));

        if (rawLength < tableSize)
        {
            throw new InvalidDataException("MPQ sector table is truncated.");
        }

        int[] positions = new int[tableEntries];
        int previous = tableSize;
        for (int i = 0; i < tableEntries; i++)
        {
            int position = BinaryPrimitives.ReadInt32LittleEndian(rawData.AsSpan(i * sizeof(int), sizeof(int)));
            if (position < previous || position > rawLength)
            {
                throw new InvalidDataException("MPQ sector table contains an invalid offset.");
            }

            positions[i] = position;
            previous = position;
        }

        if (positions[0] != tableSize || positions[^1] != rawLength)
        {
            throw new InvalidDataException("MPQ sector table does not cover the archived block exactly.");
        }

        return new SectorLayout(sectorSize, sectorCount, positions);
    }

    private static void CopyOrDecompressSector(
        byte[] source,
        int sourceOffset,
        int length,
        byte[] destination,
        int destinationOffset,
        int expectedLength,
        uint flags)
    {
        if ((flags & MpqFileCompress) != 0 && length < expectedLength)
        {
            Decompress(source, sourceOffset, length, destination, destinationOffset, expectedLength);
            return;
        }

        CopyUncompressedSector(source, sourceOffset, length, destination, destinationOffset, expectedLength);
    }

    private static void CopyUncompressedSector(
        byte[] source,
        int sourceOffset,
        int length,
        byte[] destination,
        int destinationOffset,
        int expectedLength)
    {
        if (length != expectedLength)
        {
            throw new InvalidDataException(
                $"MPQ sector length mismatch. Expected {expectedLength}, got {length}.");
        }

        source.AsSpan(sourceOffset, length).CopyTo(destination.AsSpan(destinationOffset, expectedLength));
    }

    private MPQHashTableEntry? GetHashTableEntry(string filename)
    {
        uint tableLength = checked((uint)_hashTable.Length);
        uint tableMask = tableLength - 1;
        uint initialIndex = Hash(filename, MpqHashType.TableOffset) & tableMask;
        uint hashA = Hash(filename, MpqHashType.NameA);
        uint hashB = Hash(filename, MpqHashType.NameB);

        for (uint probe = 0; probe < tableLength; probe++)
        {
            MPQHashTableEntry entry = _hashTable[(initialIndex + probe) & tableMask];
            if (entry.BlockIndex == MpqHashEntryEmpty)
            {
                return null;
            }

            if (entry.BlockIndex == MpqHashEntryDeleted)
            {
                continue;
            }

            if (entry.NameHashA == hashA && entry.NameHashB == hashB)
            {
                return entry;
            }
        }

        return null;
    }

    private static void Decompress(
        byte[] data,
        int offset,
        int length,
        byte[] destination,
        int destinationOffset,
        int expectedLength)
    {
        ValidateDecompressionRange(data, offset, length, destination, destinationOffset, expectedLength);
        byte compressionType = data[offset];

        switch (compressionType)
        {
            case 0:
                if (length - 1 != expectedLength)
                {
                    throw CreateLengthMismatch(expectedLength, length - 1);
                }

                data.AsSpan(offset + 1, expectedLength).CopyTo(destination.AsSpan(destinationOffset, expectedLength));
                return;

            case 2:
                using (var input = new MemoryStream(data, offset + 1, length - 1, writable: false))
                using (var zlib = new ZLibStream(input, CompressionMode.Decompress))
                {
                    ReadExpectedLength(zlib, destination, destinationOffset, expectedLength);
                }

                return;

            case 16:
                using (var input = new MemoryStream(data, offset + 1, length - 1, writable: false))
                using (var bzip = new ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(input))
                {
                    ReadExpectedLength(bzip, destination, destinationOffset, expectedLength);
                }

                return;

            default:
                throw new NotSupportedException($"Unsupported compression type: {compressionType}");
        }
    }

    private static async Task DecompressAsync(
        byte[] data,
        int offset,
        int length,
        byte[] destination,
        int destinationOffset,
        int expectedLength,
        CancellationToken cancellationToken)
    {
        ValidateDecompressionRange(data, offset, length, destination, destinationOffset, expectedLength);
        byte compressionType = data[offset];

        switch (compressionType)
        {
            case 0:
                if (length - 1 != expectedLength)
                {
                    throw CreateLengthMismatch(expectedLength, length - 1);
                }

                data.AsSpan(offset + 1, expectedLength).CopyTo(destination.AsSpan(destinationOffset, expectedLength));
                return;

            case 2:
                using (var input = new MemoryStream(data, offset + 1, length - 1, writable: false))
                using (var zlib = new ZLibStream(input, CompressionMode.Decompress))
                {
                    await ReadExpectedLengthAsync(zlib, destination, destinationOffset, expectedLength, cancellationToken)
                        .ConfigureAwait(false);
                }

                return;

            case 16:
                cancellationToken.ThrowIfCancellationRequested();
                using (var input = new MemoryStream(data, offset + 1, length - 1, writable: false))
                using (var bzip = new ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(input))
                {
                    ReadExpectedLength(bzip, destination, destinationOffset, expectedLength);
                }

                cancellationToken.ThrowIfCancellationRequested();
                return;

            default:
                throw new NotSupportedException($"Unsupported compression type: {compressionType}");
        }
    }

    private static void ValidateDecompressionRange(
        byte[] data,
        int offset,
        int length,
        byte[] destination,
        int destinationOffset,
        int expectedLength)
    {
        if (length <= 0)
        {
            throw new InvalidDataException("Compressed MPQ data is empty.");
        }

        if (offset < 0 || length > data.Length - offset ||
            destinationOffset < 0 || expectedLength < 0 || expectedLength > destination.Length - destinationOffset)
        {
            throw new InvalidDataException("Compressed MPQ data contains an invalid offset or length.");
        }
    }

    private static void ReadExpectedLength(Stream source, byte[] destination, int destinationOffset, int expectedLength)
    {
        int totalRead = 0;
        while (totalRead < expectedLength)
        {
            int read = source.Read(destination, destinationOffset + totalRead, expectedLength - totalRead);
            if (read == 0)
            {
                break;
            }

            totalRead += read;
        }

        VerifyExactLength(source, expectedLength, totalRead);
    }

    private static async Task ReadExpectedLengthAsync(
        Stream source,
        byte[] destination,
        int destinationOffset,
        int expectedLength,
        CancellationToken cancellationToken)
    {
        int totalRead = 0;
        while (totalRead < expectedLength)
        {
            int read = await source.ReadAsync(
                destination.AsMemory(destinationOffset + totalRead, expectedLength - totalRead),
                cancellationToken).ConfigureAwait(false);
            if (read == 0)
            {
                break;
            }

            totalRead += read;
        }

        if (totalRead != expectedLength)
        {
            throw CreateLengthMismatch(expectedLength, totalRead);
        }

        if (await source.ReadAsync(destination.AsMemory(destinationOffset, 1), cancellationToken).ConfigureAwait(false) != 0)
        {
            throw new InvalidDataException($"Decompressed MPQ data exceeds expected length of {expectedLength} bytes.");
        }
    }

    private static void VerifyExactLength(Stream source, int expectedLength, int actualLength)
    {
        if (actualLength != expectedLength)
        {
            throw CreateLengthMismatch(expectedLength, actualLength);
        }

        if (source.ReadByte() >= 0)
        {
            throw new InvalidDataException($"Decompressed MPQ data exceeds expected length of {expectedLength} bytes.");
        }
    }

    private static InvalidDataException CreateLengthMismatch(int expectedLength, int actualLength)
        => new($"Decompressed MPQ data length mismatch. Expected {expectedLength} bytes, got {actualLength} bytes.");

    private static readonly string[] separatorArray = ["\r\n", "\n"];

    /// <summary>Returns the archive listfile contents.</summary>
    public string PrintFiles()
    {
        if (_files is null)
        {
            return string.Empty;
        }

        StringBuilder builder = new();
        string text = Encoding.UTF8.GetString(_files.Value.Span);
        string[] lines = text.Split(separatorArray, StringSplitOptions.RemoveEmptyEntries);
        foreach (string line in lines)
        {
            builder.AppendLine(line);
        }

        return builder.ToString();
    }

    private readonly record struct SectorLayout(int SectorSize, int SectorCount, int[] Positions);
}

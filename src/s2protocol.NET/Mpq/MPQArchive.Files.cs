using System.Buffers.Binary;
using System.IO.Compression;
using System.Text;

namespace s2protocol.NET.Mpq;

public sealed partial class MPQArchive
{
    /// <summary>
    /// Reads the contents of a file from the archive.
    /// </summary>
    /// <remarks>This method supports reading both single-unit and multi-sector files. If the file is
    /// compressed and  <paramref name="forceDecompress"/> is <see langword="true"/>, or if the compressed size is
    /// smaller than the  uncompressed size, the file will be decompressed. Encrypted files are not supported and will
    /// throw a  <see cref="NotSupportedException"/>.</remarks>
    /// <param name="filename">The name of the file to read from the archive.</param>
    /// <param name="forceDecompress">A value indicating whether to force decompression of the file, even if it is stored in a compressed format
    /// smaller than its uncompressed size. If <see langword="true"/>, the file will always be decompressed if it is
    /// compressed.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A read-only memory block containing the file's contents, or <see langword="null"/> if the file does not exist or cannot be
    /// read.</returns>
    /// <exception cref="NotSupportedException">Thrown if the file is encrypted, as encrypted files are not supported.</exception>
    public ReadOnlyMemory<byte>? ReadFile(string filename, bool forceDecompress = false, CancellationToken cancellationToken = default)
    {
        var hashEntry = GetHashTableEntry(filename);
        if (hashEntry == null)
            return null;

        var blockEntry = _blockTable[hashEntry.Value.BlockIndex];
        if ((blockEntry.Flags & 0x80000000) == 0) // MPQ_FILE_EXISTS
            return null;

        if (blockEntry.CompressedSize == 0)
            return null;

        int compressedSize = checked((int)blockEntry.CompressedSize);
        int fileSize = checked((int)blockEntry.FileSize);
        int offset = (int)(blockEntry.FileOffset + _headerOffset);
        _ = _reader.BaseStream.Seek(offset, SeekOrigin.Begin);
        byte[] rawData = _reader.ReadBytes(compressedSize);

        // Encrypted?
        if ((blockEntry.Flags & 0x00010000) != 0) // MPQ_FILE_ENCRYPTED
            throw new NotSupportedException("Encrypted files are not supported yet.");

        // Single unit file (one chunk, maybe compressed)
        if ((blockEntry.Flags & 0x01000000) != 0) // MPQ_FILE_SINGLE_UNIT
        {
            if ((blockEntry.Flags & 0x00000200) != 0 && // MPQ_FILE_COMPRESS
                (forceDecompress || blockEntry.CompressedSize < blockEntry.FileSize))
            {
                byte[] singleUnitResult = new byte[fileSize];
                Decompress(rawData, 0, rawData.Length, singleUnitResult, 0, fileSize);
                return singleUnitResult;
            }

            return rawData;
        }

        // Multi-sector file
        int sectorSize = 512 << _header.SectorSizeShift;
        int sectorCount = (int)Math.Ceiling(blockEntry.FileSize / (double)sectorSize);
        bool hasSectorChecksum = (blockEntry.Flags & 0x00000100) != 0; // MPQ_FILE_SECTOR_CRC

        int tableEntries = sectorCount + 1 + (hasSectorChecksum ? 1 : 0);
        int[] positions = new int[tableEntries];

        for (int i = 0; i < tableEntries; i++)
            positions[i] = BinaryPrimitives.ReadInt32LittleEndian(rawData.AsSpan(i * sizeof(int), sizeof(int)));

        byte[] result = new byte[fileSize];
        int bytesCopied = 0;

        for (int i = 0; i < sectorCount; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int start = positions[i];
            int end = positions[i + 1];
            int length = end - start;

            int expectedSectorLength = Math.Min(sectorSize, fileSize - bytesCopied);

            bool isCompressedSector =
                (blockEntry.Flags & 0x00000200) != 0 && // MPQ_FILE_COMPRESS
                length < expectedSectorLength;

            if (isCompressedSector)
            {
                Decompress(rawData, start, length, result, bytesCopied, expectedSectorLength);
                bytesCopied += expectedSectorLength;
            }
            else
            {
                if (length != expectedSectorLength)
                    throw new InvalidDataException(
                        $"MPQ sector length mismatch. Expected {expectedSectorLength}, got {length}.");

                Buffer.BlockCopy(rawData, start, result, bytesCopied, length);
                bytesCopied += length;
            }
        }

        return result;
    }

    private static readonly string[] separatorArray = ["\r\n", "\n"];

    private MPQHashTableEntry? GetHashTableEntry(string filename)
    {
        uint hashA = Hash(filename, "NAMEA");
        uint hashB = Hash(filename, "NAMEB");

        for (int i = 0; i < _hashTable.Length; i++)
        {
            var entry = _hashTable[i];
            if (entry.NameHashA == hashA && entry.NameHashB == hashB)
            {
                return entry;
            }
        }
        return null;
    }

    private static void Decompress(byte[] data,
                                   int offset,
                                   int length,
                                   byte[] destination,
                                   int destinationOffset,
                                   int expectedLength)
    {
        if (length <= 0)
            throw new InvalidDataException("Compressed MPQ data is empty.");

        byte compressionType = data[offset];

        switch (compressionType)
        {
            case 0: // No compression
                if (length != expectedLength)
                    throw new InvalidDataException($"Decompressed MPQ data length mismatch. Expected {expectedLength} bytes, got {length} bytes.");

                Array.Copy(data, offset, destination, destinationOffset, expectedLength);
                return;

            case 2: // zlib/deflate
                using (var input = new MemoryStream(data, offset + 1, length - 1, writable: false))
                using (var deflate = new DeflateStream(input, CompressionMode.Decompress))
                {
                    ReadExpectedLength(deflate, destination, destinationOffset, expectedLength);
                    return;
                }

            case 16: // BZip2 (requires SharpZipLib or similar)
                using (var input = new MemoryStream(data, offset + 1, length - 1, writable: false))
                using (var bzip = new ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(input))
                {
                    ReadExpectedLength(bzip, destination, destinationOffset, expectedLength);
                    return;
                }

            default:
                throw new NotSupportedException($"Unsupported compression type: {compressionType}");
        }
    }

    private static void ReadExpectedLength(Stream source, byte[] destination, int destinationOffset, int expectedLength)
    {
        int totalRead = 0;
        while (totalRead < expectedLength)
        {
            int read = source.Read(destination, destinationOffset + totalRead, expectedLength - totalRead);
            if (read == 0)
                break;

            totalRead += read;
        }

        if (totalRead != expectedLength)
            throw new InvalidDataException($"Decompressed MPQ data length mismatch. Expected {expectedLength} bytes, got {totalRead} bytes.");

        if (source.ReadByte() >= 0)
            throw new InvalidDataException($"Decompressed MPQ data exceeds expected length of {expectedLength} bytes.");
    }

    /// <summary>
    /// Reads the contents of a file from the archive.
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="forceDecompress"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="EndOfStreamException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    public async Task<ReadOnlyMemory<byte>?> ReadFileAsync(string filename,
                                                          bool forceDecompress = false,
                                                          CancellationToken cancellationToken = default)
    {
        var hashEntry = GetHashTableEntry(filename);
        if (hashEntry == null)
            return null;

        var blockEntry = _blockTable[hashEntry.Value.BlockIndex];
        if ((blockEntry.Flags & 0x80000000) == 0) // MPQ_FILE_EXISTS
            return null;

        if (blockEntry.CompressedSize == 0)
            return null;

        int compressedSize = checked((int)blockEntry.CompressedSize);
        int fileSize = checked((int)blockEntry.FileSize);
        int offset = (int)(blockEntry.FileOffset + _headerOffset);
        _ = _reader.BaseStream.Seek(offset, SeekOrigin.Begin);

        // Read raw compressed bytes
        byte[] rawData = new byte[compressedSize];
        await _reader.BaseStream.ReadExactlyAsync(rawData, cancellationToken).ConfigureAwait(false);

        // Encrypted?
        if ((blockEntry.Flags & 0x00010000) != 0) // MPQ_FILE_ENCRYPTED
            throw new NotSupportedException("Encrypted files are not supported yet.");

        // Single unit file (one chunk, maybe compressed)
        if ((blockEntry.Flags & 0x01000000) != 0) // MPQ_FILE_SINGLE_UNIT
        {
            if ((blockEntry.Flags & 0x00000200) != 0 && // MPQ_FILE_COMPRESS
                (forceDecompress || blockEntry.CompressedSize < blockEntry.FileSize))
            {
                byte[] singleUnitResult = new byte[fileSize];
                DecompressAsync(rawData, 0, compressedSize, singleUnitResult, 0, fileSize);
                return singleUnitResult;
            }

            return rawData;
        }

        // Multi-sector file
        int sectorSize = 512 << _header.SectorSizeShift;
        int sectorCount = (int)Math.Ceiling(blockEntry.FileSize / (double)sectorSize);
        bool hasSectorChecksum = (blockEntry.Flags & 0x00000100) != 0; // MPQ_FILE_SECTOR_CRC

        int tableEntries = sectorCount + 1 + (hasSectorChecksum ? 1 : 0);
        int[] positions = new int[tableEntries];

        for (int i = 0; i < tableEntries; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            positions[i] = BinaryPrimitives.ReadInt32LittleEndian(rawData.AsSpan(i * sizeof(int), sizeof(int)));
        }

        byte[] result = new byte[fileSize];
        int bytesCopied = 0;

        for (int i = 0; i < sectorCount; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int start = positions[i];
            int end = positions[i + 1];
            int length = end - start;

            int expectedSectorLength = Math.Min(sectorSize, fileSize - bytesCopied);

            bool isCompressedSector =
                (blockEntry.Flags & 0x00000200) != 0 &&
                length < expectedSectorLength;

            if (isCompressedSector)
            {
                DecompressAsync(rawData, start, length, result, bytesCopied, expectedSectorLength);
                bytesCopied += expectedSectorLength;
            }
            else
            {
                if (length != expectedSectorLength)
                    throw new InvalidDataException(
                        $"MPQ sector length mismatch. Expected {expectedSectorLength}, got {length}.");

                Array.Copy(rawData, start, result, bytesCopied, length);
                bytesCopied += length;
            }
        }

        return result;
    }

    private static void DecompressAsync(byte[] data,
                                             int offset,
                                             int length,
                                             byte[] destination,
                                             int destinationOffset,
                                             int expectedLength)
    {
        if (length <= 0)
            throw new InvalidDataException("Compressed MPQ data is empty.");

        byte compressionType = data[offset];

        switch (compressionType)
        {
            case 0: // No compression
                if (length != expectedLength)
                    throw new InvalidDataException($"Decompressed MPQ data length mismatch. Expected {expectedLength} bytes, got {length} bytes.");

                Array.Copy(data, offset, destination, destinationOffset, expectedLength);
                return;

            case 2: // zlib/deflate
                {
                    using var input = new MemoryStream(data, offset + 1, length - 1, writable: false);
                    using var output = new MemoryStream(destination, destinationOffset, expectedLength, writable: true);
                    using var deflate = new DeflateStream(input, CompressionMode.Decompress);
                    deflate.CopyTo(output);
                    return;
                }

            case 16: // BZip2
                {
                    using var input = new MemoryStream(data, offset + 1, length - 1, writable: false);
                    using var output = new MemoryStream(destination, destinationOffset, expectedLength, writable: true);
                    ICSharpCode.SharpZipLib.BZip2.BZip2.Decompress(input, output, isStreamOwner: false);
                    return;
                }

            default:
                throw new NotSupportedException($"Unsupported compression type: {compressionType}");
        }
    }

    /// <summary>
    /// Returns the contents of the files stored in the internal buffer.
    /// </summary>
    /// <remarks>If no files are available, a message indicating "No files found." is displayed. The method
    /// assumes the file contents are encoded in UTF-8 and splits the text into lines based on newline characters before
    /// printing each line to the console.</remarks>
    public string PrintFiles()
    {
        if (_files is null)
        {
            return string.Empty;
        }
        StringBuilder sb = new();
        var text = System.Text.Encoding.UTF8.GetString(_files.Value.Span);
        var lines = text.Split(separatorArray, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            _ = sb.AppendLine(line);
        }
        return sb.ToString();
    }
}

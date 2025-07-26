using System.IO.Compression;

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
    /// <returns>A byte array containing the file's contents, or <see langword="null"/> if the file does not exist or cannot be
    /// read.</returns>
    /// <exception cref="NotSupportedException">Thrown if the file is encrypted, as encrypted files are not supported.</exception>
    public byte[]? ReadFile(string filename, bool forceDecompress = false)
    {
        var hashEntry = GetHashTableEntry(filename);
        if (hashEntry == null)
            return null;

        var blockEntry = _blockTable[hashEntry.Value.BlockIndex];
        if ((blockEntry.Flags & 0x80000000) == 0) // MPQ_FILE_EXISTS
            return null;

        if (blockEntry.CompressedSize == 0)
            return null;

        int offset = (int)(blockEntry.FileOffset + _headerOffset);
        _fileStream.Seek(offset, SeekOrigin.Begin);
        byte[] rawData = _reader.ReadBytes((int)blockEntry.CompressedSize);

        // Encrypted?
        if ((blockEntry.Flags & 0x00010000) != 0) // MPQ_FILE_ENCRYPTED
            throw new NotSupportedException("Encrypted files are not supported yet.");

        // Single unit file (one chunk, maybe compressed)
        if ((blockEntry.Flags & 0x01000000) != 0) // MPQ_FILE_SINGLE_UNIT
        {
            if ((blockEntry.Flags & 0x00000200) != 0 && // MPQ_FILE_COMPRESS
                (forceDecompress || blockEntry.CompressedSize < blockEntry.FileSize))
            {
                return Decompress(rawData);
            }

            return rawData;
        }

        // Multi-sector file
        int sectorSize = 512 << _header.SectorSizeShift;
        int sectorCount = (int)Math.Ceiling(blockEntry.FileSize / (double)sectorSize);
        bool hasSectorChecksum = (blockEntry.Flags & 0x00000100) != 0; // MPQ_FILE_SECTOR_CRC

        int tableEntries = sectorCount + 1 + (hasSectorChecksum ? 1 : 0);
        int[] positions = new int[tableEntries];

        using var ms = new MemoryStream(rawData);
        using var br = new BinaryReader(ms);

        for (int i = 0; i < tableEntries; i++)
            positions[i] = br.ReadInt32();

        byte[] result = new byte[blockEntry.FileSize];
        int bytesCopied = 0;

        for (int i = 0; i < sectorCount; i++)
        {
            int start = positions[i];
            int end = positions[i + 1];
            int length = end - start;

            ms.Seek(start, SeekOrigin.Begin);
            byte[] sector = br.ReadBytes(length);

            if ((blockEntry.Flags & 0x00000200) != 0 && // MPQ_FILE_COMPRESS
                (forceDecompress || sector.Length < sectorSize))
            {
                byte[] decompressed = Decompress(sector);
                Array.Copy(decompressed, 0, result, bytesCopied, decompressed.Length);
                bytesCopied += decompressed.Length;
            }
            else
            {
                Array.Copy(sector, 0, result, bytesCopied, sector.Length);
                bytesCopied += sector.Length;
            }
        }

        return result;
    }
    private static readonly string[] separatorArray = new[] { "\r\n", "\n" };

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

    private static byte[] Decompress(byte[] data)
    {
        byte compressionType = data[0];

        switch (compressionType)
        {
            case 0: // No compression
                return data;
            case 2: // zlib/deflate
                using (var input = new MemoryStream(data, 1, data.Length - 1))
                using (var deflate = new DeflateStream(input, CompressionMode.Decompress))
                using (var output = new MemoryStream())
                {
                    deflate.CopyTo(output);
                    return output.ToArray();
                }
            case 16: // BZip2 (requires SharpZipLib or similar)
                using (var input = new MemoryStream(data, 1, data.Length - 1))
                using (var bzip = new ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(input))
                using (var output = new MemoryStream())
                {
                    bzip.CopyTo(output);
                    return output.ToArray();
                }
            default:
                throw new NotSupportedException($"Unsupported compression type: {compressionType}");
        }
    }

    /// <summary>
    /// Prints the contents of the files stored in the internal buffer to the console.
    /// </summary>
    /// <remarks>If no files are available, a message indicating "No files found." is displayed. The method
    /// assumes the file contents are encoded in UTF-8 and splits the text into lines based on newline characters before
    /// printing each line to the console.</remarks>
    public void PrintFiles()
    {
        if (_files is null)
        {
            Console.WriteLine("No files found.");
            return;
        }
        var text = System.Text.Encoding.UTF8.GetString(_files);
        var lines = text.Split(separatorArray, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            Console.WriteLine(line);
        }
    }
}
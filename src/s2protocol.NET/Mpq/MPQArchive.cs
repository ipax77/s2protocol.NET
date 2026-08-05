namespace s2protocol.NET.Mpq;

using System.Text;

/// <summary>
/// Represents an MPQ archive, providing functionality to read and access its contents.
/// </summary>
/// <remarks>The <see cref="MPQArchive"/> class allows users to open and interact with an MPQ archive file. It
/// provides access to the archive's metadata, hash table, block table, and embedded files. Instances of this class must
/// be disposed of after use to release file handles and other resources.</remarks>
public sealed partial class MPQArchive : IDisposable
{
    private readonly string _archivePath;
    private readonly BinaryReader _reader;

    private MPQHeader _header;
    private MPQUserDataHeader? _userDataHeader;
    private long _headerOffset;
    private MPQHashTableEntry[] _hashTable;
    private MPQBlockTableEntry[] _blockTable;
    private ReadOnlyMemory<byte>? _files;

    /// <summary>
    /// Initializes a new instance of the <see cref="MPQArchive"/> class from a file path.
    /// </summary>
    public MPQArchive(string archivePath, bool readFiles = true)
        : this(new FileStream(archivePath, FileMode.Open, FileAccess.Read, FileShare.Read), archivePath, readFiles, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MPQArchive"/> class from a stream.
    /// </summary>
    /// <param name="stream">Archive stream. Must be readable and seekable.</param>
    /// <param name="readFiles"></param>
    public MPQArchive(Stream stream, bool readFiles = true)
        : this(stream, string.Empty, readFiles, true)
    {
    }

    /// <summary>
    /// Core constructor that accepts a stream and optional archive path.
    /// </summary>
    private MPQArchive(Stream stream, string archivePath, bool readFiles, bool leaveOpen)
    {
        ArgumentNullException.ThrowIfNull(stream);
        if (!stream.CanRead || !stream.CanSeek)
        {
            throw new ArgumentException("Archive stream must be readable and seekable.", nameof(stream));
        }

        _archivePath = archivePath;
        _reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen);

        try
        {
            (_header, _userDataHeader, _headerOffset) = ReadHeader();
            _hashTable = ReadTable<MPQHashTableEntry>("hash");
            _blockTable = ReadTable<MPQBlockTableEntry>("block");
            ValidateTables();
            if (readFiles)
            {
                _files = ReadFile("(listfile)");
            }
        }
        catch
        {
            _reader.Dispose();
            throw;
        }
    }

    private void ValidateTables()
    {
        foreach (MPQHashTableEntry entry in _hashTable)
        {
            if (entry.BlockIndex != MpqHashEntryEmpty &&
                entry.BlockIndex != MpqHashEntryDeleted &&
                entry.BlockIndex >= _blockTable.Length)
            {
                throw new InvalidDataException($"MPQ hash table contains invalid block index {entry.BlockIndex}.");
            }
        }

        foreach (MPQBlockTableEntry entry in _blockTable)
        {
            if ((entry.Flags & MpqFileExists) == 0)
            {
                continue;
            }

            if (entry.FileSize > MaximumFileSize)
            {
                throw new InvalidDataException(
                    $"MPQ file declaration exceeds the {MaximumFileSize}-byte safety limit.");
            }

            ValidateArchiveRange(entry.FileOffset, entry.CompressedSize, "file data");
        }
    }

    private void ValidateArchiveRange(ulong relativeOffset, ulong length, string description)
    {
        ulong archiveSize = _header.ArchiveSize;
        if (relativeOffset > archiveSize || length > archiveSize - relativeOffset)
        {
            throw new InvalidDataException($"MPQ {description} lies outside the declared archive.");
        }

        ulong absoluteOffset = checked((ulong)_headerOffset + relativeOffset);
        ulong streamLength = checked((ulong)_reader.BaseStream.Length);
        if (absoluteOffset > streamLength || length > streamLength - absoluteOffset)
        {
            throw new InvalidDataException($"MPQ {description} lies outside the input stream.");
        }
    }

    /// <summary>
    /// Releases all resources used by the current instance of the class.
    /// </summary>
    /// <remarks>This method disposes of any managed resources, such as streams or readers,  associated with
    /// the instance. After calling this method, the instance should  no longer be used.</remarks>
    public void Dispose()
    {
        _reader?.Dispose();
    }
}

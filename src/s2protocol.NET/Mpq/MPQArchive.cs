namespace s2protocol.NET.Mpq;

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
    private byte[]? _files;

    /// <summary>
    /// Initializes a new instance of the <see cref="MPQArchive"/> class from a file path.
    /// </summary>
    public MPQArchive(string archivePath, bool readFiles = true)
        : this(new FileStream(archivePath, FileMode.Open, FileAccess.Read, FileShare.Read), archivePath, readFiles)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MPQArchive"/> class from a stream.
    /// </summary>
    /// <param name="stream">Archive stream. Must be readable and seekable.</param>
    /// <param name="readFiles"></param>
    public MPQArchive(Stream stream, bool readFiles = true)
        : this(stream, string.Empty, readFiles)
    {
    }

    /// <summary>
    /// Core constructor that accepts a stream and optional archive path.
    /// </summary>
    private MPQArchive(Stream stream, string archivePath, bool readFiles)
    {
        _archivePath = archivePath;
        _reader = new BinaryReader(stream);

        (_header, _userDataHeader, _headerOffset) = ReadHeader();
        _hashTable = ReadTable<MPQHashTableEntry>("hash");
        _blockTable = ReadTable<MPQBlockTableEntry>("block");
        if (readFiles)
        {
            _files = ReadFile("(listfile)");
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

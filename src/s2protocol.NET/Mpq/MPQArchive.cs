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
    private readonly FileStream _fileStream;
    private readonly BinaryReader _reader;

    private MPQHeader _header;
    private MPQUserDataHeader? _userDataHeader;
    private long _headerOffset;
    private MPQHashTableEntry[] _hashTable;
    private MPQBlockTableEntry[] _blockTable;
    private byte[]? _files;

    /// <summary>
    /// Initializes a new instance of the <see cref="MPQArchive"/> class, providing access to the contents of the
    /// specified MPQ archive.
    /// </summary>
    /// <remarks>This constructor opens the specified MPQ archive for reading and initializes the necessary
    /// structures to access its contents. The archive is expected to conform to the MPQ format. If the archive is
    /// invalid or corrupted, subsequent operations may fail.  The caller is responsible for ensuring that the file at
    /// <paramref name="archivePath"/> exists and is accessible.</remarks>
    /// <param name="archivePath">The file path to the MPQ archive to be opened. Must be a valid, readable file path.</param>
    public MPQArchive(string archivePath)
    {
        _archivePath = archivePath;
        _fileStream = new FileStream(_archivePath, FileMode.Open, FileAccess.Read);
        _reader = new BinaryReader(_fileStream);
        (_header, _userDataHeader, _headerOffset) = ReadHeader();
        _hashTable = ReadTable<MPQHashTableEntry>("hash").ToArray();
        _blockTable = ReadTable<MPQBlockTableEntry>("block").ToArray();
        _files = ReadFile("(listfile)");
    }

    /// <summary>
    /// Releases all resources used by the current instance of the class.
    /// </summary>
    /// <remarks>This method disposes of any managed resources, such as streams or readers,  associated with
    /// the instance. After calling this method, the instance should  no longer be used.</remarks>
    public void Dispose()
    {
        _reader?.Dispose();
        _fileStream?.Dispose();
    }
}
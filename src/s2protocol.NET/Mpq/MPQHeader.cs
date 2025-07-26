namespace s2protocol.NET.Mpq;

internal struct MPQHeader
{
    public uint Magic;                  // 4 bytes
    public uint HeaderSize;             // 4 bytes
    public uint ArchiveSize;            // 4 bytes
    public ushort FormatVersion;        // 2 bytes
    public ushort SectorSizeShift;      // 2 bytes (formerly BlockSize)
    public uint HashTableOffset;        // 4 bytes (relative offset in file)
    public uint BlockTableOffset;       // 4 bytes (relative offset in file)
    public uint HashTableEntries;       // 4 bytes
    public uint BlockTableEntries;      // 4 bytes

    // Extended header fields (only present if FormatVersion == 1)
    public uint ExtendedBlockTableOffset;
    public uint HashTableOffsetHigh;
    public uint BlockTableOffsetHigh;

    // Additional: user data header offset (e.g. 1024)
    public uint Offset;
}

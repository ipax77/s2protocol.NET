namespace s2protocol.NET.Mpq;

public sealed partial class MPQArchive
{
    private const uint MpqArchiveMagic = 0x1A51504D;
    private const uint MpqUserDataMagic = 0x1B51504D;
    private const uint MpqHeaderV0Size = 32;
    private const uint MpqHeaderV1Size = 44;

    private (MPQHeader, MPQUserDataHeader?, long) ReadHeader()
    {
        EnsureStreamRange(0, sizeof(uint), "signature");
        uint initialMagic = _reader.ReadUInt32();

        MPQUserDataHeader? userDataHeader = null;
        long headerOffset;

        if (initialMagic == MpqArchiveMagic)
        {
            headerOffset = 0;
        }
        else if (initialMagic == MpqUserDataMagic)
        {
            EnsureStreamRange(0, 16, "user-data header");
            _reader.BaseStream.Position = sizeof(uint);
            uint userDataSize = _reader.ReadUInt32();
            uint mpqHeaderOffset = _reader.ReadUInt32();
            uint userDataHeaderSize = _reader.ReadUInt32();

            if (userDataHeaderSize > int.MaxValue)
            {
                throw new InvalidDataException("MPQ user-data header is too large.");
            }

            EnsureStreamRange(16, userDataHeaderSize, "user-data content");
            if (mpqHeaderOffset < 16UL + userDataHeaderSize)
            {
                throw new InvalidDataException("MPQ header overlaps the user-data header.");
            }

            byte[] content = new byte[userDataHeaderSize];
            _reader.BaseStream.ReadExactly(content);
            userDataHeader = new MPQUserDataHeader
            {
                Magic = initialMagic,
                UserDataSize = userDataSize,
                MPQHeaderOffset = mpqHeaderOffset,
                UserDataHeaderSize = userDataHeaderSize,
                Content = content,
            };
            headerOffset = mpqHeaderOffset;
        }
        else
        {
            throw new DecodeException("Invalid MPQ header.");
        }

        EnsureStreamRange(headerOffset, MpqHeaderV0Size, "archive header");
        _reader.BaseStream.Position = headerOffset;

        MPQHeader header = new()
        {
            Magic = _reader.ReadUInt32(),
            HeaderSize = _reader.ReadUInt32(),
            ArchiveSize = _reader.ReadUInt32(),
            FormatVersion = _reader.ReadUInt16(),
            SectorSizeShift = _reader.ReadUInt16(),
            HashTableOffset = _reader.ReadUInt32(),
            BlockTableOffset = _reader.ReadUInt32(),
            HashTableEntries = _reader.ReadUInt32(),
            BlockTableEntries = _reader.ReadUInt32(),
            Offset = checked((uint)headerOffset),
        };

        if (header.Magic != MpqArchiveMagic)
        {
            throw new InvalidDataException("Invalid MPQ archive signature.");
        }

        if (header.FormatVersion > 4)
        {
            throw new InvalidDataException($"Unsupported MPQ format version {header.FormatVersion}.");
        }

        uint requiredHeaderSize = header.FormatVersion >= 1 ? MpqHeaderV1Size : MpqHeaderV0Size;
        if (header.HeaderSize < requiredHeaderSize || header.ArchiveSize < header.HeaderSize)
        {
            throw new InvalidDataException("Invalid MPQ header or archive size.");
        }

        EnsureStreamRange(headerOffset, header.ArchiveSize, "archive");

        if (header.SectorSizeShift > 15)
        {
            throw new InvalidDataException("MPQ sector-size shift is too large.");
        }

        if (header.FormatVersion >= 1)
        {
            EnsureStreamRange(headerOffset, MpqHeaderV1Size, "version 1 archive header");
            header.ExtendedBlockTableOffset = _reader.ReadUInt64();
            header.HashTableOffsetHigh = _reader.ReadUInt16();
            header.BlockTableOffsetHigh = _reader.ReadUInt16();
        }

        ValidateTableDeclaration(
            GetTableOffset(header.HashTableOffset, header.HashTableOffsetHigh),
            header.HashTableEntries,
            header,
            "hash");
        ValidateTableDeclaration(
            GetTableOffset(header.BlockTableOffset, header.BlockTableOffsetHigh),
            header.BlockTableEntries,
            header,
            "block");

        if (header.HashTableEntries == 0 ||
            (header.HashTableEntries & (header.HashTableEntries - 1)) != 0)
        {
            throw new InvalidDataException("MPQ hash-table entry count must be a non-zero power of two.");
        }

        if (header.BlockTableEntries == 0)
        {
            throw new InvalidDataException("MPQ block table must contain at least one entry.");
        }

        return (header, userDataHeader, headerOffset);
    }

    private static ulong GetTableOffset(uint low, ushort high) => low | ((ulong)high << 32);

    private static void ValidateTableDeclaration(ulong offset, uint entries, MPQHeader header, string tableName)
    {
        if (entries > int.MaxValue)
        {
            throw new InvalidDataException($"MPQ {tableName} table contains too many entries.");
        }

        ulong tableSize = (ulong)entries * 16;
        if (offset < header.HeaderSize || offset > header.ArchiveSize || tableSize > header.ArchiveSize - offset)
        {
            throw new InvalidDataException($"MPQ {tableName} table lies outside the declared archive.");
        }
    }

    private void EnsureStreamRange(long offset, ulong length, string description)
    {
        if (offset < 0)
        {
            throw new InvalidDataException($"MPQ {description} has a negative offset.");
        }

        ulong unsignedOffset = (ulong)offset;
        ulong streamLength = checked((ulong)_reader.BaseStream.Length);
        if (unsignedOffset > streamLength || length > streamLength - unsignedOffset)
        {
            throw new InvalidDataException($"MPQ {description} is truncated.");
        }
    }

    /// <summary>
    /// Prints the headers of the MPQ archive to the console, including the main archive header and, if present, the
    /// user data header.
    /// </summary>
    public void PrintHeaders()
    {
#pragma warning disable CA1303
        Console.WriteLine("MPQ archive header");
        Console.WriteLine("------------------");
        PrintHeader(_header);

        if (_userDataHeader != null)
        {
            Console.WriteLine();
            Console.WriteLine("MPQ user data header");
            Console.WriteLine("--------------------");
            PrintUserDataHeader(_userDataHeader.Value);
        }

        Console.WriteLine();
    }

    private static void PrintHeader(MPQHeader header)
    {
        PrintFormattedField(nameof(header.Magic), header.Magic);
        PrintFormattedField(nameof(header.HeaderSize), header.HeaderSize);
        PrintFormattedField(nameof(header.ArchiveSize), header.ArchiveSize);
        PrintFormattedField(nameof(header.FormatVersion), header.FormatVersion);
        PrintFormattedField(nameof(header.SectorSizeShift), header.SectorSizeShift);
        PrintFormattedField(nameof(header.HashTableOffset), header.HashTableOffset);
        PrintFormattedField(nameof(header.BlockTableOffset), header.BlockTableOffset);
        PrintFormattedField(nameof(header.HashTableEntries), header.HashTableEntries);
        PrintFormattedField(nameof(header.BlockTableEntries), header.BlockTableEntries);
        PrintFormattedField(nameof(header.ExtendedBlockTableOffset), header.ExtendedBlockTableOffset);
        PrintFormattedField(nameof(header.HashTableOffsetHigh), header.HashTableOffsetHigh);
        PrintFormattedField(nameof(header.BlockTableOffsetHigh), header.BlockTableOffsetHigh);
        PrintFormattedField(nameof(header.Offset), header.Offset);
    }

    private static void PrintUserDataHeader(MPQUserDataHeader userDataHeader)
    {
        PrintFormattedField(nameof(userDataHeader.Magic), userDataHeader.Magic);
        PrintFormattedField(nameof(userDataHeader.UserDataSize), userDataHeader.UserDataSize);
        PrintFormattedField(nameof(userDataHeader.MPQHeaderOffset), userDataHeader.MPQHeaderOffset);
        PrintFormattedField(nameof(userDataHeader.UserDataHeaderSize), userDataHeader.UserDataHeaderSize);
        PrintFormattedField(nameof(userDataHeader.Content), userDataHeader.Content);
    }

    private static void PrintFormattedField(string name, object? value)
    {
        if (value is byte[] byteArray)
        {
            Console.WriteLine("{0,-30} {1}", name, $"byte[{byteArray.Length}]");
        }
        else if (value is uint || value is int || value is ushort || value is short ||
                 value is long || value is ulong)
        {
            Console.WriteLine("{0,-30} {1} (0x{1:X})", name, value);
        }
        else
        {
            Console.WriteLine("{0,-30} {1}", name, value ?? "null");
        }
    }
}
#pragma warning restore CA1303

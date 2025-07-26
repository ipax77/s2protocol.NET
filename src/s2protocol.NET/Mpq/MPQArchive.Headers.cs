namespace s2protocol.NET.Mpq;

public sealed partial class MPQArchive
{
    private (MPQHeader, MPQUserDataHeader?, long) ReadHeader()
    {
        byte[] magicBytes = _reader.ReadBytes(4);

        MPQHeader header;
        MPQUserDataHeader? userDataHeader;
        long headerOffset;

        if (magicBytes.SequenceEqual(new byte[] { 0x4D, 0x50, 0x51, 0x1A })) // 'MPQ\x1A'
        {
            // Standard MPQ Header
            headerOffset = 0;
            userDataHeader = null;
        }
        else if (magicBytes.SequenceEqual(new byte[] { 0x4D, 0x50, 0x51, 0x1B })) // 'MPQ\x1B' User Data Header
        {
            _reader.BaseStream.Seek(0, SeekOrigin.Begin);
            var magic = _reader.ReadUInt32();
            var userDataSize = _reader.ReadUInt32();
            var mPQHeaderOffset = _reader.ReadUInt32();
            var userDataHeaderSize = _reader.ReadUInt32();
            var content = _reader.ReadBytes((int)userDataHeaderSize);
            userDataHeader = new MPQUserDataHeader
            {
                Magic = magic,
                UserDataSize = userDataSize,
                MPQHeaderOffset = mPQHeaderOffset,
                UserDataHeaderSize = userDataHeaderSize,
                Content = content
            };
            headerOffset = (int)userDataHeader.Value.MPQHeaderOffset;
        }
        else
        {
            throw new DecodeException("Invalid MPQ header.");
        }

        _reader.BaseStream.Seek(headerOffset, SeekOrigin.Begin);

        header = new MPQHeader
        {
            Magic = _reader.ReadUInt32(),
            HeaderSize = _reader.ReadUInt32(),
            ArchiveSize = _reader.ReadUInt32(),
            FormatVersion = _reader.ReadUInt16(),
            SectorSizeShift = _reader.ReadUInt16(), // renamed from BlockSize
            HashTableOffset = _reader.ReadUInt32(),
            BlockTableOffset = _reader.ReadUInt32(),
            HashTableEntries = _reader.ReadUInt32(),
            BlockTableEntries = _reader.ReadUInt32(),
            Offset = (uint)_headerOffset // save user data header offset
        };

        if (header.FormatVersion == 1)
        {
            header.ExtendedBlockTableOffset = _reader.ReadUInt32(); // 4 bytes (not 8)
            header.HashTableOffsetHigh = _reader.ReadUInt32();
            header.BlockTableOffsetHigh = _reader.ReadUInt32();
        }

        // Add user data header offset to relative offsets for absolute file seeking:
        header.HashTableOffset += header.Offset;
        header.BlockTableOffset += header.Offset;
        return (header, userDataHeader, headerOffset);
    }

    /// <summary>
    /// Prints the headers of the MPQ archive to the console, including the main archive header and, if present, the
    /// user data header.
    /// </summary>
    /// <remarks>This method outputs the properties of the MPQ archive header and, if available, the user data
    /// header to the console in a formatted manner. Each header is preceded by a descriptive title and a separator for
    /// clarity.</remarks>
    public void PrintHeaders()
    {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
        Console.WriteLine("MPQ archive header");
        Console.WriteLine("------------------");

        PrintObjectProperties(_header);

        if (_userDataHeader != null)
        {
            Console.WriteLine();
            Console.WriteLine("MPQ user data header");
            Console.WriteLine("--------------------");

            PrintObjectProperties(_userDataHeader.Value);
        }
        Console.WriteLine();
    }

    private static void PrintObjectProperties<T>(T obj)
    {
        var props = typeof(T).GetProperties();
        foreach (var prop in props)
        {
            var value = prop.GetValue(obj, null);
            PrintFormattedField(prop.Name, value);
        }

        var fields = typeof(T).GetFields();
        foreach (var field in fields)
        {
            var value = field.GetValue(obj);
            PrintFormattedField(field.Name, value);
        }
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
#pragma warning restore CA1303 // Do not pass literals as localized parameters

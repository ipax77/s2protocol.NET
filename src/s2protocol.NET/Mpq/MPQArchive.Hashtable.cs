using System.Runtime.InteropServices;

namespace s2protocol.NET.Mpq;

public sealed partial class MPQArchive
{
    private List<T> ReadTable<T>(string tableType) where T : struct
    {
        uint tableOffset;
        uint tableEntries;

        if (tableType == "hash")
        {
            tableOffset = _header.HashTableOffset;
            tableEntries = _header.HashTableEntries;
        }
        else if (tableType == "block")
        {
            tableOffset = _header.BlockTableOffset;
            tableEntries = _header.BlockTableEntries;
        }
        else
        {
            throw new ArgumentException("Invalid table type.");
        }

        uint key = Hash($"({tableType} table)", "TABLE");

        long tableSize = tableEntries * 16;
        _fileStream.Seek(tableOffset + _headerOffset, SeekOrigin.Begin);
        byte[] encryptedData = _reader.ReadBytes((int)tableSize);
        byte[] decryptedData = DecryptTable(encryptedData, key);

        List<T> entries = new();
        for (int i = 0; i < tableEntries; i++)
        {
            byte[] entryBytes = new byte[16];
            Array.Copy(decryptedData, i * 16, entryBytes, 0, 16);
            T entry = ByteArrayToStructure<T>(entryBytes);
            entries.Add(entry);
        }

        return entries;
    }

    private static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
    {
        GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try
        {
            IntPtr ptr = handle.AddrOfPinnedObject();
            return Marshal.PtrToStructure<T>(ptr);
        }
        finally
        {
            handle.Free();
        }
    }

    private static uint Hash(string input, string hashType)
    {
        input = input.ToUpperInvariant();
        int type = hashType switch
        {
            "TABLE" => 3,
            "NAMEA" => 1,
            "NAMEB" => 2,
            _ => throw new ArgumentException("Invalid hash type."),
        };

        uint seed1 = 0x7FED7FED;
        uint seed2 = 0xEEEEEEEE;

        foreach (char c in input)
        {
            uint ch = c;
            var value = StormBuffer[(type << 8) + ch];
            seed1 = (value ^ (seed1 + seed2)) & 0xFFFFFFFF;
            seed2 = ch + seed1 + seed2 + (seed2 << 5) + 3 & 0xFFFFFFFF;
        }

        return seed1;
    }

    private static byte[] DecryptTable(byte[] data, uint key)
    {
        uint seed1 = key;
        uint seed2 = 0xEEEEEEEE;
        byte[] result = new byte[data.Length];

        for (int i = 0; i < data.Length / 4; i++)
        {
            seed2 = (seed2 + StormBuffer[0x400 + (seed1 & 0xFF)]) & 0xFFFFFFFF;

            uint value = BitConverter.ToUInt32(data, i * 4);
            value = (value ^ (seed1 + seed2)) & 0xFFFFFFFF;

            seed1 = ((~seed1 << 21) + 0x11111111 | (seed1 >> 11)) & 0xFFFFFFFF;
            seed2 = (value + seed2 + (seed2 << 5) + 3) & 0xFFFFFFFF;

            byte[] valueBytes = BitConverter.GetBytes(value);
            Buffer.BlockCopy(valueBytes, 0, result, i * 4, 4);
        }

        return result;
    }
    
#pragma warning disable CA1303 // Do not pass literals as localized parameters

    /// <summary>
    /// Prints the contents of the hash table to the console in a formatted table.
    /// </summary>
    /// <remarks>The output includes the hash values, locale, platform, and block index for each entry in the
    /// hash table. This method is intended for debugging or informational purposes and writes directly to the
    /// console.</remarks>
    public void PrintHashTable()
    {
        Console.WriteLine("MPQ archive hash table");
        Console.WriteLine("----------------------");
        Console.WriteLine(" Hash A   Hash B  Locl Plat BlockIdx");

        foreach (var entry in _hashTable)
        {
            Console.WriteLine("{0:X8} {1:X8} {2:X4} {3:X4} {4:X8}",
                entry.NameHashA,
                entry.NameHashB,
                entry.Locale,
                entry.Platform,
                entry.BlockIndex);
        }
        Console.WriteLine();
    }

    /// <summary>
    /// Prints the contents of the MPQ archive block table to the console.
    /// </summary>
    /// <remarks>The output includes the file offset, compressed size, real size, and flags for each entry in
    /// the block table. This method is intended for diagnostic or debugging purposes to inspect the structure of the
    /// block table.</remarks>
    public void PrintBlockTable()
    {
        Console.WriteLine("MPQ archive block table");
        Console.WriteLine("----------------------");
        Console.WriteLine("FileOffset ArchSize RealSize Flags");

        foreach (var entry in _blockTable)
        {
            Console.WriteLine("{0:X8} {1} {2} {3:X4}",
                entry.FileOffset,
                entry.CompressedSize,
                entry.FileSize,
                entry.Flags
                );
        }
        Console.WriteLine();
    }
}
#pragma warning restore CA1303 // Do not pass literals as localized parameters

using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using ICSharpCode.SharpZipLib.BZip2;

namespace s2protocol.NET.tests;

internal sealed class MpqTestArchiveBuilder
{
    private const uint FileCompress = 0x00000200;
    private const uint FileSingleUnit = 0x01000000;
    private const uint FileExists = 0x80000000;
    private static readonly uint[] StormBuffer = GenerateStormBuffer();
    private readonly List<TestFile> _files = [];

    public void AddSingleUnit(string name, byte[] contents, byte compressionType = 0)
    {
        byte[] stored = compressionType == 0 ? contents : Compress(contents, compressionType);
        uint flags = FileExists | FileSingleUnit | (compressionType == 0 ? 0 : FileCompress);
        _files.Add(new TestFile(name, stored, checked((uint)contents.Length), flags, null));
    }

    public void AddMultiSector(string name, byte[] contents, byte compressionType = 2)
    {
        const int sectorSize = 512;
        int sectorCount = (contents.Length + sectorSize - 1) / sectorSize;
        int tableSize = checked((sectorCount + 1) * sizeof(int));
        List<byte[]> sectors = new(sectorCount);
        int storedLength = tableSize;

        for (int offset = 0; offset < contents.Length; offset += sectorSize)
        {
            byte[] sector = contents.AsSpan(offset, Math.Min(sectorSize, contents.Length - offset)).ToArray();
            byte[] compressed = Compress(sector, compressionType);
            byte[] stored = compressed.Length < sector.Length ? compressed : sector;
            sectors.Add(stored);
            storedLength = checked(storedLength + stored.Length);
        }

        byte[] block = new byte[storedLength];
        int position = tableSize;
        BinaryPrimitives.WriteInt32LittleEndian(block, position);
        for (int i = 0; i < sectors.Count; i++)
        {
            sectors[i].CopyTo(block, position);
            position += sectors[i].Length;
            BinaryPrimitives.WriteInt32LittleEndian(block.AsSpan((i + 1) * sizeof(int)), position);
        }

        _files.Add(new TestFile(name, block, checked((uint)contents.Length), FileExists | FileCompress, null));
    }

    public void AddStored(
        string name,
        byte[] stored,
        uint declaredFileSize,
        uint flags = FileExists | FileSingleUnit,
        uint? hashBlockIndex = null)
        => _files.Add(new TestFile(name, stored, declaredFileSize, flags, hashBlockIndex));

    public byte[] Build()
    {
        if (_files.Count == 0)
        {
            throw new InvalidOperationException("At least one file is required.");
        }

        int hashEntryCount = 1;
        while (hashEntryCount < _files.Count * 2)
        {
            hashEntryCount <<= 1;
        }

        const int headerSize = 32;
        int fileDataSize = _files.Sum(file => file.Stored.Length);
        int hashTableOffset = checked(headerSize + fileDataSize);
        int blockTableOffset = checked(hashTableOffset + hashEntryCount * 16);
        int archiveSize = checked(blockTableOffset + _files.Count * 16);

        byte[] archive = new byte[archiveSize];
        WriteHeader(archive, archiveSize, hashTableOffset, blockTableOffset, hashEntryCount, _files.Count);

        byte[] hashTable = new byte[hashEntryCount * 16];
        hashTable.AsSpan().Fill(0xFF);
        byte[] blockTable = new byte[_files.Count * 16];
        int fileOffset = headerSize;

        for (int i = 0; i < _files.Count; i++)
        {
            TestFile file = _files[i];
            file.Stored.CopyTo(archive, fileOffset);
            WriteHashEntry(hashTable, hashEntryCount, file.Name, file.HashBlockIndex ?? checked((uint)i));
            WriteBlockEntry(blockTable.AsSpan(i * 16, 16), fileOffset, file);
            fileOffset += file.Stored.Length;
        }

        EncryptTable(hashTable, Hash("(hash table)", 3));
        EncryptTable(blockTable, Hash("(block table)", 3));
        hashTable.CopyTo(archive, hashTableOffset);
        blockTable.CopyTo(archive, blockTableOffset);
        return archive;
    }

    private static void WriteHeader(
        Span<byte> archive,
        int archiveSize,
        int hashTableOffset,
        int blockTableOffset,
        int hashEntries,
        int blockEntries)
    {
        BinaryPrimitives.WriteUInt32LittleEndian(archive, 0x1A51504D);
        BinaryPrimitives.WriteUInt32LittleEndian(archive[4..], 32);
        BinaryPrimitives.WriteUInt32LittleEndian(archive[8..], checked((uint)archiveSize));
        BinaryPrimitives.WriteUInt16LittleEndian(archive[12..], 0);
        BinaryPrimitives.WriteUInt16LittleEndian(archive[14..], 0);
        BinaryPrimitives.WriteUInt32LittleEndian(archive[16..], checked((uint)hashTableOffset));
        BinaryPrimitives.WriteUInt32LittleEndian(archive[20..], checked((uint)blockTableOffset));
        BinaryPrimitives.WriteUInt32LittleEndian(archive[24..], checked((uint)hashEntries));
        BinaryPrimitives.WriteUInt32LittleEndian(archive[28..], checked((uint)blockEntries));
    }

    private static void WriteHashEntry(byte[] table, int entryCount, string name, uint blockIndex)
    {
        int slot = checked((int)(Hash(name, 0) & (uint)(entryCount - 1)));
        while (BinaryPrimitives.ReadUInt32LittleEndian(table.AsSpan(slot * 16 + 12)) != uint.MaxValue)
        {
            slot = (slot + 1) & (entryCount - 1);
        }

        Span<byte> entry = table.AsSpan(slot * 16, 16);
        BinaryPrimitives.WriteUInt32LittleEndian(entry, Hash(name, 1));
        BinaryPrimitives.WriteUInt32LittleEndian(entry[4..], Hash(name, 2));
        BinaryPrimitives.WriteUInt16LittleEndian(entry[8..], 0);
        BinaryPrimitives.WriteUInt16LittleEndian(entry[10..], 0);
        BinaryPrimitives.WriteUInt32LittleEndian(entry[12..], blockIndex);
    }

    private static void WriteBlockEntry(Span<byte> entry, int fileOffset, TestFile file)
    {
        BinaryPrimitives.WriteUInt32LittleEndian(entry, checked((uint)fileOffset));
        BinaryPrimitives.WriteUInt32LittleEndian(entry[4..], checked((uint)file.Stored.Length));
        BinaryPrimitives.WriteUInt32LittleEndian(entry[8..], file.FileSize);
        BinaryPrimitives.WriteUInt32LittleEndian(entry[12..], file.Flags);
    }

    private static byte[] Compress(byte[] contents, byte compressionType)
    {
        using MemoryStream output = new();
        output.WriteByte(compressionType);
        using MemoryStream input = new(contents, writable: false);

        switch (compressionType)
        {
            case 2:
                using (ZLibStream zlib = new(output, CompressionLevel.SmallestSize, leaveOpen: true))
                {
                    input.CopyTo(zlib);
                }

                break;

            case 16:
                BZip2.Compress(input, output, isStreamOwner: false, level: 9);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(compressionType));
        }

        return output.ToArray();
    }

    private static uint Hash(string input, int type)
    {
        uint seed1 = 0x7FED7FED;
        uint seed2 = 0xEEEEEEEE;

        foreach (char character in input.ToUpperInvariant())
        {
            uint value = StormBuffer[(type << 8) + character];
            seed1 = value ^ (seed1 + seed2);
            seed2 = character + seed1 + seed2 + (seed2 << 5) + 3;
        }

        return seed1;
    }

    private static void EncryptTable(Span<byte> data, uint key)
    {
        uint seed1 = key;
        uint seed2 = 0xEEEEEEEE;

        for (int offset = 0; offset < data.Length; offset += sizeof(uint))
        {
            seed2 += StormBuffer[0x400 + (seed1 & 0xFF)];
            Span<byte> word = data.Slice(offset, sizeof(uint));
            uint value = BinaryPrimitives.ReadUInt32LittleEndian(word);
            BinaryPrimitives.WriteUInt32LittleEndian(word, value ^ (seed1 + seed2));
            seed1 = (~seed1 << 21) + 0x11111111 | seed1 >> 11;
            seed2 = value + seed2 + (seed2 << 5) + 3;
        }
    }

    private static uint[] GenerateStormBuffer()
    {
        uint[] result = new uint[0x500];
        uint seed = 0x00100001;
        for (int i = 0; i < 0x100; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                int index = j * 0x100 + i;
                seed = (seed * 125 + 3) % 0x2AAAAB;
                uint high = (seed & 0xFFFF) << 16;
                seed = (seed * 125 + 3) % 0x2AAAAB;
                result[index] = high | (seed & 0xFFFF);
            }
        }

        return result;
    }

    private sealed record TestFile(string Name, byte[] Stored, uint FileSize, uint Flags, uint? HashBlockIndex);
}

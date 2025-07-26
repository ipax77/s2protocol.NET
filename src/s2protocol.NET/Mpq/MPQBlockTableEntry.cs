using System.Runtime.InteropServices;

namespace s2protocol.NET.Mpq;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct MPQBlockTableEntry
{
    public uint FileOffset;
    public uint CompressedSize;
    public uint FileSize;
    public uint Flags;
}

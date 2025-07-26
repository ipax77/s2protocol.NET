using System.Runtime.InteropServices;

namespace s2protocol.NET.Mpq;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct MPQHashTableEntry
{
    public uint NameHashA;
    public uint NameHashB;
    public ushort Locale;
    public ushort Platform;
    public uint BlockIndex;
}

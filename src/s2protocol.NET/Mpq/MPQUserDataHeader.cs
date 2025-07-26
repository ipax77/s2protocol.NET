namespace s2protocol.NET.Mpq;

internal struct MPQUserDataHeader
{
    public uint Magic;
    public uint UserDataSize;
    public uint MPQHeaderOffset;
    public uint UserDataHeaderSize;
    public byte[] Content;
}

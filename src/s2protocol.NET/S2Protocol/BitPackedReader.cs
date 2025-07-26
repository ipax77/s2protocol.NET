namespace s2protocol.NET.S2Protocol;

internal class BitPackedReader
{
    private readonly byte[] _buffer;
    private int _bitOffset;

    public BitPackedReader(byte[] buffer)
    {
        _buffer = buffer ?? Array.Empty<byte>();
        _bitOffset = 0;
    }

    public long ReadBits(int count)
    {
        if (count < 0 || count > 64)
            throw new ArgumentOutOfRangeException(nameof(count), "Can only read 0 to 64 bits.");

        long value = 0;
        for (int i = 0; i < count; i++)
        {
            int byteIndex = _bitOffset / 8;
            if (byteIndex >= _buffer.Length)
                throw new DecodeException("Attempt to read beyond buffer.");

            int bitIndex = _bitOffset % 8;
            int bit = (_buffer[byteIndex] >> bitIndex) & 1;

            value |= ((long)bit << i);
            _bitOffset++;
        }
        return value;
    }

    public byte[] ReadAlignedBytes(int count)
    {
        ByteAlign();

        int byteOffset = _bitOffset / 8;
        if (byteOffset + count > _buffer.Length)
            throw new DecodeException("Attempt to read beyond buffer.");

        byte[] result = new byte[count];
        Buffer.BlockCopy(_buffer, byteOffset, result, 0, count);
        _bitOffset += count * 8;
        return result;
    }

    public void ByteAlign()
    {
        _bitOffset = ((_bitOffset + 7) / 8) * 8;
    }

    public bool Done()
    {
        return _bitOffset >= _buffer.Length * 8;
    }

    public int UsedBits() => _bitOffset;
}



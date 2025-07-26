using System.Globalization;

namespace s2protocol.NET.S2Protocol;

internal sealed class BitPackedBuffer
{
    private readonly byte[] _data;
    private long _used;
    private int _next;
    private int _nextBits;
    private readonly bool _bigEndian;

    public BitPackedBuffer(byte[] contents, string endian = "big")
    {
        _data = contents;
        _used = 0;
        _nextBits = 0;
        _bigEndian = (endian == "big");
    }

    public override string ToString()
    {
        string s = (_used < _data.Length) ? _data[_used].ToString("x2", CultureInfo.InvariantCulture) : "--";
        return $"buffer({(_nextBits > 0 ? _next : 0):x2}/{_nextBits},[{_used}]={s})";
    }

    public bool Done() => _nextBits == 0 && _used >= _data.Length;

    public long UsedBits() => _used * 8 - _nextBits;

    public void ByteAlign() => _nextBits = 0;

    public byte[] ReadAlignedBytes(long count)
    {
        ByteAlign();

        long available = _data.Length - _used;

        int bytesToRead = (int)Math.Min(count, available);

        var result = new byte[bytesToRead];
        Array.Copy(_data, (int)_used, result, 0, bytesToRead);
        _used += bytesToRead;

        if (bytesToRead != count)
            throw new DecodeException(nameof(BitPackedDecoder));

        return result;
    }

    public long ReadBits_(int bits)
    {
        if (bits < 0 || bits > 64)
            throw new ArgumentOutOfRangeException(nameof(bits), "Can only read 0 to 64 bits.");

        long result = 0;
        int resultBits = 0;

        while (resultBits != bits)
        {
            if (_nextBits == 0)
            {
                if (Done())
                    throw new DecodeException(nameof(BitPackedDecoder));
                _next = _data[_used];
                _used += 1;
                _nextBits = 8;
            }

            int copyBits = Math.Min(bits - resultBits, _nextBits);
            int mask = (1 << copyBits) - 1;
            int copy = _next & mask;

            if (_bigEndian)
            {
                result |= ((long)copy << (bits - resultBits - copyBits));
            }
            else
            {
                result |= ((long)copy << resultBits);
            }

            _next >>= copyBits;
            _nextBits -= copyBits;
            resultBits += copyBits;
        }

        return result;
    }

    public long ReadBits(int bits)
    {
        if (bits < 0)
            throw new ArgumentOutOfRangeException(nameof(bits), "Cannot read a negative number of bits.");

        // Clamp read size to 64; we will still advance the full bit count
        int readBits = Math.Min(bits, 64);

        long result = 0;
        int resultBits = 0;
        int skippedBits = 0;

        while (skippedBits != bits)
        {
            if (_nextBits == 0)
            {
                if (Done())
                    throw new DecodeException(nameof(BitPackedDecoder));
                _next = _data[_used];
                _used += 1;
                _nextBits = 8;
            }

            int bitsRemaining = bits - skippedBits;
            int copyBits = Math.Min(bitsRemaining, _nextBits);
            int mask = (1 << copyBits) - 1;
            int copy = _next & mask;

            if (resultBits + copyBits <= 64)
            {
                if (_bigEndian)
                {
                    result |= ((long)copy << (readBits - resultBits - copyBits));
                }
                else
                {
                    result |= ((long)copy << resultBits);
                }

                resultBits += copyBits;
            }

            _next >>= copyBits;
            _nextBits -= copyBits;
            skippedBits += copyBits;
        }

        return result;
    }

    public byte[] ReadUnalignedBytes(int count)
    {
        byte[] result = new byte[count];
        for (int i = 0; i < count; i++)
        {
            var val = ReadBits(8);
            if (val < 0 || val > 255)
                throw new DecodeException("Invalid byte value");
            result[i] = (byte)val;
        }
        return result;
    }
}

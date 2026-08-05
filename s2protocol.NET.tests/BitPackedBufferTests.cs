using System.Globalization;
using System.Numerics;
using s2protocol.NET.S2Protocol;
using Xunit;

namespace s2protocol.NET.tests;

public sealed class BitPackedBufferTests
{
    [Fact]
    public void ReadBigIntegerWithZeroBitsReturnsZeroWithoutConsumingInput()
    {
        BitPackedBuffer buffer = new(new byte[] { 0xFF });

        BigInteger value = buffer.ReadBigInteger(0);

        Assert.Equal(BigInteger.Zero, value);
        Assert.Equal(0, buffer.UsedBits());
    }

    [Fact]
    public void ReadBigIntegerWith64BitsPreservesUnsignedHighBit()
    {
        BitPackedBuffer buffer = new(new byte[] { 0x80, 0, 0, 0, 0, 0, 0, 1 });

        BigInteger value = buffer.ReadBigInteger(64);

        Assert.Equal((BigInteger.One << 63) | BigInteger.One, value);
        Assert.True(buffer.Done());
    }

    [Fact]
    public void ReadBigIntegerWithMoreThan64BitsPreservesAllBits()
    {
        BitPackedBuffer buffer = new(new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0x5A });

        BigInteger value = buffer.ReadBigInteger(72);

        Assert.Equal(BigInteger.Parse("123456789ABCDEF5A", NumberStyles.HexNumber, CultureInfo.InvariantCulture), value);
        Assert.True(buffer.Done());
    }

    [Fact]
    public void ReadBigIntegerWithMoreThan64BitsPreservesHighBitAsPositive()
    {
        BitPackedBuffer buffer = new(new byte[] { 0x80, 0, 0, 0, 0, 0, 0, 0, 0x01 });

        BigInteger value = buffer.ReadBigInteger(72);

        Assert.Equal((BigInteger.One << 71) | BigInteger.One, value);
        Assert.True(buffer.Done());
    }
}

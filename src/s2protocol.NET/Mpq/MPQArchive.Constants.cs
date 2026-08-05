namespace s2protocol.NET.Mpq;

public sealed partial class MPQArchive
{
    private const uint MpqFileCompress = 0x00000200;
    private const uint MpqFileEncrypted = 0x00010000;
    private const uint MpqFileSingleUnit = 0x01000000;
    private const uint MpqFileSectorCrc = 0x04000000;
    private const uint MpqFileExists = 0x80000000;
    private const uint MpqHashEntryDeleted = 0xFFFFFFFE;
    private const uint MpqHashEntryEmpty = 0xFFFFFFFF;
    private const int MaximumFileSize = 512 * 1024 * 1024;
    private static readonly uint[] StormBuffer = GenerateStormBuffer();

    private static uint[] GenerateStormBuffer()
    {
        uint[] stormBuffer = new uint[0x500]; // 1280 entries
        uint seed = 0x00100001;

        for (int i = 0; i < 0x100; i++) // outer loop: 256 times
        {
            for (int j = 0; j < 5; j++) // inner loop: 5 times per outer
            {
                int index = j * 0x100 + i;

                seed = (seed * 125 + 3) % 0x2AAAAB;
                uint temp1 = (seed & 0xFFFF) << 16;

                seed = (seed * 125 + 3) % 0x2AAAAB;
                uint temp2 = seed & 0xFFFF;

                stormBuffer[index] = temp1 | temp2;
            }
        }
        return stormBuffer;
    }
}

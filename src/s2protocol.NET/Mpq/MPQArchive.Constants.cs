namespace s2protocol.NET.Mpq;

public sealed partial class MPQArchive
{
    //private const uint MPQ_FILE_IMPLODE = 0x00000100;
    //private const uint MPQ_FILE_COMPRESS = 0x00000200;
    //private const uint MPQ_FILE_ENCRYPTED = 0x00010000;
    //private const uint MPQ_FILE_FIX_KEY = 0x00020000;
    //private const uint MPQ_FILE_SINGLE_UNIT = 0x01000000;
    //private const uint MPQ_FILE_DELETE_MARKER = 0x02000000;
    //private const uint MPQ_FILE_SECTOR_CRC = 0x04000000;
    //private const uint MPQ_FILE_EXISTS = 0x80000000;
    private static uint[] StormBuffer = GenerateStormBuffer();

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
using System.Runtime.CompilerServices;

namespace MBW.Utilities.IPAddresses.Helpers;

internal static class BitUtilities
{
    private static readonly byte[] BytePrefix = {
        8, 7, 6, 6, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 4,
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
        2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
        2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
    };

    public static byte FindCommonPrefixSize(uint a, uint b)
    {
        byte prefix = 0;
        uint byteA = (a >> 24) & 0xFF;
        uint byteB = (b >> 24) & 0xFF;
        byte common = BytePrefix[byteA ^ byteB];

        prefix += common;
        if (common == 8)
        {
            byteA = (a >> 16) & 0xFF;
            byteB = (b >> 16) & 0xFF;
            common = BytePrefix[byteA ^ byteB];

            prefix += common;

            if (common == 8)
            {
                byteA = (a >> 8) & 0xFF;
                byteB = (b >> 8) & 0xFF;
                common = BytePrefix[byteA ^ byteB];

                prefix += common;

                if (common == 8)
                {
                    byteA = a & 0xFF;
                    byteB = b & 0xFF;
                    common = BytePrefix[byteA ^ byteB];

                    prefix += common;
                }
            }
        }

        return prefix;
    }

    public static byte FindCommonPrefixSize(ulong a, ulong b)
    {
        byte prefix = 0;
        ulong byteA = (a >> 56) & 0xFF;
        ulong byteB = (b >> 56) & 0xFF;
        byte common = BytePrefix[byteA ^ byteB];

        prefix += common;
        if (common == 8)
        {
            byteA = (a >> 48) & 0xFF;
            byteB = (b >> 48) & 0xFF;
            common = BytePrefix[byteA ^ byteB];

            prefix += common;

            if (common == 8)
            {
                byteA = (a >> 40) & 0xFF;
                byteB = (b >> 40) & 0xFF;
                common = BytePrefix[byteA ^ byteB];

                prefix += common;

                if (common == 8)
                {
                    byteA = (a >> 32) & 0xFF;
                    byteB = (b >> 32) & 0xFF;
                    common = BytePrefix[byteA ^ byteB];

                    prefix += common;

                    if (common == 8)
                    {
                        byteA = (a >> 24) & 0xFF;
                        byteB = (b >> 24) & 0xFF;
                        common = BytePrefix[byteA ^ byteB];

                        prefix += common;

                        if (common == 8)
                        {
                            byteA = (a >> 16) & 0xFF;
                            byteB = (b >> 16) & 0xFF;
                            common = BytePrefix[byteA ^ byteB];

                            prefix += common;

                            if (common == 8)
                            {
                                byteA = (a >> 8) & 0xFF;
                                byteB = (b >> 8) & 0xFF;
                                common = BytePrefix[byteA ^ byteB];

                                prefix += common;

                                if (common == 8)
                                {
                                    byteA = a & 0xFF;
                                    byteB = b & 0xFF;
                                    common = BytePrefix[byteA ^ byteB];

                                    prefix += common;


                                }
                            }
                        }
                    }
                }
            }
        }

        return prefix;
    }

    public static uint Reverse(uint value)
    {
        return (value & 0xFF000000) >> 24 |
               (value & 0x00FF0000) >> 8 |
               (value & 0x0000FF00) << 8 |
               ((value & 0x000000FF) << 24);
    }

    public static ulong Reverse(ulong value)
    {
        return ((value & 0xFF00000000000000) >> 56) |
               ((value & 0x00FF000000000000) >> 40) |
               ((value & 0x0000FF0000000000) >> 24) |
               ((value & 0x000000FF00000000) >> 8) |
               ((value & 0x00000000FF000000) << 8) |
               ((value & 0x0000000000FF0000) << 24) |
               ((value & 0x000000000000FF00) << 40) |
               ((value & 0x00000000000000FF) << 56);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetTuplet(ref ulong low, ref ulong high, byte index, ulong newValue)
    {
        if (index < 4)
            high |= newValue << ((3 - index) * 16);
        else
            low |= newValue << ((7 - index) * 16);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetByte(ref ulong low, ref ulong high, byte index, ulong newValue)
    {
        if (index < 8)
            high |= newValue << ((7 - index) * 8);
        else
            low |= newValue << ((15 - index) * 8);
    }
}
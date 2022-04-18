using Dirichlet.Numerics;
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
        uint xor = a ^ b;

        byte common = BytePrefix[(xor >> 24) & 0xFF];
        if (common < 8)
            return common;

        common += BytePrefix[(xor >> 16) & 0xFF];
        if (common < 16)
            return common;

        common += BytePrefix[(xor >> 8) & 0xFF];
        if (common < 24)
            return common;

        common += BytePrefix[xor & 0xFF];
        return common;
    }

    public static byte FindCommonPrefixSize(UInt128 a, UInt128 b)
    {
        // A XOR B provides non-matching bits; count them
        UInt128 xor = a ^ b;

        byte common = BytePrefix[(xor.S0 >> 56) & 0xFF];
        if (common < 8)
            return common;

        common += BytePrefix[(xor.S0 >> 48) & 0xFF];
        if (common < 16)
            return common;

        common += BytePrefix[(xor.S0 >> 40) & 0xFF];
        if (common < 24)
            return common;

        common += BytePrefix[(xor.S0 >> 32) & 0xFF];
        if (common < 32)
            return common;

        common += BytePrefix[(xor.S0 >> 24) & 0xFF];
        if (common < 40)
            return common;

        common += BytePrefix[(xor.S0 >> 16) & 0xFF];
        if (common < 48)
            return common;

        common += BytePrefix[(xor.S0 >> 8) & 0xFF];
        if (common < 56)
            return common;

        common += BytePrefix[(xor.S0) & 0xFF];
        if (common < 64)
            return common;

        common += BytePrefix[(xor.S1 >> 56) & 0xFF];
        if (common < 72)
            return common;

        common += BytePrefix[(xor.S1 >> 48) & 0xFF];
        if (common < 80)
            return common;

        common += BytePrefix[(xor.S1 >> 40) & 0xFF];
        if (common < 88)
            return common;

        common += BytePrefix[(xor.S1 >> 32) & 0xFF];
        if (common < 96)
            return common;

        common += BytePrefix[(xor.S1 >> 24) & 0xFF];
        if (common < 104)
            return common;

        common += BytePrefix[(xor.S1 >> 16) & 0xFF];
        if (common < 112)
            return common;

        common += BytePrefix[(xor.S1 >> 8) & 0xFF];
        if (common < 120)
            return common;

        common += BytePrefix[(xor.S1) & 0xFF];
        return common;
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
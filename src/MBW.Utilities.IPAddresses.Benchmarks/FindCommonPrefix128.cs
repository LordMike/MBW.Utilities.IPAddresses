using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using Dirichlet.Numerics;
using MBW.Utilities.IPAddresses.Helpers;

namespace MBW.Utilities.IPAddresses.Benchmarks;

public class FindCommonPrefix128
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

    public struct InputTuple
    {
        internal readonly UInt128 A, B;
        public readonly int Expected;

        internal InputTuple(UInt128 a, UInt128 b, int expected)
        {
            A = a;
            B = b;
            Expected = expected;
        }

        public override string ToString() => $"(prefix: {Expected})";
    }

    [ParamsSource(nameof(Parameters))]
    public InputTuple Input;

    public IEnumerable<InputTuple> Parameters()
    {
        UInt128.Create(out UInt128 a, 0b0110010011000100110010100001110001100100110001001100101000011100UL, 0b0110010011000100110010100001110001100100110001001100101000011100UL);
        UInt128.Create(out UInt128 b, 0b0110000011000100110010100001110001100100110001001100101000011100UL, 0b0110010011000100110010100001110001100100110001001100101000011100UL);
        yield return new InputTuple(a, b, 5);

        UInt128.Create(out a, 0b0110010011000100110010100001110001100100110001001100101000011100UL, 0b0110010011000100110010100001110001100100110001001100101000011100UL);
        UInt128.Create(out b, 0b0110010011000100110010100001110001100100110001000100101000011100UL, 0b0110010011000100110010100001110001100100110001001100101000011100UL);
        yield return new InputTuple(a, b, 48);

        UInt128.Create(out a, 0b0110010011000100110010100001110001100100110001001100101000011100UL, 0b0110010011000100110010100001110001100100110001001100101000011100UL);
        UInt128.Create(out b, 0b0110010011000100110010100001110001100100110001001100101000011100UL, 0b0110010011000000110010100001110001100100110001001100101000011100UL);
        yield return new InputTuple(a, b, 77);
    }

    [Benchmark]
    public byte Current()
    {
        return BitUtilities.FindCommonPrefixSize(Input.A, Input.B);
    }

    [Benchmark]
    public byte ByteLookup()
    {
        // A XNOR B provides matching bits; count them
        UInt128 xor = Input.A ^ Input.B;

        byte common = BytePrefix[(xor.S1 >> 56) & 0xFF];
        if (common < 8)
            return common;

        common += BytePrefix[(xor.S1 >> 48) & 0xFF];
        if (common < 16)
            return common;

        common += BytePrefix[(xor.S1 >> 40) & 0xFF];
        if (common < 24)
            return common;

        common += BytePrefix[(xor.S1 >> 32) & 0xFF];
        if (common < 32)
            return common;

        common += BytePrefix[(xor.S1 >> 24) & 0xFF];
        if (common < 40)
            return common;

        common += BytePrefix[(xor.S1 >> 16) & 0xFF];
        if (common < 48)
            return common;

        common += BytePrefix[(xor.S1 >> 8) & 0xFF];
        if (common < 56)
            return common;

        common += BytePrefix[(xor.S1) & 0xFF];
        if (common < 64)
            return common;

        common = BytePrefix[(xor.S0 >> 56) & 0xFF];
        if (common < 72)
            return common;

        common += BytePrefix[(xor.S0 >> 48) & 0xFF];
        if (common < 80)
            return common;

        common += BytePrefix[(xor.S0 >> 40) & 0xFF];
        if (common < 88)
            return common;

        common += BytePrefix[(xor.S0 >> 32) & 0xFF];
        if (common < 96)
            return common;

        common += BytePrefix[(xor.S0 >> 24) & 0xFF];
        if (common < 104)
            return common;

        common += BytePrefix[(xor.S0 >> 16) & 0xFF];
        if (common < 112)
            return common;

        common += BytePrefix[(xor.S0 >> 8) & 0xFF];
        if (common < 120)
            return common;

        common += BytePrefix[(xor.S0) & 0xFF];
        if (common < 128)
            return common;

        return common;
    }

    [Benchmark]
    public byte ByteLookupComparisons()
    {
        byte GetCount(ulong value)
        {
            return value switch
            {
                0 => 8,
                1 => 7,
                <= 3 => 6,
                <= 7 => 5,
                <= 15 => 4,
                <= 31 => 3,
                <= 63 => 2,
                <= 127 => 1,
                _ => 0
            };
        }

        // A XNOR B provides matching bits; count them
        UInt128 xor = Input.A ^ Input.B;

        byte common = GetCount((xor.S1 >> 56) & 0xFF);
        if (common < 8)
            return common;

        common += GetCount((xor.S1 >> 48) & 0xFF);
        if (common < 16)
            return common;

        common += GetCount((xor.S1 >> 40) & 0xFF);
        if (common < 24)
            return common;

        common += GetCount((xor.S1 >> 32) & 0xFF);
        if (common < 32)
            return common;

        common += GetCount((xor.S1 >> 24) & 0xFF);
        if (common < 40)
            return common;

        common += GetCount((xor.S1 >> 16) & 0xFF);
        if (common < 48)
            return common;

        common += GetCount((xor.S1 >> 8) & 0xFF);
        if (common < 56)
            return common;

        common += GetCount((xor.S1) & 0xFF);
        if (common < 64)
            return common;

        common = GetCount((xor.S0 >> 56) & 0xFF);
        if (common < 72)
            return common;

        common += GetCount((xor.S0 >> 48) & 0xFF);
        if (common < 80)
            return common;

        common += GetCount((xor.S0 >> 40) & 0xFF);
        if (common < 88)
            return common;

        common += GetCount((xor.S0 >> 32) & 0xFF);
        if (common < 96)
            return common;

        common += GetCount((xor.S0 >> 24) & 0xFF);
        if (common < 104)
            return common;

        common += GetCount((xor.S0 >> 16) & 0xFF);
        if (common < 112)
            return common;

        common += GetCount((xor.S0 >> 8) & 0xFF);
        if (common < 120)
            return common;

        common += GetCount((xor.S0) & 0xFF);
        if (common < 128)
            return common;

        return common;
    }

    [Benchmark]
    public byte CountSetBitsLoop()
    {
        // A XNOR B provides matching bits; count them
        UInt128 xnor = ~(Input.A ^ Input.B);
        UInt128.Create(out UInt128 mask, 0UL, 0x8000_0000_0000_0000UL);

        // Shift & count
        byte prefix = 0;
        while ((xnor & mask) != 0)
        {
            // Eat one bit
            xnor <<= 1;
            prefix++;
        }

        return prefix;
    }
}
using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using MBW.Utilities.IPAddresses.Helpers;

namespace MBW.Utilities.IPAddresses.Benchmarks;

public class FindCommonPrefix
{
    public struct InputTuple
    {
        public readonly uint A, B;
        public readonly int Expected;

        public InputTuple(uint a, uint b, int expected)
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
        yield return new InputTuple(0b11110100110001001100101000011100, 0b01100100110001001100001010000000, 0);
        yield return new InputTuple(0b01110100110001001100101000011100, 0b01100100110001001100001010000000, 3);
        yield return new InputTuple(0b01100100110011001100101000011100, 0b01100100110001001100001010000000, 12);
        yield return new InputTuple(0b01100100110001001100101000011100, 0b01100100110001001100001010000000, 20);
        yield return new InputTuple(0b01100100110001001100101000011100, 0b01100100110001001100101000010100, 28);
        yield return new InputTuple(0b01100100110001001100101000011100, 0b01100100110001001100101000011101, 31);
        yield return new InputTuple(0b01100100110001001100101000011100, 0b01100100110001001100101000011100, 32);
    }

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

    [Benchmark]
    public byte Current()
    {
        return BitUtilities.FindCommonPrefixSize(Input.A, Input.B);
    }

    [Benchmark]
    public void LookupTable()
    {
        int prefix = 0;
        for (int i = 24; i >= 0; i -= 8)
        {
            uint byteA = (Input.A >> i) & 0xFF;
            uint byteB = (Input.B >> i) & 0xFF;
            byte common = BytePrefix[byteA ^ byteB];

            prefix += common;
            if (common != 8)
                break;
        }

        if (prefix != Input.Expected)
            throw new ArgumentException();
    }

    [Benchmark]
    public void LookupTable_Unrolled()
    {
        int prefix = 0;
        uint byteA = (Input.A >> 24) & 0xFF;
        uint byteB = (Input.B >> 24) & 0xFF;
        byte common = BytePrefix[byteA ^ byteB];

        prefix += common;
        if (common == 8)
        {
            byteA = (Input.A >> 16) & 0xFF;
            byteB = (Input.B >> 16) & 0xFF;
            common = BytePrefix[byteA ^ byteB];

            prefix += common;

            if (common == 8)
            {
                byteA = (Input.A >> 8) & 0xFF;
                byteB = (Input.B >> 8) & 0xFF;
                common = BytePrefix[byteA ^ byteB];

                prefix += common;

                if (common == 8)
                {
                    byteA = Input.A & 0xFF;
                    byteB = Input.B & 0xFF;
                    common = BytePrefix[byteA ^ byteB];

                    prefix += common;
                }
            }
        }

        if (prefix != Input.Expected)
            throw new ArgumentException();
    }

    [Benchmark]
    public void LookupTable_Unrolled_SumAsByte()
    {
        int prefix = 0;
        uint byteA = (Input.A >> 24) & 0xFF;
        uint byteB = (Input.B >> 24) & 0xFF;
        byte common = BytePrefix[byteA ^ byteB];

        prefix += common;
        if (common == 8)
        {
            byteA = (Input.A >> 16) & 0xFF;
            byteB = (Input.B >> 16) & 0xFF;
            common = BytePrefix[byteA ^ byteB];

            prefix += common;

            if (common == 8)
            {
                byteA = (Input.A >> 8) & 0xFF;
                byteB = (Input.B >> 8) & 0xFF;
                common = BytePrefix[byteA ^ byteB];

                prefix += common;

                if (common == 8)
                {
                    byteA = Input.A & 0xFF;
                    byteB = Input.B & 0xFF;
                    common = BytePrefix[byteA ^ byteB];

                    prefix += common;
                }
            }
        }

        if (prefix != Input.Expected)
            throw new ArgumentException();
    }

    [Benchmark]
    public void ShiftTillEqual()
    {
        // https://stackoverflow.com/a/3313963/1246988
        uint a = Input.A;
        uint b = Input.B;

        int prefix = 32;
        while (a != b)
        {
            a >>= 1;
            b >>= 1;
            prefix--;
        }

        if (prefix != Input.Expected)
            throw new ArgumentException();
    }

    [Benchmark]
    public void Logarithmic()
    {
        // https://stackoverflow.com/a/3314434/1246988
        int prefix = (Input.A != Input.B) ? (31 - (int)Math.Log(Input.A ^ Input.B, 2)) : 32;

        if (prefix != Input.Expected)
            throw new ArgumentException();
    }

    [Benchmark]
    public uint KernighanXNOR()
    {
        // http://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetKernighan
        // A XNOR B provides matching bits; count them
        uint v = ~(Input.A ^ Input.B); // count the number of bits set in v
        uint c = 0; // c accumulates the total bits set in v
        for (; v > 0; c++)
        {
            v &= v - 1; // clear the least significant bit set
        }

        return c;
    }

    [Benchmark]
    public uint ParallelXNOR()
    {
        // http://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetParallel
        // A XNOR B provides matching bits; count them
        uint v = ~(Input.A ^ Input.B);

        v = v - ((v >> 1) & 0x55555555);                    // reuse input as temporary
        v = (v & 0x33333333) + ((v >> 2) & 0x33333333);     // temp
        uint c = ((v + (v >> 4) & 0xF0F0F0F) * 0x1010101) >> 24; // count

        return c;
    }
}

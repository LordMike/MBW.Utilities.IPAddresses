using BenchmarkDotNet.Attributes;
using System.Runtime.InteropServices;

namespace MBW.Utilities.IPAddresses.Benchmarks;

public class StructExplicitBenchmarks
{
    [Benchmark]
    public ushort UseShift()
    {
        TestShift tmp = new TestShift();
        tmp.Value1 = 3074457345618258602UL;

        return tmp.Short2;
    }

    [Benchmark]
    public ushort UseOffsets()
    {
        TestLayout tmp = new TestLayout();
        tmp.Value1 = 3074457345618258602UL;

        return tmp.Short2;
    }

    [StructLayout(LayoutKind.Explicit)]
    struct TestShift
    {
        [FieldOffset(0)]
        public ulong Value1;
        [FieldOffset(8)]
        public ulong Value2;

        public ushort Short1 => (ushort)(Value1 >> 48);
        public ushort Short2 => (ushort)(Value1 >> 32);
    }

    [StructLayout(LayoutKind.Explicit)]
    struct TestLayout
    {
        [FieldOffset(0)]
        public ulong Value1;
        [FieldOffset(8)]
        public ulong Value2;

        [FieldOffset(0)]
        public ushort Short1;
        [FieldOffset(2)]
        public ushort Short2;
    }
}

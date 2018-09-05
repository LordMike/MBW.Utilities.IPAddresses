using System;
using BenchmarkDotNet.Attributes;
using MBW.Utilities.IPAddresses.Helpers;

namespace MBW.Utilities.IPAddresses.Benchmarks
{
    public class ReverseNumbers
    {
        [Benchmark]
        public void CurrentUint()
        {
            uint expected = 0x21436587;
            uint val = 0x87654321;
            uint res = BitUtilities.Reverse(val);

            if (res != expected)
                throw new ArgumentException();
        }

        [Benchmark]
        public void CurrentUlong()
        {
            ulong expected = 0x8765432155648934;
            ulong val = 0x3489645521436587;
            ulong res = BitUtilities.Reverse(val);

            if (res != expected)
                throw new ArgumentException();
        }
    }
}
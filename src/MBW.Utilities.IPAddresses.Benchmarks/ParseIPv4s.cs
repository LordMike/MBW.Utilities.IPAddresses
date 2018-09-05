using BenchmarkDotNet.Attributes;

namespace IPAddresses.Benchmarks
{
    public class ParseIPv4s
    {
        [Benchmark]
        public IpAddressRangeV4 WithCidr04() => IpAddressRangeV4.Parse("192.168.10.1/32");

        [Benchmark]
        public IpAddressRangeV4 WithCidr03() => IpAddressRangeV4.Parse("192.168.10/32");

        [Benchmark]
        public IpAddressRangeV4 WithCidr02() => IpAddressRangeV4.Parse("192.168/32");

        [Benchmark]
        public IpAddressRangeV4 WithCidr01() => IpAddressRangeV4.Parse("192/32");

        [Benchmark]
        public IpAddressRangeV4 NoCidr04() => IpAddressRangeV4.Parse("192.168.10.1");

        [Benchmark]
        public IpAddressRangeV4 NoCidr03() => IpAddressRangeV4.Parse("192.168.10");

        [Benchmark]
        public IpAddressRangeV4 NoCidr02() => IpAddressRangeV4.Parse("192.168");

        [Benchmark]
        public IpAddressRangeV4 NoCidr01() => IpAddressRangeV4.Parse("192");
    }
}

using BenchmarkDotNet.Attributes;

namespace MBW.Utilities.IPAddresses.Benchmarks;

public class ParseIPv4s
{
    [Benchmark]
    public IpAddressNetworkV4 WithCidr04() => IpAddressNetworkV4.Parse("192.168.10.1/32");

    [Benchmark]
    public IpAddressNetworkV4 WithCidr03() => IpAddressNetworkV4.Parse("192.168.10/32");

    [Benchmark]
    public IpAddressNetworkV4 WithCidr02() => IpAddressNetworkV4.Parse("192.168/32");

    [Benchmark]
    public IpAddressNetworkV4 WithCidr01() => IpAddressNetworkV4.Parse("192/32");

    [Benchmark]
    public IpAddressNetworkV4 NoCidr04() => IpAddressNetworkV4.Parse("192.168.10.1");

    [Benchmark]
    public IpAddressNetworkV4 NoCidr03() => IpAddressNetworkV4.Parse("192.168.10");

    [Benchmark]
    public IpAddressNetworkV4 NoCidr02() => IpAddressNetworkV4.Parse("192.168");

    [Benchmark]
    public IpAddressNetworkV4 NoCidr01() => IpAddressNetworkV4.Parse("192");
}
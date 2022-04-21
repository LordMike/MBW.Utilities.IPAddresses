using BenchmarkDotNet.Attributes;
using System.IO;
using System.Text;

namespace MBW.Utilities.IPAddresses.Benchmarks;

[MemoryDiagnoser]
[InvocationCount(1000000, 16)] // BDN will force low invocation counts when using IterationCleanup; to get proper timings we raise it
public class ToStringBenchmarks
{
    private IpAddressV4 _addressV4;
    private IpAddressNetworkV4 _networkV4;
    private IpAddressV6 _addressV6;
    private IpAddressNetworkV6 _networkV6;
    private StringBuilder _sb;
    private char[] _charBuffer;

    [GlobalSetup]
    public void Setup()
    {
        _addressV4 = (IpAddressV4)"174.64.25.18";
        _networkV4 = (IpAddressNetworkV4)"174.64.25.18/24";
        _addressV6 = (IpAddressV6)"2001:0dff:44ff:0:1744:ffff";
        _networkV6 = (IpAddressNetworkV6)"2001:0dff:44ff:0:1744:ffff/64";
        _sb = new StringBuilder();
        _charBuffer = new char[60];
    }

    [IterationCleanup]
    public void IterationCleanup() => _sb.Clear();

    [Benchmark]
    [BenchmarkCategory(nameof(IpAddressV4))]
    public string IPv4_ToString() => _addressV4.ToString();

    [Benchmark]
    [BenchmarkCategory(nameof(IpAddressV4))]
    public void IPv4_ToStringStringBuilder() => _addressV4.ToString(_sb);

    [Benchmark]
    [BenchmarkCategory(nameof(IpAddressV4))]
    public void IPv4_ToStringTextWriter() => _addressV4.ToString(TextWriter.Null);

    [Benchmark]
    [BenchmarkCategory(nameof(IpAddressV4))]
    public void IPv4_ToStringSpan() => _addressV4.ToString(_charBuffer);

    [Benchmark]
    [BenchmarkCategory(nameof(IpAddressNetworkV4))]
    public string IPv4Network_ToString() => _networkV4.ToString();

    [Benchmark]
    [BenchmarkCategory(nameof(IpAddressNetworkV4))]
    public void IPv4Network_ToStringStringBuilder() => _networkV4.ToString(_sb);

    [Benchmark]
    [BenchmarkCategory(nameof(IpAddressNetworkV4))]
    public void IPv4Network_ToStringTextWriter() => _networkV4.ToString(TextWriter.Null);

    [Benchmark]
    [BenchmarkCategory(nameof(IpAddressNetworkV4))]
    public void IPv4Network_ToStringSpan() => _networkV4.ToString(_charBuffer);

    [Benchmark]
    [BenchmarkCategory(nameof(IpAddressV6))]
    public string IPv6_ToString() => _addressV6.ToString();

    [Benchmark]
    [BenchmarkCategory(nameof(IpAddressV6))]
    public void IPv6_ToStringStringBuilder() => _addressV6.ToString(_sb);

    [Benchmark]
    [BenchmarkCategory(nameof(IpAddressV6))]
    public void IPv6_ToStringTextWriter() => _addressV6.ToString(TextWriter.Null);

    [Benchmark]
    [BenchmarkCategory(nameof(IpAddressV6))]
    public void IPv6_ToStringSpan() => _addressV6.ToString(_charBuffer);

    [Benchmark]
    [BenchmarkCategory(nameof(IpAddressNetworkV6))]
    public string IPv6Network_ToString() => _networkV6.ToString();

    [Benchmark]
    [BenchmarkCategory(nameof(IpAddressNetworkV6))]
    public void IPv6Network_ToStringStringBuilder() => _networkV6.ToString(_sb);

    [Benchmark]
    [BenchmarkCategory(nameof(IpAddressNetworkV6))]
    public void IPv6Network_ToStringTextWriter() => _networkV6.ToString(TextWriter.Null);

    [Benchmark]
    [BenchmarkCategory(nameof(IpAddressNetworkV6))]
    public void IPv6Network_ToStringSpan() => _networkV6.ToString(_charBuffer);

}

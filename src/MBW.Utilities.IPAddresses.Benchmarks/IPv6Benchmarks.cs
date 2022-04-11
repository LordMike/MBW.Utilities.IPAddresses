using BenchmarkDotNet.Attributes;

namespace MBW.Utilities.IPAddresses.Benchmarks;

public class IPv6Benchmarks
{
    private IpAddressV6 _address1;
    private IpAddressV6 _address2;
    private IpAddressNetworkV6 _net1;
    private IpAddressNetworkV6 _net2;

    public IPv6Benchmarks()
    {
        _net1 = (IpAddressNetworkV6)"2001:db8:8a2e:370::/80";
        _address1 = (IpAddressV6)"2001:0db8:8a2e:0370::7334";

        _net2 = (IpAddressNetworkV6)"fe80:ffff:1100::/40";
        _address2 = (IpAddressV6)"fe80:ffff:1111:2222:3333:4444:5555:6666";
    }

    [Benchmark]
    public bool NetworkContainsShort() => _net1.Contains(_address1);

    [Benchmark]
    public bool NetworkContainsLong() => _net2.Contains(_address2);

    [Benchmark]
    public bool AddressCompareLessThanAddresses() => _address1 < _address2;

    [Benchmark]
    public bool AddressCompareGreaterThanEqualsAddresses() => _address1 >= _address2;

    [Benchmark]
    public bool AddressNotEquals() => _address1 != _address2;

    [Benchmark]
    public bool NetworkNotEquals() => _net1 != _net2;

    [Benchmark]
    public int AddressCompareTo() => _address1.CompareTo(_address2);

    [Benchmark]
    public IpAddressV6 NetworkGetNetworkMask() => _net1.NetworkMask;

    [Benchmark]
    public IpAddressV6 NetworkGetNetworkWildcardMask() => _net1.NetworkWildcardMask;

    [Benchmark]
    public string NetworkToString() => _net1.ToString();

    [Benchmark]
    public string AddressToString() => _address2.ToString();

    [Benchmark]
    public int GetNetworkHashCode()
    {
        return _net1.GetHashCode();
    }
}

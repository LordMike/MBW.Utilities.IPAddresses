using FluentAssertions;
using MBW.Utilities.IPAddresses.DataStructures;
using Xunit;

namespace MBW.Utilities.IPAddresses.Tests;

public class IPSetTests
{
    [Fact]
    public void GenericTest()
    {
        IPv4Set<string> store = new IPv4Set<string>();
        store.Add((IpAddressNetworkV4)"192.168.10.0/24", "A");
        store.Add((IpAddressNetworkV4)"192.168.20.0/24", "B");
        store.Add((IpAddressNetworkV4)"10.0.0.0/8", "C");

        store.TryGet((IpAddressV4)"10.11.22.33", out IpAddressNetworkV4? net, out string val).Should().BeTrue();
        net.Should().Be((IpAddressNetworkV4)"10.0.0.0/8");
        val.Should().Be("C");
    }

    [Fact]
    public void LargeTest()
    {
        IPv4Set<int> store = new IPv4Set<int>();
        for (int i = 0; i < 256; i++)
            store.Add((IpAddressNetworkV4)$"192.168.{i}.0/24", i);

        store.TryGet((IpAddressV4)"192.168.33.16", out IpAddressNetworkV4? net, out int val).Should().BeTrue();
        net.Should().Be((IpAddressNetworkV4)"192.168.33.0/24");
        val.Should().Be(33);
    }
}

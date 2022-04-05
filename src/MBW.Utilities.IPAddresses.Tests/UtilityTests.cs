using Xunit;

namespace MBW.Utilities.IPAddresses.Tests;

public class UtilityTests
{
    [Fact]
    public void IPv4Utilities()
    {
        IpAddressNetworkV4 network = IpAddressNetworkV4.Parse("192.168.1.0/24");
        IpAddressNetworkV4 ip = IpAddressNetworkV4.Parse("192.168.1.1");

        Assert.True(network.Contains(ip));
        Assert.True(network.ContainsOrEqual(ip));
        Assert.True(ip.IsContainedIn((IpAddressNetworkV4)"192.168.1.0/24"));
        Assert.True(ip.IsContainedIn((IpAddressNetworkV4)"192.168.0.0/16"));
        Assert.False(ip.IsContainedIn((IpAddressNetworkV4)"192.168.1.128/25"));

        Assert.Equal("192.168.1.1", ip.ToString());
        Assert.Equal("192.168.1.1/32", ip.ToString(true));
        Assert.Equal("192.168.1.0/24", network.ToString());
        Assert.Equal("192.168.1/24", network.ToPrefixString());
    }
}

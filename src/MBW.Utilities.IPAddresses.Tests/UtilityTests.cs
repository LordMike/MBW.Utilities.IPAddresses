using Xunit;

namespace MBW.Utilities.IPAddresses.Tests;

public class UtilityTests
{
    [Fact]
    public void IPv4Utilities()
    {
        IpAddressRangeV4 network = IpAddressRangeV4.Parse("192.168.1.0/24");
        IpAddressRangeV4 ip = IpAddressRangeV4.Parse("192.168.1.1");

        Assert.True(network.Contains(ip));
        Assert.True(network.ContainsOrEqual(ip));
        Assert.True(ip.IsContainedIn("192.168.1.0/24"));
        Assert.True(ip.IsContainedIn("192.168.0.0/16"));
        Assert.False(ip.IsContainedIn("192.168.1.128/25"));

        Assert.Equal("192.168.1.1", ip.ToString());
        Assert.Equal("192.168.1.1/32", ip.ToString(true));
        Assert.Equal("192.168.1.0/24", network.ToString());
        Assert.Equal("192.168.1/24", network.ToPrefixString());
    }
}
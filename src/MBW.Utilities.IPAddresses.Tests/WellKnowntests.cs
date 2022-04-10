using FluentAssertions;
using MBW.Utilities.IPAddresses.WellKnown;
using Xunit;

namespace MBW.Utilities.IPAddresses.Tests;

public class WellKnowntests
{
    [Fact]
    public void IPv4()
    {
        WellKnownIPv4.Instance.GetType((IpAddressV4)"192.168.10.22").Should().Be(WellKnownIPv4Type.PrivateNetworking);
        WellKnownIPv4.Instance.GetType((IpAddressV4)"1.1.1.1").Should().Be(WellKnownIPv4Type.Unknown);
    }

    [Fact]
    public void IPv6()
    {
        WellKnownIPv6.Instance.GetType((IpAddressV6)"fe80:1224::7ef0").Should().Be(WellKnownIPv6Type.LinkLocal);
        WellKnownIPv6.Instance.GetType((IpAddressV6)"fe02:5::342").Should().Be(WellKnownIPv6Type.MulticastLinkLocal);
    }
}

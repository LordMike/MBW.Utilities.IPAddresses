using FluentAssertions;
using Xunit;

namespace MBW.Utilities.IPAddresses.Tests;

public class IPv4AddressTests
{
    [Theory]
    [InlineData("192.168.10.1", "192.168.10.1")]
    [InlineData("192.168.10", "192.168.10.0")]
    public void ToStringTests(string str, string expected)
    {
        IpAddressV4 network = (IpAddressV4)str;

        network.ToString().Should().Be(expected);
    }

    [Theory]
    [InlineData("192.168.10.1", new byte[] { 192, 168, 10, 1 })]
    [InlineData("1.2.3.4", new byte[] { 1, 2, 3, 4 })]
    [InlineData("4.3.2.1", new byte[] { 4, 3, 2, 1 })]
    public void AddressToBytesTests(string networks, byte[] expected)
    {
        IpAddressV4 network = (IpAddressV4)networks;

        network.AddressToBytes().Should().Equal(expected);

        byte[] res = new byte[4];
        network.AddressToBytes(res);

        res.Should().Equal(expected);
    }

    [Theory]
    [InlineData("192.168.10.1", "200.168.10.1")]
    [InlineData("0.0.0.1", "255.255.255.255")]
    [InlineData("1.2.3.4", "4.3.2.1")]
    [InlineData("192.168.10.1", "192.168.10.2")]
    [InlineData("192.168.10.255", "192.168.11.0")]
    public void ComparisonTests(string smallers, string largers)
    {
        IpAddressV4 smaller = (IpAddressV4)smallers;
        IpAddressV4 larger = (IpAddressV4)largers;

        (smaller < larger).Should().BeTrue();
        (smaller <= larger).Should().BeTrue();
        (larger > smaller).Should().BeTrue();
        (larger >= smaller).Should().BeTrue();

        smaller.CompareTo(larger).Should().BeNegative();
        larger.CompareTo(smaller).Should().BePositive();
    }

    [Theory]
    [InlineData("192.168.10.0", "192.168.10")]
    [InlineData("192.168.10.1", "192.168.10.1")]
    public void EqualTests(string lefts, string rights)
    {
        IpAddressV4 left = (IpAddressV4)lefts;
        IpAddressV4 right = (IpAddressV4)rights;

        (left < right).Should().BeFalse();
        (left <= right).Should().BeTrue();
        (left == right).Should().BeTrue();
        (right > left).Should().BeFalse();
        (right >= left).Should().BeTrue();
        (right == left).Should().BeTrue();

        left.CompareTo(right).Should().Be(0);
        right.CompareTo(left).Should().Be(0);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FluentAssertions;
using MBW.Utilities.IPAddresses.Tests.SignaturesLib;
using Xunit;

namespace MBW.Utilities.IPAddresses.Tests;

public class IPv4NetworkTests
{
    [Theory]
    [InlineData("192.168.10.1", "192.168.10.1", 32)]
    [InlineData("192.120.12.99/24", "192.120.12.0", 24)]
    [InlineData("192.120.12/24", "192.120.12.0", 24)]
    [InlineData("192.120/24", "192.120.0.0", 24)]
    [InlineData("192/24", "192.0.0.0", 24)]
    [InlineData("255.255.255.255/20", "255.255.240.0", 20)]
    public void ValidFormatsTests(string input, string expectedIp, byte expectedMask)
    {
        IpAddressNetworkV4 ip = IpAddressNetworkV4.Parse(input);

        ip.Mask.Should().Be(expectedMask);
        ip.NetworkAddress.Should().Be(IPAddress.Parse(expectedIp));
    }

    [Theory]
    [InlineData("1922.168.1.0")]
    [InlineData("257.168.1.0")]
    [InlineData("192.168.1.2000")]
    [InlineData("192.168.1.0/")]
    [InlineData("192.168.1..0")]
    [InlineData("192.168.1.0/111")]
    [InlineData("192.168.1.0/33")]
    [InlineData("192.168.1.0/256")]
    [InlineData("192.168.1.0/ ")]
    [InlineData("192.168.1./4")]
    [InlineData("192.168.1.")]
    [InlineData("192.168..1")]
    [InlineData("192.168..")]
    [InlineData("192...")]
    [InlineData("192..")]
    [InlineData("192.")]
    [InlineData(".192")]
    [InlineData(" 192")]
    public void InvalidFormatsTests(string test)
    {
        IpAddressNetworkV4.TryParse(test, out _).Should().BeFalse();
    }

    [Theory]
    [InlineData("100.120.140.189", "100.120.140.189", "100.120.140.189", 32, 1, 1)]
    [InlineData("100.120.140.189/31", "100.120.140.188", "100.120.140.189", 31, 2, 0)]
    [InlineData("100.120.140.189/30", "100.120.140.188", "100.120.140.191", 30, 4, 2)]
    [InlineData("100.120.140.189/29", "100.120.140.184", "100.120.140.191", 29, 8, 6)]
    [InlineData("100.120.140.189/28", "100.120.140.176", "100.120.140.191", 28, 16, 14)]
    [InlineData("100.120.140.189/27", "100.120.140.160", "100.120.140.191", 27, 32, 30)]
    [InlineData("100.120.140.189/26", "100.120.140.128", "100.120.140.191", 26, 64, 62)]
    [InlineData("100.120.140.189/25", "100.120.140.128", "100.120.140.255", 25, 128, 126)]
    [InlineData("100.120.140.189/24", "100.120.140.0", "100.120.140.255", 24, 256, 254)]
    [InlineData("100.120.140.189/23", "100.120.140.0", "100.120.141.255", 23, 512, 510)]
    [InlineData("100.120.140.189/22", "100.120.140.0", "100.120.143.255", 22, 1024, 1022)]
    [InlineData("100.120.140.189/21", "100.120.136.0", "100.120.143.255", 21, 2048, 2046)]
    [InlineData("100.120.140.189/17", "100.120.128.0", "100.120.255.255", 17, 32768, 32766)]
    [InlineData("100.120.140.189/16", "100.120.0.0", "100.120.255.255", 16, 65536, 65534)]
    [InlineData("100.120.140.189/15", "100.120.0.0", "100.121.255.255", 15, 131072, 131070)]
    [InlineData("100.120.140.189/9", "100.0.0.0", "100.127.255.255", 9, 8388608, 8388606)]
    [InlineData("100.120.140.189/8", "100.0.0.0", "100.255.255.255", 8, 16777216, 16777214)]
    [InlineData("100.120.140.189/7", "100.0.0.0", "101.255.255.255", 7, 33554432, 33554430)]
    [InlineData("100.120.140.189/6", "100.0.0.0", "103.255.255.255", 6, 67108864, 67108862)]
    [InlineData("100.120.140.189/2", "64.0.0.0", "127.255.255.255", 2, 1073741824, 1073741822)]
    [InlineData("100.120.140.189/1", "0.0.0.0", "127.255.255.255", 1, 2147483648, 2147483646)]
    [InlineData("100.120.140.189/0", "0.0.0.0", "255.255.255.255", 0, uint.MaxValue, uint.MaxValue - 2)]
    public void BasicTest(string test, string expectedNetwork, string expectedBroadcast, byte expectedMask, uint expectedSubnetSize, uint expectedSubnetHostsSize)
    {
        IpAddressNetworkV4 net = IpAddressNetworkV4.Parse(test);

        net.NetworkAddress.Should().Be((IpAddressV4)expectedNetwork);
        net.BroadcastAddress.Should().Be((IpAddressV4)expectedBroadcast);
        net.Mask.Should().Be(expectedMask);
        net.SubnetSize.Should().Be(expectedSubnetSize);
        net.SubnetHostsSize.Should().Be(expectedSubnetHostsSize);
    }

    [Theory]
    [InlineData("192.168.10.1", 24, "192.168.10.0", "192.168.10.255")]
    [InlineData("192.168.10.1", 16, "192.168.0.0", "192.168.255.255")]
    [InlineData("192.168.10.1", 9, "192.128.0.0", "192.255.255.255")]
    [InlineData("192.168.10.1", 8, "192.0.0.0", "192.255.255.255")]
    public void OtherTests(string address, byte mask, string start, string end)
    {
        IpAddressNetworkV4 ip = new IpAddressNetworkV4(IPAddress.Parse(address), mask);

        ip.Mask.Should().Be(mask);
        ip.NetworkAddress.Should().Be(IPAddress.Parse(start));
        ip.BroadcastAddress.Should().Be(IPAddress.Parse(end));
    }

    [Theory]
    [InlineData("192.168.10.1/24", "192.168.10.255/32")]
    [InlineData("192.168.12.1/24", "192.168.12.10/32")]
    [InlineData("192.168.12.1/24", "192.168.12.0/32")]
    [InlineData("192.168/16", "192.168.188.10")]
    [InlineData("192.168.12.0/24", "192.168.12.0/25")]
    public void Contains(string networks, string othernets)
    {
        IpAddressNetworkV4 network = (IpAddressNetworkV4)networks;
        IpAddressNetworkV4 othernet = (IpAddressNetworkV4)othernets;

        network.Contains(othernet).Should().BeTrue();
        othernet.IsContainedIn(network).Should().BeTrue();
    }

    [Theory]
    [InlineData("192.168.10.1/24", "192.168.10.255/32")]
    [InlineData("192.168.12.1/24", "192.168.12.10/32")]
    [InlineData("192.168.12.1/24", "192.168.12.0/32")]
    [InlineData("192.168/16", "192.168.188.10")]
    [InlineData("192.168.12.0/24", "192.168.12.0/25")]
    [InlineData("192.168.10.1/24", "192.168.10.255/24")]
    [InlineData("192.168.12.1/24", "192.168.12.10/24")]
    [InlineData("192.168.12.0/24", "192.168.12.0/24")]
    public void ContainsOrEqual(string networks, string othernets)
    {
        IpAddressNetworkV4 network = (IpAddressNetworkV4)networks;
        IpAddressNetworkV4 othernet = (IpAddressNetworkV4)othernets;

        network.ContainsOrEqual(othernet).Should().BeTrue();
        othernet.IsContainedInOrEqual(network).Should().BeTrue();
    }

    [Theory]
    [InlineData("192.168.0.0/16", "192.169.188.10")]
    [InlineData("192.168.1.128/25", "192.168.1.1")]
    [InlineData("192.168.1.128/25", "192.168.1.127")]
    [InlineData("192.168.10.1/24", "192.168.10.1/23")]
    public void NotContains(string networks, string othernets)
    {
        IpAddressNetworkV4 network = (IpAddressNetworkV4)networks;
        IpAddressNetworkV4 othernet = (IpAddressNetworkV4)othernets;

        network.Contains(othernet).Should().BeFalse();
    }

    [Theory]
    [InlineData("192.168.10.1/24", "192.168.10/24")]
    [InlineData("192.168.10.1/32", "192.168.10.1")]
    [InlineData("192.168.10/17", "192.168.0/17")]
    [InlineData("192.168.128/17", "192.168.128/17")]
    [InlineData("192.168.10.1/16", "192.168/16")]
    public void ToPrefixStringTests(string networks, string expected)
    {
        IpAddressNetworkV4 network = (IpAddressNetworkV4)networks;

        network.ToPrefixString().Should().Be(expected);
    }

    [Theory]
    [InlineData("192.168.10.1/24", "192.168.10.0/24")]
    [InlineData("192.168.10.1/32", "192.168.10.1/32")]
    public void ToCidrStringTests(string networks, string expected)
    {
        IpAddressNetworkV4 network = (IpAddressNetworkV4)networks;

        network.ToString().Should().Be(expected);
    }

    [Theory]
    [InlineData("192.168.10.1/24", "192.168.10.0/24")]
    [InlineData("192.168.10.1/32", "192.168.10.1")]
    public void ToStringTests(string networks, string expected)
    {
        IpAddressNetworkV4 network = (IpAddressNetworkV4)networks;
        TestHelpers.TestToStringMethods(network, expected);
    }

    [Theory]
    [InlineData("192.168.10.1", "200.168.10.1")]
    [InlineData("0.0.0.1", "255.255.255.255")]
    [InlineData("1.2.3.4", "4.3.2.1")]
    public void ComparisonTests(string smallers, string largers)
    {
        IpAddressNetworkV4 smaller = (IpAddressNetworkV4)smallers;
        IpAddressNetworkV4 larger = (IpAddressNetworkV4)largers;

        (smaller < larger).Should().BeTrue();
        (smaller <= larger).Should().BeTrue();
        (larger > smaller).Should().BeTrue();
        (larger >= smaller).Should().BeTrue();

        smaller.CompareTo(larger).Should().BeNegative();
        larger.CompareTo(smaller).Should().BePositive();
    }

    [Theory]
    [InlineData("192.168.10.1/24", "192.168.10/24")]
    [InlineData("192.168.10.1/32", "192.168.10.1/32")]
    public void EqualTests(string lefts, string rights)
    {
        IpAddressNetworkV4 left = (IpAddressNetworkV4)lefts;
        IpAddressNetworkV4 right = (IpAddressNetworkV4)rights;

        (left < right).Should().BeFalse();
        (left <= right).Should().BeTrue();
        (left == right).Should().BeTrue();
        (right > left).Should().BeFalse();
        (right >= left).Should().BeTrue();
        (right == left).Should().BeTrue();

        left.CompareTo(right).Should().Be(0);
        right.CompareTo(left).Should().Be(0);
    }


    [Theory]
    [InlineData("192.168.0/23", new[] { "192.168.0/24", "192.168.1/24" })]
    [InlineData("192.168.0/16", new[] { "192.168.0/24", "192.168.255/24" })]
    [InlineData("192.168.96/20", new[] { "192.168.98/24", "192.168.99/24", "192.168.100/24", "192.168.101/24", "192.168.102/24", "192.168.105/24" })]
    public void SupernetTests(string expecteds, string[] nets)
    {
        IpAddressNetworkV4 expected = (IpAddressNetworkV4)expecteds;

        IpAddressNetworkV4 supernet = IpAddressNetworkV4.MakeSupernet(nets.Select(s => (IpAddressNetworkV4)s));

        supernet.Should().Be(expected);
    }

    [Theory]
    [InlineData("192.168.0.0", "192.168.0.255", "192.168.0.0/24")]
    [InlineData("10.0.0.0", "10.0.0.255", "10.0.0.0/24")]
    [InlineData("10.0.0.0", "10.0.255.255", "10.0.0.0/16")]
    [InlineData("10.0.0.0", "10.255.255.255", "10.0.0.0/8")]
    [InlineData("10.0.0.0", "10.127.255.255", "10.0.0.0/9")] //Test that we support uneven bits
    [InlineData("10.0.0.1", "10.255.255.255", "10.0.0.0/8")] //Test that we remove address bits to create a network id address
    public void RangeCtor(string start, string end, string expected)
    {
        IpAddressNetworkV4 ip = new IpAddressNetworkV4(IPAddress.Parse(start), IPAddress.Parse(end));
        ip.Should().Be((IpAddressNetworkV4)expected);
    }

    [Fact]
    public void FullPropertyTest()
    {
        IpAddressNetworkV4 net = IpAddressNetworkV4.Parse("192.168.45.17/22");

        net.Mask.Should().Be(22);
        net.ToString().Should().Be("192.168.44.0/22");
        net.NetworkAddress.Should().Be((IpAddressV4)"192.168.44.0");
        net.BroadcastAddress.Should().Be((IpAddressV4)"192.168.47.255");
        net.NetworkMask.Should().Be((IpAddressV4)"255.255.252.0");
        net.NetworkWildcardMask.Should().Be((IpAddressV4)"0.0.3.255");
        net.SubnetSize.Should().Be(1024);
        net.SubnetHostsSize.Should().Be(1022);
    }

    [Fact]
    public void SplitTest()
    {
        // Check that Split fails fast on mask
        IpAddressNetworkV4 net = (IpAddressNetworkV4)"192.168.1.0/24";
        Assert.Throws<ArgumentOutOfRangeException>(() => net.Split(33));
        Assert.Throws<ArgumentOutOfRangeException>(() => net.Split(9));
        Assert.Throws<ArgumentOutOfRangeException>(() => net.Split(0));

        // Generic split in the lower-end of the IPv4 struct
        net = (IpAddressNetworkV4)"192.168.1.0/24";
        List<IpAddressNetworkV4> splits = net.Split(2).ToList();
        splits.Should().ContainInOrder(new IpAddressNetworkV4[]
        {
            (IpAddressNetworkV4)"192.168.1.0/26",
            (IpAddressNetworkV4)"192.168.1.64/26",
            (IpAddressNetworkV4)"192.168.1.128/26",
            (IpAddressNetworkV4)"192.168.1.192/26"
        });

        // Generic split in the lower-end of the IPv6 struct
        net = (IpAddressNetworkV4)"10.0.0.0/8";
        splits = net.Split(2).ToList();
        splits.Should().ContainInOrder(new IpAddressNetworkV4[]
        {
            (IpAddressNetworkV4)"10.0.0.0/10",
            (IpAddressNetworkV4)"10.64.0.0/10",
            (IpAddressNetworkV4)"10.128.0.0/10",
            (IpAddressNetworkV4)"10.192.0.0/10"
        });

        // Major split from 0.0.0.0/0 to 0.0.0.0/32, to ensure it can work (we will _not_ enumerate it)
        net = (IpAddressNetworkV4)"0.0.0.0/0";
        splits = net.Split(32).Take(5).ToList();
        splits.Should().ContainInOrder(new IpAddressNetworkV4[]
        {
            (IpAddressNetworkV4)"0.0.0.0/32",
            (IpAddressNetworkV4)"0.0.0.1/32",
            (IpAddressNetworkV4)"0.0.0.2/32",
            (IpAddressNetworkV4)"0.0.0.3/32",
            (IpAddressNetworkV4)"0.0.0.4/32"
        });
    }
}

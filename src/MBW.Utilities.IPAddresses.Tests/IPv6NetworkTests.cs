using System.Linq;
using System.Net;
using System.Numerics;
using FluentAssertions;
using Xunit;

namespace MBW.Utilities.IPAddresses.Tests;

public class IPv6NetworkTests
{
    /// <summary>
    /// Special IPv6 transition format, "IPv4-Mapped IPv6 Address"
    /// https://datatracker.ietf.org/doc/html/rfc4291#section-2.5.5.2
    /// </summary>
    [Theory]
    [InlineData("::ffff:0:0/96", "::ffff:192.168.10.1/96", 96)]
    [InlineData("::ffff:255.240.0.0/112", "::ffff:255.240.220.200/112", 112)]
    public void ValidIPv4InV6FormatTests(string expected, string test, byte cidr)
    {
        string expectedWithoutSlash = expected.Split('/').First();

        IpAddressNetworkV6 parsed = IpAddressNetworkV6.Parse(test);
        IPAddress expectedIp = IPAddress.Parse(expectedWithoutSlash);

        parsed.NetworkAddress.Should().Be(expectedIp);
        if (cidr == 128)
            parsed.EndAddress.Should().Be(expectedIp);
        parsed.Mask.Should().Be(cidr);

        // This representation is special, and should still be the IPv4 in IPv6 mapped
        expectedIp.ToString().Should().Be(expectedWithoutSlash);
        parsed.ToString().Should().Be(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData(":")]
    [InlineData(":::")]
    [InlineData("2001:::4")]
    [InlineData("2001:54678:ffff::")]
    [InlineData("2001::54678:ff")]
    [InlineData("2001::/129")]
    [InlineData("2001::/2000")]
    [InlineData("2001::/20000")]
    [InlineData("2001::/ff")]
    [InlineData("2001:::/ff")]
    [InlineData("2001:::/")]
    [InlineData("2001:::/\t")]
    [InlineData("2001:0db8::8a2e:192.168.0.1")] // Badly formatted IPv4 in IPv6
    public void InvalidFormatsTests(string test)
    {
        IpAddressNetworkV6.TryParse(test, out _).Should().BeFalse();
    }

    [Theory]
    [InlineData("2001:db8::8a2e:371:7334", "2001:db8::8a2e:371:7334", "2001:db8::8a2e:371:7334", 128, "1")]
    [InlineData("2001:db8::8a2e:371:7334/120", "2001:db8::8a2e:371:7300", "2001:db8::8a2e:371:73ff", 120, "256")]
    [InlineData("2001:db8::8a2e:371:7334/112", "2001:db8::8a2e:371:0", "2001:db8::8a2e:371:ffff", 112, "65536")]
    [InlineData("2001:db8::8a2e:371:7334/96", "2001:db8::8a2e:0:0", "2001:db8::8a2e:ffff:ffff", 96, "4294967296")]
    [InlineData("2001:db8::8a2e:371:7334/65", "2001:db8::", "2001:db8::7fff:ffff:ffff:ffff", 65, "9223372036854775808")]
    [InlineData("2001:db8::8a2e:371:7334/64", "2001:db8::", "2001:db8::ffff:ffff:ffff:ffff", 64, "18446744073709551616")]
    [InlineData("2001:db8::8a2e:371:7334/63", "2001:db8::", "2001:db8::0001:ffff:ffff:ffff:ffff", 63, "36893488147419103232")]
    [InlineData("2001:db8::8a2e:371:7334/56", "2001:db8::", "2001:db8::00ff:ffff:ffff:ffff:ffff", 56, "4722366482869645213696")]
    [InlineData("2001:db8::8a2e:371:7334/3", "2000::", "3fff:ffff:ffff:ffff:ffff:ffff:ffff:ffff", 3, "42535295865117307932921825928971026432")]
    [InlineData("2001:db8::8a2e:371:7334/1", "::", "7fff:ffff:ffff:ffff:ffff:ffff:ffff:ffff", 1, "170141183460469231731687303715884105728")]
    [InlineData("2001:db8::8a2e:371:7334/0", "::", "ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff", 0, "340282366920938463463374607431768211456")]
    public void BasicTest(string test, string expectedNetwork, string expectedEnd, byte expectedMask, string expectedSubnetSize)
    {
        IpAddressNetworkV6 net = IpAddressNetworkV6.Parse(test);

        net.Mask.Should().Be(expectedMask);
        net.NetworkAddress.Should().Be((IpAddressV6)expectedNetwork);
        net.EndAddress.Should().Be((IpAddressV6)expectedEnd);

        BigInteger bigExpectedSubnetSize = BigInteger.Parse(expectedSubnetSize);
        net.SubnetSize.Should().Be(bigExpectedSubnetSize);
    }

    [Theory]
    [InlineData("fe00::/10", "fe00:4444::7ef0", true)]
    [InlineData("fe00::/10", "fe80:1224::7ef0", false)]
    public void ContainsTest(string network, string test, bool shouldContain)
    {
        IpAddressNetworkV6 net = (IpAddressNetworkV6)network;
        IpAddressV6 ip = (IpAddressV6)test;

        if (shouldContain)
            net.Contains(ip).Should().BeTrue();
        else
            net.Contains(ip).Should().BeFalse();
    }

    [Fact]
    public void FullPropertyTest()
    {
        IpAddressNetworkV6 net = IpAddressNetworkV6.Parse("2001:db8::8a2e:371:7334/96");

        net.Mask.Should().Be(96);
        net.ToString().Should().Be("2001:db8::8a2e:0:0/96");
        net.NetworkAddress.Should().Be((IpAddressV6)"2001:db8::8a2e:0:0");
        net.EndAddress.Should().Be((IpAddressV6)"2001:db8::8a2e:ffff:ffff");
        net.NetworkMask.Should().Be((IpAddressV6)"ffff:ffff:ffff:ffff:ffff:ffff::");
        net.NetworkWildcardMask.Should().Be((IpAddressV6)"::ffff:ffff");
        net.SubnetSize.Should().Be(4294967296);
    }
}
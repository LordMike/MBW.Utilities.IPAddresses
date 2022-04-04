using System.Linq;
using System.Net;
using FluentAssertions;
using Xunit;

namespace MBW.Utilities.IPAddresses.Tests;

public class IPv6Tests
{
    [Theory]
    [InlineData("0000:0000:0000:0000:0000:0000:0000:0000", "::")]
    [InlineData("ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff", "ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff")]
    [InlineData("ffff:ffff:0000:0000:0000:0000:ffff:ffff", "ffff:ffff::ffff:ffff")]
    [InlineData("abcd:ef99:000a:000b:000c:000d:fffe:ffff", "abcd:ef99:a:b:c:d:fffe:ffff")]
    [InlineData("abcd:ef99:000a:0000:000c:000d:fffe:ffff", "abcd:ef99:a::c:d:fffe:ffff")]
    [InlineData("abcd:ef99:000a:0000:0000:000d:fffe:ffff", "abcd:ef99:a::d:fffe:ffff")]
    [InlineData("2001:0db8:0000:0000:0000:8a2e:0370:7334", "2001:0db8::8a2e:0370:7334")] //Ordinary IPv6
    //[InlineData("2001:0db8:0000:0000:0000:8a2e:0370:7334", "[2001:0db8:0000:0000:0000:8a2e:0370:7334]")] //We should support square brackets around the IPv6
    //[InlineData("2001:0db8:0000:0000:0000:8a2e:0370:7334", "2001:0db8::8a2e:0370:7334%3")] //We should support scope
    public void ValidFormatsTests(string expected, string test)
    {
        IpAddressRangeV6 parsed = IpAddressRangeV6.Parse(test);
        IPAddress expectedIp = IPAddress.Parse(expected);

        parsed.Mask.Should().Be(128);
        parsed.Address.Should().Be(expectedIp);
        parsed.EndAddress.Should().Be(expectedIp);
    }

    /// <summary>
    /// Special format used for IPv6 transition, now deprecated
    /// https://datatracker.ietf.org/doc/html/rfc4291#section-2.5.5.1
    /// </summary>
    [Theory(Skip = "Deprecated RFC")]
    [InlineData("::192.168.10.1", "0000:0000:0000:0000:0000:0000:192.168.10.1")]
    [InlineData("::192.168.10.1", "::192.168.10.1")]
    public void ValidIPv4CompatibleIPv6FormatTests(string expected, string test)
    {
        IpAddressRangeV6 parsed = IpAddressRangeV6.Parse(test);
        IPAddress expectedIp = IPAddress.Parse(expected);

        parsed.Mask.Should().Be(128);
        parsed.Address.Should().Be(expectedIp);
        parsed.EndAddress.Should().Be(expectedIp);

        // This representation is special, and should still be the IPv4 in IPv6 mapped
        expectedIp.ToString().Should().Be(expected);
        parsed.ToString().Should().Be(expected);
    }

    /// <summary>
    /// Special IPv6 transition format, "IPv4-Mapped IPv6 Address"
    /// https://datatracker.ietf.org/doc/html/rfc4291#section-2.5.5.2
    /// </summary>
    [Theory]
    [InlineData("::ffff:192.168.10.1", "0000:0000:0000:0000:0000:ffff:192.168.10.1", 128)]
    [InlineData("::ffff:192.168.10.1", "::ffff:192.168.10.1", 128)]
    [InlineData("::ffff:255.0.10.254", "::ffff:255.0.10.254", 128)]
    [InlineData("::ffff:0:0/96", "::ffff:192.168.10.1/96", 96)]
    [InlineData("::ffff:255.240.0.0/112", "::ffff:255.240.220.200/112", 112)]
    public void ValidIPv4InV6FormatTests(string expected, string test, byte cidr)
    {
        var expectedWithoutSlash = expected.Split('/').First();

        IpAddressRangeV6 parsed = IpAddressRangeV6.Parse(test);
        IPAddress expectedIp = IPAddress.Parse(expectedWithoutSlash);

        parsed.Address.Should().Be(expectedIp);
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
        IpAddressRangeV6.TryParse(test, out _).Should().BeFalse();
    }

    [Theory]
    [InlineData("ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff", "ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff", 128)]
    [InlineData("ffff:ffff:ffff:ffff:ffff:ffff:ffff:ff00", "ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff", 120)]
    [InlineData("ffff:ffff:ffff:ffff:ffff:ffff:ffff:0000", "ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff", 112)]
    [InlineData("ffff:ffff:ffff:ffff:ffff:ffff:0000:0000", "ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff", 96)]
    [InlineData("ffff:ffff:ffff:ffff:8000:0000:0000:0000", "ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff", 65)]
    [InlineData("ffff:ffff:ffff:ffff:0000:0000:0000:0000", "ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff", 64)]
    [InlineData("ffff:ffff:ffff:fffe:0000:0000:0000:0000", "ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff", 63)]
    [InlineData("ffff:ffff:ffff:ff00:0000:0000:0000:0000", "ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff", 56)]
    [InlineData("e000:0000:0000:0000:0000:0000:0000:0000", "ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff", 3)]
    [InlineData("8000:0000:0000:0000:0000:0000:0000:0000", "ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff", 1)]
    [InlineData("0000:0000:0000:0000:0000:0000:0000:0000", "ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff", 0)]
    public void BasicTest(string address, string end, byte mask)
    {
        IpAddressRangeV6 ip = new IpAddressRangeV6(IPAddress.Parse(address), mask);

        ip.Mask.Should().Be(mask);
        ip.Address.Should().Be(IPAddress.Parse(address));
        ip.EndAddress.Should().Be(IPAddress.Parse(end));

        IpAddressRangeV6 asCustomParsed = IpAddressRangeV6.Parse(address + "/" + mask);

        asCustomParsed.Should().Be(ip);
    }
}
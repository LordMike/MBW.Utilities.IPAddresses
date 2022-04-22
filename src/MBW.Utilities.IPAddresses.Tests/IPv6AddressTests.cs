using System.Linq;
using FluentAssertions;
using MBW.Utilities.IPAddresses.Tests.SignaturesLib;
using Xunit;

namespace MBW.Utilities.IPAddresses.Tests;

public class IPv6AddressTests
{
    [Theory]
    [InlineData(0x0000_0000_0000_0000, 0x0000_0000_0000_0000, "::")]
    [InlineData(0xffff_ffff_ffff_ffff, 0xffff_ffff_ffff_ffff, "ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff")]
    [InlineData(0xffff_ffff_0000_0000, 0x0000_0000_ffff_ffff, "ffff:ffff::ffff:ffff")]
    [InlineData(0xabcd_ef99_000a_000b, 0x000c_000d_fffe_ffff, "abcd:ef99:a:b:c:d:fffe:ffff")]
    [InlineData(0xabcd_ef99_000a_0000, 0x000c_000d_fffe_ffff, "abcd:ef99:a::c:d:fffe:ffff")]
    [InlineData(0xabcd_ef99_000a_0000, 0x0000_000d_fffe_ffff, "abcd:ef99:a::d:fffe:ffff")]
    [InlineData(0x2001_0db8_0000_0000, 0x0000_8a2e_0370_7334, "2001:0db8::8a2e:0370:7334")] //Ordinary IPv6
    //[InlineData("2001:0db8:0000:0000:0000:8a2e:0370:7334", "[2001:0db8:0000:0000:0000:8a2e:0370:7334]")] //We should support square brackets around the IPv6
    //[InlineData("2001:0db8:0000:0000:0000:8a2e:0370:7334", "2001:0db8::8a2e:0370:7334%3")] //We should support scope
    public void ValidFormatsTests(ulong expectedHigh, ulong expectedLow, string test)
    {
        IpAddressV6 parsed = IpAddressV6.Parse(test);

        parsed.AddressHigh.Should().Be(expectedHigh);
        parsed.AddressLow.Should().Be(expectedLow);
    }

    /// <summary>
    /// Special format used for IPv6 transition, now deprecated
    /// https://datatracker.ietf.org/doc/html/rfc4291#section-2.5.5.1
    /// </summary>
    [Theory(Skip = "Deprecated RFC")]
    [InlineData(0x0000_0000_c0a8_0a01, "0000:0000:0000:0000:0000:0000:192.168.10.1")]
    [InlineData(0x0000_0000_c0a8_0a01, "::192.168.10.1")]
    public void VcalidIPv4CompatibleIPv6FormatTests(ulong expectedLow, string test)
    {
        IpAddressV6 parsed = IpAddressV6.Parse(test);

        parsed.AddressHigh.Should().Be(0);
        parsed.AddressLow.Should().Be(expectedLow);

        // This representation is special, and should still be the IPv4 in IPv6 mapped
        TestHelpers.TestToStringMethods(parsed, "::192.168.10.1");
    }

    /// <summary>
    /// Special IPv6 transition format, "IPv4-Mapped IPv6 Address"
    /// https://datatracker.ietf.org/doc/html/rfc4291#section-2.5.5.2
    /// </summary>
    [Theory]
    [InlineData(0x0000_ffff_c0a8_0a01, "0000:0000:0000:0000:0000:ffff:192.168.10.1")]
    [InlineData(0x0000_ffff_c0a8_0a01, "::ffff:192.168.10.1")]
    [InlineData(0x0000_ffff_ff00_0afe, "::ffff:255.0.10.254")]
    public void ValidIPv4InV6FormatTests(ulong expectedLow, string test)
    {
        IpAddressV6 parsed = IpAddressV6.Parse(test);

        parsed.AddressHigh.Should().Be(0);
        parsed.AddressLow.Should().Be(expectedLow);

        // This representation is special, and should still be the IPv4 in IPv6 mapped
        TestHelpers.TestToStringMethods(parsed, "::ffff:" + test.Split(':').Last());
    }

    [Theory]
    [InlineData("")]
    [InlineData(":")]
    [InlineData(":::")]
    [InlineData("2001:::4")]
    [InlineData("2001:54678:ffff::")]
    [InlineData("2001::54678:ff")]
    [InlineData("2001:0db8::8a2e:192.168.0.1")] // Badly formatted IPv4 in IPv6
    [InlineData("2001::1234:ff/33")] // IPAddressV6 should not support masks
    public void InvalidFormatsTests(string test)
    {
        IpAddressV6.TryParse(test, out _).Should().BeFalse();
    }
}

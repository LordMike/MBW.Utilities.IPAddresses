using System.Net;
using FluentAssertions;
using Xunit;

namespace IPAddresses.Tests
{
    public class IPv6Tests
    {
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

        [Theory]
        [InlineData("ffff:ffff:0000:0000:0000:0000:ffff:ffff", "ffff:ffff::ffff:ffff")]
        [InlineData("0000:0000:0000:0000:0000:0000:192.168.10.1", "::192.168.10.1")]
        [InlineData("ffff:ffff:ffff:ffff:ffff:ffff:192.168.10.1", "ffff:ffff:ffff:ffff:ffff:ffff:192.168.10.1")]
        [InlineData("abcd:ef99:000a:000b:000c:000d:fffe:ffff", "abcd:ef99:a:b:c:d:fffe:ffff")]
        [InlineData("abcd:ef99:000a:0000:000c:000d:fffe:ffff", "abcd:ef99:a::c:d:fffe:ffff")]
        [InlineData("abcd:ef99:000a:0000:0000:000d:fffe:ffff", "abcd:ef99:a::d:fffe:ffff")]
        public void FormatParsingTests(string expected, string test)
        {
            IpAddressRangeV6 parsed = IpAddressRangeV6.Parse(test);
            IPAddress expectedIp = IPAddress.Parse(expected);

            parsed.Mask.Should().Be(128);
            parsed.Address.Should().Be(expectedIp);
            parsed.EndAddress.Should().Be(expectedIp);
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
        public void InvalidFormats(string test)
        {
            IpAddressRangeV6.TryParse(test, out _).Should().BeFalse();
        }
    }
}
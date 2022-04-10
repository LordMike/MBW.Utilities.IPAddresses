using FluentAssertions;
using System.Net;
using Xunit;

namespace MBW.Utilities.IPAddresses.Tests;

public class EndianTests
{
    [Fact]
    public void IPv4Endianness()
    {
        void Test(IpAddressV4 ip) => ip.AddressUint.Should().Be(0xC0A80A40);

        // Tests that IP adresses are parsed to Big-endian order internally
        Test(new IpAddressV4(IPAddress.Parse("192.168.10.64")));
        Test(new IpAddressV4(0xC0A80A40));
        Test(new IpAddressV4(192, 168, 10, 64));
        Test(IpAddressV4.Parse("192.168.10.64"));
    }

    [Fact]
    public void IPv6Endianness()
    {
        void Test(IpAddressV6 ip)
        {
            ip.AddressHigh.Should().Be(0x2606_2800_0220_0001);
            ip.AddressLow.Should().Be(0x0248_1893_25c8_1946);
        }

        // Tests that IP adresses are parsed to Big-endian order internally
        Test(new IpAddressV6(IPAddress.Parse("2606:2800:220:1:248:1893:25c8:1946")));
        Test(new IpAddressV6(0x2606_2800_0220_0001, 0x0248_1893_25c8_1946));
        Test(new IpAddressV6(0x2606, 0x2800, 0x0220, 0x0001, 0x0248, 0x1893, 0x25c8, 0x1946));
        Test(new IpAddressV6(0x26, 0x06, 0x28, 0x00, 0x02, 0x20, 0x00, 0x01, 0x02, 0x48, 0x18, 0x93, 0x25, 0xc8, 0x19, 0x46));
        Test(IpAddressV6.Parse("2606:2800:220:1:248:1893:25c8:1946"));
    }
}

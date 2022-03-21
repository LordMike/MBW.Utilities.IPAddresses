using Xunit;

namespace MBW.Utilities.IPAddresses.Tests;

public class FormatTests
{
    /// <summary>
    /// Showcase supported IPv4 formats
    /// </summary>
    [Theory]
    [InlineData("192.168.1.1/24")]
    [InlineData("1.0.0.1")]
    [InlineData("8.8.8.0/32")]
    [InlineData("192.168/16")]
    [InlineData("10/8")]
    public void IPv4Formats(string address)
    {
        Assert.True(IpAddressRangeV4.TryParse(address, out _));
    }

    /// <summary>
    /// Showcase supported IPv6 formats
    /// </summary>
    [Theory]
    [InlineData("ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff")]
    [InlineData("fe20::1")]
    [InlineData("2a00:1450:4005:800::200e")]
    [InlineData("fe20::1/64")]
    [InlineData("::1/128")]
    public void IPv6Formats(string address)
    {
        Assert.True(IpAddressRangeV6.TryParse(address, out _));
    }
}
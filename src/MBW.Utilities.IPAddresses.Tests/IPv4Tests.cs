using System.Linq;
using System.Net;
using FluentAssertions;
using Xunit;

namespace MBW.Utilities.IPAddresses.Tests
{
    public class IPv4Tests
    {
        [Theory]
        [InlineData("255.255.255.255", "255.255.255.255", 32, 1)]
        [InlineData("255.255.255.254", "255.255.255.255", 31, 2)]
        [InlineData("255.255.255.252", "255.255.255.255", 30, 4)]
        [InlineData("255.255.255.248", "255.255.255.255", 29, 8)]
        [InlineData("255.255.255.240", "255.255.255.255", 28, 16)]
        [InlineData("255.255.255.224", "255.255.255.255", 27, 32)]
        [InlineData("255.255.255.192", "255.255.255.255", 26, 64)]
        [InlineData("255.255.255.128", "255.255.255.255", 25, 128)]
        [InlineData("255.255.255.0", "255.255.255.255", 24, 256)]
        [InlineData("255.255.254.0", "255.255.255.255", 23, 512)]
        [InlineData("255.255.128.0", "255.255.255.255", 17, 32768)]
        [InlineData("255.255.0.0", "255.255.255.255", 16, 65536)]
        [InlineData("255.254.0.0", "255.255.255.255", 15, 131072)]
        [InlineData("255.128.0.0", "255.255.255.255", 9, 8388608)]
        [InlineData("255.0.0.0", "255.255.255.255", 8, 16777216)]
        [InlineData("254.0.0.0", "255.255.255.255", 7, 33554432)]
        [InlineData("192.0.0.0", "255.255.255.255", 2, 1073741824)]
        [InlineData("128.0.0.0", "255.255.255.255", 1, 2147483648)]
        [InlineData("0.0.0.0", "255.255.255.255", 0, uint.MaxValue)]
        public void BasicTest(string address, string end, byte mask, uint expectedSubnetSize)
        {
            IpAddressRangeV4 ip = new IpAddressRangeV4(IPAddress.Parse(address), mask);

            ip.Address.Should().Be(IPAddress.Parse(address));
            ip.EndAddress.Should().Be(IPAddress.Parse(end));
            ip.Mask.Should().Be(mask);
            ip.SubnetSize.Should().Be(expectedSubnetSize);
            ip.SubnetHostsSize.Should().Be(expectedSubnetSize == 1 ? 1 : expectedSubnetSize - 2);

            IpAddressRangeV4 asCustomParsed = IpAddressRangeV4.Parse(address + "/" + mask);

            asCustomParsed.Should().Be(ip);
        }

        [Theory]
        [InlineData("192.168.10.1", 24, "192.168.10.0", "192.168.10.255")]
        [InlineData("192.168.10.1", 16, "192.168.0.0", "192.168.255.255")]
        [InlineData("192.168.10.1", 9, "192.128.0.0", "192.255.255.255")]
        [InlineData("192.168.10.1", 8, "192.0.0.0", "192.255.255.255")]
        public void OtherTests(string address, byte mask, string start, string end)
        {
            IpAddressRangeV4 ip = new IpAddressRangeV4(IPAddress.Parse(address), mask);

            ip.Mask.Should().Be(mask);
            ip.Address.Should().Be(IPAddress.Parse(start));
            ip.EndAddress.Should().Be(IPAddress.Parse(end));
        }

        [Theory]
        [InlineData("192.168.10.1/24", "192.168.10.255/32")]
        [InlineData("192.168.12.1/24", "192.168.12.10/32")]
        [InlineData("192.168.12.1/24", "192.168.12.0/32")]
        [InlineData("192.168/16", "192.168.188.10")]
        [InlineData("192.168.12.0/24", "192.168.12.0/25")]
        public void Contains(string networks, string othernets)
        {
            IpAddressRangeV4 network = networks;
            IpAddressRangeV4 othernet = othernets;

            network.Contains(othernet).Should().BeTrue();
            othernet.IsContainedIn(network).Should().BeTrue();
        }

        [Theory]
        [InlineData("192.168.10.1/24", "192.168.10.255/32")]
        [InlineData("192.168.10.1/24", "192.168.10.1/23")]
        [InlineData("192.168.12.1/24", "192.168.12.10/32")]
        [InlineData("192.168.12.1/24", "192.168.12.0/32")]
        [InlineData("192.168/16", "192.168.188.10")]
        [InlineData("192.168.12.0/24", "192.168.12.0/25")]
        [InlineData("192.168.10.1/24", "192.168.10.255/24")]
        [InlineData("192.168.12.1/24", "192.168.12.10/24")]
        [InlineData("192.168.12.0/24", "192.168.12.0/24")]
        public void ContainsOrEqual(string networks, string othernets)
        {
            IpAddressRangeV4 network = networks;
            IpAddressRangeV4 othernet = othernets;

            network.ContainsOrEqual(othernet).Should().BeTrue();
            othernet.IsContainedInOrEqual(network).Should().BeTrue();
        }

        [Theory]
        [InlineData("192.168.0.0/16", "192.169.188.10")]
        [InlineData("192.168.1.128/25", "192.168.1.1")]
        [InlineData("192.168.1.128/25", "192.168.1.127")]
        public void NotContains(string networks, string othernets)
        {
            IpAddressRangeV4 network = networks;
            IpAddressRangeV4 othernet = othernets;

            network.Contains(othernet).Should().BeFalse();
        }

        [Theory]
        [InlineData("192.168.10.1/24", "192.168.10.0/24")]
        [InlineData("192.168.10.1/32", "192.168.10.1")]
        public void ToStringTests(string networks, string expected)
        {
            IpAddressRangeV4 network = networks;

            network.ToString().Should().Be(expected);
        }

        [Theory]
        [InlineData("192.168.10.1/24", "192.168.10/24")]
        [InlineData("192.168.10.1/32", "192.168.10.1")]
        [InlineData("192.168.10/17", "192.168.0/17")]
        [InlineData("192.168.128/17", "192.168.128/17")]
        [InlineData("192.168.10.1/16", "192.168/16")]
        public void ToPrefixStringTests(string networks, string expected)
        {
            IpAddressRangeV4 network = networks;

            network.ToPrefixString().Should().Be(expected);
        }

        [Theory]
        [InlineData("192.168.10.1/24", "192.168.10.0/24")]
        [InlineData("192.168.10.1/32", "192.168.10.1/32")]
        public void ToCidrStringTests(string networks, string expected)
        {
            IpAddressRangeV4 network = networks;

            network.ToString(true).Should().Be(expected);
        }

        [Theory]
        [InlineData("192.168.10.1", new byte[] { 192, 168, 10, 1 })]
        [InlineData("1.2.3.4", new byte[] { 1, 2, 3, 4 })]
        [InlineData("4.3.2.1", new byte[] { 4, 3, 2, 1 })]
        public void AddressToBytesTests(string networks, byte[] expected)
        {
            IpAddressRangeV4 network = networks;

            network.AddressToBytes().Should().Equal(expected);

            byte[] res = new byte[4];
            network.AddressToBytes(res);

            res.Should().Equal(expected);
        }

        [Theory]
        [InlineData("192.168.10.1", "200.168.10.1")]
        [InlineData("0.0.0.1", "255.255.255.255")]
        [InlineData("1.2.3.4", "4.3.2.1")]
        public void ComparisonTests(string smallers, string largers)
        {
            IpAddressRangeV4 smaller = smallers;
            IpAddressRangeV4 larger = largers;

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
            IpAddressRangeV4 left = lefts;
            IpAddressRangeV4 right = rights;

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
            IpAddressRangeV4 expected = expecteds;

            IpAddressRangeV4 supernet = IpAddressRangeV4.MakeSupernet(nets.Select(s => (IpAddressRangeV4)s));

            supernet.Should().Be(expected);
        }

        [Theory]
        [InlineData("192.168.10.1", "192.168.10.1", 32)]
        [InlineData("192.120.12.99/24", "192.120.12.0", 24)]
        [InlineData("192.120.12/24", "192.120.12.0", 24)]
        [InlineData("192.120/24", "192.120.0.0", 24)]
        [InlineData("192/24", "192.0.0.0", 24)]
        [InlineData("255.255.255.255/20", "255.255.240.0", 20)]
        public void ParserTests(string input, string expectedIp, byte expectedMask)
        {
            IpAddressRangeV4 ip = IpAddressRangeV4.Parse(input);

            ip.Mask.Should().Be(expectedMask);
            ip.Address.Should().Be(IPAddress.Parse(expectedIp));
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
        public void InvalidFormats(string test)
        {
            IpAddressRangeV4.TryParse(test, out _).Should().BeFalse();
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
            IpAddressRangeV4 ip = new IpAddressRangeV4(IPAddress.Parse(start), IPAddress.Parse(end));
            ip.Should().Be(expected);
        }
    }
}

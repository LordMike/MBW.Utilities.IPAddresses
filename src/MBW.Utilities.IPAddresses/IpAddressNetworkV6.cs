using System.Numerics;

namespace MBW.Utilities.IPAddresses;

/// <summary>
/// Represents an IPv6 network, consisting of an address and a subnet mask.
/// This is typically represented as a CIDR notation IP address like '2001:aaff::8873:fee0/128'
/// </summary>
public partial struct IpAddressNetworkV6
{
    private readonly IpAddressV6 _networkAddress;
    private readonly byte _mask;

    public byte Mask => _mask;

    public IpAddressV6 NetworkAddress => _networkAddress;
    public IpAddressV6 EndAddress => _networkAddress | NetworkWildcardMask;
    public IpAddressV6 NetworkMask
    {
        get
        {
            return _mask switch
            {
                0 => IpAddressV6.Min,
                128 => IpAddressV6.Max,
                64 => new IpAddressV6(ulong.MaxValue, ulong.MinValue),
                > 64 => new IpAddressV6(ulong.MaxValue, ulong.MaxValue << (128 - _mask)),
                < 64 => new IpAddressV6(ulong.MaxValue << (64 - _mask), ulong.MinValue)
                //_ => throw new InvalidOperationException("Expected mask in the 0..128 range")
            };
        }
    }
    public IpAddressV6 NetworkWildcardMask => new IpAddressV6(~NetworkMask.AddressHigh, ~NetworkMask.AddressLow);

    public BigInteger SubnetSize
    {
        get
        {
            return BigInteger.One << (128 - _mask);
        }
    }
}
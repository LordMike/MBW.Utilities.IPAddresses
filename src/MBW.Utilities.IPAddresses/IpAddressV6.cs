using Dirichlet.Numerics;

namespace MBW.Utilities.IPAddresses;

/// <summary>
/// Represents an IPv6 address. This is typically represented as '2001:db8::8873:fee0'
/// </summary>
/// <remarks>Internally, network addresses are stored as two 64 bit integers, in big-endian order</remarks>
public partial struct IpAddressV6
{
    public static IpAddressV6 Min { get; } = new IpAddressV6(0, 0);
    public static IpAddressV6 Max { get; } = new IpAddressV6(ulong.MaxValue, ulong.MaxValue);

    private readonly UInt128 _address;
    internal UInt128 Address => _address;

    public ulong AddressHigh => _address.S0;
    public ulong AddressLow => _address.S1;
}
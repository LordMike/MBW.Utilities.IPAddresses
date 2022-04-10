namespace MBW.Utilities.IPAddresses;

/// <summary>
/// Represents an IPv6 address. This is typically represented as '2001:db8::8873:fee0'
/// </summary>
/// <remarks>Internally, network addresses are stored as two 64 bit integers, in big-endian order</remarks>
public partial struct IpAddressV6
{
    public static IpAddressV6 Min { get; } = new IpAddressV6(0, 0);
    public static IpAddressV6 Max { get; } = new IpAddressV6(ulong.MaxValue, ulong.MaxValue);

    private readonly ulong _addressHigh;
    private readonly ulong _addressLow;

    public ulong AddressHigh => _addressHigh;
    public ulong AddressLow => _addressLow;
}
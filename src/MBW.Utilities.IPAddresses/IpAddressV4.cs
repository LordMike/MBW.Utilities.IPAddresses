namespace MBW.Utilities.IPAddresses;

/// <summary>
/// Represents an IPv4 address, example: '198.51.100.44'
/// </summary>
/// <remarks>Internally, network addresses are stored as 32 bit integers, in big-endian order</remarks>
public partial struct IpAddressV4
{
    public static IpAddressV4 Min { get; } = new IpAddressV4(0);
    public static IpAddressV4 Max { get; } = new IpAddressV4(uint.MaxValue);

    private readonly uint _address;

    /// <summary>
    /// Gets the internal representation of this IPv4, in Big-endian order.
    /// </summary>
    public uint AddressUint => _address;
}
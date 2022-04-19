namespace MBW.Utilities.IPAddresses;

/// <summary>
/// Represents an IPv4 network, consisting of an address and a subnet mask.
/// This is typically represented as a CIDR notation IP address like '244.112.84.2/31'
/// </summary>
public partial struct IpAddressNetworkV4
{
    private readonly byte _mask;
    private readonly IpAddressV4 _networkAddress;

    public byte Mask => _mask;
    public IpAddressV4 NetworkAddress => _networkAddress;
    public IpAddressV4 BroadcastAddress => _networkAddress | NetworkWildcardMask;
    public IpAddressV4 NetworkMask => _mask == 0 ? IpAddressV4.Min : new IpAddressV4(uint.MaxValue << (32 - _mask));
    public IpAddressV4 NetworkWildcardMask => new IpAddressV4(~NetworkMask.Address);

    /// <summary>
    /// Number of addresses in this network, this includes the normally unusuable Network address and broadcast address.
    /// As an example, 192.168.1.0/24 will give you 256 addresses.
    /// </summary>
    public uint SubnetSize
    {
        get
        {
            if (_mask == 0)
                return uint.MaxValue;

            if (_mask == 32)
                return 1;

            return (uint)(1 << (32 - _mask));
        }
    }

    /// <summary>
    /// Number of host addresses in this network, this excludes the network address and the broadcast address.
    /// As an example, 192.168.1.0/24 will give you 254 addresses (1 through 254).
    /// </summary>
    /// <remarks>The smallest possible network with 1 address will return 1, even though there is no space for broadcast/network address</remarks>
    public uint SubnetHostsSize
    {
        get
        {
            if (_mask == 32)
                return 1;

            return SubnetSize - 2;
        }
    }
}
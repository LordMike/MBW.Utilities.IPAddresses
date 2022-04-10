using System;
using System.Net;
using MBW.Utilities.IPAddresses.Helpers;

namespace MBW.Utilities.IPAddresses;

/// <summary>
/// Represents an IPv4 network, consisting of an address and a subnet mask.
/// This is typically represented as a CIDR notation IP address like '244.112.84.2/31'
/// </summary>
public partial struct IpAddressNetworkV4
{
    public IpAddressNetworkV4(IpAddressV4 address, byte mask)
    {
        if (mask > 32)
            throw new ArgumentException("Mask cannot be greater than 32 bits", nameof(mask));

        _mask = mask;
        if (mask == 0)
            _networkAddress = IpAddressV4.Min;
        else
            _networkAddress = new IpAddressV4(address.AddressUint & (uint.MaxValue << (32 - _mask)));
    }

    public IpAddressNetworkV4(uint address, byte mask)
    {
        if (mask > 32)
            throw new ArgumentException("Mask cannot be greater than 32 bits", nameof(mask));

        _mask = mask;
        if (mask == 0)
            _networkAddress = IpAddressV4.Min;
        else
            _networkAddress = new IpAddressV4(address & (uint.MaxValue << (32 - _mask)));
    }

    /// <summary>
    /// Creates an instance of <see cref="IpAddressNetworkV4"/> from a network and broadcast address.
    /// </summary>
    /// <param name="networkAddress">The network address. Note that it must be a network start address such as 10.0.0.0</param>
    /// <param name="broadcastAddress">The broadcast address. Note that it must be a broadcast address such as 10.255.255.255</param>
#pragma warning disable CS0618
    public IpAddressNetworkV4(IPAddress networkAddress, IPAddress broadcastAddress) : this(BitUtilities.Reverse((uint)networkAddress.Address), BitUtilities.Reverse((uint)broadcastAddress.Address))
#pragma warning restore CS0618
    {
    }

    /// <summary>
    /// Creates an instance of <see cref="IpAddressNetworkV4"/> from a network and broadcast address.
    /// </summary>
    /// <param name="networkAddress">The network address. Note that it must be a network start address such as 10.0.0.0</param>
    /// <param name="broadcastAddress">The broadcast address. Note that it must be a broadcast address such as 10.255.255.255</param>
    public IpAddressNetworkV4(uint networkAddress, uint broadcastAddress)
    {
        uint ip = networkAddress ^ broadcastAddress;

        //count the number of 0 bits set
        byte mask = 32;
        while (ip != 0)
        {
            ip >>= 1;
            mask--;
        }

        _mask = mask;

        if (mask == 0)
            _networkAddress = IpAddressV4.Min;
        else
            _networkAddress = new IpAddressV4(networkAddress & (uint.MaxValue << (32 - _mask)));
    }
}
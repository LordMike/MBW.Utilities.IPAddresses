using System;
using System.Net;
using System.Net.Sockets;
using MBW.Utilities.IPAddresses.Helpers;

namespace MBW.Utilities.IPAddresses;

/// <summary>
/// Represents an IPv4 network, consisting of an address and a subnet mask.
/// This is typically represented as a CIDR notation IP address like '244.112.84.2/31'
/// </summary>
public partial struct IpAddressNetworkV4
{
    private static readonly IPAddress Max = IPAddress.Parse("255.255.255.255");

    private readonly byte _mask;
    private readonly uint _address;

    public byte Mask => _mask;

    public uint AddressUint => _address;

    public uint NetworkMask => uint.MaxValue << (32 - _mask);

    public IPAddress Address => new IPAddress(BitUtilities.Reverse(_address));

    public IPAddress EndAddress
    {
        get
        {
            if (_mask == 0)
                return Max;

            if (_mask == 32)
                return Address;

            return ConversionUtilities.ToIp(EndAddressUint);
        }
    }

    public uint EndAddressUint => _address | ~NetworkMask;

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

    public uint SubnetHostsSize
    {
        get
        {
            if (_mask == 0)
                return uint.MaxValue - 2;

            if (_mask == 32)
                return 1;

            return SubnetSize - 2;
        }
    }

#pragma warning disable CS0618
    public IpAddressNetworkV4(IPAddress address, byte mask = 32) : this(BitUtilities.Reverse((uint)address.Address), mask)
#pragma warning restore CS0618
    {
        if (address.AddressFamily != AddressFamily.InterNetwork)
            throw new ArgumentException();
    }

    public IpAddressNetworkV4(uint address, byte mask = 32)
    {
        if (mask > 32)
            throw new ArgumentException("Mask cannot be greater than 32 bits", nameof(mask));

        _mask = mask;
        if (mask == 0)
            _address = 0;
        else
            _address = address & (uint.MaxValue << (32 - _mask));
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
            _address = 0;
        else
            _address = networkAddress & (uint.MaxValue << (32 - _mask));
    }
}
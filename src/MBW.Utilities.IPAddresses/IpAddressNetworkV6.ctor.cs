using System;
using System.Net;
using System.Net.Sockets;
using MBW.Utilities.IPAddresses.Helpers;

namespace MBW.Utilities.IPAddresses;

/// <summary>
/// Represents an IPv6 network, consisting of an address and a subnet mask.
/// This is typically represented as a CIDR notation IP address like '2001:aaff::8873:fee0/128'
/// </summary>
public partial struct IpAddressNetworkV6
{
    public IpAddressNetworkV6(IPAddress address, byte mask = 128)
        : this(address.GetAddressBytes(), mask)
    {
        if (address.AddressFamily != AddressFamily.InterNetworkV6)
            throw new ArgumentException();
    }

    private IpAddressNetworkV6(byte[] address, byte mask = 128)
        : this(BitUtilities.Reverse(BitConverter.ToUInt64(address, 0)), BitUtilities.Reverse(BitConverter.ToUInt64(address, 8)), mask)
    {
        if (address.Length != 16)
            throw new ArgumentException();
    }

    public IpAddressNetworkV6(ulong addressHigh, ulong addressLow, byte mask = 128)
    {

        _mask = mask;
        if (mask == 0)
        {
            addressHigh = addressLow = 0;
        }
        else if (mask == 128)
        {
        }
        else if (mask > 64)
        {
            // Keep the high bits, truncate the lows
            addressLow &= ulong.MaxValue << (64 - (_mask - 64));
        }
        else if (mask == 64)
        {
            addressLow = 0;
        }
        else
        {
            // Truncate the high bits, remove the lows
            addressHigh &= ulong.MaxValue << (64 - _mask);
            addressLow = 0;
        }

        _networkAddress = new IpAddressV6(addressHigh, addressLow);
    }
}
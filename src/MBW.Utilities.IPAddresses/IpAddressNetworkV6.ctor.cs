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
    private static IpAddressV6[] _networkMasks;

    static IpAddressNetworkV6()
    {
        _networkMasks = new IpAddressV6[129];
        _networkMasks[0] = IpAddressV6.Min;
        _networkMasks[64] = new IpAddressV6(ulong.MaxValue, ulong.MinValue);
        _networkMasks[128] = IpAddressV6.Max;

        for (int i = 1; i < 64; i++)
            _networkMasks[i] = new IpAddressV6(ulong.MaxValue << (64 - i), ulong.MinValue);
        for (int i = 65; i < 128; i++)
            _networkMasks[i] = new IpAddressV6(ulong.MaxValue, ulong.MaxValue << (128 - i));
    }

    public IpAddressNetworkV6(IPAddress address, byte mask)
        : this(address.GetAddressBytes(), mask)
    {
        if (address.AddressFamily != AddressFamily.InterNetworkV6)
            throw new ArgumentException();
    }

    public IpAddressNetworkV6(IpAddressV6 address, byte mask)
        : this(address.AddressHigh, address.AddressLow, mask)
    {
    }

    private IpAddressNetworkV6(byte[] address, byte mask)
        : this(BitUtilities.Reverse(BitConverter.ToUInt64(address, 0)), BitUtilities.Reverse(BitConverter.ToUInt64(address, 8)), mask)
    {
        if (address.Length != 16)
            throw new ArgumentException();
    }

    public IpAddressNetworkV6(ulong addressHigh, ulong addressLow, byte mask)
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
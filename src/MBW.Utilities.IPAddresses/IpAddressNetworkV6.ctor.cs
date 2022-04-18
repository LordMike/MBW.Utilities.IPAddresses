using System;
using System.Net;
using System.Net.Sockets;
using Dirichlet.Numerics;
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
    {
        if (address.AddressFamily != AddressFamily.InterNetworkV6)
            throw new ArgumentException();

        byte[] bytes = address.GetAddressBytes();
        ulong high = BitUtilities.Reverse(BitConverter.ToUInt64(bytes, 0));
        ulong low = BitUtilities.Reverse(BitConverter.ToUInt64(bytes, 8));

        _networkAddress = new IpAddressV6(high, low);
        _mask = mask;

        TruncateNetworkAddress(ref _networkAddress);
    }

    public IpAddressNetworkV6(IpAddressV6 address, byte mask)
    {
        _networkAddress = address;
        _mask = mask;

        TruncateNetworkAddress(ref _networkAddress);
    }

    public IpAddressNetworkV6(ulong addressHigh, ulong addressLow, byte mask)
    {
        _networkAddress = new IpAddressV6(addressHigh, addressLow);
        _mask = mask;

        TruncateNetworkAddress(ref _networkAddress);
    }

    private void TruncateNetworkAddress(ref IpAddressV6 address)
    {
        IpAddressV6 networkMask = _networkMasks[_mask];
        address &= networkMask;
    }
}
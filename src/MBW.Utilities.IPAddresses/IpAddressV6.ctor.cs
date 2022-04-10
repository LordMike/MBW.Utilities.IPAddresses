using System;
using System.Net;
using System.Net.Sockets;
using MBW.Utilities.IPAddresses.Helpers;

namespace MBW.Utilities.IPAddresses;

/// <summary>
/// Represents an IPv6 address. This is typically represented as '2001:db8::8873:fee0'
/// </summary>
public partial struct IpAddressV6
{
    public IpAddressV6(IPAddress address)
        : this(address.GetAddressBytes())
    {
        if (address.AddressFamily != AddressFamily.InterNetworkV6)
            throw new ArgumentException();
    }

    private IpAddressV6(byte[] address)
        : this(BitUtilities.Reverse(BitConverter.ToUInt64(address, 0)), BitUtilities.Reverse(BitConverter.ToUInt64(address, 8)))
    {
        if (address.Length != 16)
            throw new ArgumentException();
    }

    public IpAddressV6(ulong addressHigh, ulong addressLow)
    {
        _addressLow = addressLow;
        _addressHigh = addressHigh;
    }

    public IpAddressV6(ushort tuple0, ushort tuple1, ushort tuple2, ushort tuple3, ushort tuple4, ushort tuple5, ushort tuple6, ushort tuple7)
    {
        _addressLow = default;
        _addressHigh = default;

        BitUtilities.SetTuplet(ref _addressLow, ref _addressHigh, 0, tuple0);
        BitUtilities.SetTuplet(ref _addressLow, ref _addressHigh, 1, tuple1);
        BitUtilities.SetTuplet(ref _addressLow, ref _addressHigh, 2, tuple2);
        BitUtilities.SetTuplet(ref _addressLow, ref _addressHigh, 3, tuple3);
        BitUtilities.SetTuplet(ref _addressLow, ref _addressHigh, 4, tuple4);
        BitUtilities.SetTuplet(ref _addressLow, ref _addressHigh, 5, tuple5);
        BitUtilities.SetTuplet(ref _addressLow, ref _addressHigh, 6, tuple6);
        BitUtilities.SetTuplet(ref _addressLow, ref _addressHigh, 7, tuple7);
    }

    public IpAddressV6(byte octet0, byte octet1, byte octet2, byte octet3, byte octet4, byte octet5, byte octet6, byte octet7, byte octet8, byte octet9, byte octet10, byte octet11, byte octet12, byte octet13, byte octet14, byte octet15)
    {
        _addressLow = default;
        _addressHigh = default;

        BitUtilities.SetByte(ref _addressLow, ref _addressHigh, 0, octet0);
        BitUtilities.SetByte(ref _addressLow, ref _addressHigh, 1, octet1);
        BitUtilities.SetByte(ref _addressLow, ref _addressHigh, 2, octet2);
        BitUtilities.SetByte(ref _addressLow, ref _addressHigh, 3, octet3);
        BitUtilities.SetByte(ref _addressLow, ref _addressHigh, 4, octet4);
        BitUtilities.SetByte(ref _addressLow, ref _addressHigh, 5, octet5);
        BitUtilities.SetByte(ref _addressLow, ref _addressHigh, 6, octet6);
        BitUtilities.SetByte(ref _addressLow, ref _addressHigh, 7, octet7);
        BitUtilities.SetByte(ref _addressLow, ref _addressHigh, 8, octet8);
        BitUtilities.SetByte(ref _addressLow, ref _addressHigh, 9, octet9);
        BitUtilities.SetByte(ref _addressLow, ref _addressHigh, 10, octet10);
        BitUtilities.SetByte(ref _addressLow, ref _addressHigh, 11, octet11);
        BitUtilities.SetByte(ref _addressLow, ref _addressHigh, 12, octet12);
        BitUtilities.SetByte(ref _addressLow, ref _addressHigh, 13, octet13);
        BitUtilities.SetByte(ref _addressLow, ref _addressHigh, 14, octet14);
        BitUtilities.SetByte(ref _addressLow, ref _addressHigh, 15, octet15);
    }
}
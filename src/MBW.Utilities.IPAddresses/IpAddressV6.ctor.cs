using System;
using System.Net;
using System.Net.Sockets;
using Dirichlet.Numerics;
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
        UInt128.Create(out _address, addressHigh, addressLow);
    }

    public IpAddressV6(ushort tuple0, ushort tuple1, ushort tuple2, ushort tuple3, ushort tuple4, ushort tuple5, ushort tuple6, ushort tuple7)
    {
        uint u0 = (uint)(tuple0 << 16) | tuple1;
        uint u1 = (uint)(tuple2 << 16) | tuple3;
        uint u2 = (uint)(tuple4 << 16) | tuple5;
        uint u3 = (uint)(tuple6 << 16) | tuple7;

        UInt128.Create(out _address, u0, u1, u2, u3);
    }

    public IpAddressV6(byte octet0, byte octet1, byte octet2, byte octet3, byte octet4, byte octet5, byte octet6, byte octet7, byte octet8, byte octet9, byte octet10, byte octet11, byte octet12, byte octet13, byte octet14, byte octet15)
    {
        uint u0 = (uint)(octet0 << 24) | (uint)(octet1 << 16) | (uint)(octet2 << 8) | octet3;
        uint u1 = (uint)(octet4 << 24) | (uint)(octet5 << 16) | (uint)(octet6 << 8) | octet7;
        uint u2 = (uint)(octet8 << 24) | (uint)(octet9 << 16) | (uint)(octet10 << 8) | octet11;
        uint u3 = (uint)(octet12 << 24) | (uint)(octet13 << 16) | (uint)(octet14 << 8) | octet15;

        UInt128.Create(out _address, u0, u1, u2, u3);
    }

    internal IpAddressV6(UInt128 address)
    {
        _address = address;
    }
}
using System;
using System.Net;
using System.Net.Sockets;
using MBW.Utilities.IPAddresses.Helpers;

namespace MBW.Utilities.IPAddresses;

/// <summary>
/// Represents an IPv4 address, example: '198.51.100.44'
/// </summary>
public partial struct IpAddressV4
{
#pragma warning disable CS0618
    public IpAddressV4(IPAddress address) : this(BitUtilities.Reverse((uint)address.Address))
#pragma warning restore CS0618
    {
        if (address.AddressFamily != AddressFamily.InterNetwork)
            throw new ArgumentException();
    }

    /// <summary>
    /// Constructs an <see cref="IpAddressV4"/> from this integer. The provided integer must be in Big-endian order
    /// </summary>
    public IpAddressV4(uint address)
    {
        _address = address;
    }

    /// <summary>
    /// Constructs an <see cref="IpAddressV4"/> from these bytes. The provided bytes must be in Big-endian order, meaning <paramref name="octet1"/> is the most-significant byte of the four bytes. Example: new IpAddressV4(192, 168, 1, 0) == "192.168.1.0"
    /// </summary>
    public IpAddressV4(byte octet1, byte octet2, byte octet3, byte octet4)
    {
        _address = (uint)(octet1 << 24);
        _address |= (uint)(octet2 << 16);
        _address |= (uint)(octet3 << 8);
        _address |= octet4;
    }
}
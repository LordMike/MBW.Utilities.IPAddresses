using System;
using System.Net;

namespace MBW.Utilities.IPAddresses;

public partial struct IpAddressNetworkV4
{
    public static explicit operator IpAddressNetworkV4(string value) => Parse(value);
    public static explicit operator IpAddressNetworkV4(ReadOnlySpan<char> value) => Parse(value);

    public static implicit operator IpAddressNetworkV4(IpAddressV4 value) => new IpAddressNetworkV4(value, 32);
    public static implicit operator IpAddressNetworkV4(IPAddress value) => new IpAddressNetworkV4(value, 32);
    public static explicit operator IPAddress(IpAddressNetworkV4 value) => (IPAddress)value.NetworkAddress;
}
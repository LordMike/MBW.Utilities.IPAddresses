using System;
using System.Net;

namespace MBW.Utilities.IPAddresses;

public partial struct IpAddressNetworkV6
{
    public static explicit operator IpAddressNetworkV6(string value) => Parse(value);
    public static explicit operator IpAddressNetworkV6(ReadOnlySpan<char> value) => Parse(value);

    public static implicit operator IpAddressNetworkV6(IpAddressV6 value) => new IpAddressNetworkV6(value, 128);
    public static implicit operator IpAddressNetworkV6(IPAddress value) => new IpAddressNetworkV6(value, 128);
    public static explicit operator IPAddress(IpAddressNetworkV6 value) => (IPAddress)value.NetworkAddress;
}
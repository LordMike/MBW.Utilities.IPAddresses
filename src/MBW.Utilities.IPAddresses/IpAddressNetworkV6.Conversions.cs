using System;
using System.Net;

namespace MBW.Utilities.IPAddresses;

public partial struct IpAddressNetworkV6
{
    public static explicit operator IpAddressNetworkV6(string value)
    {
        return Parse(value);
    }

    public static explicit operator IpAddressNetworkV6(ReadOnlySpan<char> value)
    {
        return Parse(value);
    }

    public static implicit operator IpAddressNetworkV6(IpAddressV6 value)
    {
        throw new NotImplementedException();
    }

    public static implicit operator IpAddressNetworkV6(IPAddress value)
    {
        return new IpAddressNetworkV6(value);
    }

    public static explicit operator IPAddress(IpAddressNetworkV6 value)
    {
        return (IPAddress)value.NetworkAddress;
    }
}
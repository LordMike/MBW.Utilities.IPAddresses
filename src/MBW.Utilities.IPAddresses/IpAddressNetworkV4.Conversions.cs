using System;
using System.Net;

namespace MBW.Utilities.IPAddresses;

public partial struct IpAddressNetworkV4
{
    public static explicit operator IpAddressNetworkV4(string value)
    {
        return Parse(value);
    }

    public static explicit operator IpAddressNetworkV4(ReadOnlySpan<char> value)
    {
        return Parse(value);
    }

    public static implicit operator IpAddressNetworkV4(IpAddressV4 value)
    {
        throw new NotImplementedException();
    }

    public static implicit operator IpAddressNetworkV4(IPAddress value)
    {
        return new IpAddressNetworkV4(value, 32);
    }

    public static explicit operator IPAddress(IpAddressNetworkV4 value)
    {
        return (IPAddress)value.NetworkAddress;
    }
}
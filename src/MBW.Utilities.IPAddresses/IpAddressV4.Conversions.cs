using System;
using System.Net;
using MBW.Utilities.IPAddresses.Helpers;

namespace MBW.Utilities.IPAddresses;

public partial struct IpAddressV4
{
    public static explicit operator IpAddressV4(string value)
    {
        return Parse(value);
    }

    public static explicit operator IpAddressV4(ReadOnlySpan<char> value)
    {
        return Parse(value);
    }

    public static implicit operator IpAddressV4(IPAddress value)
    {
        return new IpAddressV4(value);
    }

    public static explicit operator IPAddress(IpAddressV4 value)
    {
        return new IPAddress(BitUtilities.Reverse(value.Address));
    }
}
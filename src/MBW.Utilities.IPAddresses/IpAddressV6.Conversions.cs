using System;
using System.Net;
using MBW.Utilities.IPAddresses.Helpers;

namespace MBW.Utilities.IPAddresses;

public partial struct IpAddressV6
{
    public static explicit operator IpAddressV6(string value)
    {
        return Parse(value);
    }

    public static explicit operator IpAddressV6(ReadOnlySpan<char> value)
    {
        return Parse(value);
    }

    public static implicit operator IpAddressV6(IPAddress value)
    {
        return new IpAddressV6(value);
    }

    public static explicit operator IPAddress(IpAddressV6 value)
    {
        return ConversionUtilities.ToIp(value.AddressHigh, value.AddressLow);
    }
}
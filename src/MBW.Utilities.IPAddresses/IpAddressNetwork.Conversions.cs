using System;
using System.Net;

namespace MBW.Utilities.IPAddresses;

public partial class IpAddressNetwork
{
    public static IpAddressNetwork Parse(ReadOnlySpan<char> input)
    {
        if (!TryParse(input, out IpAddressNetwork parsed))
            throw new ArgumentException("Input was not a valid ip range", nameof(input));

        return parsed;
    }

    public static bool TryParse(ReadOnlySpan<char> input, out IpAddressNetwork range)
    {
        if (IpAddressNetworkV4.TryParse(input, out IpAddressNetworkV4 v4))
        {
            range = new IpAddressNetwork(v4, null);
            return true;
        }

        if (IpAddressNetworkV6.TryParse(input, out IpAddressNetworkV6 v6))
        {
            range = new IpAddressNetwork(null, v6);
            return true;
        }

        range = null;
        return false;
    }

    [Obsolete]
    public static explicit operator IpAddressNetwork(string value)
    {
        return Parse(value);
    }

    public static explicit operator IpAddressNetwork(ReadOnlySpan<char> value)
    {
        return Parse(value);
    }

    public static implicit operator IpAddressNetwork(IPAddress value)
    {
        return new IpAddressNetwork(value);
    }

    public static implicit operator IpAddressNetwork(IpAddressNetworkV4 value)
    {
        return new IpAddressNetwork(value);
    }

    public static implicit operator IpAddressNetwork(IpAddressNetworkV6 value)
    {
        return new IpAddressNetwork(value);
    }

    public static explicit operator IPAddress(IpAddressNetwork value)
    {
        return value.Address;
    }
}
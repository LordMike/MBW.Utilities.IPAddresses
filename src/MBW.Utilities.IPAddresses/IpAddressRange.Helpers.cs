using System;

namespace MBW.Utilities.IPAddresses;

public partial class IpAddressRange
{
    private IpAddressRangeV4 GetV4()
    {
        if (Type != IpAddressRangeType.IPv4)
            throw new InvalidOperationException("This IpAddressRange does not represent an IPv4 range");

        return _v4!.Value;
    }

    private IpAddressRangeV6 GetV6()
    {
        if (Type != IpAddressRangeType.IPv6)
            throw new InvalidOperationException("This IpAddressRange does not represent an IPv6 range");

        return _v6!.Value;
    }
}
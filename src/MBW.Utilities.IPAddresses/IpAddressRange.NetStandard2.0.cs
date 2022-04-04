using System;

namespace MBW.Utilities.IPAddresses;

public partial class IpAddressRange
{
    [Obsolete("Will be removed post NetStandard2.0")]
    public static IpAddressRange Parse(string input)
    {
        return Parse(input.AsSpan());
    }

    [Obsolete("Will be removed post NetStandard2.0")]
    public static bool TryParse(string input, out IpAddressRange range)
    {
        return TryParse(input.AsSpan(), out range);
    }
}
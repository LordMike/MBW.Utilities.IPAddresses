using System;

namespace MBW.Utilities.IPAddresses;

public partial class IpAddressNetwork
{
    [Obsolete("Will be removed post NetStandard2.0")]
    public static IpAddressNetwork Parse(string input)
    {
        return Parse(input.AsSpan());
    }

    [Obsolete("Will be removed post NetStandard2.0")]
    public static bool TryParse(string input, out IpAddressNetwork range)
    {
        return TryParse(input.AsSpan(), out range);
    }
}
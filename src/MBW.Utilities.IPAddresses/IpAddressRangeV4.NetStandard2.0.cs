using System;

namespace MBW.Utilities.IPAddresses;

public partial struct IpAddressRangeV4
{
    [Obsolete("Will be removed post NetStandard2.0")]
    public static IpAddressRangeV4 Parse(string value)
    {
        return Parse(value.AsSpan());
    }

    [Obsolete("Will be removed post NetStandard2.0")]
    public static bool TryParse(string value, out IpAddressRangeV4 result)
    {
        return TryParse(value.AsSpan(), out result);
    }

    /// <summary>
    /// Parses an IPv4 using faster code, but with fewer safety checks.
    /// </summary>
    /// <remarks>Use this only if you know the input /is/ an IPv4</remarks>
    [Obsolete("Will be removed post NetStandard2.0")]
    public static IpAddressRangeV4 ParseUnstable(string value)
    {
        return ParseUnstable(value.AsSpan());
    }
}
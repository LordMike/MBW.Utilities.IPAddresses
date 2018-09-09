using System;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace MBW.Utilities.IPAddresses
{
    public partial struct IpAddressRangeV6
    {
        public static IpAddressRangeV6 Parse(string value)
        {
            if (TryParse(value, out IpAddressRangeV6 result))
                return result;

            throw new ArgumentException("Argument was not a valid IPv6 range", nameof(value));
        }

        public static bool TryParse(string value, out IpAddressRangeV6 result)
        {
            if (string.IsNullOrEmpty(value) || value.Length > 49)
            {
                result = default;
                return false;
            }

            // TODO: High perf parser
            Match match = IpRegex.Match(value);
            if (!match.Success)
            {
                result = default;
                return false;
            }

            byte mask = 128;

            if (!IPAddress.TryParse(match.Groups[1].Value, out IPAddress ip) || ip.AddressFamily != AddressFamily.InterNetworkV6)
            {
                result = default;
                return false;
            }

            Group maskMatch = match.Groups[2];
            if (maskMatch.Success)
            {
                mask = byte.Parse(maskMatch.Value);
                if (mask > 128)
                {
                    result = default;
                    return false;
                }
            }

            result = new IpAddressRangeV6(ip, mask);
            return true;
        }

        public static implicit operator IpAddressRangeV6(string value)
        {
            return Parse(value);
        }

        public static implicit operator IpAddressRangeV6(IPAddress value)
        {
            return new IpAddressRangeV6(value);
        }
    }
}
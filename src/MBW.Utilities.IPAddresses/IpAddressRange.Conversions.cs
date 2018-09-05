using System;
using System.Net;

namespace MBW.Utilities.IPAddresses
{
    public partial class IpAddressRange
    {
        public static IpAddressRange Parse(string input)
        {
            if (!TryParse(input, out IpAddressRange parsed))
                throw new ArgumentException("Input was not a valid ip range", nameof(input));

            return parsed;
        }

        public static bool TryParse(string input, out IpAddressRange range)
        {
            if (IpAddressRangeV4.TryParse(input, out IpAddressRangeV4 v4))
            {
                range = new IpAddressRange(v4, null);
                return true;
            }

            if (IpAddressRangeV6.TryParse(input, out IpAddressRangeV6 v6))
            {
                range = new IpAddressRange(null, v6);
                return true;
            }

            range = null;
            return false;
        }

        public static implicit operator IpAddressRange(string value)
        {
            return Parse(value);
        }

        public static implicit operator IpAddressRange(IPAddress value)
        {
            return new IpAddressRange(value);
        }

        public static implicit operator IpAddressRange(IpAddressRangeV4 value)
        {
            return new IpAddressRange(value);
        }

        public static implicit operator IpAddressRange(IpAddressRangeV6 value)
        {
            return new IpAddressRange(value);
        }
    }
}
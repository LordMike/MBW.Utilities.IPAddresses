using System;
using System.Net;

namespace MBW.Utilities.IPAddresses
{
    public partial struct IpAddressRangeV4
    {
        public static IpAddressRangeV4 Parse(string value)
        {
            if (TryParse(value, out IpAddressRangeV4 result))
                return result;

            throw new ArgumentException("Argument was not a valid IPv4 range", nameof(value));
        }

        public static bool TryParse(string value, out IpAddressRangeV4 result)
        {
            if (string.IsNullOrEmpty(value) || value.Length > 18)
            {
                result = default;
                return false;
            }

            byte currentOctet = 0;
            byte dots = 0;
            uint ip = 0;
            bool hadData = false;
            bool isCidr = false;

            for (int i = 0; i < value.Length; i++)
            {
                char ch = value[i];

                if (ch == '.')
                {
                    if (!hadData)
                    {
                        result = default;
                        return false;
                    }

                    ip <<= 8;
                    ip += currentOctet;
                    currentOctet = 0;
                    hadData = false;

                    if (++dots > 3)
                    {
                        result = default;
                        return false;
                    }

                    continue;
                }

                if ('0' <= ch && ch <= '9')
                {
                    hadData = true;
                    currentOctet *= 10;
                    currentOctet += (byte)(ch - '0');

                    continue;
                }

                if (ch == '/')
                {
                    ip <<= 8;
                    ip += currentOctet;
                    currentOctet = 0;
                    hadData = false;

                    isCidr = true;

                    continue;
                }

                result = default;
                return false;
            }

            if (isCidr)
            {
                // Add last octet as mask
                if (currentOctet > 32 || !hadData)
                {
                    result = default;
                    return false;
                }
            }
            else
            {
                ip <<= 8;
                ip += currentOctet;
                currentOctet = 32;
            }

            result = new IpAddressRangeV4(ip, currentOctet);
            return true;
        }

        public static implicit operator IpAddressRangeV4(string value)
        {
            return Parse(value);
        }

        public static implicit operator IpAddressRangeV4(IPAddress value)
        {
            return new IpAddressRangeV4(value);
        }
    }
}

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
            return TryParse(value.AsSpan(), out result);
        }

        public static bool TryParse(ReadOnlySpan<char> value, out IpAddressRangeV4 result)
        {
            if (value.Length == 0 || value.Length > 18)
            {
                result = default;
                return false;
            }

            ushort currentOctet = 0;
            byte dots = 0;
            uint ip = 0;
            bool isCidr = false;
            bool expectNumber = true;

            for (int i = 0; i < value.Length; i++)
            {
                char ch = value[i];

                if (ch == '.')
                {
                    if (expectNumber)
                    {
                        result = default;
                        return false;
                    }

                    if (currentOctet > byte.MaxValue)
                    {
                        result = default;
                        return false;
                    }

                    ip <<= 8;
                    ip += currentOctet;
                    currentOctet = 0;
                    expectNumber = true;

                    if (++dots > 3)
                    {
                        result = default;
                        return false;
                    }

                    continue;
                }

                if ('0' <= ch && ch <= '9')
                {
                    expectNumber = false;
                    currentOctet *= 10;
                    currentOctet += (byte)(ch - '0');

                    continue;
                }

                if (ch == '/')
                {
                    if (expectNumber)
                    {
                        result = default;
                        return false;
                    }

                    if (currentOctet > byte.MaxValue)
                    {
                        result = default;
                        return false;
                    }

                    ip <<= 8;
                    ip += currentOctet;
                    currentOctet = 0;
                    expectNumber = true;

                    isCidr = true;

                    continue;
                }

                result = default;
                return false;
            }

            if (expectNumber)
            {
                result = default;
                return false;
            }

            if (isCidr)
            {
                // Add last octet as mask
                if (currentOctet > 32)
                {
                    result = default;
                    return false;
                }
            }
            else
            {
                if (currentOctet > byte.MaxValue)
                {
                    result = default;
                    return false;
                }

                ip <<= 8;
                ip += currentOctet;
                currentOctet = 32;
            }

            // Handle cases like "192.168" => "192.168.0.0"
            int toMove = 3 - dots;
            if (toMove > 0)
                ip <<= 8 * toMove;

            result = new IpAddressRangeV4(ip, (byte)currentOctet);
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

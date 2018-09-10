using System;
using System.Net;
using MBW.Utilities.IPAddresses.Tokenization;

namespace MBW.Utilities.IPAddresses
{
    public partial struct IpAddressRangeV4
    {
        public static IpAddressRangeV4 Parse(string value)
        {
            if (TryParse(value, out IpAddressRangeV4 result))
                return result;

            throw new ArgumentException($"Argument was not a valid IPv4 range, value: {value}", nameof(value));
        }

        public static bool TryParse(string value, out IpAddressRangeV4 result)
        {
            return TryParse(value.AsSpan(), out result);
        }

        public static bool TryParse(ReadOnlySpan<char> value, out IpAddressRangeV4 result)
        {
            int read;
            (TokenType type, ushort value) tkn = default;

            uint ip = 0;
            byte cidr = 32;

            // Parse the IP
            int i;
            for (i = 0; i < 4; i++)
            {
                tkn = Tokenizer.ReadToken(value, false, out read);
                value = value.Slice(read);

                // Read a dot, or break on slashes
                if (i > 0)
                {
                    if (tkn.type == TokenType.Slash)
                        break;

                    if (tkn.type != TokenType.Dot)
                    {
                        // We expected a dot, but we didn't get one
                        result = default;
                        return false;
                    }

                    // Advance once more
                    tkn = Tokenizer.ReadToken(value, false, out read);
                    value = value.Slice(read);
                }

                // Read a number
                if (tkn.type != TokenType.Number || tkn.value > byte.MaxValue)
                {
                    // We expected a 0..255 number, but we didn't get one
                    result = default;
                    return false;
                }

                ip <<= 8;
                ip += tkn.value;
            }

            // Assume the remainder of the IP is 0's
            for (; i < 4; i++)
                ip <<= 8;

            // Parse the Cidr
            if (tkn.type != TokenType.Slash)
            {
                tkn = Tokenizer.ReadToken(value, false, out read);
                value = value.Slice(read);
            }

            if (tkn.type == TokenType.Slash)
            {
                // Read a number, as the cidr
                tkn = Tokenizer.ReadToken(value, false, out read);
                value = value.Slice(read);

                if (tkn.type != TokenType.Number || tkn.value > 32)
                {
                    // We expected a 0..32 number, but we didn't get one
                    result = default;
                    return false;
                }

                cidr = (byte)tkn.value;

                // Advance once more
                tkn = Tokenizer.ReadToken(value, false, out read);
                value = value.Slice(read);
            }

            // We now expect the end
            if (tkn.type != TokenType.None)
            {
                // Something other than EOF was found
                result = default;
                return false;
            }

            result = new IpAddressRangeV4(ip, cidr);
            return true;
        }

        /// <summary>
        /// Parses an IPv4 using faster code, but with fewer safety checks.
        /// </summary>
        /// <remarks>Use this only if you know the input /is/ an IPv4</remarks>
        public static IpAddressRangeV4 ParseUnstable(string value)
        {
            return ParseUnstable(value.AsSpan());
        }
        
        /// <summary>
        /// Parses an IPv4 using faster code, but with fewer safety checks.
        /// </summary>
        /// <remarks>Use this only if you know the input /is/ an IPv4</remarks>
        public static IpAddressRangeV4 ParseUnstable(ReadOnlySpan<char> value)
        {
            // TODO: Remove more checks, assume more of the input
            if (value.Length == 0 || value.Length > 18)
                return default;

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
                        return default;

                    if (currentOctet > byte.MaxValue)
                        return default;

                    ip <<= 8;
                    ip += currentOctet;
                    currentOctet = 0;
                    expectNumber = true;

                    if (++dots > 3)
                        return default;

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
                        return default;

                    if (currentOctet > byte.MaxValue)
                        return default;

                    ip <<= 8;
                    ip += currentOctet;
                    currentOctet = 0;
                    expectNumber = true;

                    isCidr = true;

                    continue;
                }

                return default;
            }

            if (expectNumber)
                return default;

            if (isCidr)
            {
                // Add last octet as mask
                if (currentOctet > 32)
                    return default;
            }
            else
            {
                if (currentOctet > byte.MaxValue)
                    return default;

                ip <<= 8;
                ip += currentOctet;
                currentOctet = 32;
            }

            // Handle cases like "192.168" => "192.168.0.0"
            int toMove = 3 - dots;
            if (toMove > 0)
                ip <<= 8 * toMove;

            return new IpAddressRangeV4(ip, (byte)currentOctet);
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

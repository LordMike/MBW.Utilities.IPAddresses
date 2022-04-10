using System;
using MBW.Utilities.IPAddresses.Tokenization;

namespace MBW.Utilities.IPAddresses;

public partial struct IpAddressV4
{
    public static IpAddressV4 Parse(ReadOnlySpan<char> value)
    {
        if (TryParse(value, out IpAddressV4 result))
            return result;

        throw new ArgumentException($"Argument was not a valid IPv4 range, value: {value.ToString()}", nameof(value));
    }

    public static IpAddressV4 Parse(string value)
    {
        return Parse(value.AsSpan());
    }

    public static bool TryParse(string value, out IpAddressV4 result)
    {
        return TryParse(value.AsSpan(), out result);
    }

    public static bool TryParse(ReadOnlySpan<char> value, out IpAddressV4 result)
    {
        Tokenizer tokenizer = new Tokenizer(value);
        if (!TryParse(ref tokenizer, out result))
            return false;

        // We now expect the end
        ParsedToken token = tokenizer.ParseAndAdvance();
        if (token.Type != TokenType.None)
        {
            // Something other than EOF was found
            result = default;
            return false;
        }

        return true;
    }

    internal static bool TryParse(ref Tokenizer tokenizer, out IpAddressV4 result)
    {
        // Shortest IPv4 is 1 char (0)
        // Longest IPv4 is 15 chars (255.255.255.255)
        if (tokenizer.Length < 1 || tokenizer.Length > 15)
        {
            result = default;
            return false;
        }

        uint ip = 0;

        ParsedToken token = default;

        // Parse the IP
        int i;
        for (i = 0; i < 4; i++)
        {
            token = tokenizer.ParseAndAdvance(false);

            // Read a dot, or break on slashes
            if (i > 0)
            {
                if (token.Type == TokenType.None)
                {
                    // We're at the end, and we could have parsed a number. To support partial IPv4's ("192.168"), we break here
                    break;
                }

                if (token.Type != TokenType.Dot)
                {
                    // We expected a dot, but we didn't get one
                    result = default;
                    return false;
                }

                // Advance once more
                token = tokenizer.ParseAndAdvance(false);
            }

            // Read a number
            if (token.Type != TokenType.Number || token.Value > byte.MaxValue)
            {
                // We expected a 0..255 number, but we didn't get one
                result = default;
                return false;
            }

            ip <<= 8;
            ip += token.Value;
        }

        // Assume the remainder of the IP is 0's
        for (; i < 4; i++)
            ip <<= 8;

        result = new IpAddressV4(ip);
        return true;
    }
}
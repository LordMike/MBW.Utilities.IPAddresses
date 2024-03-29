﻿using System;
using MBW.Utilities.IPAddresses.Tokenization;

namespace MBW.Utilities.IPAddresses;

public partial struct IpAddressNetworkV4
{
    public static IpAddressNetworkV4 Parse(ReadOnlySpan<char> value)
    {
        if (TryParse(value, out IpAddressNetworkV4 result))
            return result;

        throw new ArgumentException($"Argument was not a valid IPv4 range, value: {value.ToString()}", nameof(value));
    }

    public static IpAddressNetworkV4 Parse(string value)
    {
        return Parse(value.AsSpan());
    }

    public static bool TryParse(string value, out IpAddressNetworkV4 result)
    {
        return TryParse(value.AsSpan(), out result);
    }

    public static bool TryParse(ReadOnlySpan<char> value, out IpAddressNetworkV4 result)
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

    internal static bool TryParse(ref Tokenizer tokenizer, out IpAddressNetworkV4 result)
    {
        // Shortest IPv4 is 1 char (0)
        // Longest IPv4 is 18 chars (255.255.255.255/32)
        if (tokenizer.Length < 1 || tokenizer.Length > 18)
        {
            result = default;
            return false;
        }

        uint ip = 0;
        byte cidr = 32;

        ParsedToken token = default;

        // Parse the IP
        int i;
        for (i = 0; i < 4; i++)
        {
            token = tokenizer.ParseAndAdvance(false);

            // Read a dot, or break on slashes
            if (i > 0)
            {
                if (token.Type == TokenType.Slash)
                    break;

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

        // Parse the Cidr
        if (token.Type != TokenType.Slash)
            token = tokenizer.ParseAndAdvance(false);

        if (token.Type == TokenType.Slash)
        {
            // Read a number, as the cidr
            token = tokenizer.ParseAndAdvance(false);

            if (token.Type != TokenType.Number || token.Value > 32)
            {
                // We expected a 0..32 number, but we didn't get one
                result = default;
                return false;
            }

            cidr = (byte)token.Value;
        }

        result = new IpAddressNetworkV4(ip, cidr);
        return true;
    }

    /// <summary>
    /// Parses an IPv4 using faster code, but with fewer safety checks.
    /// </summary>
    /// <remarks>Use this only if you know the input /is/ an IPv4</remarks>
    public static IpAddressNetworkV4 ParseUnstable(string value)
    {
        return ParseUnstable(value.AsSpan());
    }

    /// <summary>
    /// Parses an IPv4 using faster code, but with fewer safety checks.
    /// </summary>
    /// <remarks>Use this only if you know the input /is/ an IPv4</remarks>
    public static IpAddressNetworkV4 ParseUnstable(ReadOnlySpan<char> value)
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

            if (ch is >= '0' and <= '9')
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

        return new IpAddressNetworkV4(ip, (byte)currentOctet);
    }
}
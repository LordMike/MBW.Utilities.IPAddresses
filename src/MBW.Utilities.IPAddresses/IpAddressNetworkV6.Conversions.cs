using System;
using System.Net;
using System.Runtime.CompilerServices;
using MBW.Utilities.IPAddresses.Helpers;
using MBW.Utilities.IPAddresses.Tokenization;

namespace MBW.Utilities.IPAddresses;

public partial struct IpAddressNetworkV6
{
    public static IpAddressNetworkV6 Parse(ReadOnlySpan<char> value)
    {
        if (TryParse(value, out IpAddressNetworkV6 result))
            return result;

        throw new ArgumentException($"Argument was not a valid IPv6 range, value: {value.ToString()}", nameof(value));
    }

    public static bool TryParse(ReadOnlySpan<char> value, out IpAddressNetworkV6 result)
    {
        // Longest regular IPv6 is 43 chars (0000:0000:0000:0000:0000:0000:0000:0000/128)
        // Longest IPv4 mapped IPv6 is 49 chars (0000:0000:0000:0000:0000:ffff:255.255.255.255/128)
        if (value.Length == 0 || value.Length > 49)
        {
            result = default;
            return false;
        }

        Tokenizer tokenizer = new(value);

        ulong high = 0;
        ulong low = 0;
        byte cidr = 128;
        byte segmentsRead = 0;

        bool doReverse = false;

        // First pass, try reading a mask from the end of the input
        ParsedToken tkn = tokenizer.PeekEnd(false);
        if (tkn.Type == TokenType.Number)
        {
            // Could be a number, therefor a CIDR range or IPv4
            ParsedToken slashTkn = tokenizer.PeekEnd(false);
            if (slashTkn.Type == TokenType.Slash && tkn.Value <= 128)
            {
                cidr = (byte)tkn.Value;
                tokenizer.AdoptPeekOffsets();
            }
            else if (slashTkn.Type == TokenType.Dot)
            {
                // This could be an IPv4 mapped in IPv6
                // Carry on, see where it gets us
                tokenizer.ResetPeekOffsets();
            }
            else if (slashTkn.Type != TokenType.Number && slashTkn.Type != TokenType.Colon && slashTkn.Type != TokenType.DoubleColon && slashTkn.Type != TokenType.None)
            {
                // Any IPv6 should end on a number or double-colon (excluding the cidr mask), and the cidr mask should be 128 or lower
                // Single-token IPv6's are allowed, so we check for None as well
                result = default;
                return false;
            }
        }

        // Test if this could be an IPv4 mapped IPv6
        // This could be the case if the last two tokens are [Dot, Number]
        // Like '::ffff:192.168.1.0'
        tkn = tokenizer.PeekEnd(false);
        if (tkn.Type == TokenType.Number)
        {
            // If the next-to-last is a Dot, pass it on
            var tmpTkn = tokenizer.PeekEnd(false);

            tokenizer.ResetPeekOffsets();

            if (tmpTkn.Type == TokenType.Dot)
                return TryReadIPv4MappedIPv6(tokenizer, cidr, out result);
        }

        tokenizer.ResetPeekOffsets();

        // Read up till a double-colon, eof or slash
        for (byte i = 0; i < 8; i++)
        {
            tkn = tokenizer.ParseAndAdvanceStart(true);
            if (tkn.Type == TokenType.None)
                break;

            if (i > 0)
            {
                // The read token MUST be a colon or a double-colon
                if (tkn.Type == TokenType.Colon)
                {
                    // Advance once more
                    tkn = tokenizer.ParseAndAdvanceStart(true);
                }
                else if (tkn.Type != TokenType.DoubleColon)
                {
                    result = default;
                    return false;
                }
            }

            // Read a number or double-colon
            if (tkn.Type == TokenType.Number)
            {
                SetTuplet(ref low, ref high, i, tkn.Value);
                segmentsRead++;
            }
            else if (tkn.Type == TokenType.DoubleColon)
            {
                doReverse = true;
                break;
            }
            else if (tkn.Type != TokenType.DoubleColon)
            {
                result = default;
                return false;
            }
        }

        // Read reverse
        if (doReverse)
        {
            byte toRead = (byte)(8 - segmentsRead);

            for (byte i = 0; i < toRead; i++)
            {
                tkn = tokenizer.ParseAndAdvanceEnd(true);
                if (tkn.Type == TokenType.None)
                    break;

                if (i > 0)
                {
                    // The read token MUST be a colon
                    if (tkn.Type != TokenType.Colon)
                    {
                        result = default;
                        return false;
                    }

                    // Advance once more
                    tkn = tokenizer.ParseAndAdvanceEnd(true);
                }

                // Read a number
                if (tkn.Type == TokenType.Number)
                {
                    SetTuplet(ref low, ref high, (byte)(7 - i), tkn.Value);
                    segmentsRead++;
                }
                else
                {
                    result = default;
                    return false;
                }
            }
        }

        result = new IpAddressNetworkV6(high, low, cidr);
        return true;
    }

    private static bool TryReadIPv4MappedIPv6(Tokenizer tokenizer, byte cidr, out IpAddressNetworkV6 result)
    {
        ulong high = 0;
        ulong low = 0;
        byte segmentsRead = 0;
        ParsedToken token;

        // Read reverse, we're only interested in the IPv4 on the end
        byte toRead = 4;

        for (byte i = 0; i < toRead; i++)
        {
            token = tokenizer.ParseAndAdvanceEnd(false);
            if (token.Type == TokenType.None)
                break;

            if (i > 0)
            {
                // The read token MUST be a dot
                if (token.Type != TokenType.Dot)
                {
                    result = default;
                    return false;
                }

                // Advance once more
                token = tokenizer.ParseAndAdvanceEnd(false);
            }

            // Read a number
            if (token.Type == TokenType.Number)
            {
                SetByte(ref low, ref high, (byte)(15 - i), token.Value);
                segmentsRead++;
            }
            else
            {
                result = default;
                return false;
            }
        }

        // Assert that the next tokens are [Double-/Colon, ffff, Colon]
        token = tokenizer.ParseAndAdvanceEnd(false);

        if (token.Type != TokenType.Colon)
        {
            result = default;
            return false;
        }

        token = tokenizer.ParseAndAdvanceEnd(true);

        if (token.Type != TokenType.Number || token.Value != 0xffff)
        {
            result = default;
            return false;
        }

        token = tokenizer.ParseAndAdvanceEnd(false);

        if (token.Type is not TokenType.Colon && token.Type is not TokenType.DoubleColon)
        {
            result = default;
            return false;
        }

        // Place 0xFFFF in the correct position
        SetTuplet(ref low, ref high, 5, 0xFFFF);

        result = new IpAddressNetworkV6(high, low, cidr);
        return true;
    }

    /// <summary>
    /// Parses an IPv6 using faster code, but with fewer safety checks.
    /// </summary>
    /// <remarks>Use this only if you know the input /is/ an IPv6</remarks>
    public static IpAddressNetworkV6 ParseUnstable(ReadOnlySpan<char> value)
    {
        if (value.Length == 0 || value.Length > 49)
            return default;

        ulong high = 0;
        ulong low = 0;
        byte mask = 0;
        byte idx = 1;

        // Find mask, read reverse
        for (int i = value.Length - 1; i >= 0; i--)
        {
            char ch = value[i];

            if (ch == ':' || ch == '.' || ch is >= 'A' and <= 'F' || ch is >= 'a' and <= 'f')
            {
                // No mask provided
                mask = 128;
                break;
            }

            if (ch == '/')
            {
                // Mask has been read, cut off the mask
                value = value[..i];
                break;
            }

            if (ch is >= '0' and <= '9')
            {
                mask += (byte)((ch - '0') * idx);
                idx *= 10;

                continue;
            }

            throw new ArgumentException();
        }

        // Tuple offset
        idx = 0;

        // Read from front, until :: is met
        bool doReverse = false;

        while (value.Length > 0)
        {
            ushort currentTuplet = 0;

            // Read up to 4 chars
            int i;
            for (i = 0; i < 4; i++)
            {
                char ch = value[i];

                if (ch == ':')
                    break;

                // Read and append char
                byte val = ParseChar(ch);

                if (val == byte.MaxValue)
                    throw new ArgumentException();

                currentTuplet <<= 4;
                currentTuplet += val;
            }

            SetTuplet(ref low, ref high, idx++, currentTuplet);
            value = value[i..];

            if (value.Length == 0)
                break;

            // Check if next char is ':'
            if (value[0] == ':')
            {
                value = value[1..];

                // Check if next again is ':' too
                if (value[0] == ':')
                {
                    // We need to parse backwards now
                    doReverse = true;
                    break;
                }
            }
            else
                throw new ArgumentException();
        }

        if (doReverse)
        {
            idx = 7;

            while (value.Length > 0)
            {
                // Read up to 4 chars
                // Find ':'

                int lastIdx = StringUtilities.ReverseIndexOf(value, ':');

                if (lastIdx == 0 && value.Length >= 7)
                {
                    // This could be an IPv4 address - definitely not IPv6
                    if (!IpAddressNetworkV4.TryParse(value[1..], out IpAddressNetworkV4 ipv4))
                        return default;

                    low += ipv4.AddressUint;
                    return new IpAddressNetworkV6(high, low, mask);
                }

                if (lastIdx <= 0 && value.Length > 5)
                {
                    // This is definitely not IPv6
                    return default;
                }

                ushort currentTuplet = 0;

                ReadOnlySpan<char> segment = value[(lastIdx + 1)..];
                value = value[..lastIdx];

                if (segment.Length == 0 || segment.Length > 4)
                    // We cannot have two "::" segments, cannot have longer than 4 chars
                    throw new Exception();

                int i;
                for (i = 0; i < segment.Length; i++)
                {
                    char ch = segment[i];

                    if (ch == ':')
                        break;

                    // Read and append char
                    byte val = ParseChar(ch);

                    if (val == byte.MaxValue)
                        throw new ArgumentException();

                    currentTuplet <<= 4;
                    currentTuplet += val;
                }

                SetTuplet(ref low, ref high, idx--, currentTuplet);
            }
        }

        return new IpAddressNetworkV6(high, low, mask);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SetTuplet(ref ulong low, ref ulong high, byte index, ulong newValue)
    {
        if (index < 4)
            high |= newValue << ((3 - index) * 16);
        else
            low |= newValue << ((7 - index) * 16);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SetByte(ref ulong low, ref ulong high, byte index, ulong newValue)
    {
        if (index < 8)
            high |= newValue << ((7 - index) * 8);
        else
            low |= newValue << ((15 - index) * 8);
    }

    private static byte ParseChar(char ch)
    {
        if (ch is >= '0' and <= '9')
            return (byte)(ch - '0');

        if (ch is >= 'A' and <= 'F')
            return (byte)(10 + ch - 'A');

        if (ch is >= 'a' and <= 'f')
            return (byte)(10 + ch - 'a');

        return byte.MaxValue;
    }

    [Obsolete]
    public static explicit operator IpAddressNetworkV6(string value)
    {
        return Parse(value);
    }

    public static explicit operator IpAddressNetworkV6(ReadOnlySpan<char> value)
    {
        return Parse(value);
    }

    public static implicit operator IpAddressNetworkV6(IPAddress value)
    {
        return new IpAddressNetworkV6(value);
    }

    public static explicit operator IPAddress(IpAddressNetworkV6 value)
    {
        return value.Address;
    }
}
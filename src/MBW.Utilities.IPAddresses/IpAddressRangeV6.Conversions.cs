using System;
using System.Net;
using System.Runtime.CompilerServices;
using MBW.Utilities.IPAddresses.Helpers;
using MBW.Utilities.IPAddresses.Tokenization;

namespace MBW.Utilities.IPAddresses;

public partial struct IpAddressRangeV6
{
    public static IpAddressRangeV6 Parse(string value)
    {
        if (TryParse(value, out IpAddressRangeV6 result))
            return result;

        throw new ArgumentException($"Argument was not a valid IPv6 range, value: {value}", nameof(value));
    }

    public static bool TryParse(string value, out IpAddressRangeV6 result)
    {
        return TryParse(value.AsSpan(), out result);
    }

    public static bool TryParse(ReadOnlySpan<char> value, out IpAddressRangeV6 result)
    {
        if (value.Length == 0 || value.Length > 49)
        {
            result = default;
            return false;
        }

        ulong high = 0;
        ulong low = 0;
        byte cidr = 128;
        byte segmentsRead = 0;

        bool doReverse = false;

        // Try reading a mask
        (TokenType type, ushort value) tkn = Tokenizer.ReadTokenReverse(value, false, out int read);
        if (tkn.type == TokenType.Number)
        {
            (TokenType type, ushort value) slashTkn = Tokenizer.ReadTokenReverse(value[..^read], false, out int read2);
            if (slashTkn.type == TokenType.Slash && tkn.value <= 128)
            {
                cidr = (byte)tkn.value;
                value = value[..(value.Length - read - read2)];
            }
            else if (slashTkn.type != TokenType.Number && slashTkn.type != TokenType.DoubleColon && slashTkn.type != TokenType.None)
            {
                // Any IPv6 should end on a number or double-colon (excluding the cidr mask), and the cidr mask should be 128 or lower
                // Single-token IPv6's are allowed, so we check for None as well
                result = default;
                return false;
            }
        }

        // Read up till a double-colon, eof or slash
        for (byte i = 0; i < 8; i++)
        {
            tkn = Tokenizer.ReadToken(value, true, out read);
            value = value[read..];

            if (tkn.type == TokenType.None)
                break;

            if (i > 0)
            {
                // The read token MUST be a colon or a double-colon
                if (tkn.type == TokenType.Colon)
                {
                    // Advance once more
                    tkn = Tokenizer.ReadToken(value, true, out read);
                    value = value[read..];
                }
                else if (tkn.type != TokenType.DoubleColon)
                {
                    result = default;
                    return false;
                }
            }

            // Read a number or double-colon
            if (tkn.type == TokenType.Number)
            {
                SetTuplet(ref low, ref high, i, tkn.value);
                segmentsRead++;
            }
            else if (tkn.type == TokenType.DoubleColon)
            {
                doReverse = true;
                break;
            }
            else if (tkn.type != TokenType.DoubleColon)
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
                tkn = Tokenizer.ReadTokenReverse(value, true, out read);
                value = value[..^read];

                if (tkn.type == TokenType.None)
                    break;

                if (i > 0)
                {
                    // The read token MUST be a colon
                    if (tkn.type != TokenType.Colon)
                    {
                        result = default;
                        return false;
                    }

                    // Advance once more
                    tkn = Tokenizer.ReadTokenReverse(value, true, out read);
                    value = value[..^read];
                }

                // Read a number
                if (tkn.type == TokenType.Number)
                {
                    SetTuplet(ref low, ref high, (byte)(7 - i), tkn.value);
                    segmentsRead++;
                }
                else
                {
                    result = default;
                    return false;
                }
            }
        }

        result = new IpAddressRangeV6(high, low, cidr);
        return true;
    }

    /// <summary>
    /// Parses an IPv6 using faster code, but with fewer safety checks.
    /// </summary>
    /// <remarks>Use this only if you know the input /is/ an IPv6</remarks>
    public static IpAddressRangeV6 ParseUnstable(string value)
    {
        return ParseUnstable(value.AsSpan());
    }

    /// <summary>
    /// Parses an IPv6 using faster code, but with fewer safety checks.
    /// </summary>
    /// <remarks>Use this only if you know the input /is/ an IPv6</remarks>
    public static IpAddressRangeV6 ParseUnstable(ReadOnlySpan<char> value)
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
                    if (!IpAddressRangeV4.TryParse(value[1..], out IpAddressRangeV4 ipv4))
                        return default;

                    low += ipv4.AddressUint;
                    return new IpAddressRangeV6(high, low, mask);
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

        return new IpAddressRangeV6(high, low, mask);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SetTuplet(ref ulong low, ref ulong high, byte index, ulong newValue)
    {
        if (index < 4)
            high |= newValue << ((3 - index) * 16);
        else
            low |= newValue << ((7 - index) * 16);
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

    public static implicit operator IpAddressRangeV6(string value)
    {
        return Parse(value);
    }

    public static implicit operator IpAddressRangeV6(IPAddress value)
    {
        return new IpAddressRangeV6(value);
    }
}
using System;
using MBW.Utilities.IPAddresses.Helpers;
using MBW.Utilities.IPAddresses.Tokenization;

namespace MBW.Utilities.IPAddresses;

public partial struct IpAddressV6
{
    public static IpAddressV6 Parse(ReadOnlySpan<char> value)
    {
        if (TryParse(value, out IpAddressV6 result))
            return result;

        throw new ArgumentException($"Argument was not a valid IPv6 range, value: {value.ToString()}", nameof(value));
    }

    public static bool TryParse(ReadOnlySpan<char> value, out IpAddressV6 result)
    {
        Tokenizer tokenizer = new(value);
        return TryParse(ref tokenizer, out result);
    }

    internal static bool TryParse(ref Tokenizer tokenizer, out IpAddressV6 result)
    {
        // Shortest IPv6 is 2 chars (::)
        // Longest regular IPv6 is 39 chars (0000:0000:0000:0000:0000:0000:0000:0000)
        // Longest IPv4 mapped IPv6 is 45 chars (0000:0000:0000:0000:0000:ffff:255.255.255.255)
        if (tokenizer.Length < 2 || tokenizer.Length > 45)
        {
            result = default;
            return false;
        }

        ulong high = 0;
        ulong low = 0;
        byte segmentsRead = 0;

        bool doReverse = false;

        // First pass, try reading a mask from the end of the input
        ParsedToken tkn = tokenizer.PeekReverse(false);
        if (tkn.Type == TokenType.Number)
        {
            // Could be a number, therefore an IPv4 mapped IPv6
            ParsedToken slashTkn = tokenizer.PeekReverse(false);
            if (slashTkn.Type == TokenType.Dot)
            {
                // This could be an IPv4 mapped in IPv6
                // Carry on, see where it gets us
                tokenizer.ResetPeekOffsets();
            }
            else if (slashTkn.Type != TokenType.Number && slashTkn.Type != TokenType.Colon && slashTkn.Type != TokenType.DoubleColon && slashTkn.Type != TokenType.None)
            {
                // Any IPv6 should end on a number or double-colon
                // Single-token IPv6's are allowed, so we check for None as well
                result = default;
                return false;
            }
        }

        // Test if this could be an IPv4 mapped IPv6
        // This could be the case if the last two tokens are [Dot, Number]
        // Like '::ffff:192.168.1.0'
        tkn = tokenizer.PeekReverse(false);
        if (tkn.Type == TokenType.Number)
        {
            // If the next-to-last is a Dot, pass it on
            ParsedToken tmpTkn = tokenizer.PeekReverse(false);

            tokenizer.ResetPeekOffsets();

            if (tmpTkn.Type == TokenType.Dot)
                return TryReadIPv4MappedIPv6(tokenizer, out result);
        }

        tokenizer.ResetPeekOffsets();

        // Read up till a double-colon, eof or slash
        for (byte i = 0; i < 8; i++)
        {
            tkn = tokenizer.ParseAndAdvance(true);
            if (tkn.Type == TokenType.None)
                break;

            if (i > 0)
            {
                // The read token MUST be a colon or a double-colon
                if (tkn.Type == TokenType.Colon)
                {
                    // Advance once more
                    tkn = tokenizer.ParseAndAdvance(true);
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
                BitUtilities.SetTuplet(ref low, ref high, i, tkn.Value);
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
                tkn = tokenizer.ParseAndAdvanceReverse(true);
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
                    tkn = tokenizer.ParseAndAdvanceReverse(true);
                }

                // Read a number
                if (tkn.Type == TokenType.Number)
                {
                    BitUtilities.SetTuplet(ref low, ref high, (byte)(7 - i), tkn.Value);
                    segmentsRead++;
                }
                else
                {
                    result = default;
                    return false;
                }
            }
        }

        result = new IpAddressV6(high, low);
        return true;
    }

    public static IpAddressV6 Parse(string value)
    {
        return Parse(value.AsSpan());
    }

    public static bool TryParse(string value, out IpAddressV6 result)
    {
        return TryParse(value.AsSpan(), out result);
    }

    private static bool TryReadIPv4MappedIPv6(Tokenizer tokenizer, out IpAddressV6 result)
    {
        ulong high = 0;
        ulong low = 0;
        byte segmentsRead = 0;
        ParsedToken token;

        // Read reverse, we're only interested in the IPv4 on the end
        byte toRead = 4;

        for (byte i = 0; i < toRead; i++)
        {
            token = tokenizer.ParseAndAdvanceReverse(false);
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
                token = tokenizer.ParseAndAdvanceReverse(false);
            }

            // Read a number
            if (token.Type == TokenType.Number)
            {
                BitUtilities.SetByte(ref low, ref high, (byte)(15 - i), token.Value);
                segmentsRead++;
            }
            else
            {
                result = default;
                return false;
            }
        }

        // Assert that the next tokens are [Double-/Colon, ffff, Colon]
        token = tokenizer.ParseAndAdvanceReverse(false);

        if (token.Type != TokenType.Colon)
        {
            result = default;
            return false;
        }

        token = tokenizer.ParseAndAdvanceReverse(true);

        if (token.Type != TokenType.Number || token.Value != 0xffff)
        {
            result = default;
            return false;
        }

        token = tokenizer.ParseAndAdvanceReverse(false);

        if (token.Type is not TokenType.Colon && token.Type is not TokenType.DoubleColon)
        {
            result = default;
            return false;
        }

        // Place 0xFFFF in the correct position
        BitUtilities.SetTuplet(ref low, ref high, 5, 0xFFFF);

        result = new IpAddressV6(high, low);
        return true;
    }
}
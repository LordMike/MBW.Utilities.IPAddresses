using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MBW.Utilities.IPAddresses.Tokenization;

internal record struct ParsedToken(TokenType Type, ushort Value);

internal ref struct Tokenizer
{
    private ReadOnlySpan<char> _value;
    private ReadOnlySpan<char> _temp;

    public Tokenizer(ReadOnlySpan<char> input)
    {
        _value = input;
        _temp = _value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ParsedToken Interpret(ReadOnlySpan<char> input, bool forward, bool isHexadecimal, out int read)
    {
        read = 0;

        if (input.IsEmpty)
            return new(TokenType.None, 0);

        int idx, increment;

        if (forward)
        {
            idx = 0;
            increment = 1;
        }
        else
        {
            idx = input.Length - 1;
            increment = -1;
        }

        char ch = input[idx];
        if (ch == '/')
        {
            read = 1;
            return new(TokenType.Slash, 0);
        }

        if (ch == '.')
        {
            read = 1;
            return new(TokenType.Dot, 0);
        }

        if (ch == ':')
        {
            if (input.Length >= 2 && input[idx + increment] == ':')
            {
                read = 2;
                return new(TokenType.DoubleColon, 0);
            }

            read = 1;
            return new(TokenType.Colon, 0);
        }

        if (ParseChar(ch, isHexadecimal) != byte.MaxValue)
        {
            // Read entire number
            ushort val = 0;
            ushort multiplier = 1;
            int toRead = Math.Min(input.Length, 4);

            for (int i = 0; i < toRead; i++)
            {
                ch = input[idx + increment * i];
                byte bt = ParseChar(ch, isHexadecimal);

                if (bt == byte.MaxValue)
                    break;

                read++;

                if (forward)
                {
                    // Shift the current value, and add the new in
                    if (isHexadecimal)
                        val <<= 4;
                    else
                        val *= 10;

                    val += bt;
                }
                else
                {
                    // Shift the new value, add it in
                    val += (ushort)(multiplier * bt);

                    if (isHexadecimal)
                        multiplier *= 16;
                    else
                        multiplier *= 10;
                }
            }

            if (read > 0)
                return new(TokenType.Number, val);
        }

        return new(TokenType.Unknown, 0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte ParseChar(char ch, bool isHexadecimal)
    {
        return ch switch
        {
            >= '0' and <= '9' => (byte)(ch - '0'),
            _ when !isHexadecimal => byte.MaxValue,
            >= 'A' and <= 'F' => (byte)(10 + ch - 'A'),
            >= 'a' and <= 'f' => (byte)(10 + ch - 'a'),
            _ => byte.MaxValue
        };
    }

    public ParsedToken ParseAndAdvanceStart(bool isHexadecimal = true)
    {
        ParsedToken parsed = Interpret(_value, true, isHexadecimal, out int read);

        _value = _value[read..];
        _temp = _value;

        return parsed;
    }

    public ParsedToken PeekStart(bool isHexadecimal = true)
    {
        ParsedToken parsed = Interpret(_temp, true, isHexadecimal, out int read);

        _temp = _temp[read..];

        return parsed;
    }

    public ParsedToken ParseAndAdvanceEnd(bool isHexadecimal = true)
    {
        ParsedToken parsed = Interpret(_value, false, isHexadecimal, out int read);

        _value = _value[..^read];
        _temp = _value;

        return parsed;
    }

    public ParsedToken PeekEnd(bool isHexadecimal = true)
    {
        ParsedToken parsed = Interpret(_temp, false, isHexadecimal, out int read);

        _temp = _temp[..^read];

        return parsed;
    }

    public IEnumerable<ParsedToken> ParseAllStart(bool isHexadecimal = true)
    {
        List<ParsedToken> tokens = new List<ParsedToken>(10);

        ParsedToken parsed;
        while ((parsed = ParseAndAdvanceStart(isHexadecimal)).Type != TokenType.None)
            tokens.Add(parsed);

        return tokens;
    }

    public IEnumerable<ParsedToken> ParseAllEnd(bool isHexadecimal = false)
    {
        List<ParsedToken> tokens = new List<ParsedToken>(10);

        ParsedToken parsed;
        while ((parsed = ParseAndAdvanceEnd(isHexadecimal)).Type != TokenType.None)
            tokens.Add(parsed);

        return tokens;
    }

    public void AdoptPeekOffsets()
    {
        _value = _temp;
    }

    public void ResetPeekOffsets()
    {
        _temp = _value;
    }
}

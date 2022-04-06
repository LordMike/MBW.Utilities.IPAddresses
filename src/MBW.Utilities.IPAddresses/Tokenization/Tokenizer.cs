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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool isHex(char ch) => '0' <= ch && ch <= '9' || 'A' <= ch && ch <= 'F' || 'a' <= ch && ch <= 'f';

        if (isHex(ch))
        {
            // Determine how many chars to parse
            // Note: We know the first char is already hex, so we skip checking that
            int maxRead = Math.Min(input.Length, 4);
            for (read = 1; read < maxRead; read++)
            {
                if (!isHex(input[idx + increment * read]))
                    break;
            }

            // Read entire number from 'char' chars
            ReadOnlySpan<char> numberChars;
            if (forward)
                numberChars = input[..read];
            else
                numberChars = input[^read..];

            int val = 0;

            foreach (char numberChar in numberChars)
            {
                int bt = ParseChar(numberChar, isHexadecimal);

                // Shift the current value, and add the new in
                if (isHexadecimal)
                    val <<= 4;
                else
                    val *= 10;

                val += bt;
            }

            return new(TokenType.Number, (ushort)val);
        }

        return new(TokenType.Unknown, 0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ParseChar(int ch, bool isHexadecimal)
    {
        if ((uint)(ch - '0') <= 9)
            return ch - '0';

        if (!isHexadecimal)
            return int.MaxValue;

        if ((uint)(ch - 'A') <= 5)
            return 10 + ch - 'A';

        if ((uint)(ch - 'a') <= 5)
            return 10 + ch - 'a';

        return int.MaxValue;
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

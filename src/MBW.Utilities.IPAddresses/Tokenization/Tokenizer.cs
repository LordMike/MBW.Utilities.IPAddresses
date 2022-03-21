using System;
using System.Runtime.CompilerServices;

namespace MBW.Utilities.IPAddresses.Tokenization;

internal static class Tokenizer
{
    public static (TokenType type, ushort value) ReadTokenReverse(ReadOnlySpan<char> str, bool isHexadecimal, out int read)
    {
        read = 0;

        if (str.IsEmpty)
            return (TokenType.None, 0);

        char ch = str[str.Length - 1];
        if (ch == '/')
        {
            read = 1;
            return (TokenType.Slash, 0);
        }

        if (ch == '.')
        {
            read = 1;
            return (TokenType.Dot, 0);
        }

        if (ch == ':')
        {
            if (str.Length >= 2 && str[str.Length - 2] == ':')
            {
                read = 2;
                return (TokenType.DoubleColon, 0);
            }

            read = 1;
            return (TokenType.Colon, 0);
        }

        if (ch is >= '0' and <= '9' || ch is >= 'a' and <= 'f' || ch is >= 'A' and <= 'F')
        {
            // Read entire number
            ushort val = 0;
            ushort multiplier = 1;
            int toRead = Math.Min(str.Length, 4) + 1;

            for (int i = 1; i < toRead; i++)
            {
                ch = str[str.Length - i];
                byte bt = ParseChar(ch, isHexadecimal);

                if (bt == byte.MaxValue)
                    break;

                read++;

                val += (ushort)(multiplier * bt);

                if (isHexadecimal)
                    multiplier *= 16;
                else
                    multiplier *= 10;
            }

            return (TokenType.Number, val);
        }

        return (TokenType.Unknown, 0);
    }


    public static (TokenType type, ushort value) ReadToken(ReadOnlySpan<char> str, bool isHexadecimal, out int read)
    {
        read = 0;

        if (str.IsEmpty)
            return (TokenType.None, 0);

        char ch = str[0];
        if (ch == '/')
        {
            read = 1;
            return (TokenType.Slash, 0);
        }

        if (ch == '.')
        {
            read = 1;
            return (TokenType.Dot, 0);
        }

        if (ch == ':')
        {
            if (str.Length >= 2 && str[1] == ':')
            {
                read = 2;
                return (TokenType.DoubleColon, 0);
            }

            read = 1;
            return (TokenType.Colon, 0);
        }

        if (ch is >= '0' and <= '9' || ch is >= 'a' and <= 'f' || ch is >= 'A' and <= 'F')
        {
            // Read entire number
            ushort val = 0;
            int toRead = Math.Min(str.Length, 4);

            for (int i = 0; i < toRead; i++)
            {
                ch = str[i];
                byte bt = ParseChar(ch, isHexadecimal);

                if (bt == byte.MaxValue)
                    break;

                read++;

                if (isHexadecimal)
                    val <<= 4;
                else
                    val *= 10;

                val += bt;
            }

            return (TokenType.Number, val);
        }

        return (TokenType.Unknown, 0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte ParseChar(char ch, bool isHexadecimal)
    {
        if (ch is >= '0' and <= '9')
            return (byte)(ch - '0');

        if (!isHexadecimal)
            return byte.MaxValue;

        if (ch is >= 'A' and <= 'F')
            return (byte)(10 + ch - 'A');

        if (ch is >= 'a' and <= 'f')
            return (byte)(10 + ch - 'a');

        return byte.MaxValue;
    }
}
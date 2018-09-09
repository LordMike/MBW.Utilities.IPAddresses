using System;
using System.Net;

namespace MBW.Utilities.IPAddresses
{
    public partial struct IpAddressRangeV6
    {
        public static IpAddressRangeV6 Parse(string value)
        {
            if (TryParse(value, out IpAddressRangeV6 result))
                return result;

            throw new ArgumentException("Argument was not a valid IPv6 range", nameof(value));
        }

        public static bool TryParse(string value, out IpAddressRangeV6 result)
        {
            if (string.IsNullOrEmpty(value) || value.Length > 49)
            {
                result = default;
                return false;
            }

            ReadOnlySpan<char> @string = value.AsSpan();

            ulong high = 0;
            ulong low = 0;
            byte mask = 0;
            byte idx = 1;

            void SetTuplet(byte index, ulong newValue)
            {
                if (index < 5)
                    high |= newValue << ((3 - index) * 16);
                else
                    low |= newValue << ((7 - index) * 16);
            }

            byte ParseChar(char ch)
            {
                if ('0' <= ch && ch <= '9')
                    return (byte)(ch - '0');

                if ('A' <= ch && ch <= 'F')
                    return (byte)(10 + ch - 'A');

                if ('a' <= ch && ch <= 'f')
                    return (byte)(10 + ch - 'a');

                return byte.MaxValue;
            }

            // Find mask, read reverse
            for (int i = @string.Length - 1; i >= 0; i--)
            {
                char ch = @string[i];

                if (ch == ':' || 'A' <= ch && ch <= 'F' || 'a' <= ch && ch <= 'f')
                {
                    // No mask provided
                    mask = 128;
                    break;
                }

                if (ch == '/')
                {
                    // Mask has been read, cut off the mask
                    @string = @string.Slice(0, i);
                    break;
                }

                if ('0' <= ch && ch <= '9')
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

            while (@string.Length > 0)
            {
                ushort currentTuplet = 0;

                // Read up to 4 chars
                int i;
                for (i = 0; i < 4; i++)
                {
                    char ch = @string[i];

                    if (ch == ':')
                        break;

                    // Read and append char
                    byte val = ParseChar(ch);

                    if (val == byte.MaxValue)
                        throw new ArgumentException();

                    currentTuplet <<= 4;
                    currentTuplet += val;
                }

                SetTuplet(idx++, currentTuplet);
                @string = @string.Slice(i);

                // Check if next char is ':'
                if (@string[0] == ':')
                {
                    @string = @string.Slice(1);

                    // Check if next again is ':' too
                    if (@string[0] == ':')
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

                while (@string.Length > 0)
                {
                    ushort currentTuplet = 0;

                    // Read up to 4 chars
                    // Find ':'

                    int lastIdx = @string.LastIndexOf(':');

                    ReadOnlySpan<char> segment = @string.Slice(lastIdx + 1);
                    @string = @string.Slice(0, lastIdx);

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

                    SetTuplet(idx--, currentTuplet);
                }
            }

            result = new IpAddressRangeV6(high, low, mask);
            return true;
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
}
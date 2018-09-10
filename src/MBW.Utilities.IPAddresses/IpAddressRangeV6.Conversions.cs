using System;
using System.Net;
using MBW.Utilities.IPAddresses.Helpers;

namespace MBW.Utilities.IPAddresses
{
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
            byte mask = 0;
            byte idx = 1;

            void SetTuplet(byte index, ulong newValue)
            {
                if (index < 4)
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
            for (int i = value.Length - 1; i >= 0; i--)
            {
                char ch = value[i];

                if (ch == ':' || ch == '.' || 'A' <= ch && ch <= 'F' || 'a' <= ch && ch <= 'f')
                {
                    // No mask provided
                    mask = 128;
                    break;
                }

                if (ch == '/')
                {
                    // Mask has been read, cut off the mask
                    value = value.Slice(0, i);
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

                SetTuplet(idx++, currentTuplet);
                value = value.Slice(i);

                if (value.Length == 0)
                    break;

                // Check if next char is ':'
                if (value[0] == ':')
                {
                    value = value.Slice(1);

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
                        if (!IpAddressRangeV4.TryParse(value.Slice(1), out var ipv4))
                        {
                            result = default;
                            return false;
                        }

                        low += ipv4.AddressUint;
                        result = new IpAddressRangeV6(high, low, mask);

                        return true;
                    }

                    if (lastIdx <= 0 && value.Length > 5)
                    {
                        // This is definitely not IPv6
                        result = default;
                        return false;
                    }

                    ushort currentTuplet = 0;

                    ReadOnlySpan<char> segment = value.Slice(lastIdx + 1);
                    value = value.Slice(0, lastIdx);

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
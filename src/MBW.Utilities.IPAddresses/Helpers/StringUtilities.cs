using System;

namespace MBW.Utilities.IPAddresses.Helpers
{
    internal static class StringUtilities
    {
        public static int ReverseIndexOf(ReadOnlySpan<char> str, char needle)
        {
            for (int i = str.Length - 1; i >= 0; i--)
            {
                if (str[i] == needle)
                    return i;
            }

            return -1;
        }
    }
}
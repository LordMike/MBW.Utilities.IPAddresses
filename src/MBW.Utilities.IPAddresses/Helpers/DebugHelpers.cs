using System;
using System.Linq;

namespace IPAddresses
{
    internal static class DebugHelpers
    {
        public static string ToHex(this ulong val)
        {
            return BitConverter.ToString(BitConverter.GetBytes(val).Reverse().ToArray()).Replace("-", "");
        }
        public static string ToHex(this uint val)
        {
            return BitConverter.ToString(BitConverter.GetBytes(val).Reverse().ToArray()).Replace("-", "");
        }
    }
}
using System;
using System.Linq;

namespace MBW.Utilities.IPAddresses.Helpers;

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
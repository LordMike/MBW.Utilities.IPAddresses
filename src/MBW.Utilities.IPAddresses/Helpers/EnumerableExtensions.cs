using System.Collections.Generic;
using System.Linq;

namespace MBW.Utilities.IPAddresses.Helpers;

internal static class EnumerableExtensions
{
    public static IList<T> ToListNoCopy<T>(this IEnumerable<T> enumerable)
    {
        return enumerable as IList<T> ?? enumerable.ToArray();
    }
}
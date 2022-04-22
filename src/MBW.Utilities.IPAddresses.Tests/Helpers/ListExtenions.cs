using System;
using System.Collections.Generic;

namespace MBW.Utilities.IPAddresses.Tests.SignaturesLib;

internal static class ListExtenions
{
    public static T RemoveGet<T>(this IList<T> set, Func<T, bool> filter)
    {
        for (int i = 0; i < set.Count; i++)
        {
            var item = set[i];
            if (!filter(item))
                continue;

            set.RemoveAt(i);
            return item;
        }

        throw new Exception("Unable to locate item");
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Xunit;

namespace MBW.Utilities.IPAddresses.Tests;

public class ApiSurfaceStandardizationTests
{
    private static readonly Dictionary<Type, HashSet<string>> _types;

    static ApiSurfaceStandardizationTests()
    {
        HashSet<string> GetMethods<T>()
        {
            Type type = typeof(T);
            Regex replacer = new Regex("\\b" + Regex.Escape(type.FullName) + "\\b", RegexOptions.Compiled);

            return type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                .Select(s => (s.IsStatic ? "static " : "instance ") + s)
                .Select(s => replacer.Replace(s, "T"))
                .ToHashSet(StringComparer.Ordinal);
        }

        _types = new Dictionary<Type, HashSet<string>>
        {
            { typeof(IpAddressNetworkV4), GetMethods<IpAddressNetworkV4>() },
            { typeof(IpAddressNetworkV6), GetMethods<IpAddressNetworkV6>() },
            { typeof(IpAddressNetwork), GetMethods<IpAddressNetwork>() }
        };
    }

    public static IEnumerable<object[]> GetMethodSignatures()
    {
        Regex tReplacer = new Regex(@"\bT\b");

        var specificTypes = new[] { typeof(IpAddressNetworkV4), typeof(IpAddressNetworkV6) };

        object[] Make<T>(string signature) => new object[] { typeof(T), signature };

        IEnumerable<object[]> MakeAll(string signature) => new[] { Make<IpAddressNetworkV4>(signature), Make<IpAddressNetworkV6>(signature), Make<IpAddressNetwork>(signature) };
        IEnumerable<object[]> MakeWithT<T>(string signature, Type[] tTypes) => tTypes.Select(s => tReplacer.Replace(signature, s.FullName)).Select(Make<T>);

        return
            Enumerable.Empty<object[]>()
                // .NET Stuff
                .Concat(MakeAll("instance System.String ToString()"))
                // Generic equality
                .Concat(MakeAll("instance Int32 CompareTo(System.Object)"))
                .Concat(MakeAll("instance Int32 CompareTo(T)"))
                .Concat(MakeAll("instance Boolean Equals(T)"))
                .Concat(MakeAll("instance Boolean Equals(System.Object)"))
                // All address ranges have a mask
                .Concat(MakeAll("instance Byte get_Mask()"))
                // Set methods
                .Concat(MakeAll("instance Boolean Contains(T)"))
                .Concat(MakeWithT<IpAddressNetwork>("instance Boolean Contains(T)", specificTypes))
                .Concat(MakeAll("instance Boolean ContainsOrEqual(T)"))
                .Concat(MakeWithT<IpAddressNetwork>("instance Boolean ContainsOrEqual(T)", specificTypes))
                .Concat(MakeAll("instance Boolean IsContainedIn(T)"))
                .Concat(MakeWithT<IpAddressNetwork>("instance Boolean IsContainedIn(T)", specificTypes))
                .Concat(MakeAll("instance Boolean IsContainedInOrEqual(T)"))
                .Concat(MakeWithT<IpAddressNetwork>("instance Boolean IsContainedInOrEqual(T)", specificTypes))
                .Concat(MakeAll("static T MakeSupernet(System.Collections.Generic.IEnumerable`1[T])"))
                .Concat(MakeAll("static T MakeSupernet(T[])"))
                // Utilities to convert to .NET
                .Concat(MakeAll("instance System.Net.IPAddress get_Address()"))
                .Concat(MakeAll("instance System.Net.IPAddress get_EndAddress()"))
                // Conversions
                .Concat(MakeAll("static Boolean TryParse(System.ReadOnlySpan`1[System.Char], T ByRef)"))
                .Concat(MakeAll("static T Parse(System.ReadOnlySpan`1[System.Char])"))
                .Concat(MakeAll("static T op_Explicit(System.String)"))
                .Concat(MakeAll("static T op_Explicit(System.ReadOnlySpan`1[System.Char])"))
                .Concat(MakeAll("static T op_Implicit(System.Net.IPAddress)"))
                .Concat(MakeAll("static System.Net.IPAddress op_Explicit(T)"))
                .Concat(MakeAll("instance Byte[] AddressToBytes()"))
                // Operators methods
                .Concat(MakeAll("static Boolean op_Equality(T, T)"))
                .Concat(MakeAll("static Boolean op_Inequality(T, T)"))
                .Concat(MakeAll("static Boolean op_LessThan(T, T)"))
                .Concat(MakeAll("static Boolean op_LessThanOrEqual(T, T)"))
                .Concat(MakeAll("static Boolean op_GreaterThan(T, T)"))
                .Concat(MakeAll("static Boolean op_GreaterThanOrEqual(T, T)"))
                // IpAddressRange specific
                .Append(Make<IpAddressNetwork>("static T op_Implicit(MBW.Utilities.IPAddresses.IpAddressNetworkV4)"))
                .Append(Make<IpAddressNetwork>("static T op_Implicit(MBW.Utilities.IPAddresses.IpAddressNetworkV6)"));
    }

    /// <summary>
    /// Ensure that the specific types have the specified methods
    /// This guarantees the API surface is consistent
    /// </summary>
    [Theory]
    [MemberData(nameof(GetMethodSignatures))]
    public void MustHaveMethod2(Type type, string signature)
    {
        Assert.Contains(signature, _types[type]);
    }
}
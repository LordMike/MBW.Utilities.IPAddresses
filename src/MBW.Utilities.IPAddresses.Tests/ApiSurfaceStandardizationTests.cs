using MBW.Tests.SignaturesTester;
using MBW.Utilities.IPAddresses.Tests.SignaturesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Xunit;

namespace MBW.Utilities.IPAddresses.Tests;

public class ApiSurfaceStandardizationTests
{
    private static readonly TypesMethodStore _store;
    private static readonly Dictionary<Type, HashSet<string>> _types = new();

    static ApiSurfaceStandardizationTests()
    {
        _store = new();

        _store.Add<IpAddressNetworkV4>();
        _store.Add<IpAddressNetworkV6>();
        _store.Add<IpAddressV4>();
        _store.Add<IpAddressV6>();
    }

    public static IEnumerable<object[]> GetMethodSignatures()
    {
        Type GetAddressType(Type networkType)
        {
            if (networkType == typeof(IpAddressNetworkV4))
                return typeof(IpAddressV4);
            if (networkType == typeof(IpAddressNetworkV6))
                return typeof(IpAddressV6);
            throw new ArgumentOutOfRangeException(nameof(networkType));
        }
        Type GetNetworkType(Type addressType)
        {
            if (addressType == typeof(IpAddressV4))
                return typeof(IpAddressNetworkV4);
            if (addressType == typeof(IpAddressV6))
                return typeof(IpAddressNetworkV6);
            throw new ArgumentOutOfRangeException(nameof(addressType));
        }

        SignaturesBuilder ipTypes = SignaturesBuilder.Types<IpAddressV4, IpAddressV6>()
            .MustHaveComparer()
            .MustHaveEquality()
            .MustHaveParseMethods()
            .MustHaveToStringMethods()
            .MustHaveProperty("Min", type => type, hasSet: false, isInstance: false)
            .MustHaveProperty("Max", type => type, hasSet: false, isInstance: false)
            .MustHave("instance Boolean IsContainedIn(T1)", GetNetworkType)
            .MustHave("static System.Net.IPAddress op_Explicit(TSelf)")
            .MustHave("static TSelf op_BitwiseAnd(TSelf, TSelf)")
            .MustHave("static TSelf op_BitwiseOr(TSelf, TSelf)");

        SignaturesBuilder networkTypes = SignaturesBuilder.Types<IpAddressNetworkV4, IpAddressNetworkV6>()
            .MustHaveComparer()
            .MustHaveEquality()
            .MustHaveParseMethods()
            .MustHaveToStringMethods()
            .MustHaveProperty<byte>("Mask", hasSet: false)
            .MustHave("instance Boolean Contains(TSelf)")
            .MustHave("instance Boolean Contains(T1)", GetAddressType)
            .MustHave("instance Boolean ContainsOrEqual(TSelf)")
            .MustHave("instance System.Collections.Generic.IEnumerable`1[TSelf] Split(Byte)")
            .MustHave("static TSelf MakeSupernet(System.Collections.Generic.IEnumerable`1[TSelf])")
            .MustHave("static TSelf MakeSupernet(System.Collections.Generic.IEnumerable`1[T1])", GetAddressType)
            .MustHave("static TSelf op_Implicit(T1)", GetAddressType)
            .MustHaveProperty("NetworkAddress", GetAddressType, hasSet: false)
            .MustHaveProperty("NetworkWildcardMask", GetAddressType, hasSet: false)
            .MustHaveProperty("NetworkMask", GetAddressType, hasSet: false);

        SignaturesBuilder networkV4Special = SignaturesBuilder.Types<IpAddressNetworkV4>()
            .MustHaveProperty<uint>("SubnetSize", hasSet: false)
            .MustHaveProperty<uint>("SubnetHostsSize", hasSet: false)
            .MustHaveProperty("BroadcastAddress", GetAddressType, hasSet: false);

        SignaturesBuilder networkV6Special = SignaturesBuilder.Types<IpAddressNetworkV6>()
            .MustHaveProperty<BigInteger>("SubnetSize", hasSet: false)
            .MustHaveProperty("EndAddress", GetAddressType, hasSet: false);

        return ipTypes.GetDesiredSignatures()
            .Concat(networkTypes.GetDesiredSignatures())
            .Concat(networkV4Special.GetDesiredSignatures())
            .Concat(networkV6Special.GetDesiredSignatures())
            .Select(s => new object[] { s.signature, s.type });
    }

    /// <summary>
    /// Ensure that the specific types have the specified methods
    /// This guarantees the API surface is consistent
    /// </summary>
    [Theory]
    [MemberData(nameof(GetMethodSignatures))]
    public void ApiSurfaceTest(string signature, Type type)
    {
        // Note: Do not use Should().Contain(), it produces very long and useless assertions
        bool res = _store.HasMethod(type, signature);

        if (!res)
        {
            string closest = _store.GetClosestMethod(type, signature);

            Assert.False(true, $"Type '{type.Name}'\n" +
                $"  Should contain '{signature}'\n" +
                $"  Closest:       '{closest}'");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Dirichlet.Numerics;
using MBW.Utilities.IPAddresses.Helpers;

namespace MBW.Utilities.IPAddresses;

public partial struct IpAddressNetworkV6
{
    /// <inheritdoc cref="Docs.IIPAddressNetworkDocs{IpAddressNetworkV6}.Contains(IpAddressNetworkV6)"/>
    public bool Contains(IpAddressNetworkV6 other)
    {
        if (other.Mask <= _mask)
            return false;

        IpAddressV6 sharedNetwork = NetworkMask & other._networkAddress;
        return sharedNetwork == _networkAddress;
    }

    /// <inheritdoc cref="Docs.IIPAddressNetworkDocs{IpAddressNetworkV6}.ContainsOrEqual(IpAddressNetworkV6)"/>
    public bool ContainsOrEqual(IpAddressNetworkV6 other)
    {
        if (other.Mask < _mask)
            return false;

        IpAddressV6 sharedNetwork = NetworkMask & other._networkAddress;
        return sharedNetwork == _networkAddress;
    }

    public bool Contains(IpAddressV6 other)
    {
        IpAddressV6 mask = NetworkMask;
        return (mask & other) == NetworkAddress;
    }

    public bool IsContainedIn(IpAddressNetworkV6 other)
    {
        return other.Contains(this);
    }

    public bool IsContainedInOrEqual(IpAddressNetworkV6 other)
    {
        return other.ContainsOrEqual(this);
    }

    public static IpAddressNetworkV6 MakeSupernet(IEnumerable<IpAddressNetworkV6> others)
    {
        bool hadAny = false;
        byte shortestMask = 128;
        UInt128 final = UInt128.MaxValue;

        foreach (IpAddressNetworkV6 other in others)
        {
            final &= other._networkAddress.Address;

            byte lowestCommon = BitUtilities.FindCommonPrefixSize(final, other._networkAddress.Address);
            shortestMask = Math.Min(shortestMask, lowestCommon);

            hadAny = true;
        }

        if (!hadAny)
            throw new ArgumentException("Input was empty", nameof(others));

        return new IpAddressNetworkV6(new IpAddressV6(final), shortestMask);
    }

    public static IpAddressNetworkV6 MakeSupernet(params IpAddressNetworkV6[] others) => MakeSupernet((IEnumerable<IpAddressNetworkV6>)others);
    public static IpAddressNetworkV6 MakeSupernet(params IpAddressV6[] others) => MakeSupernet((IEnumerable<IpAddressV6>)others);
    public static IpAddressNetworkV6 MakeSupernet(IEnumerable<IpAddressV6> others) => MakeSupernet(others.Cast<IpAddressNetworkV6>());

    public void AddressToBytes(Span<byte> bytes)
    {
        if (bytes.Length < 16)
            throw new ArgumentOutOfRangeException(nameof(bytes));

        ulong high = _networkAddress.AddressHigh;
        ulong low = _networkAddress.AddressLow;

        bytes[0] = (byte)((high >> 56) & 0xFF);
        bytes[1] = (byte)((high >> 48) & 0xFF);
        bytes[2] = (byte)((high >> 40) & 0xFF);
        bytes[3] = (byte)((high >> 32) & 0xFF);
        bytes[4] = (byte)((high >> 24) & 0xFF);
        bytes[5] = (byte)((high >> 16) & 0xFF);
        bytes[6] = (byte)((high >> 8) & 0xFF);
        bytes[7] = (byte)(high & 0xFF);
        bytes[8] = (byte)((low >> 56) & 0xFF);
        bytes[9] = (byte)((low >> 48) & 0xFF);
        bytes[10] = (byte)((low >> 40) & 0xFF);
        bytes[11] = (byte)((low >> 32) & 0xFF);
        bytes[12] = (byte)((low >> 24) & 0xFF);
        bytes[13] = (byte)((low >> 16) & 0xFF);
        bytes[14] = (byte)((low >> 8) & 0xFF);
        bytes[15] = (byte)(low & 0xFF);
    }

    public byte[] AddressToBytes()
    {
        byte[] res = new byte[16];
        AddressToBytes(res);

        return res;
    }

    public void AddressToBytes(byte[] bytes, int offset = 0)
    {
        if (bytes == null)
            throw new ArgumentNullException(nameof(bytes));

        AddressToBytes(bytes.AsSpan().Slice(offset, 16));
    }
}
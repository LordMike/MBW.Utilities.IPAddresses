using System;
using System.Collections.Generic;
using MBW.Utilities.IPAddresses.Helpers;

namespace MBW.Utilities.IPAddresses;

public partial struct IpAddressNetworkV6
{
    /// <inheritdoc cref="Docs.IIPAddressNetworkDocs{IpAddressNetworkV6}.Contains(IpAddressNetworkV6)"/>
    public bool Contains(IpAddressNetworkV6 other)
    {
        bool canBeInNetwork = _mask < other._mask;

        if (_mask >= 64)
        {
            ulong otherMaskedToThisNetworkLow = other.NetworkAddress.AddressLow & (uint.MaxValue << _mask);
            bool highMatches = other.NetworkAddress.AddressHigh == _networkAddress.AddressHigh;
            bool isInNetwork = (otherMaskedToThisNetworkLow & _networkAddress.AddressLow) == otherMaskedToThisNetworkLow;

            return canBeInNetwork && highMatches && isInNetwork;
        }
        else
        {
            ulong otherMaskedToThisNetworkHigh = other.NetworkAddress.AddressHigh & (uint.MaxValue << _mask);
            bool isInNetwork = (otherMaskedToThisNetworkHigh & _networkAddress.AddressHigh) == otherMaskedToThisNetworkHigh;

            return canBeInNetwork && isInNetwork;
        }
    }

    /// <inheritdoc cref="Docs.IIPAddressNetworkDocs{IpAddressNetworkV6}.ContainsOrEqual(IpAddressNetworkV6)"/>
    public bool ContainsOrEqual(IpAddressNetworkV6 other)
    {
        bool canBeInNetwork = _mask <= other._mask;

        if (_mask >= 64)
        {
            ulong otherMaskedToThisNetworkLow = other.NetworkAddress.AddressLow & (uint.MaxValue << _mask);
            bool highMatches = other.NetworkAddress.AddressHigh == _networkAddress.AddressHigh;
            bool isInNetwork = (otherMaskedToThisNetworkLow & _networkAddress.AddressLow) == otherMaskedToThisNetworkLow;

            return canBeInNetwork && highMatches && isInNetwork;
        }
        else
        {
            ulong otherMaskedToThisNetworkHigh = other.NetworkAddress.AddressHigh & (uint.MaxValue << _mask);
            bool isInNetwork = (otherMaskedToThisNetworkHigh & _networkAddress.AddressHigh) == otherMaskedToThisNetworkHigh;

            return canBeInNetwork && isInNetwork;
        }
    }

    public bool Contains(IpAddressV6 other)
    {
        throw new NotImplementedException();
    }

    public bool ContainsOrEqual(IpAddressV6 other)
    {
        throw new NotImplementedException();
    }

    public bool IsContainedIn(IpAddressNetworkV6 other)
    {
        return other.Contains(this);
    }

    public bool IsContainedInOrEqual(IpAddressNetworkV6 other)
    {
        return other.ContainsOrEqual(this);
    }

    public static IpAddressNetworkV6 MakeSupernet(params IpAddressNetworkV6[] others)
    {
        return MakeSupernet((IEnumerable<IpAddressNetworkV6>)others);
    }

    public static IpAddressNetworkV6 MakeSupernet(IEnumerable<IpAddressNetworkV6> others)
    {
        bool hadAny = false;
        byte shortestMask = 128;
        ulong finalHigh = uint.MaxValue;
        ulong finalLow = uint.MaxValue;

        foreach (IpAddressNetworkV6 range in others)
        {
            finalHigh &= range.NetworkAddress.AddressHigh;
            finalLow &= range.NetworkAddress.AddressLow;

            byte highMask = BitUtilities.FindCommonPrefixSize(finalHigh, range.NetworkAddress.AddressHigh);
            if (highMask == 64)
            {
                byte lowMask = (byte)(64 + BitUtilities.FindCommonPrefixSize(finalLow, range.NetworkAddress.AddressLow));
                shortestMask = Math.Min(shortestMask, lowMask);
            }
            else
            {
                shortestMask = Math.Min(shortestMask, highMask);
            }

            hadAny = true;
        }

        if (!hadAny)
            throw new ArgumentException("Input was empty", nameof(others));

        return new IpAddressNetworkV6(finalHigh, finalLow, shortestMask);
    }

    public static IpAddressNetworkV6 MakeSupernet(params IpAddressV6[] others)
    {
        return MakeSupernet((IEnumerable<IpAddressV6>)others);
    }

    public static IpAddressNetworkV6 MakeSupernet(IEnumerable<IpAddressV6> others)
    {
        throw new NotImplementedException();
    }

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
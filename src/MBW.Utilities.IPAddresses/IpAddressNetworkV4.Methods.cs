using System;
using System.Collections.Generic;
using System.Text;
using MBW.Utilities.IPAddresses.Helpers;

namespace MBW.Utilities.IPAddresses;

public partial struct IpAddressNetworkV4
{
    public bool Contains(IpAddressNetworkV4 other)
    {
        // In addition to ContainsOrEqual, the mask should be smaller as to not be equal
        return _mask < other._mask && ContainsOrEqual(other);
    }

    public bool ContainsOrEqual(IpAddressNetworkV4 other)
    {
        // Ensure the network part of both this and other are the same
        uint thisNetwork = _address;
        uint otherNetwork = other._address & NetworkMask;

        return thisNetwork == otherNetwork;
    }

    public bool IsContainedIn(IpAddressNetworkV4 other)
    {
        return other.Contains(this);
    }

    public bool IsContainedInOrEqual(IpAddressNetworkV4 other)
    {
        return other.ContainsOrEqual(this);
    }

    public static IpAddressNetworkV4 MakeSupernet(params IpAddressNetworkV4[] others)
    {
        return MakeSupernet((IEnumerable<IpAddressNetworkV4>)others);
    }

    public static IpAddressNetworkV4 MakeSupernet(IEnumerable<IpAddressNetworkV4> others)
    {
        byte shortestMask = 32;
        bool hadAny = false;
        uint final = uint.MaxValue;

        foreach (IpAddressNetworkV4 range in others)
        {
            final &= range._address;

            byte lowestCommon = BitUtilities.FindCommonPrefixSize(final, range._address);
            shortestMask = Math.Min(shortestMask, lowestCommon);

            hadAny = true;
        }

        if (!hadAny)
            throw new ArgumentException("Input was empty", nameof(others));

        return new IpAddressNetworkV4(final, shortestMask);
    }

    public override string ToString()
    {
        return ToString(false);
    }

    public string ToString(bool forceCidr)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(Address);

        if (forceCidr || _mask != 32)
            sb.Append("/").Append(_mask.ToString());

        return sb.ToString();
    }

    public string ToPrefixString()
    {
        if (_mask <= 8 && (_address & 0xFFFFFF) == 0)
        {
            // Return 1 octet
            return (_address >> 24) + "/" + _mask;
        }

        if (_mask <= 16 && (_address & 0xFFFF) == 0)
        {
            // Return 2 octets
            return (_address >> 24) + "." + ((_address >> 16) & 0xFF) + "/" + _mask;
        }
        if (_mask <= 24 && (_address & 0xFF) == 0)
        {
            // Return 3 octets
            return (_address >> 24) + "." + ((_address >> 16) & 0xFF) + "." + ((_address >> 8) & 0xFF) + "/" + _mask;

        }

        return ToString(false);
    }

    public byte[] AddressToBytes()
    {
        byte[] res = new byte[4];
        res[0] = (byte)((_address >> 24) & 0xFF);
        res[1] = (byte)((_address >> 16) & 0xFF);
        res[2] = (byte)((_address >> 8) & 0xFF);
        res[3] = (byte)(_address & 0xFF);

        return res;
    }

    public void AddressToBytes(byte[] bytes, int offset = 0)
    {
        if (bytes == null)
            throw new ArgumentNullException(nameof(bytes));

        if (bytes.Length - offset < 4)
            throw new ArgumentOutOfRangeException(nameof(offset));

        bytes[offset] = (byte)((_address >> 24) & 0xFF);
        bytes[offset + 1] = (byte)((_address >> 16) & 0xFF);
        bytes[offset + 2] = (byte)((_address >> 8) & 0xFF);
        bytes[offset + 3] = (byte)(_address & 0xFF);
    }
}
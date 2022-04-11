using System;
using System.Collections.Generic;
using System.Linq;
using MBW.Utilities.IPAddresses.Helpers;

namespace MBW.Utilities.IPAddresses;

public partial struct IpAddressNetworkV4
{
    /// <inheritdoc cref="Docs.IIPAddressNetworkDocs{IpAddressNetworkV4}.Contains(IpAddressNetworkV4)"/>
    public bool Contains(IpAddressNetworkV4 other)
    {
        // In addition to ContainsOrEqual, the mask should be smaller as to not be equal
        return _mask < other._mask && ContainsOrEqual(other);
    }

    /// <inheritdoc cref="Docs.IIPAddressNetworkDocs{IpAddressNetworkV4}.ContainsOrEqual(IpAddressNetworkV4)"/>
    public bool ContainsOrEqual(IpAddressNetworkV4 other)
    {
        if (other.Mask < _mask)
            return false;

        IpAddressV4 sharedNetwork = NetworkMask & other.NetworkAddress;
        return sharedNetwork == _networkAddress;
    }

    public bool Contains(IpAddressV4 other)
    {
        IpAddressV4 mask = NetworkMask;
        return (mask & other) == NetworkAddress;
    }

    public bool IsContainedIn(IpAddressNetworkV4 other)
    {
        return other.Contains(this);
    }

    public bool IsContainedInOrEqual(IpAddressNetworkV4 other)
    {
        return other.ContainsOrEqual(this);
    }

    public static IpAddressNetworkV4 MakeSupernet(IEnumerable<IpAddressNetworkV4> others)
    {
        byte shortestMask = 32;
        bool hadAny = false;
        uint final = uint.MaxValue;

        foreach (IpAddressNetworkV4 range in others)
        {
            final &= range._networkAddress.AddressUint;

            byte lowestCommon = BitUtilities.FindCommonPrefixSize(final, range._networkAddress.AddressUint);
            shortestMask = Math.Min(shortestMask, lowestCommon);

            hadAny = true;
        }

        if (!hadAny)
            throw new ArgumentException("Input was empty", nameof(others));

        return new IpAddressNetworkV4(final, shortestMask);
    }

    public static IpAddressNetworkV4 MakeSupernet(params IpAddressNetworkV4[] others) => MakeSupernet((IEnumerable<IpAddressNetworkV4>)others);
    public static IpAddressNetworkV4 MakeSupernet(params IpAddressV4[] others) => MakeSupernet((IEnumerable<IpAddressV4>)others);
    public static IpAddressNetworkV4 MakeSupernet(IEnumerable<IpAddressV4> others) => MakeSupernet(others.Cast<IpAddressNetworkV4>());

    public void AddressToBytes(Span<byte> bytes)
    {
        if (bytes.Length < 4)
            throw new ArgumentOutOfRangeException(nameof(bytes));

        uint asUint = _networkAddress.AddressUint;
        bytes[0] = (byte)((asUint >> 24) & 0xFF);
        bytes[1] = (byte)((asUint >> 16) & 0xFF);
        bytes[2] = (byte)((asUint >> 8) & 0xFF);
        bytes[3] = (byte)(asUint & 0xFF);
    }

    public byte[] AddressToBytes()
    {
        byte[] res = new byte[4];
        AddressToBytes(res);

        return res;
    }

    public void AddressToBytes(byte[] bytes, int offset = 0)
    {
        if (bytes == null)
            throw new ArgumentNullException(nameof(bytes));

        AddressToBytes(bytes.AsSpan().Slice(offset, 4));
    }

    public IEnumerable<IpAddressNetworkV4> Split(byte maskIncrement = 1)
    {
        int newMaskInt = _mask + maskIncrement;
        if (newMaskInt <= _mask || newMaskInt > 32)
            throw new ArgumentOutOfRangeException($"The increment ({maskIncrement}) must place the new mask between this networks mask ({_mask}) and 32", nameof(maskIncrement));

        byte newMask = (byte)newMaskInt;

        // Determine last network
        // We use this to determine when to stop producing networks. Other approaches may lead to infinite loops (when running unchecked arithmetics)
        uint networksBitmask = uint.MaxValue >> (32 - maskIncrement);
        IpAddressV4 lastNetwork = new IpAddressV4(_networkAddress.AddressUint | (networksBitmask << (32 - newMask)));

        // Create increment value
        uint incrementVal = 1U << (32 - newMask);
        IpAddressV4 currentNet = _networkAddress;

        // Ensure we can fail fast on the arguments
        IEnumerable<IpAddressNetworkV4> Enumerate()
        {
            // Produce networks as long as we're within our current network
            yield return new IpAddressNetworkV4(currentNet, newMask);

            do
            {
                currentNet = new IpAddressV4(currentNet.AddressUint + incrementVal);
                yield return new IpAddressNetworkV4(currentNet, newMask);
            } while (currentNet != lastNetwork);
        }

        return Enumerate();
    }
}
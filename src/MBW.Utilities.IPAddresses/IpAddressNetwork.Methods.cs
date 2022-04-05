using System;
using System.Collections.Generic;
using System.Linq;
using MBW.Utilities.IPAddresses.Helpers;

namespace MBW.Utilities.IPAddresses;

public partial class IpAddressNetwork
{
    public bool Contains(IpAddressNetwork other)
    {
        return other.Type switch
        {
            IpAddressRangeType.IPv4 when other.Type == IpAddressRangeType.IPv4 => _v4!.Value.Contains(other._v4!.Value),
            IpAddressRangeType.IPv6 when other.Type == IpAddressRangeType.IPv6 => _v6!.Value.Contains(other._v6!.Value),
            _ => throw new ArgumentException("This range is not of the same type as the other range")
        };
    }

    public bool Contains(IpAddressNetworkV4 other) => GetV4().Contains(other);
    public bool Contains(IpAddressNetworkV6 other) => GetV6().Contains(other);

    public bool ContainsOrEqual(IpAddressNetwork other)
    {
        return other.Type switch
        {
            IpAddressRangeType.IPv4 when other.Type == IpAddressRangeType.IPv4 => _v4!.Value.ContainsOrEqual(other._v4!.Value),
            IpAddressRangeType.IPv6 when other.Type == IpAddressRangeType.IPv6 => _v6!.Value.ContainsOrEqual(other._v6!.Value),
            _ => throw new ArgumentException("This range is not of the same type as the other range")
        };
    }

    public bool ContainsOrEqual(IpAddressNetworkV4 other) => GetV4().ContainsOrEqual(other);
    public bool ContainsOrEqual(IpAddressNetworkV6 other) => GetV6().ContainsOrEqual(other);

    public bool IsContainedIn(IpAddressNetwork other)
    {
        return other.Type switch
        {
            IpAddressRangeType.IPv4 when other.Type == IpAddressRangeType.IPv4 => _v4!.Value.IsContainedIn(other._v4!.Value),
            IpAddressRangeType.IPv6 when other.Type == IpAddressRangeType.IPv6 => _v6!.Value.IsContainedIn(other._v6!.Value),
            _ => throw new ArgumentException("This range is not of the same type as the other range")
        };
    }

    public bool IsContainedIn(IpAddressNetworkV4 other) => GetV4().IsContainedIn(other);
    public bool IsContainedIn(IpAddressNetworkV6 other) => GetV6().IsContainedIn(other);

    public bool IsContainedInOrEqual(IpAddressNetwork other)
    {
        return other.Type switch
        {
            IpAddressRangeType.IPv4 when other.Type == IpAddressRangeType.IPv4 => _v4!.Value.IsContainedInOrEqual(other._v4!.Value),
            IpAddressRangeType.IPv6 when other.Type == IpAddressRangeType.IPv6 => _v6!.Value.IsContainedInOrEqual(other._v6!.Value),
            _ => throw new ArgumentException("This range is not of the same type as the other range")
        };
    }

    public bool IsContainedInOrEqual(IpAddressNetworkV4 other) => GetV4().IsContainedInOrEqual(other);
    public bool IsContainedInOrEqual(IpAddressNetworkV6 other) => GetV6().IsContainedInOrEqual(other);

    public static IpAddressNetwork MakeSupernet(params IpAddressNetwork[] others)
    {
        return MakeSupernet((IEnumerable<IpAddressNetwork>)others);
    }

    public static IpAddressNetwork MakeSupernet(IEnumerable<IpAddressNetwork> others)
    {
        IList<IpAddressNetwork> input = others.ToListNoCopy();

        if (!input.Any())
            throw new ArgumentException("Input was empty", nameof(others));

        IpAddressRangeType type = input[0].Type;

        foreach (IpAddressNetwork range in input)
        {
            if (type != IpAddressRangeType.Unknown && type != range.Type)
                throw new ArgumentException("All ranges must be either IPv4 or IPv6, not a mixture of both.");
        }

        return type switch
        {
            IpAddressRangeType.IPv4 => IpAddressNetworkV4.MakeSupernet(input.Select(s => s.AsV4)),
            IpAddressRangeType.IPv6 => IpAddressNetworkV6.MakeSupernet(input.Select(s => s.AsV6)),
            _ => throw new InvalidOperationException()
        };
    }

    public override string ToString()
    {
        return ToString(false);
    }

    public string ToString(bool forceCidr)
    {
        return Type switch
        {
            IpAddressRangeType.IPv4 => _v4!.Value.ToString(forceCidr),
            IpAddressRangeType.IPv6 => _v6!.Value.ToString(forceCidr),
            _ => throw new InvalidOperationException()
        };
    }

    public string ToPrefixString()
    {
        return Type switch
        {
            IpAddressRangeType.IPv4 => _v4!.Value.ToPrefixString(),
            IpAddressRangeType.IPv6 => _v6!.Value.ToPrefixString(),
            _ => throw new InvalidOperationException()
        };
    }

    public byte[] AddressToBytes()
    {
        return Type switch
        {
            IpAddressRangeType.IPv4 => _v4!.Value.AddressToBytes(),
            IpAddressRangeType.IPv6 => _v6!.Value.AddressToBytes(),
            _ => throw new InvalidOperationException()
        };
    }

    public void AddressToBytes(byte[] bytes, int offset = 0)
    {
        switch (Type)
        {
            case IpAddressRangeType.IPv4:
                _v4!.Value.AddressToBytes(bytes, offset);
                break;
            case IpAddressRangeType.IPv6:
                _v6!.Value.AddressToBytes(bytes, offset);
                break;
            default:
                throw new InvalidOperationException();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace MBW.Utilities.IPAddresses;

public partial class IpAddressRange
{
    public bool Contains(IpAddressRangeV4 other)
    {
        if (Type != IpAddressRangeType.IPv4)
            throw new ArgumentException("This range is not an IPv4 range");

        return _v4.Value.Contains(other);
    }

    public bool Contains(IpAddressRangeV6 other)
    {
        if (Type != IpAddressRangeType.IPv6)
            throw new ArgumentException("This range is not an IPv6 range");

        return _v6.Value.Contains(other);
    }

    public bool ContainsOrEqual(IpAddressRangeV4 other)
    {
        if (Type != IpAddressRangeType.IPv4)
            throw new ArgumentException("This range is not an IPv4 range");

        return _v4.Value.ContainsOrEqual(other);
    }

    public bool ContainsOrEqual(IpAddressRangeV6 other)
    {
        if (Type != IpAddressRangeType.IPv6)
            throw new ArgumentException("This range is not an IPv6 range");

        return _v6.Value.ContainsOrEqual(other);
    }

    public bool IsContainedIn(IpAddressRangeV4 other)
    {
        if (Type != IpAddressRangeType.IPv4)
            throw new ArgumentException("This range is not an IPv4 range");

        return other.Contains(_v4.Value);
    }

    public bool IsContainedIn(IpAddressRangeV6 other)
    {
        if (Type != IpAddressRangeType.IPv6)
            throw new ArgumentException("This range is not an IPv6 range");

        return other.Contains(_v6.Value);
    }

    public bool IsContainedInOrEqual(IpAddressRangeV4 other)
    {
        if (Type != IpAddressRangeType.IPv4)
            throw new ArgumentException("This range is not an IPv4 range");

        return _v4.Value.IsContainedInOrEqual(other);
    }

    public bool IsContainedInOrEqual(IpAddressRangeV6 other)
    {
        if (Type != IpAddressRangeType.IPv6)
            throw new ArgumentException("This range is not an IPv6 range");

        return _v6.Value.IsContainedInOrEqual(other);
    }

    public static IpAddressRange MakeSupernet(params IpAddressRange[] others)
    {
        return MakeSupernet((IEnumerable<IpAddressRange>)others);
    }

    public static IpAddressRange MakeSupernet(IEnumerable<IpAddressRange> others)
    {
        IpAddressRange[] input = others.ToArray();

        if (!input.Any())
            throw new ArgumentException("Input was empty", nameof(others));

        IpAddressRangeType type = input[0].Type;

        foreach (IpAddressRange range in input)
        {
            if (type != IpAddressRangeType.Unknown && type != range.Type)
                throw new ArgumentException("All ranges must be either IPv4 or IPv6, not a mixture of both.");
        }

        if (type == IpAddressRangeType.IPv4)
            return IpAddressRangeV4.MakeSupernet(input.Select(s => s.AsV4));

        if (type == IpAddressRangeType.IPv6)
            return IpAddressRangeV6.MakeSupernet(input.Select(s => s.AsV6));

        throw new InvalidOperationException();
    }

    public override string ToString()
    {
        return ToString(false);
    }

    public string ToString(bool forceCidr)
    {
        if (Type == IpAddressRangeType.IPv4)
            return _v4.Value.ToString(forceCidr);

        if (Type == IpAddressRangeType.IPv6)
            return _v6.Value.ToString(forceCidr);

        throw new InvalidOperationException();
    }

    public string ToPrefixString()
    {
        if (Type == IpAddressRangeType.IPv4)
            return _v4.Value.ToPrefixString();

        if (Type == IpAddressRangeType.IPv6)
            return _v6.Value.ToPrefixString();

        throw new InvalidOperationException();
    }

    public byte[] AddressToBytes()
    {
        if (Type == IpAddressRangeType.IPv4)
            return _v4.Value.AddressToBytes();

        if (Type == IpAddressRangeType.IPv6)
            return _v6.Value.AddressToBytes();

        throw new InvalidOperationException();
    }

    public void AddressToBytes(byte[] bytes, int offset = 0)
    {
        if (Type == IpAddressRangeType.IPv4)
            _v4.Value.AddressToBytes(bytes, offset);
        else if (Type == IpAddressRangeType.IPv6)
            _v6.Value.AddressToBytes(bytes, offset);
        else
            throw new InvalidOperationException();
    }
}
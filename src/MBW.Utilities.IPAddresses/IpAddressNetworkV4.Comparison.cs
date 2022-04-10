using System;

namespace MBW.Utilities.IPAddresses;

public partial struct IpAddressNetworkV4 : IComparable, IComparable<IpAddressNetworkV4>, IEquatable<IpAddressNetworkV4>
{
    public int CompareTo(object obj)
    {
        if (!(obj is IpAddressNetworkV4 other))
            throw new ArgumentException();

        return CompareTo(other);
    }

    public int CompareTo(IpAddressNetworkV4 other)
    {
        int res = _networkAddress.CompareTo(other._networkAddress);

        if (res == 0)
            return _mask.CompareTo(other._mask);

        return res;
    }

    public bool Equals(IpAddressNetworkV4 other)
    {
        return _mask == other._mask && _networkAddress == other._networkAddress;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
            return false;

        return obj is IpAddressNetworkV4 v4 && Equals(v4);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return _networkAddress.GetHashCode() ^ _mask * 397;
        }
    }

    public static bool operator ==(IpAddressNetworkV4 left, IpAddressNetworkV4 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(IpAddressNetworkV4 left, IpAddressNetworkV4 right)
    {
        return !left.Equals(right);
    }

    public static bool operator <(IpAddressNetworkV4 left, IpAddressNetworkV4 right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(IpAddressNetworkV4 left, IpAddressNetworkV4 right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(IpAddressNetworkV4 left, IpAddressNetworkV4 right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(IpAddressNetworkV4 left, IpAddressNetworkV4 right)
    {
        return left.CompareTo(right) >= 0;
    }
}
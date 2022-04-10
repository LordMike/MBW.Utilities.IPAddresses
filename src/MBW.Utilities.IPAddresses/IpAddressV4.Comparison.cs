using System;

namespace MBW.Utilities.IPAddresses;

public partial struct IpAddressV4 : IComparable, IComparable<IpAddressV4>, IEquatable<IpAddressV4>
{
    public int CompareTo(object obj)
    {
        if (!(obj is IpAddressV4 other))
            throw new ArgumentException();

        return CompareTo(other);
    }

    public int CompareTo(IpAddressV4 other)
    {
        return _address.CompareTo(other._address);
    }

    public bool Equals(IpAddressV4 other)
    {
        return _address == other._address;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
            return false;

        return obj is IpAddressV4 v4 && Equals(v4);
    }

    public override int GetHashCode()
    {
        return _address.GetHashCode();
    }

    public static bool operator ==(IpAddressV4 left, IpAddressV4 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(IpAddressV4 left, IpAddressV4 right)
    {
        return !left.Equals(right);
    }

    public static bool operator <(IpAddressV4 left, IpAddressV4 right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(IpAddressV4 left, IpAddressV4 right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(IpAddressV4 left, IpAddressV4 right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(IpAddressV4 left, IpAddressV4 right)
    {
        return left.CompareTo(right) >= 0;
    }
}
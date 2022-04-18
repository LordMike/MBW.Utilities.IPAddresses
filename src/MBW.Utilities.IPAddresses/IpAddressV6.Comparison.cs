using System;

namespace MBW.Utilities.IPAddresses;

public partial struct IpAddressV6 : IComparable, IComparable<IpAddressV6>, IEquatable<IpAddressV6>
{
    public int CompareTo(object obj)
    {
        if (!(obj is IpAddressV6 other))
            throw new ArgumentException();

        return CompareTo(other);
    }

    public int CompareTo(IpAddressV6 other)
    {
        return _address.CompareTo(other._address);
    }

    public override int GetHashCode()
    {
        return _address.GetHashCode();
    }

    public bool Equals(IpAddressV6 other)
    {
        return _address == other._address;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
            return false;

        return obj is IpAddressV6 v6 && Equals(v6);
    }

    public static bool operator ==(IpAddressV6 left, IpAddressV6 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(IpAddressV6 left, IpAddressV6 right)
    {
        return !left.Equals(right);
    }

    public static bool operator <(IpAddressV6 left, IpAddressV6 right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(IpAddressV6 left, IpAddressV6 right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(IpAddressV6 left, IpAddressV6 right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(IpAddressV6 left, IpAddressV6 right)
    {
        return left.CompareTo(right) >= 0;
    }
}
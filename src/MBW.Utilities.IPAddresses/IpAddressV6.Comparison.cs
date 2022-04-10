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
        int res = _addressHigh.CompareTo(other._addressHigh);

        if (res != 0)
            return res;

        return _addressLow.CompareTo(other._addressLow);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = _addressHigh.GetHashCode();
            hashCode = (hashCode * 397) ^ _addressLow.GetHashCode();
            return hashCode;
        }
    }

    public bool Equals(IpAddressV6 other)
    {
        return _addressHigh == other._addressHigh &&
               _addressLow == other._addressLow;
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
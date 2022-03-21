using System;

namespace MBW.Utilities.IPAddresses;

public partial struct IpAddressRangeV6 : IComparable, IComparable<IpAddressRangeV6>, IEquatable<IpAddressRangeV6>
{
    public int CompareTo(object obj)
    {
        if (!(obj is IpAddressRangeV6 other))
            throw new ArgumentException();

        return CompareTo(other);
    }

    public int CompareTo(IpAddressRangeV6 other)
    {
        int res = _addressHigh.CompareTo(other._addressHigh);

        if (res != 0)
            return res;

        res = _addressLow.CompareTo(other._addressLow);

        if (res != 0)
            return res;

        return _mask.CompareTo(other._mask);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = _addressHigh.GetHashCode();
            hashCode = (hashCode * 397) ^ _addressLow.GetHashCode();
            hashCode = (hashCode * 397) ^ _mask.GetHashCode();
            return hashCode;
        }
    }

    public bool Equals(IpAddressRangeV6 other)
    {
        return _mask == other._mask &&
               _addressHigh == other._addressHigh &&
               _addressLow == other._addressLow;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
            return false;

        return obj is IpAddressRangeV6 v6 && Equals(v6);
    }

    public static bool operator ==(IpAddressRangeV6 left, IpAddressRangeV6 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(IpAddressRangeV6 left, IpAddressRangeV6 right)
    {
        return !left.Equals(right);
    }

    public static bool operator <(IpAddressRangeV6 left, IpAddressRangeV6 right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(IpAddressRangeV6 left, IpAddressRangeV6 right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(IpAddressRangeV6 left, IpAddressRangeV6 right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(IpAddressRangeV6 left, IpAddressRangeV6 right)
    {
        return left.CompareTo(right) >= 0;
    }
}
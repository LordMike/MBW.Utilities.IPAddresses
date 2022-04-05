using System;

namespace MBW.Utilities.IPAddresses;

public partial class IpAddressNetwork : IComparable, IComparable<IpAddressNetwork>, IEquatable<IpAddressNetwork>
{
    public int CompareTo(object obj)
    {
        if (!(obj is IpAddressNetwork other))
            throw new ArgumentException();

        return CompareTo(other);
    }

    public int CompareTo(IpAddressNetwork other)
    {
        if (ReferenceEquals(this, other))
            return 0;

        if (ReferenceEquals(null, other))
            return 1;

        int res = Type.CompareTo(other.Type);
        if (res != 0)
            return res;

        if (Type == IpAddressRangeType.IPv4)
            return Nullable.Compare(_v4, other._v4);

        if (Type == IpAddressRangeType.IPv6)
            return Nullable.Compare(_v6, other._v6);

        throw new InvalidOperationException();
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (obj.GetType() != GetType())
            return false;

        return Equals((IpAddressNetwork)obj);
    }

    public bool Equals(IpAddressNetwork other)
    {
        if (ReferenceEquals(null, other))
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (Type != other.Type)
            return false;

        if (Type == IpAddressRangeType.IPv4)
            return _v4.Equals(other._v4);

        if (Type == IpAddressRangeType.IPv6)
            return _v6.Equals(other._v6);

        throw new InvalidOperationException();
    }

    public override int GetHashCode()
    {
        unchecked
        {
            if (Type == IpAddressRangeType.IPv4)
                return Type.GetHashCode() * 397 ^ _v4.Value.GetHashCode();

            if (Type == IpAddressRangeType.IPv6)
                return Type.GetHashCode() * 397 ^ _v6.Value.GetHashCode();
        }

        throw new InvalidOperationException();
    }

    public static bool operator ==(IpAddressNetwork left, IpAddressNetwork right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(IpAddressNetwork left, IpAddressNetwork right)
    {
        return !left.Equals(right);
    }

    public static bool operator <(IpAddressNetwork left, IpAddressNetwork right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(IpAddressNetwork left, IpAddressNetwork right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(IpAddressNetwork left, IpAddressNetwork right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(IpAddressNetwork left, IpAddressNetwork right)
    {
        return left.CompareTo(right) >= 0;
    }
}
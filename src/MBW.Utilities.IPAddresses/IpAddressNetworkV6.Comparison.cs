using System;

namespace MBW.Utilities.IPAddresses;

public partial struct IpAddressNetworkV6 : IComparable, IComparable<IpAddressNetworkV6>, IEquatable<IpAddressNetworkV6>
{
    public int CompareTo(object obj)
    {
        if (!(obj is IpAddressNetworkV6 other))
            throw new ArgumentException();

        return CompareTo(other);
    }

    public int CompareTo(IpAddressNetworkV6 other)
    {
        int res = _networkAddress.AddressHigh.CompareTo(other.NetworkAddress.AddressHigh);

        if (res != 0)
            return res;

        res = _networkAddress.AddressLow.CompareTo(other.NetworkAddress.AddressLow);

        if (res != 0)
            return res;

        return _mask.CompareTo(other._mask);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = _networkAddress.AddressHigh.GetHashCode();
            hashCode = (hashCode * 397) ^ _networkAddress.AddressLow.GetHashCode();
            hashCode = (hashCode * 397) ^ _mask.GetHashCode();
            return hashCode;
        }
    }

    public bool Equals(IpAddressNetworkV6 other)
    {
        return _mask == other._mask &&
               _networkAddress.AddressHigh == other.NetworkAddress.AddressHigh &&
               _networkAddress.AddressLow == other.NetworkAddress.AddressLow;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
            return false;

        return obj is IpAddressNetworkV6 v6 && Equals(v6);
    }

    public static bool operator ==(IpAddressNetworkV6 left, IpAddressNetworkV6 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(IpAddressNetworkV6 left, IpAddressNetworkV6 right)
    {
        return !left.Equals(right);
    }

    public static bool operator <(IpAddressNetworkV6 left, IpAddressNetworkV6 right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(IpAddressNetworkV6 left, IpAddressNetworkV6 right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(IpAddressNetworkV6 left, IpAddressNetworkV6 right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(IpAddressNetworkV6 left, IpAddressNetworkV6 right)
    {
        return left.CompareTo(right) >= 0;
    }
}
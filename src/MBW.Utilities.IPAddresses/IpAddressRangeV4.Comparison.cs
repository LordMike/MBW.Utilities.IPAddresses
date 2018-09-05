using System;

namespace MBW.Utilities.IPAddresses
{
    public partial struct IpAddressRangeV4 : IComparable, IComparable<IpAddressRangeV4>, IEquatable<IpAddressRangeV4>
    {
        public int CompareTo(object obj)
        {
            if (!(obj is IpAddressRangeV4 other))
                throw new ArgumentException();

            return CompareTo(other);
        }

        public int CompareTo(IpAddressRangeV4 other)
        {
            int res = _address.CompareTo(other._address);

            if (res == 0)
                return _mask.CompareTo(other._mask);

            return res;
        }

        public bool Equals(IpAddressRangeV4 other)
        {
            return _mask == other._mask && _address == other._address;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            return obj is IpAddressRangeV4 v4 && Equals(v4);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return _address.GetHashCode() ^ _mask * 397;
            }
        }

        public static bool operator ==(IpAddressRangeV4 left, IpAddressRangeV4 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(IpAddressRangeV4 left, IpAddressRangeV4 right)
        {
            return !left.Equals(right);
        }

        public static bool operator <(IpAddressRangeV4 left, IpAddressRangeV4 right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(IpAddressRangeV4 left, IpAddressRangeV4 right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(IpAddressRangeV4 left, IpAddressRangeV4 right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(IpAddressRangeV4 left, IpAddressRangeV4 right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}

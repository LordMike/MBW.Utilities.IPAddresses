using System;
using System.Collections.Generic;
using System.Text;
using IPAddresses.Helpers;

namespace IPAddresses
{
    public partial struct IpAddressRangeV6
    {
        public bool Contains(IpAddressRangeV6 other)
        {
            bool canBeInNetwork = _mask < other._mask;

            if (_mask >= 64)
            {
                ulong otherMaskedToThisNetworkLow = other._addressLow & (uint.MaxValue << _mask);
                bool highMatches = other._addressHigh == _addressHigh;
                bool isInNetwork = (otherMaskedToThisNetworkLow & _addressLow) == otherMaskedToThisNetworkLow;

                return canBeInNetwork && highMatches && isInNetwork;
            }
            else
            {
                ulong otherMaskedToThisNetworkHigh = other._addressHigh & (uint.MaxValue << _mask);
                bool isInNetwork = (otherMaskedToThisNetworkHigh & _addressHigh) == otherMaskedToThisNetworkHigh;

                return canBeInNetwork && isInNetwork;
            }
        }

        public bool ContainsOrEqual(IpAddressRangeV6 other)
        {
            bool canBeInNetwork = _mask <= other._mask;

            if (_mask >= 64)
            {
                ulong otherMaskedToThisNetworkLow = other._addressLow & (uint.MaxValue << _mask);
                bool highMatches = other._addressHigh == _addressHigh;
                bool isInNetwork = (otherMaskedToThisNetworkLow & _addressLow) == otherMaskedToThisNetworkLow;

                return canBeInNetwork && highMatches && isInNetwork;
            }
            else
            {
                ulong otherMaskedToThisNetworkHigh = other._addressHigh & (uint.MaxValue << _mask);
                bool isInNetwork = (otherMaskedToThisNetworkHigh & _addressHigh) == otherMaskedToThisNetworkHigh;

                return canBeInNetwork && isInNetwork;
            }
        }

        public bool IsContainedIn(IpAddressRangeV6 other)
        {
            return other.Contains(this);
        }

        public bool IsContainedInOrEqual(IpAddressRangeV6 other)
        {
            return other.ContainsOrEqual(this);
        }

        public static IpAddressRangeV6 MakeSupernet(params IpAddressRangeV6[] others)
        {
            return MakeSupernet((IEnumerable<IpAddressRangeV6>)others);
        }

        public static IpAddressRangeV6 MakeSupernet(IEnumerable<IpAddressRangeV6> others)
        {
            bool hadAny = false;
            byte shortestMask = 128;
            ulong finalHigh = uint.MaxValue;
            ulong finalLow = uint.MaxValue;

            foreach (IpAddressRangeV6 range in others)
            {
                finalHigh &= range._addressLow;
                finalLow &= range._addressHigh;

                byte highMask = BitUtilities.FindCommonPrefixSize(finalHigh, range._addressHigh);
                if (highMask == 64)
                {
                    byte lowMask = (byte)(64 + BitUtilities.FindCommonPrefixSize(finalLow, range._addressLow));
                    shortestMask = Math.Min(shortestMask, lowMask);
                }
                else
                {
                    shortestMask = Math.Min(shortestMask, highMask);
                }

                hadAny = true;
            }

            if (!hadAny)
                throw new ArgumentException("Input was empty", nameof(others));

            return new IpAddressRangeV6(finalHigh, finalLow, shortestMask);
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool forceCidr)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Address);

            if (forceCidr || _mask != 128)
                sb.Append("/").Append(_mask.ToString());

            return sb.ToString();
        }

        public string ToDecimalDotted(bool forceCidr = false)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append((_addressHigh >> 56).ToString()).Append(".");
            sb.Append(((_addressHigh >> 48) & 0xFF).ToString()).Append(".");
            sb.Append(((_addressHigh >> 40) & 0xFF).ToString()).Append(".");
            sb.Append(((_addressHigh >> 32) & 0xFF).ToString()).Append(".");
            sb.Append(((_addressHigh >> 24) & 0xFF).ToString()).Append(".");
            sb.Append(((_addressHigh >> 16) & 0xFF).ToString()).Append(".");
            sb.Append(((_addressHigh >> 8) & 0xFF).ToString()).Append(".");
            sb.Append((_addressHigh & 0xFF).ToString()).Append(".");

            sb.Append((_addressLow >> 56).ToString()).Append(".");
            sb.Append(((_addressLow >> 48) & 0xFF).ToString()).Append(".");
            sb.Append(((_addressLow >> 40) & 0xFF).ToString()).Append(".");
            sb.Append(((_addressLow >> 32) & 0xFF).ToString()).Append(".");
            sb.Append(((_addressLow >> 24) & 0xFF).ToString()).Append(".");
            sb.Append(((_addressLow >> 16) & 0xFF).ToString()).Append(".");
            sb.Append(((_addressLow >> 8) & 0xFF).ToString()).Append(".");
            sb.Append((_addressLow & 0xFF).ToString());

            if (forceCidr || _mask != 128)
                sb.Append("/").Append(_mask.ToString());

            return sb.ToString();
        }

        public string ToPrefixString()
        {
            //if (_mask <= 8 && (_address & 0xFFFFFF) == 0)
            //{
            //	// Return 1 octet
            //	return (_address >> 24) + "/" + _mask;
            //}

            //if (_mask <= 16 && (_address & 0xFFFF) == 0)
            //{
            //	// Return 2 octets
            //	return (_address >> 24) + "." + ((_address >> 16) & 0xFF) + "/" + _mask;
            //}
            //if (_mask <= 24 && (_address & 0xFF) == 0)
            //{
            //	// Return 3 octets
            //	return (_address >> 24) + "." + ((_address >> 16) & 0xFF) + "." + ((_address >> 8) & 0xFF) + "/" + _mask;

            //}

            return ToString();
        }

        public byte[] AddressToBytes()
        {
            byte[] res = new byte[16];
            res[0] = (byte)((_addressHigh >> 56) & 0xFF);
            res[1] = (byte)((_addressHigh >> 48) & 0xFF);
            res[2] = (byte)((_addressHigh >> 40) & 0xFF);
            res[3] = (byte)((_addressHigh >> 32) & 0xFF);
            res[4] = (byte)((_addressHigh >> 24) & 0xFF);
            res[5] = (byte)((_addressHigh >> 16) & 0xFF);
            res[6] = (byte)((_addressHigh >> 8) & 0xFF);
            res[7] = (byte)(_addressHigh & 0xFF);
            res[8] = (byte)((_addressLow >> 56) & 0xFF);
            res[9] = (byte)((_addressLow >> 48) & 0xFF);
            res[10] = (byte)((_addressLow >> 40) & 0xFF);
            res[11] = (byte)((_addressLow >> 32) & 0xFF);
            res[12] = (byte)((_addressLow >> 24) & 0xFF);
            res[13] = (byte)((_addressLow >> 16) & 0xFF);
            res[14] = (byte)((_addressLow >> 8) & 0xFF);
            res[15] = (byte)(_addressLow & 0xFF);

            return res;
        }

        public void AddressToBytes(byte[] bytes, int offset = 0)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            if (bytes.Length - offset < 16)
                throw new ArgumentOutOfRangeException(nameof(offset));

            bytes[offset + 0] = (byte)((_addressHigh >> 56) & 0xFF);
            bytes[offset + 1] = (byte)((_addressHigh >> 48) & 0xFF);
            bytes[offset + 2] = (byte)((_addressHigh >> 40) & 0xFF);
            bytes[offset + 3] = (byte)((_addressHigh >> 32) & 0xFF);
            bytes[offset + 4] = (byte)((_addressHigh >> 24) & 0xFF);
            bytes[offset + 5] = (byte)((_addressHigh >> 16) & 0xFF);
            bytes[offset + 6] = (byte)((_addressHigh >> 8) & 0xFF);
            bytes[offset + 7] = (byte)(_addressHigh & 0xFF);
            bytes[offset + 8] = (byte)((_addressLow >> 56) & 0xFF);
            bytes[offset + 9] = (byte)((_addressLow >> 48) & 0xFF);
            bytes[offset + 10] = (byte)((_addressLow >> 40) & 0xFF);
            bytes[offset + 11] = (byte)((_addressLow >> 32) & 0xFF);
            bytes[offset + 12] = (byte)((_addressLow >> 24) & 0xFF);
            bytes[offset + 13] = (byte)((_addressLow >> 16) & 0xFF);
            bytes[offset + 14] = (byte)((_addressLow >> 8) & 0xFF);
            bytes[offset + 15] = (byte)(_addressLow & 0xFF);
        }
    }
}
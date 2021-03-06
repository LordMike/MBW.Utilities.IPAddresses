using System;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using MBW.Utilities.IPAddresses.Helpers;

namespace MBW.Utilities.IPAddresses
{
    public partial struct IpAddressRangeV4
    {
        private static readonly Regex ParseRegex = new Regex(@"^([0-9]{1,3})(?:\.([0-9]{1,3}))?(?:\.([0-9]{1,3}))?(?:\.([0-9]{1,3}))?(?:/([0-9]{1,2}))?$", RegexOptions.Compiled);
        private static readonly IPAddress Max = IPAddress.Parse("255.255.255.255");

        private readonly byte _mask;
        private readonly uint _address;

        public byte Mask => _mask;

        public uint AddressUint => _address;

        public uint NetworkMask => uint.MaxValue << (32 - _mask);

        public IPAddress Address => new IPAddress(BitUtilities.Reverse(_address));

        public IPAddress EndAddress
        {
            get
            {
                if (_mask == 0)
                    return Max;

                if (_mask == 32)
                    return Address;

                return ConversionUtilities.ToIp(EndAddressUint);
            }
        }

        public uint EndAddressUint => _address | ~NetworkMask;

        public uint SubnetSize
        {
            get
            {
                if (_mask == 0)
                    return uint.MaxValue;

                if (_mask == 32)
                    return 1;

                return (uint)(1 << (32 - _mask));
            }
        }

        public uint SubnetHostsSize
        {
            get
            {
                if (_mask == 0)
                    return uint.MaxValue - 2;

                if (_mask == 32)
                    return 1;

                return SubnetSize - 2;
            }
        }

#if NETSTANDARD1_6
        public IpAddressRangeV4(IPAddress address, byte mask = 32)
#pragma warning disable 618
            : this(BitUtilities.Reverse(BitConverter.ToUInt32(address.GetAddressBytes(), 0)), mask)
#pragma warning restore 618
        {
            if (address.AddressFamily != AddressFamily.InterNetwork)
                throw new ArgumentException();
        }
#else
        public IpAddressRangeV4(IPAddress address, byte mask = 32)
#pragma warning disable 618
            : this(BitUtilities.Reverse((uint)address.Address), mask)
#pragma warning restore 618
        {
            if (address.AddressFamily != AddressFamily.InterNetwork)
                throw new ArgumentException();
        }
#endif


        public IpAddressRangeV4(uint address, byte mask = 32)
        {
            if (mask > 32)
                throw new ArgumentException("Mask cannot be greater than 32 bits", nameof(mask));

            _mask = mask;
            if (mask == 0)
                _address = 0;
            else
                _address = address & (uint.MaxValue << (32 - _mask));
        }
    }
}

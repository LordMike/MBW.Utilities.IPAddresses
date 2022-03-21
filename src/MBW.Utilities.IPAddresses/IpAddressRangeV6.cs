using System;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using MBW.Utilities.IPAddresses.Helpers;

namespace MBW.Utilities.IPAddresses;

public partial struct IpAddressRangeV6
{
    private static readonly Regex IpRegex = new Regex(@"^([a-fA-F0-9:\.]*)(?:/([0-9]{1,3}))?$", RegexOptions.Compiled);
    private static readonly IPAddress Max = IPAddress.Parse("ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff");

    private readonly byte _mask;
    private readonly ulong _addressHigh;
    private readonly ulong _addressLow;

    public byte Mask => _mask;

    public ulong AddressLow => _addressLow;

    public ulong AddressHigh => _addressHigh;

    public IPAddress Address => ConversionUtilities.ToIp(_addressHigh, _addressLow);

    public IPAddress EndAddress
    {
        get
        {
            if (_mask == 0)
                return Max;
            if (_mask == 128)
                return Address;

            ulong high = EndAddressHigh;
            ulong low = EndAddressLow;

            return ConversionUtilities.ToIp(high, low);
        }
    }

    public ulong EndAddressLow
    {
        get
        {
            if (_mask == 0)
                return ulong.MaxValue;
            if (_mask == 128)
                return _addressLow;

            ulong low = _addressLow;

            if (_mask > 64)
            {
                // Keep the high bits, fix the lows
                low = low | ~(ulong.MaxValue << (128 - _mask));
            }
            else if (_mask == 64)
            {
                // Set the lows to high
                low = ulong.MaxValue;
            }
            else
            {
                // Fix the high bits, set the lows to high
                low = ulong.MaxValue;
            }

            return low;
        }
    }

    public ulong EndAddressHigh
    {
        get
        {
            if (_mask == 0)
                return ulong.MaxValue;
            if (_mask == 128)
                return _addressHigh;

            ulong high = _addressHigh;

            if (_mask < 64)
            {
                // Fix the high bits, set the lows to high
                high = high | ~(ulong.MaxValue << (64 - _mask));
            }

            return high;
        }
    }

    public IpAddressRangeV6(IPAddress address, byte mask = 128)
        : this(address.GetAddressBytes(), mask)
    {
        if (address.AddressFamily != AddressFamily.InterNetworkV6)
            throw new ArgumentException();
    }

    private IpAddressRangeV6(byte[] address, byte mask = 128)
        : this(BitUtilities.Reverse(BitConverter.ToUInt64(address, 0)), BitUtilities.Reverse(BitConverter.ToUInt64(address, 8)), mask)
    {
        if (address.Length != 16)
            throw new ArgumentException();
    }

    public IpAddressRangeV6(ulong addressHigh, ulong addressLow, byte mask = 128)
    {
        _addressLow = addressLow;
        _addressHigh = addressHigh;

        _mask = mask;
        if (mask == 0)
            _addressHigh = _addressLow = 0;
        else if (mask == 128)
        {
        }
        else if (mask > 64)
        {
            // Keep the high bits, truncate the lows
            _addressLow = _addressLow & (ulong.MaxValue << (64 - (_mask - 64)));
        }
        else if (mask == 64)
        {
            _addressLow = 0;
        }
        else
        {
            // Truncate the high bits, remove the lows
            _addressHigh = _addressHigh & (ulong.MaxValue << (64 - _mask));
            _addressLow = 0;
        }
    }
}
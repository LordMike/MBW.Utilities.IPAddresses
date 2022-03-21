using System;
using System.Net;
using System.Net.Sockets;

namespace MBW.Utilities.IPAddresses;

public partial class IpAddressRange
{
    private IpAddressRangeV4? _v4;
    private IpAddressRangeV6? _v6;

    public IpAddressRangeType Type { get; }

    public bool IsMultipleHosts => Type == IpAddressRangeType.IPv4 ? _v4.Value.Mask < 32 : _v6.Value.Mask < 128;

    public IpAddressRangeV4 AsV4
    {
        get
        {
            if (Type != IpAddressRangeType.IPv4)
                throw new InvalidOperationException("This IpAddressRange does not represent an IPv4 range");

            return _v4.Value;
        }
    }

    public IpAddressRangeV6 AsV6
    {
        get
        {
            if (Type != IpAddressRangeType.IPv6)
                throw new InvalidOperationException("This IpAddressRange does not represent an IPv6 range");

            return _v6.Value;
        }
    }

    public IpAddressRange(IpAddressRangeV4 value)
    {
        _v4 = value;
        Type = IpAddressRangeType.IPv4;
    }

    public IpAddressRange(IpAddressRangeV6 value)
    {
        _v6 = value;
        Type = IpAddressRangeType.IPv6;
    }

    public IpAddressRange(IPAddress value)
    {
        if (value.AddressFamily == AddressFamily.InterNetwork)
        {
            _v4 = value;
            Type = IpAddressRangeType.IPv4;
        }
        else if (value.AddressFamily == AddressFamily.InterNetworkV6)
        {
            _v6 = value;
            Type = IpAddressRangeType.IPv6;
        }
        else
            throw new ArgumentException();
    }

    private IpAddressRange(IpAddressRangeV4? v4, IpAddressRangeV6? v6)
    {
        if (v4.HasValue)
        {
            _v4 = v4;
            Type = IpAddressRangeType.IPv4;
        }
        else if (v6.HasValue)
        {
            _v6 = v6;
            Type = IpAddressRangeType.IPv6;
        }
        else
            throw new ArgumentException();
    }
}
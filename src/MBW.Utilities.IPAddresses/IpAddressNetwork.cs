using System;
using System.Net;
using System.Net.Sockets;

namespace MBW.Utilities.IPAddresses;

public partial class IpAddressNetwork
{
    private IpAddressNetworkV4? _v4;
    private IpAddressNetworkV6? _v6;

    public byte Mask => Type == IpAddressRangeType.IPv4 ? _v4!.Value.Mask : _v6!.Value.Mask;

    public IPAddress Address => Type == IpAddressRangeType.IPv4 ? _v4!.Value.Address : _v6!.Value.Address;

    public IPAddress EndAddress => Type == IpAddressRangeType.IPv4 ? _v4!.Value.EndAddress : _v6!.Value.EndAddress;

    public IpAddressRangeType Type { get; }

    public IpAddressNetworkV4 AsV4 => GetV4();
    public IpAddressNetworkV6 AsV6 => GetV6();

    public IpAddressNetwork(IpAddressNetworkV4 value)
    {
        _v4 = value;
        Type = IpAddressRangeType.IPv4;
    }

    public IpAddressNetwork(IpAddressNetworkV6 value)
    {
        _v6 = value;
        Type = IpAddressRangeType.IPv6;
    }

    public IpAddressNetwork(IPAddress value)
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

    private IpAddressNetwork(IpAddressNetworkV4? v4, IpAddressNetworkV6? v6)
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
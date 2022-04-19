using MBW.Utilities.IPAddresses.DataStructures;

namespace MBW.Utilities.IPAddresses.WellKnown;

/// <summary>
/// Well known IPv4 networks, see https://en.wikipedia.org/wiki/Reserved_IP_addresses#IPv4 for more details
/// </summary>
public sealed class WellKnownIPv4 : WellKnownBase<IpAddressV4, IpAddressNetworkV4, WellKnownIPv4Type, IPv4Set<WellKnownIPv4Type>>
{
    public static WellKnownIPv4 Instance { get; } = new();

    private WellKnownIPv4()
    {
        Add(WellKnownIPv4Type.PrivateNetworking, (IpAddressNetworkV4)"10.0.0.0/8");
        Add(WellKnownIPv4Type.PrivateNetworking, (IpAddressNetworkV4)"172.16.0.0/12");
        Add(WellKnownIPv4Type.PrivateNetworking, (IpAddressNetworkV4)"192.168.0.0/16");
        Add(WellKnownIPv4Type.Loopback, (IpAddressNetworkV4)"127.0.0.0/8");

        Add(WellKnownIPv4Type.LinkLocal, (IpAddressNetworkV4)"169.254.0.0/16");
        Add(WellKnownIPv4Type.Documentation, (IpAddressNetworkV4)"192.0.0.0/24");
        Add(WellKnownIPv4Type.Documentation, (IpAddressNetworkV4)"192.0.2.0/24");
        Add(WellKnownIPv4Type.Documentation, (IpAddressNetworkV4)"198.51.100.0/24");
        Add(WellKnownIPv4Type.Documentation, (IpAddressNetworkV4)"203.0.113.0/24");
        Add(WellKnownIPv4Type.Documentation, (IpAddressNetworkV4)"233.252.0.0/24");
        Add(WellKnownIPv4Type.Benchmark, (IpAddressNetworkV4)"198.18.0.0/15");

        Add(WellKnownIPv4Type.Multicast, (IpAddressNetworkV4)"224.0.0.0/4");
        Add(WellKnownIPv4Type.CarrierGradeNat, (IpAddressNetworkV4)"100.64.0.0/10");

        Add(WellKnownIPv4Type.Broadcast, (IpAddressNetworkV4)"255.255.255.255/32");
    }

    protected override bool Contains(IpAddressNetworkV4 network, IpAddressNetworkV4 candidate) => network.Contains(candidate);
    protected override bool Contains(IpAddressNetworkV4 network, IpAddressV4 candidate) => network.Contains(candidate);

    public override WellKnownIPv4Type GetType(IpAddressV4 address) => GetType((IpAddressNetworkV4)address);
}

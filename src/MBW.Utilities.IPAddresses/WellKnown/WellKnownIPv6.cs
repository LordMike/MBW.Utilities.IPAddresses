using MBW.Utilities.IPAddresses.DataStructures;

namespace MBW.Utilities.IPAddresses.WellKnown;

/// <summary>
/// Well known IPv6 networks, see https://en.wikipedia.org/wiki/Reserved_IP_addresses#IPv6 for more details
/// </summary>
public sealed class WellKnownIPv6 : WellKnownBase<IpAddressV6, IpAddressNetworkV6, WellKnownIPv6Type, IPv6Set<WellKnownIPv6Type>>
{
    public static WellKnownIPv6 Instance { get; } = new();

    private WellKnownIPv6()
    {
        Add(WellKnownIPv6Type.Unspecified, new IpAddressNetworkV6(0, 0, 128));
        Add(WellKnownIPv6Type.Loopback, new IpAddressNetworkV6(0, 1, 128));
        Add(WellKnownIPv6Type.LinkLocal, new IpAddressNetworkV6(0xfe80_0000_0000_0000, 0, 10));

        Add(WellKnownIPv6Type.MulticastReserved, new IpAddressNetworkV6(0xfe00_0000_0000_0000, 0, 16));
        Add(WellKnownIPv6Type.MulticastInterfaceLocal, new IpAddressNetworkV6(0xfe01_0000_0000_0000, 0, 16));
        Add(WellKnownIPv6Type.MulticastLinkLocal, new IpAddressNetworkV6(0xfe02_0000_0000_0000, 0, 16));
        Add(WellKnownIPv6Type.MulticastRealmLocal, new IpAddressNetworkV6(0xfe03_0000_0000_0000, 0, 16));
        Add(WellKnownIPv6Type.MulticastAdminLocal, new IpAddressNetworkV6(0xfe04_0000_0000_0000, 0, 16));
        Add(WellKnownIPv6Type.MulticastSiteLocal, new IpAddressNetworkV6(0xfe05_0000_0000_0000, 0, 16));
        Add(WellKnownIPv6Type.MulticastOrganizationLocal, new IpAddressNetworkV6(0xfe08_0000_0000_0000, 0, 16));
        Add(WellKnownIPv6Type.MulticastGlobal, new IpAddressNetworkV6(0xfe0e_0000_0000_0000, 0, 16));
        Add(WellKnownIPv6Type.MulticastReserved, new IpAddressNetworkV6(0xfe0f_0000_0000_0000, 0, 16));

        Add(WellKnownIPv6Type.Discard, new IpAddressNetworkV6(0x0100_0000_0000_0000, 0, 64));

        Add(WellKnownIPv6Type.Documentation, new IpAddressNetworkV6(0x2001_0db8_0000_0000, 0, 32));
        Add(WellKnownIPv6Type.UniqueLocalAddress, new IpAddressNetworkV6(0xfc00_0000_0000_0000, 0, 7));
        Add(WellKnownIPv6Type.TeredoTunneling, new IpAddressNetworkV6(0x2001_0000_0000_0000, 0, 32));
        Add(WellKnownIPv6Type.Benchmarking, new IpAddressNetworkV6(0x2001_0002_0000_0000, 0, 48));
        Add(WellKnownIPv6Type.OrchidV2, new IpAddressNetworkV6(0x2001_0020_0000_0000, 0, 28));

        Add(WellKnownIPv6Type.Ipv4To6Mapped, new IpAddressNetworkV6(0x0000_0000_0000_0000, 0x0000_ffff_0000_0000, 96));
        Add(WellKnownIPv6Type.Ipv4To6Translated, new IpAddressNetworkV6(0x0000_0000_0000_0000, 0xffff_0000_0000_0000, 96));
        Add(WellKnownIPv6Type.Ipv4To6WellKnown, new IpAddressNetworkV6(0x0064_ff9b_0000_0000, 0x0000_0000_0000_0000, 96));
        Add(WellKnownIPv6Type.Ipv4To6LocallyTranslated, new IpAddressNetworkV6(0x0064_ff9b_0001_0000, 0x0000_0000_0000_0000, 48));
    }

    protected override bool Contains(IpAddressNetworkV6 network, IpAddressNetworkV6 candidate) => network.Contains(candidate);
    protected override bool Contains(IpAddressNetworkV6 network, IpAddressV6 candidate) => network.Contains(candidate);
    public override WellKnownIPv6Type GetType(IpAddressV6 address) => GetType((IpAddressNetworkV6)address);
}

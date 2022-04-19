namespace MBW.Utilities.IPAddresses.DataStructures;

public sealed class IPv6Set<TValue> : IPSet<IpAddressNetworkV6, TValue>
{
    public IPv6Set() : base()
    {

    }

    protected override bool Contains(IpAddressNetworkV6 network, IpAddressNetworkV6 other) => network.ContainsOrEqual(other);
    protected override int Compare(IpAddressNetworkV6 network, IpAddressNetworkV6 other) => network.CompareTo(other);
    protected override int Mask(IpAddressNetworkV6 network) => network.Mask;

    public bool TryGet(IpAddressV6 address, out IpAddressNetworkV6? foundNetwork, out TValue? value) => TryGet((IpAddressNetworkV6)address, out foundNetwork, out value);
}

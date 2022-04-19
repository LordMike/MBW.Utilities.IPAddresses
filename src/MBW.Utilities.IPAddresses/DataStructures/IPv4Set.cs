namespace MBW.Utilities.IPAddresses.DataStructures;

public sealed class IPv4Set<TValue> : IPSet<IpAddressNetworkV4, TValue>
{
    public IPv4Set() : base()
    {

    }

    protected override bool Contains(IpAddressNetworkV4 network, IpAddressNetworkV4 other) => network.ContainsOrEqual(other);
    protected override int Compare(IpAddressNetworkV4 network, IpAddressNetworkV4 other) => network.CompareTo(other);
    protected override int Mask(IpAddressNetworkV4 network) => network.Mask;

    public bool TryGet(IpAddressV4 address, out IpAddressNetworkV4? foundNetwork, out TValue? value) => TryGet((IpAddressNetworkV4)address, out foundNetwork, out value);
}

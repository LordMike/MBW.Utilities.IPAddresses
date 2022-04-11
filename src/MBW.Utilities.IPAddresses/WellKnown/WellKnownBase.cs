using MBW.Utilities.IPAddresses.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MBW.Utilities.IPAddresses.WellKnown;

public abstract class WellKnownBase<TAddress, TNetwork, TEnum, TIPSet> where TEnum : Enum where TAddress : struct where TNetwork : struct where TIPSet : IPSet<TNetwork, TEnum>, new()
{
    private readonly TIPSet _types = new();

    internal protected WellKnownBase()
    {
    }

    protected abstract bool Contains(TNetwork network, TNetwork candidate);
    protected abstract bool Contains(TNetwork network, TAddress candidate);

    protected void Add(TEnum type, TNetwork network)
    {
        _types.Add(network, type);
    }

    public abstract TEnum GetType(TAddress address);

    public TEnum GetType(TNetwork network)
    {
        if (!_types.TryGet(network, out _, out var type))
            return default;

        return type;
    }

    public IEnumerable<TNetwork> GetNetworks(TEnum type)
    {
        return _types
            .GetValues()
            .Where(s => Equals(s.value, type))
            .Select(s => s.network);
    }

    public IEnumerable<(TEnum type, TNetwork network)> GetWellKnownNetworks() => _types.GetValues().Select(s => (s.value, s.network));
}

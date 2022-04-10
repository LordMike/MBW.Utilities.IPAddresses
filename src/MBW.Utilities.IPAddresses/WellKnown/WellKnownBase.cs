using System;
using System.Collections.Generic;

namespace MBW.Utilities.IPAddresses.WellKnown;

public abstract class WellKnownBase<TAddress, TNetwork, TEnum> where TEnum : Enum where TAddress : struct where TNetwork : struct
{
    private readonly List<(TEnum type, TNetwork network)> _types = new();

    internal protected WellKnownBase()
    {
    }

    protected abstract bool Contains(TNetwork network, TNetwork candidate);
    protected abstract bool Contains(TNetwork network, TAddress candidate);

    protected void Add(TEnum type, TNetwork network)
    {
        _types.Add((type, network));
    }

    public TEnum GetType(TAddress address)
    {
        foreach ((TEnum type, TNetwork network) entry in _types)
        {
            if (Contains(entry.network, address))
                return entry.type;
        }

        return default;
    }

    public TEnum GetType(TNetwork network)
    {
        foreach ((TEnum type, TNetwork network) entry in _types)
        {
            if (Contains(entry.network, network))
                return entry.type;
        }

        return default;
    }

    public IEnumerable<TNetwork> GetNetworks(TEnum type)
    {
        foreach ((TEnum type, TNetwork network) entry in _types)
        {
            if (Equals(entry.type, type))
                yield return entry.network;
        }
    }

    public IEnumerable<(TEnum type, TNetwork network)> GetWellKnownNetworks() => _types;
}

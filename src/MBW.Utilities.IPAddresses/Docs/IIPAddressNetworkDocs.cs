namespace MBW.Utilities.IPAddresses.Docs;

/// <summary>
/// Docs interface to copy xmldocs from, do not use.
/// </summary>
internal interface IIPAddressNetworkDocs<TType>
{
    /// <summary>
    /// Determines if this network instance completely contains the specified network.
    /// </summary>
    bool Contains(TType other);

    /// <summary>
    /// Determines if this network instance completely contains or is equal to the specified network.
    /// </summary>
    bool ContainsOrEqual(TType other);
}

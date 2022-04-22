using System;
using System.IO;
using System.Text;

namespace MBW.Utilities.IPAddresses;

public partial struct IpAddressNetworkV4
{
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(NetworkAddress);
        sb.Append("/").Append(_mask.ToString());

        return sb.ToString();
    }

    public string ToPrefixString()
    {
        uint asUint = _networkAddress.Address;
        if (_mask <= 8 && (asUint & 0xFFFFFF) == 0)
        {
            // Return 1 octet
            return (asUint >> 24) + "/" + _mask;
        }

        if (_mask <= 16 && (asUint & 0xFFFF) == 0)
        {
            // Return 2 octets
            return (asUint >> 24) + "." + ((asUint >> 16) & 0xFF) + "/" + _mask;
        }
        if (_mask <= 24 && (asUint & 0xFF) == 0)
        {
            // Return 3 octets
            return (asUint >> 24) + "." + ((asUint >> 16) & 0xFF) + "." + ((asUint >> 8) & 0xFF) + "/" + _mask;

        }

        return ToString();
    }

    public void ToString(StringBuilder sb)
    {
        sb.Append(ToString());
    }

    public void ToString(TextWriter tw)
    {
        tw.Write(ToString());
    }

    public void ToString(Span<char> span) => ToString(span, out _);

    public void ToString(Span<char> span, out int written)
    {
        ReadOnlySpan<char> str = ToString().AsSpan();
        str.CopyTo(span);
        written = str.Length;
    }
}
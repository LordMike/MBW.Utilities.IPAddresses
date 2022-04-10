using System;
using System.IO;
using System.Text;

namespace MBW.Utilities.IPAddresses;

public partial struct IpAddressNetworkV6
{
    public override string ToString()
    {
        return ToString(false);
    }

    public string ToString(bool forceCidr)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(NetworkAddress);

        if (forceCidr || _mask != 128)
            sb.Append("/").Append(_mask.ToString());

        return sb.ToString();
    }

    public string ToDecimalDotted(bool forceCidr = false)
    {
        StringBuilder sb = new StringBuilder();

        ulong high = _networkAddress.AddressHigh;
        ulong low = _networkAddress.AddressLow;

        sb.Append((high >> 56).ToString()).Append(".");
        sb.Append(((high >> 48) & 0xFF).ToString()).Append(".");
        sb.Append(((high >> 40) & 0xFF).ToString()).Append(".");
        sb.Append(((high >> 32) & 0xFF).ToString()).Append(".");
        sb.Append(((high >> 24) & 0xFF).ToString()).Append(".");
        sb.Append(((high >> 16) & 0xFF).ToString()).Append(".");
        sb.Append(((high >> 8) & 0xFF).ToString()).Append(".");
        sb.Append((high & 0xFF).ToString()).Append(".");

        sb.Append((low >> 56).ToString()).Append(".");
        sb.Append(((low >> 48) & 0xFF).ToString()).Append(".");
        sb.Append(((low >> 40) & 0xFF).ToString()).Append(".");
        sb.Append(((low >> 32) & 0xFF).ToString()).Append(".");
        sb.Append(((low >> 24) & 0xFF).ToString()).Append(".");
        sb.Append(((low >> 16) & 0xFF).ToString()).Append(".");
        sb.Append(((low >> 8) & 0xFF).ToString()).Append(".");
        sb.Append((low & 0xFF).ToString());

        if (forceCidr || _mask != 128)
            sb.Append("/").Append(_mask.ToString());

        return sb.ToString();
    }

    public string ToPrefixString()
    {
        //if (_mask <= 8 && (_address & 0xFFFFFF) == 0)
        //{
        //	// Return 1 octet
        //	return (_address >> 24) + "/" + _mask;
        //}

        //if (_mask <= 16 && (_address & 0xFFFF) == 0)
        //{
        //	// Return 2 octets
        //	return (_address >> 24) + "." + ((_address >> 16) & 0xFF) + "/" + _mask;
        //}
        //if (_mask <= 24 && (_address & 0xFF) == 0)
        //{
        //	// Return 3 octets
        //	return (_address >> 24) + "." + ((_address >> 16) & 0xFF) + "." + ((_address >> 8) & 0xFF) + "/" + _mask;

        //}

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

    public void ToString(Span<char> span)
    {
        ReadOnlySpan<char> str = ToString().AsSpan();
        str.CopyTo(span);
    }
}
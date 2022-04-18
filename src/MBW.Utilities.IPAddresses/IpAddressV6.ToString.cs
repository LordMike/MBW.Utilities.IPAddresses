using System;
using System.IO;
using System.Net;
using System.Text;

namespace MBW.Utilities.IPAddresses;

public partial struct IpAddressV6
{
    public override string ToString()
    {
        return ((IPAddress)this).ToString();
    }

    public string ToDecimalDotted()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append((AddressHigh >> 56).ToString()).Append(".");
        sb.Append(((AddressHigh >> 48) & 0xFF).ToString()).Append(".");
        sb.Append(((AddressHigh >> 40) & 0xFF).ToString()).Append(".");
        sb.Append(((AddressHigh >> 32) & 0xFF).ToString()).Append(".");
        sb.Append(((AddressHigh >> 24) & 0xFF).ToString()).Append(".");
        sb.Append(((AddressHigh >> 16) & 0xFF).ToString()).Append(".");
        sb.Append(((AddressHigh >> 8) & 0xFF).ToString()).Append(".");
        sb.Append((AddressHigh & 0xFF).ToString()).Append(".");

        sb.Append((AddressLow >> 56).ToString()).Append(".");
        sb.Append(((AddressLow >> 48) & 0xFF).ToString()).Append(".");
        sb.Append(((AddressLow >> 40) & 0xFF).ToString()).Append(".");
        sb.Append(((AddressLow >> 32) & 0xFF).ToString()).Append(".");
        sb.Append(((AddressLow >> 24) & 0xFF).ToString()).Append(".");
        sb.Append(((AddressLow >> 16) & 0xFF).ToString()).Append(".");
        sb.Append(((AddressLow >> 8) & 0xFF).ToString()).Append(".");
        sb.Append((AddressLow & 0xFF).ToString());

        return sb.ToString();
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
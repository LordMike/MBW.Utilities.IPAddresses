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

        sb.Append((_addressHigh >> 56).ToString()).Append(".");
        sb.Append(((_addressHigh >> 48) & 0xFF).ToString()).Append(".");
        sb.Append(((_addressHigh >> 40) & 0xFF).ToString()).Append(".");
        sb.Append(((_addressHigh >> 32) & 0xFF).ToString()).Append(".");
        sb.Append(((_addressHigh >> 24) & 0xFF).ToString()).Append(".");
        sb.Append(((_addressHigh >> 16) & 0xFF).ToString()).Append(".");
        sb.Append(((_addressHigh >> 8) & 0xFF).ToString()).Append(".");
        sb.Append((_addressHigh & 0xFF).ToString()).Append(".");

        sb.Append((_addressLow >> 56).ToString()).Append(".");
        sb.Append(((_addressLow >> 48) & 0xFF).ToString()).Append(".");
        sb.Append(((_addressLow >> 40) & 0xFF).ToString()).Append(".");
        sb.Append(((_addressLow >> 32) & 0xFF).ToString()).Append(".");
        sb.Append(((_addressLow >> 24) & 0xFF).ToString()).Append(".");
        sb.Append(((_addressLow >> 16) & 0xFF).ToString()).Append(".");
        sb.Append(((_addressLow >> 8) & 0xFF).ToString()).Append(".");
        sb.Append((_addressLow & 0xFF).ToString());

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
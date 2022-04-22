using System;
using System.IO;
using System.Net;
using System.Text;

namespace MBW.Utilities.IPAddresses;

public partial struct IpAddressV4
{
    public override string ToString()
    {
        return ((IPAddress)this).ToString();
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
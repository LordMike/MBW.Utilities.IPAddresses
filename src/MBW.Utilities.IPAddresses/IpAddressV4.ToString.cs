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
        throw new NotImplementedException();
    }

    public void ToString(TextWriter tw)
    {
        throw new NotImplementedException();
    }

    public void ToString(Span<char> span)
    {
        throw new NotImplementedException();
    }
}
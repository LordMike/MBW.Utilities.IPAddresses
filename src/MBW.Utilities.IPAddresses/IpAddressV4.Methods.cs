using System;

namespace MBW.Utilities.IPAddresses;

public partial struct IpAddressV4
{
    public bool IsContainedIn(IpAddressNetworkV4 network) => network.Contains(this);

    public void AddressToBytes(Span<byte> bytes)
    {
        if (bytes.Length < 4)
            throw new ArgumentOutOfRangeException(nameof(bytes));

        bytes[0] = (byte)((_address >> 24) & 0xFF);
        bytes[1] = (byte)((_address >> 16) & 0xFF);
        bytes[2] = (byte)((_address >> 8) & 0xFF);
        bytes[3] = (byte)(_address & 0xFF);
    }

    public byte[] AddressToBytes()
    {
        byte[] res = new byte[4];
        AddressToBytes(res);

        return res;
    }

    public void AddressToBytes(byte[] bytes, int offset = 0)
    {
        if (bytes == null)
            throw new ArgumentNullException(nameof(bytes));

        AddressToBytes(bytes.AsSpan().Slice(offset, 4));
    }

    public static IpAddressV4 operator &(IpAddressV4 left, IpAddressV4 right)
    {
        return new IpAddressV4(left.Address & right.Address);
    }

    public static IpAddressV4 operator |(IpAddressV4 left, IpAddressV4 right)
    {
        return new IpAddressV4(left.Address | right.Address);
    }
}
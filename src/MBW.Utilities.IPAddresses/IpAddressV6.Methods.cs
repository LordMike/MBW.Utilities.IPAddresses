using System;

namespace MBW.Utilities.IPAddresses;

public partial struct IpAddressV6
{
    public bool IsContainedIn(IpAddressNetworkV6 network) => network.Contains(this);

    public void AddressToBytes(Span<byte> bytes)
    {
        if (bytes.Length < 16)
            throw new ArgumentOutOfRangeException(nameof(bytes));

        bytes[0] = (byte)((_addressHigh >> 56) & 0xFF);
        bytes[1] = (byte)((_addressHigh >> 48) & 0xFF);
        bytes[2] = (byte)((_addressHigh >> 40) & 0xFF);
        bytes[3] = (byte)((_addressHigh >> 32) & 0xFF);
        bytes[4] = (byte)((_addressHigh >> 24) & 0xFF);
        bytes[5] = (byte)((_addressHigh >> 16) & 0xFF);
        bytes[6] = (byte)((_addressHigh >> 8) & 0xFF);
        bytes[7] = (byte)(_addressHigh & 0xFF);
        bytes[8] = (byte)((_addressLow >> 56) & 0xFF);
        bytes[9] = (byte)((_addressLow >> 48) & 0xFF);
        bytes[10] = (byte)((_addressLow >> 40) & 0xFF);
        bytes[11] = (byte)((_addressLow >> 32) & 0xFF);
        bytes[12] = (byte)((_addressLow >> 24) & 0xFF);
        bytes[13] = (byte)((_addressLow >> 16) & 0xFF);
        bytes[14] = (byte)((_addressLow >> 8) & 0xFF);
        bytes[15] = (byte)(_addressLow & 0xFF);
    }

    public byte[] AddressToBytes()
    {
        byte[] res = new byte[16];
        AddressToBytes(res);

        return res;
    }

    public void AddressToBytes(byte[] bytes, int offset = 0)
    {
        if (bytes == null)
            throw new ArgumentNullException(nameof(bytes));

        AddressToBytes(bytes.AsSpan().Slice(offset, 16));
    }

    public static IpAddressV6 operator &(IpAddressV6 left, IpAddressV6 right)
    {
        ulong high = left.AddressHigh & right.AddressHigh;
        ulong low = left.AddressLow & right.AddressLow;
        return new IpAddressV6(high, low);
    }

    public static IpAddressV6 operator |(IpAddressV6 left, IpAddressV6 right)
    {
        ulong high = left.AddressHigh | right.AddressHigh;
        ulong low = left.AddressLow | right.AddressLow;
        return new IpAddressV6(high, low);
    }
}
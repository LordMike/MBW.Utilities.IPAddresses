using System;

namespace MBW.Utilities.IPAddresses;

public partial struct IpAddressV6
{
    public bool IsContainedIn(IpAddressNetworkV6 network) => network.Contains(this);

    public void AddressToBytes(Span<byte> bytes)
    {
        if (bytes.Length < 16)
            throw new ArgumentOutOfRangeException(nameof(bytes));

        bytes[0] = (byte)((AddressHigh >> 56) & 0xFF);
        bytes[1] = (byte)((AddressHigh >> 48) & 0xFF);
        bytes[2] = (byte)((AddressHigh >> 40) & 0xFF);
        bytes[3] = (byte)((AddressHigh >> 32) & 0xFF);
        bytes[4] = (byte)((AddressHigh >> 24) & 0xFF);
        bytes[5] = (byte)((AddressHigh >> 16) & 0xFF);
        bytes[6] = (byte)((AddressHigh >> 8) & 0xFF);
        bytes[7] = (byte)(AddressHigh & 0xFF);
        bytes[8] = (byte)((AddressLow >> 56) & 0xFF);
        bytes[9] = (byte)((AddressLow >> 48) & 0xFF);
        bytes[10] = (byte)((AddressLow >> 40) & 0xFF);
        bytes[11] = (byte)((AddressLow >> 32) & 0xFF);
        bytes[12] = (byte)((AddressLow >> 24) & 0xFF);
        bytes[13] = (byte)((AddressLow >> 16) & 0xFF);
        bytes[14] = (byte)((AddressLow >> 8) & 0xFF);
        bytes[15] = (byte)(AddressLow & 0xFF);
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
using System.Net;

namespace IPAddresses.Helpers
{
	internal static class ConversionUtilities
	{
		public static IPAddress ToIp(uint value)
		{
			return new IPAddress(BitUtilities.Reverse(value));
		}

		public static IPAddress ToIp(ulong high, ulong low)
		{
			byte[] data = new byte[16];
			data[0] = (byte)(high >> 56);
			data[1] = (byte)(high >> 48);
			data[2] = (byte)(high >> 40);
			data[3] = (byte)(high >> 32);
			data[4] = (byte)(high >> 24);
			data[5] = (byte)(high >> 16);
			data[6] = (byte)(high >> 8);
			data[7] = (byte)high;

			data[8] = (byte)(low >> 56);
			data[9] = (byte)(low >> 48);
			data[10] = (byte)(low >> 40);
			data[11] = (byte)(low >> 32);
			data[12] = (byte)(low >> 24);
			data[13] = (byte)(low >> 16);
			data[14] = (byte)(low >> 8);
			data[15] = (byte)low;

			return new IPAddress(data);
		}
	}
}
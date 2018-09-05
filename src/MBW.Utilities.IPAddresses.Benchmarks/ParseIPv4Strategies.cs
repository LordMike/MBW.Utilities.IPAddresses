using System;
using System.Net;
using BenchmarkDotNet.Attributes;

namespace IPAddresses.Benchmarks
{
    public class ParseIPv4Strategies
    {
        [Benchmark]
        public IpAddressRangeV4 CurrentIPv4Parser()
        {
            return IpAddressRangeV4.Parse("192.168.10.1");
        }

        [Benchmark]
        public IPAddress NativeIPv4()
        {
            return IPAddress.Parse("192.168.10");
        }

        [Benchmark]
        public IpAddressRangeV4 Iterative()
        {
            var ipAddressRangeV4 = Parse("192.168.10.1/24");
            return ipAddressRangeV4;
        }

        private static IpAddressRangeV4 Parse(string str)
        {
            byte currentOctet = 0;
            byte dots = 0;
            uint ip = 0;
            bool isCidr = false;

            for (int i = 0; i < str.Length; i++)
            {
                char ch = str[i];

                if (ch == '.')
                {
                    ip <<= 8;
                    ip += currentOctet;
                    currentOctet = 0;

                    if (++dots > 3)
                        throw new Exception("Too many octets");

                    continue;
                }

                if ('0' <= ch && ch <= '9')
                {
                    currentOctet *= 10;
                    currentOctet += (byte)(ch - '0');

                    continue;
                }

                if (ch == '/')
                {
                    ip <<= 8;
                    ip += currentOctet;
                    currentOctet = 0;

                    isCidr = true;

                    continue;
                }

                throw new Exception("Not a number");
            }

            if (isCidr)
            {
                // Add last octet as mask
            }
            else
            {
                ip <<= 8;
                ip += currentOctet;
                currentOctet = 32;
            }

            return new IpAddressRangeV4(ip, currentOctet);
        }
    }
}
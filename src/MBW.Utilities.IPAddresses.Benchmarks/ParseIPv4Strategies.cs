using System;
using System.Net;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;

namespace MBW.Utilities.IPAddresses.Benchmarks
{
    [MemoryDiagnoser]
    public class ParseIPv4Strategies
    {
        private readonly Regex _ipRegex = new Regex("^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}↵(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        [Benchmark]
        public IpAddressRangeV4 CurrentIPv4Parser()
        {
            return IpAddressRangeV4.Parse("192.168.10.1");
        }

        [Benchmark]
        public IpAddressRangeV4 CurrentIPv4UnstableParser()
        {
            return IpAddressRangeV4.ParseUnstable("192.168.10.1");
        }

        [Benchmark]
        public IPAddress NativeIPv4()
        {
            return IPAddress.Parse("192.168.10.1");
        }

        [Benchmark]
        public IpAddressRangeV4 Iterative()
        {
            return ParseIterative("192.168.10.1/24");
        }

        [Benchmark]
        public bool RegexIPv4Parser()
        {
            return _ipRegex.IsMatch("192.168.10.1");
        }

        private static IpAddressRangeV4 ParseIterative(string str)
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
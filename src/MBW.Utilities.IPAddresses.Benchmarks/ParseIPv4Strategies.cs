﻿using BenchmarkDotNet.Attributes;
using System;
using System.Net;
using System.Text.RegularExpressions;

namespace MBW.Utilities.IPAddresses.Benchmarks;

[MemoryDiagnoser]
[ShortRunJob]
//[SimpleJob(runtimeMoniker: RuntimeMoniker.Net50)]
//[SimpleJob(runtimeMoniker: RuntimeMoniker.Net60)]
public class ParseIPv4Strategies
{
    private readonly Regex _ipRegex = new Regex("^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}↵(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    [Benchmark]
    public IpAddressNetworkV4 CurrentIPv4Parser()
    {
        return IpAddressNetworkV4.Parse("192.168.10.1");
    }

    [Benchmark]
    public IpAddressNetworkV4 CurrentIPv4UnstableParser()
    {
        return IpAddressNetworkV4.ParseUnstable("192.168.10.1");
    }

    [Benchmark(Baseline = true)]
    public IPAddress NativeIPv4()
    {
        return IPAddress.Parse("192.168.10.1");
    }

    [Benchmark]
    public IpAddressNetworkV4 Iterative()
    {
        return ParseIterative("192.168.10.1/24");
    }

    [Benchmark]
    public bool RegexIPv4Parser()
    {
        return _ipRegex.IsMatch("192.168.10.1");
    }

    private static IpAddressNetworkV4 ParseIterative(string str)
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

            if (ch is >= '0' and <= '9')
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

        return new IpAddressNetworkV4(ip, currentOctet);
    }
}
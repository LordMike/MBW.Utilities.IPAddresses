﻿using BenchmarkDotNet.Attributes;
using System;
using System.Net;

namespace MBW.Utilities.IPAddresses.Benchmarks;

[MemoryDiagnoser]
[ShortRunJob]
//[SimpleJob(runtimeMoniker: RuntimeMoniker.Net50)]
//[SimpleJob(runtimeMoniker: RuntimeMoniker.Net60)]
public class ParseIPv6Strategies
{
    [Benchmark]
    public IpAddressNetworkV6 CurrentIPv6Parser()
    {
        return IpAddressNetworkV6.Parse("2001:0dff:44ff:0:1744::ffff/64");
    }

    [Benchmark]
    public IpAddressNetworkV6 CurrentIPv6UnstableParser()
    {
        return IpAddressNetworkV6.ParseUnstable("2001:0dff:44ff:0:1744::ffff/64");
    }

    [Benchmark(Baseline = true)]
    public IPAddress NativeIPv6()
    {
        return IPAddress.Parse("2001:0dff:44ff:0:1744::ffff");
    }

    [Benchmark]
    public IpAddressNetworkV6 Iterative()
    {
        IpAddressNetworkV6 ip = IterativeParse("2001:dff:44ff:0:1744::ffff:4d4d/120");
        return ip;
    }

    private static IpAddressNetworkV6 IterativeParse(string str)
    {
        ReadOnlySpan<char> @string = str.AsSpan();

        ulong high = 0;
        ulong low = 0;
        byte mask = 0;
        byte idx = 1;

        void SetTuplet(byte index, ulong newValue)
        {
            if (index < 5)
                high |= newValue << ((3 - index) * 16);
            else
                low |= newValue << ((7 - index) * 16);
        }

        byte ParseChar(char ch)
        {
            if (ch is >= '0' and <= '9')
                return (byte)(ch - '0');

            if (ch is >= 'A' and <= 'F')
                return (byte)(10 + ch - 'A');

            if (ch is >= 'a' and <= 'f')
                return (byte)(10 + ch - 'a');

            return byte.MaxValue;
        }

        // Find mask, read reverse
        for (int i = @string.Length - 1; i >= 0; i--)
        {
            char ch = @string[i];

            if (ch == ':' || ch is >= 'A' and <= 'F' || ch is >= 'a' and <= 'f')
            {
                // No mask provided
                mask = 128;
                break;
            }

            if (ch == '/')
            {
                // Mask has been read, cut off the mask
                @string = @string[..i];
                break;
            }

            if (ch is >= '0' and <= '9')
            {
                mask += (byte)((ch - '0') * idx);
                idx *= 10;

                continue;
            }

            throw new ArgumentException();
        }

        // Tuple offset
        idx = 0;

        // Read from front, until :: is met
        bool doReverse = false;

        while (@string.Length > 0)
        {
            ushort currentTuplet = 0;

            // Read up to 4 chars
            int i;
            for (i = 0; i < 4; i++)
            {
                char ch = @string[i];

                if (ch == ':')
                    break;

                // Read and append char
                byte val = ParseChar(ch);

                if (val == byte.MaxValue)
                    throw new ArgumentException();

                currentTuplet <<= 4;
                currentTuplet += val;
            }

            SetTuplet(idx++, currentTuplet);
            @string = @string[i..];

            // Check if next char is ':'
            if (@string[0] == ':')
            {
                @string = @string[1..];

                // Check if next again is ':' too
                if (@string[0] == ':')
                {
                    // We need to parse backwards now
                    doReverse = true;
                    break;
                }
            }
            else
                throw new ArgumentException();
        }

        if (doReverse)
        {
            idx = 7;

            while (@string.Length > 0)
            {
                ushort currentTuplet = 0;

                // Read up to 4 chars
                // Find ':'

                int lastIdx = @string.LastIndexOf(':');

                ReadOnlySpan<char> segment = @string[(lastIdx + 1)..];
                @string = @string[..lastIdx];

                if (segment.Length == 0 || segment.Length > 4)
                    // We cannot have two "::" segments, cannot have longer than 4 chars
                    throw new Exception();

                int i;
                for (i = 0; i < segment.Length; i++)
                {
                    char ch = segment[i];

                    if (ch == ':')
                        break;

                    // Read and append char
                    byte val = ParseChar(ch);

                    if (val == byte.MaxValue)
                        throw new ArgumentException();

                    currentTuplet <<= 4;
                    currentTuplet += val;
                }

                SetTuplet(idx--, currentTuplet);
            }
        }

        return new IpAddressNetworkV6(high, low, mask);
    }
}
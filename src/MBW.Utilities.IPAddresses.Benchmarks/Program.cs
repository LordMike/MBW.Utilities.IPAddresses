﻿using BenchmarkDotNet.Running;

namespace IPAddresses.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new ParseIPv6Strategies().Iterative();

            var switcher = new BenchmarkSwitcher(new[] {
                typeof(FindCommonPrefix),
                typeof(ParseIPv4s),
                typeof(ReverseNumbers),
                typeof(ParseIPv4Strategies),
                typeof(ParseIPv6Strategies)
            });

            switcher.Run(args);
        }
    }
}
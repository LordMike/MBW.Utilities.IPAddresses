using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Linq;
using System.Reflection;

namespace MBW.Utilities.IPAddresses.Benchmarks;

public class Program
{
    public static void Main(string[] args)
    {
        new ParseIPv6Strategies().Iterative();

        Type[] benchmarkTypes = typeof(Program).Assembly
            .GetTypes()
            .Where(s => s.GetMethods().Any(x => x.GetCustomAttribute<BenchmarkAttribute>() != null))
            .ToArray();

        BenchmarkSwitcher switcher = new BenchmarkSwitcher(benchmarkTypes);

        switcher.Run(args);
    }
}
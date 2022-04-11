using BenchmarkDotNet.Running;

namespace MBW.Utilities.IPAddresses.Benchmarks;

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkSwitcher switcher = new BenchmarkSwitcher(typeof(Program).Assembly);
        switcher.Run(args);
    }
}
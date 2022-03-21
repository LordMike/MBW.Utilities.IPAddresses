using System;
using BenchmarkDotNet.Attributes;
using MBW.Utilities.IPAddresses.Tokenization;

namespace MBW.Utilities.IPAddresses.Benchmarks;

public class TokenizerTest
{
    [Benchmark]
    public int DoTokenizationIPv6()
    {
        ReadOnlySpan<char> span = "2001:0dff:44ff:0:1744:ffff/64".AsSpan();

        int cnt = 0;
        while (!span.IsEmpty)
        {
            (TokenType type, ushort value) tkn = Tokenizer.ReadToken(span, true, out int read);
            span = span[read..];

            cnt += tkn.value;
        }

        return cnt;
    }

    [Benchmark]
    public int DoTokenizationIPv6_Rev()
    {
        ReadOnlySpan<char> span = "2001:0dff:44ff:0:1744:ffff/64".AsSpan();

        int cnt = 0;
        while (!span.IsEmpty)
        {
            (TokenType type, ushort value) tkn = Tokenizer.ReadTokenReverse(span, true, out int read);
            span = span[..^read];

            cnt += tkn.value;
        }

        return cnt;
    }

    [Benchmark]
    public int DoTokenizationIPv4()
    {
        ReadOnlySpan<char> span = "192.168.255.45/32".AsSpan();

        int cnt = 0;
        while (!span.IsEmpty)
        {
            (TokenType type, ushort value) tkn = Tokenizer.ReadToken(span, false, out int read);
            span = span[read..];

            cnt += tkn.value;
        }

        return cnt;
    }

    [Benchmark]
    public int DoTokenizationIPv4_Rev()
    {
        ReadOnlySpan<char> span = "192.168.255.45/32".AsSpan();

        int cnt = 0;
        while (!span.IsEmpty)
        {
            (TokenType type, ushort value) tkn = Tokenizer.ReadTokenReverse(span, false, out int read);
            span = span[..^read];

            cnt += tkn.value;
        }

        return cnt;
    }
}
using System;
using BenchmarkDotNet.Attributes;
using MBW.Utilities.IPAddresses.Tokenization;

namespace MBW.Utilities.IPAddresses.Benchmarks;

[MemoryDiagnoser]
public class TokenizerTest
{
    [Benchmark]
    public void DoTokenizationIPv6()
    {
        ReadOnlySpan<char> span = "2001:0dff:44ff:0:1744:ffff/64".AsSpan();

        Tokenizer tokenizer = new(span);

        ParsedToken parsed;
        do
        {
            parsed = tokenizer.ParseAndAdvance();
        } while (parsed.Type != TokenType.None);
    }

    [Benchmark]
    public void DoTokenizationIPv6_Rev()
    {
        ReadOnlySpan<char> span = "2001:0dff:44ff:0:1744:ffff/64".AsSpan();

        Tokenizer tokenizer = new(span);

        ParsedToken parsed;
        do
        {
            parsed = tokenizer.ParseAndAdvanceReverse();
        } while (parsed.Type != TokenType.None);
    }

    [Benchmark]
    public void DoTokenizationIPv4()
    {
        ReadOnlySpan<char> span = "192.168.144.244/32".AsSpan();

        Tokenizer tokenizer = new(span);

        ParsedToken parsed;
        do
        {
            parsed = tokenizer.ParseAndAdvance(false);
        } while (parsed.Type != TokenType.None);
    }

    [Benchmark]
    public void DoTokenizationIPv4_Rev()
    {
        ReadOnlySpan<char> span = "192.168.144.244/32".AsSpan();

        Tokenizer tokenizer = new(span);

        ParsedToken parsed;
        do
        {
            parsed = tokenizer.ParseAndAdvanceReverse(false);
        } while (parsed.Type != TokenType.None);
    }

    [Benchmark]
    public int PrevVersion_DoTokenizationIPv6()
    {
        ReadOnlySpan<char> span = "2001:0dff:44ff:0:1744:ffff/64".AsSpan();

        int cnt = 0;
        while (!span.IsEmpty)
        {
            (TokenType type, ushort value) tkn = Tokenizer_Methods.ReadToken(span, true, out int read);
            span = span[read..];

            cnt += tkn.value;
        }

        return cnt;
    }

    [Benchmark]
    public int PrevVersion_DoTokenizationIPv6_Rev()
    {
        ReadOnlySpan<char> span = "2001:0dff:44ff:0:1744:ffff/64".AsSpan();

        int cnt = 0;
        while (!span.IsEmpty)
        {
            (TokenType type, ushort value) tkn = Tokenizer_Methods.ReadTokenReverse(span, true, out int read);
            span = span[..^read];

            cnt += tkn.value;
        }

        return cnt;
    }

    [Benchmark]
    public int PrevVersion_DoTokenizationIPv4()
    {
        ReadOnlySpan<char> span = "192.168.255.45/32".AsSpan();

        int cnt = 0;
        while (!span.IsEmpty)
        {
            (TokenType type, ushort value) tkn = Tokenizer_Methods.ReadToken(span, false, out int read);
            span = span[read..];

            cnt += tkn.value;
        }

        return cnt;
    }

    [Benchmark]
    public int PrevVersion_DoTokenizationIPv4_Rev()
    {
        ReadOnlySpan<char> span = "192.168.255.45/32".AsSpan();

        int cnt = 0;
        while (!span.IsEmpty)
        {
            (TokenType type, ushort value) tkn = Tokenizer_Methods.ReadTokenReverse(span, false, out int read);
            span = span[..^read];

            cnt += tkn.value;
        }

        return cnt;
    }
}
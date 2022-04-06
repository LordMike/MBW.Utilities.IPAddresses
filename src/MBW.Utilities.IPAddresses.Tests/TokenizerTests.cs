using FluentAssertions;
using MBW.Utilities.IPAddresses.Tokenization;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MBW.Utilities.IPAddresses.Tests;

public class TokenizerTests
{
    [Fact]
    public void EmptyTest()
    {
        Tokenizer tokenizer = new Tokenizer(string.Empty);

        ParsedToken parsed = tokenizer.ParseAndAdvanceStart();
        parsed.Type.Should().Be(TokenType.None);

        parsed = tokenizer.ParseAndAdvanceEnd();
        parsed.Type.Should().Be(TokenType.None);

        parsed = tokenizer.PeekStart();
        parsed.Type.Should().Be(TokenType.None);

        parsed = tokenizer.PeekEnd();
        parsed.Type.Should().Be(TokenType.None);
    }

    [Fact]
    public void ReverseTest()
    {
        Tokenizer tokenizer = new Tokenizer("192.168");

        ParsedToken parsed = tokenizer.PeekEnd(false);
        parsed.Type.Should().Be(TokenType.Number);
        parsed.Value.Should().Be(168);

        parsed = tokenizer.ParseAndAdvanceEnd(false);
        parsed.Type.Should().Be(TokenType.Number);
        parsed.Value.Should().Be(168);

        parsed = tokenizer.PeekEnd(false);
        parsed.Type.Should().Be(TokenType.Dot);

        parsed = tokenizer.ParseAndAdvanceEnd(false);
        parsed.Type.Should().Be(TokenType.Dot);

        parsed = tokenizer.PeekEnd(false);
        parsed.Type.Should().Be(TokenType.Number);
        parsed.Value.Should().Be(192);

        parsed = tokenizer.ParseAndAdvanceEnd(false);
        parsed.Type.Should().Be(TokenType.Number);
        parsed.Value.Should().Be(192);

        parsed = tokenizer.PeekEnd(false);
        parsed.Type.Should().Be(TokenType.None);

        parsed = tokenizer.ParseAndAdvanceEnd(false);
        parsed.Type.Should().Be(TokenType.None);
    }

    [Fact]
    public void GenericNumberTest()
    {
        Tokenizer tokenizer = new Tokenizer("192.168");

        ParsedToken parsed = tokenizer.PeekStart(false);
        parsed.Type.Should().Be(TokenType.Number);
        parsed.Value.Should().Be(192);

        parsed = tokenizer.ParseAndAdvanceStart(false);
        parsed.Type.Should().Be(TokenType.Number);
        parsed.Value.Should().Be(192);

        parsed = tokenizer.PeekStart(false);
        parsed.Type.Should().Be(TokenType.Dot);

        parsed = tokenizer.ParseAndAdvanceStart(false);
        parsed.Type.Should().Be(TokenType.Dot);

        parsed = tokenizer.PeekStart(false);
        parsed.Type.Should().Be(TokenType.Number);
        parsed.Value.Should().Be(168);

        tokenizer.ParseAndAdvanceStart(false);
        parsed.Type.Should().Be(TokenType.Number);
        parsed.Value.Should().Be(168);

        parsed =  tokenizer.PeekStart(false);
        parsed.Type.Should().Be(TokenType.None);

        parsed = tokenizer.ParseAndAdvanceStart(false);
        parsed.Type.Should().Be(TokenType.None);
    }

    [Fact]
    public void ComplexValueIPv4()
    {
        Tokenizer tokenizer = new("192.168.114.255/32");

        List<ParsedToken> parsed = tokenizer.ParseAllStart(false).ToList();
        Assert.Equal(9, parsed.Count);
    }

    [Fact]
    public void ComplexValueIPv6()
    {
        Tokenizer tokenizer = new("2001:0dff:44ff:0:1744:ffff/64");

        List<ParsedToken> parsed = tokenizer.ParseAllStart().ToList();
        Assert.Equal(13, parsed.Count);
    }
}
using FluentAssertions;
using MBW.Utilities.IPAddresses.Tokenization;
using System;
using System.Collections.Generic;
using Xunit;

namespace MBW.Utilities.IPAddresses.Tests;

public class TokenizerTests
{
    [Fact]
    public void EmptyTest()
    {
        void Test(Func<ParsedToken> func) => func().Type.Should().Be(TokenType.None);

        Test(() => new Tokenizer(string.Empty).ParseAndAdvanceStart());
        Test(() => new Tokenizer(string.Empty).ParseAndAdvanceEnd());
        Test(() => new Tokenizer(string.Empty).PeekStart());
        Test(() => new Tokenizer(string.Empty).PeekEnd());
    }

    [Fact]
    public void ReverseTest()
    {
        Tokenizer tokenizer = new Tokenizer("192.168.55/44");

        ParsedToken[] expected = new ParsedToken[]
        {
            new(TokenType.Number, 44),
            new(TokenType.Slash, 0),
            new(TokenType.Number, 55),
            new(TokenType.Dot, 0),
            new(TokenType.Number, 168),
            new(TokenType.Dot, 0),
            new(TokenType.Number, 192)
        };

        IEnumerable<ParsedToken> tokens = tokenizer.ParseAllEnd(false);
        tokens.Should().Equal(expected);

        tokenizer.ParseAndAdvanceStart().Type.Should().Be(TokenType.None);
    }

    [Fact]
    public void ComplexValueIPv4()
    {
        Tokenizer tokenizer = new("192.168.114.255/32");

        ParsedToken[] expected = new ParsedToken[]
        {
            new(TokenType.Number, 192),
            new(TokenType.Dot, 0),
            new(TokenType.Number, 168),
            new(TokenType.Dot, 0),
            new(TokenType.Number, 114),
            new(TokenType.Dot, 0),
            new(TokenType.Number, 255),
            new(TokenType.Slash, 0),
            new(TokenType.Number, 32)
        };

        IEnumerable<ParsedToken> tokens = tokenizer.ParseAllStart(false);
        tokens.Should().Equal(expected);

        tokenizer.ParseAndAdvanceStart().Type.Should().Be(TokenType.None);
    }

    [Fact]
    public void ComplexValueIPv6()
    {
        Tokenizer tokenizer = new("2001:0dff:44ff::0:1744:ffff/64");

        ParsedToken[] expected = new ParsedToken[]
        {
            new(TokenType.Number, 0x2001),
            new(TokenType.Colon, 0),
            new(TokenType.Number, 0x0dff),
            new(TokenType.Colon, 0),
            new(TokenType.Number, 0x44ff),
            new(TokenType.DoubleColon, 0),
            new(TokenType.Number, 0),
            new(TokenType.Colon, 0),
            new(TokenType.Number, 0x1744),
            new(TokenType.Colon, 0),
            new(TokenType.Number, 0xffff),
            new(TokenType.Slash, 0),
            new(TokenType.Number, 0x64) // This would normally be base-10, but in this test it will be come hex.
        };

        IEnumerable<ParsedToken> tokens = tokenizer.ParseAllStart();
        tokens.Should().Equal(expected);

        tokenizer.ParseAndAdvanceStart().Type.Should().Be(TokenType.None);
    }
}
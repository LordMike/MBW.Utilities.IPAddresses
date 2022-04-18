﻿using Dirichlet.Numerics;
using FluentAssertions;
using MBW.Utilities.IPAddresses.Helpers;
using System.Numerics;
using Xunit;

namespace MBW.Utilities.IPAddresses.Tests;

public class BitUtilitiesTests
{
    [Theory]
    [InlineData(0b11110100110001001100101000011100, 0b01100100110001001100001010000000, 0)]
    [InlineData(0b01110100110001001100101000011100, 0b01100100110001001100001010000000, 3)]
    [InlineData(0b01100100110011001100101000011100, 0b01100100110001001100001010000000, 12)]
    [InlineData(0b01100100110001001100101000011100, 0b01100100110001001100001010000000, 20)]
    [InlineData(0b01100100110001001100101000011100, 0b01100100110001001100101000010100, 28)]
    [InlineData(0b01100100110001001100101000011100, 0b01100100110001001100101000011101, 31)]
    [InlineData(0b01100100110001001100101000011100, 0b01100100110001001100101000011100, 32)]
    public void TestUint(uint a, uint b, byte expected)
    {
        BitUtilities.FindCommonPrefixSize(a, b).Should().Be(expected);
    }

    [Theory]
    [InlineData("133944589059079306426977146018092861980", "304085772519548538158664449733976967708", 0)]
    [InlineData("133944589059079306426978271917999704604", "128627677075939642935363043676878326300", 5)]
    [InlineData("133944589059079306426978271917999704604", "133944589059078701964068464603412351516", 48)]
    [InlineData("133944589059079306426978271917999704604", "133944589059079306426977146018092861980", 77)]
    [InlineData("133944589059079306426977146018092861980", "133944589059079306426977146018092861980", 128)]
    public void TestUint128(string aStr, string bStr, byte expected)
    {
        UInt128 a = new UInt128(BigInteger.Parse(aStr));
        UInt128 b = new UInt128(BigInteger.Parse(bStr));

        BitUtilities.FindCommonPrefixSize(a, b).Should().Be(expected);
    }
}
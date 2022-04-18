/*
 * The MIT License (MIT)
 * 
 * Copyright (c) .NET Foundation and Contributors
 * 
 * All rights reserved.
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 * Source: https://github.com/ricksladkey/dirichlet-numerics
 * 
 * Changes:
 * - Removed internal UInt256 to simplify
 * - Changed internal representation to be MSB first (big endian) to match IP addresses
 * - Simplified operations, removing fractional & signed types, to simplify for IPv6 usage
 */

using System;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;

namespace Dirichlet.Numerics;

internal struct UInt128 : IFormattable, IComparable, IComparable<UInt128>, IEquatable<UInt128>
{
    private ulong s0;
    private ulong s1;

    private static readonly UInt128 maxValue = ~(UInt128)0;
    private static readonly UInt128 zero = (UInt128)0;
    private static readonly UInt128 one = (UInt128)1;

    public static UInt128 MinValue => zero;
    public static UInt128 MaxValue => maxValue;
    public static UInt128 Zero => zero;
    public static UInt128 One => one;

    public static UInt128 Parse(string value)
    {
        UInt128 c;
        if (!TryParse(value, out c))
            throw new FormatException();
        return c;
    }

    public static bool TryParse(string value, out UInt128 result) => TryParse(value, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out result);

    public static bool TryParse(string value, NumberStyles style, IFormatProvider provider, out UInt128 result)
    {
        BigInteger a;
        if (!BigInteger.TryParse(value, style, provider, out a))
        {
            result = Zero;
            return false;
        }
        Create(out result, a);
        return true;
    }

    public UInt128(long value) => Create(out this, value);
    public UInt128(ulong value) => Create(out this, value);
    public UInt128(BigInteger value) => Create(out this, value);

    public static void Create(out UInt128 c, uint r0, uint r1, uint r2, uint r3)
    {
        c.s0 = (ulong)r0 << 32 | r1;
        c.s1 = (ulong)r2 << 32 | r3;
    }

    public static void Create(out UInt128 c, ulong s0, ulong s1)
    {
        c.s0 = s0;
        c.s1 = s1;
    }

    public static void Create(out UInt128 c, ulong a)
    {
        c.s0 = 0;
        c.s1 = a;
    }

    public static void Create(out UInt128 c, BigInteger a)
    {
        a = BigInteger.Abs(a);

        c.s0 = (ulong)(a >> 64);
        c.s1 = (ulong)(a & ulong.MaxValue);
    }

    public ulong S0 => s0;
    public ulong S1 => s1;

    public bool IsZero => (s0 | s1) == 0;
    public bool IsOne => s0 == 0 && s1 == 1;
    public bool IsPowerOfTwo => (this & (this - 1)).IsZero;
    public bool IsEven => (s1 & 1) == 0;
    public int Sign => IsZero ? 0 : 1;

    public override string ToString() => ((BigInteger)this).ToString();
    public string ToString(string format) => ((BigInteger)this).ToString(format);
    public string ToString(IFormatProvider provider) => ToString(null, provider);
    public string ToString(string format, IFormatProvider provider) => ((BigInteger)this).ToString(format, provider);

    public static implicit operator UInt128(byte a)
    {
        UInt128 c;
        Create(out c, a);
        return c;
    }
    public static implicit operator UInt128(ushort a)
    {
        UInt128 c;
        Create(out c, a);
        return c;
    }
    public static implicit operator UInt128(uint a)
    {
        UInt128 c;
        Create(out c, a);
        return c;
    }
    public static implicit operator UInt128(ulong a)
    {
        UInt128 c;
        Create(out c, a);
        return c;
    }
    public static explicit operator UInt128(BigInteger a)
    {
        UInt128 c;
        Create(out c, a);
        return c;
    }
    public static explicit operator byte(UInt128 a) => (byte)a.s1;
    public static explicit operator ushort(UInt128 a) => (ushort)a.s1;
    public static explicit operator uint(UInt128 a) => (uint)a.s1;
    public static explicit operator ulong(UInt128 a) => a.s1;
    public static implicit operator BigInteger(UInt128 a)
    {
        if (a.s0 == 0)
            return a.s1;
        return (BigInteger)a.s0 << 64 | a.s1;
    }

    public static UInt128 operator <<(UInt128 a, int b)
    {
        UInt128 c;
        LeftShift(out c, ref a, b);
        return c;
    }
    public static UInt128 operator >>(UInt128 a, int b)
    {
        UInt128 c;
        RightShift(out c, ref a, b);
        return c;
    }
    public static UInt128 operator &(UInt128 a, UInt128 b)
    {
        UInt128 c;
        And(out c, ref a, ref b);
        return c;
    }
    public static UInt128 operator |(UInt128 a, UInt128 b)
    {
        UInt128 c;
        Or(out c, ref a, ref b);
        return c;
    }
    public static UInt128 operator ^(UInt128 a, UInt128 b)
    {
        UInt128 c;
        ExclusiveOr(out c, ref a, ref b);
        return c;
    }
    public static UInt128 operator ~(UInt128 a)
    {
        UInt128 c;
        Not(out c, ref a);
        return c;
    }
    public static UInt128 operator +(UInt128 a, UInt128 b)
    {
        UInt128 c;
        Add(out c, ref a, ref b);
        return c;
    }
    public static UInt128 operator ++(UInt128 a)
    {
        UInt128 c;
        UInt128 tmpOne = one;
        Add(out c, ref a, ref tmpOne);
        return c;
    }
    public static UInt128 operator -(UInt128 a, UInt128 b)
    {
        UInt128 c;
        Subtract(out c, ref a, ref b);
        return c;
    }
    public static UInt128 operator -(UInt128 a, ulong b)
    {
        UInt128 c;
        Subtract(out c, ref a, b);
        return c;
    }
    public static UInt128 operator -(ulong a, UInt128 b)
    {
        UInt128 c;
        Subtract(out c, a, ref b);
        return c;
    }
    public static UInt128 operator --(UInt128 a)
    {
        UInt128 c;
        Subtract(out c, ref a, 1);
        return c;
    }
    public static UInt128 operator +(UInt128 a) => a;
    
    public static bool operator <(UInt128 a, UInt128 b) => LessThan(ref a, ref b);
    public static bool operator <(UInt128 a, uint b) => LessThan(ref a, b);
    public static bool operator <(uint a, UInt128 b) => LessThan(a, ref b);
    public static bool operator <(UInt128 a, ulong b) => LessThan(ref a, b);
    public static bool operator <(ulong a, UInt128 b) => LessThan(a, ref b);
    public static bool operator <=(UInt128 a, UInt128 b) => !LessThan(ref b, ref a);
    public static bool operator <=(UInt128 a, uint b) => !LessThan(b, ref a);
    public static bool operator <=(uint a, UInt128 b) => !LessThan(ref b, a);
    public static bool operator <=(UInt128 a, ulong b) => !LessThan(b, ref a);
    public static bool operator <=(ulong a, UInt128 b) => !LessThan(ref b, a);
    public static bool operator >(UInt128 a, UInt128 b) => LessThan(ref b, ref a);
    public static bool operator >(UInt128 a, uint b) => LessThan(b, ref a);
    public static bool operator >(uint a, UInt128 b) => LessThan(ref b, a);
    public static bool operator >(UInt128 a, ulong b) => LessThan(b, ref a);
    public static bool operator >(ulong a, UInt128 b) => LessThan(ref b, a);
    public static bool operator >=(UInt128 a, UInt128 b) => !LessThan(ref a, ref b);
    public static bool operator >=(UInt128 a, uint b) => !LessThan(ref a, b);
    public static bool operator >=(uint a, UInt128 b) => !LessThan(a, ref b);
    public static bool operator >=(UInt128 a, ulong b) => !LessThan(ref a, b);
    public static bool operator >=(ulong a, UInt128 b) => !LessThan(a, ref b);
    public static bool operator ==(UInt128 a, UInt128 b) => a.Equals(b);
    public static bool operator ==(UInt128 a, uint b) => a.Equals(b);
    public static bool operator ==(uint a, UInt128 b) => b.Equals(a);
    public static bool operator ==(UInt128 a, ulong b) => a.Equals(b);
    public static bool operator ==(ulong a, UInt128 b) => b.Equals(a);
    public static bool operator !=(UInt128 a, UInt128 b) => !a.Equals(b);
    public static bool operator !=(UInt128 a, uint b) => !a.Equals(b);
    public static bool operator !=(uint a, UInt128 b) => !b.Equals(a);
    public static bool operator !=(UInt128 a, ulong b) => !a.Equals(b);
    public static bool operator !=(ulong a, UInt128 b) => !b.Equals(a);

    public int CompareTo(UInt128 other)
    {
        if (s0 != other.s0)
            return s0.CompareTo(other.s0);
        return s1.CompareTo(other.s1);
    }
    public int CompareTo(uint other)
    {
        if (s0 != 0)
            return 1;
        return s1.CompareTo((ulong)other);
    }
    public int CompareTo(ulong other)
    {
        if (s0 != 0)
            return 1;
        return s1.CompareTo(other);
    }
    public int CompareTo(object obj)
    {
        if (obj == null)
            return 1;
        if (!(obj is UInt128))
            throw new ArgumentException();
        return CompareTo((UInt128)obj);
    }

    private static bool LessThan(ref UInt128 a, ulong b) => a.s0 == 0 && a.s1 < b;
    private static bool LessThan(ulong a, ref UInt128 b) => b.s0 != 0 || a < b.s1;

    private static bool LessThan(ref UInt128 a, ref UInt128 b)
    {
        if (a.s0 != b.s0)
            return a.s0 < b.s0;
        return a.s1 < b.s1;
    }

    public static bool Equals(ref UInt128 a, ref UInt128 b) => a.s0 == b.s0 && a.s1 == b.s1;
    public bool Equals(UInt128 other) => s0 == other.s0 && s1 == other.s1;
    public bool Equals(uint other) => s0 == 0 && s1 == other;
    public bool Equals(ulong other) => s0 == 0 && s1 == other;

    public override bool Equals(object obj)
    {
        if (!(obj is UInt128))
            return false;
        return Equals((UInt128)obj);
    }

    public override int GetHashCode() => s0.GetHashCode() ^ s1.GetHashCode();

    public static UInt128 Abs(UInt128 a) => a;

    public static void Add(out UInt128 c, ref UInt128 a, ref UInt128 b)
    {
        c.s0 = a.s0 + b.s0;
        c.s1 = a.s1 + b.s1;
        if (c.s1 < a.s1 && c.s1 < b.s1)
            ++c.s0;
        Debug.Assert((BigInteger)c == ((BigInteger)a + (BigInteger)b) % ((BigInteger)1 << 128));
    }

    private static ulong Add(ulong a, ulong b, ref uint carry)
    {
        var c = a + b;
        if (c < a && c < b)
            ++carry;
        return c;
    }

    public static void Add(ref UInt128 a, ulong b)
    {
        var sum = a.s1 + b;
        if (sum < a.s1 && sum < b)
            ++a.s0;
        a.s1 = sum;
    }

    public static void Add(ref UInt128 a, ref UInt128 b)
    {
        var sum = a.s1 + b.s1;
        if (sum < a.s1 && sum < b.s1)
            ++a.s0;
        a.s1 = sum;
        a.s0 += b.s0;
    }

    public static void Add(ref UInt128 a, UInt128 b)
    {
        Add(ref a, ref b);
    }

    public static void Subtract(out UInt128 c, ref UInt128 a, ulong b)
    {
        c.s1 = a.s1 - b;
        c.s0 = a.s0;
        if (a.s1 < b)
            --c.s0;
        Debug.Assert((BigInteger)c == ((BigInteger)a - (BigInteger)b + ((BigInteger)1 << 128)) % ((BigInteger)1 << 128));
    }

    public static void Subtract(out UInt128 c, ulong a, ref UInt128 b)
    {
        c.s1 = a - b.s1;
        c.s0 = 0 - b.s0;
        if (a < b.s1)
            --c.s0;
        Debug.Assert((BigInteger)c == ((BigInteger)a - (BigInteger)b + ((BigInteger)1 << 128)) % ((BigInteger)1 << 128));
    }

    public static void Subtract(out UInt128 c, ref UInt128 a, ref UInt128 b)
    {
        c.s1 = a.s1 - b.s1;
        c.s0 = a.s0 - b.s0;
        if (a.s1 < b.s1)
            --c.s0;
        Debug.Assert((BigInteger)c == ((BigInteger)a - (BigInteger)b + ((BigInteger)1 << 128)) % ((BigInteger)1 << 128));
    }

    public static void Subtract(ref UInt128 a, ulong b)
    {
        if (a.s1 < b)
            --a.s0;
        a.s1 -= b;
    }

    public static void Subtract(ref UInt128 a, ref UInt128 b)
    {
        if (a.s1 < b.s1)
            --a.s0;
        a.s1 -= b.s1;
        a.s0 -= b.s0;
    }

    public static void Subtract(ref UInt128 a, UInt128 b) => Subtract(ref a, ref b);

    public static void Shift(out UInt128 c, ref UInt128 a, int d)
    {
        if (d < 0)
            RightShift(out c, ref a, -d);
        else
            LeftShift(out c, ref a, d);
    }

    public static ulong LeftShift64(out UInt128 c, ref UInt128 a, int d)
    {
        if (d == 0)
        {
            c = a;
            return 0;
        }
        var dneg = 64 - d;
        c.s0 = a.s0 << d | a.s1 >> dneg;
        c.s1 = a.s1 << d;
        return a.s0 >> dneg;
    }

    public static void LeftShift(out UInt128 c, ref UInt128 a, int b)
    {
        if (b < 64)
            LeftShift64(out c, ref a, b);
        else if (b == 64)
        {
            c.s1 = 0;
            c.s0 = a.s1;
            return;
        }
        else
        {
            c.s1 = 0;
            c.s0 = a.s1 << (b - 64);
        }
    }

    public static void RightShift64(out UInt128 c, ref UInt128 a, int b)
    {
        if (b == 0)
            c = a;
        else
        {
            c.s1 = a.s1 >> b | a.s0 << (64 - b);
            c.s0 = a.s0 >> b;
        }
    }

    public static void RightShift(out UInt128 c, ref UInt128 a, int b)
    {
        if (b < 64)
            RightShift64(out c, ref a, b);
        else if (b == 64)
        {
            c.s1 = a.s0;
            c.s0 = 0;
        }
        else
        {
            c.s1 = a.s0 >> (b - 64);
            c.s0 = 0;
        }
    }

    public static void And(out UInt128 c, ref UInt128 a, ref UInt128 b)
    {
        c.s1 = a.s1 & b.s1;
        c.s0 = a.s0 & b.s0;
    }

    public static void Or(out UInt128 c, ref UInt128 a, ref UInt128 b)
    {
        c.s1 = a.s1 | b.s1;
        c.s0 = a.s0 | b.s0;
    }

    public static void ExclusiveOr(out UInt128 c, ref UInt128 a, ref UInt128 b)
    {
        c.s1 = a.s1 ^ b.s1;
        c.s0 = a.s0 ^ b.s0;
    }

    public static void Not(out UInt128 c, ref UInt128 a)
    {
        c.s1 = ~a.s1;
        c.s0 = ~a.s0;
    }

    public static UInt128 Min(UInt128 a, UInt128 b)
    {
        if (LessThan(ref a, ref b))
            return a;
        return b;
    }

    public static UInt128 Max(UInt128 a, UInt128 b)
    {
        if (LessThan(ref b, ref a))
            return a;
        return b;
    }

    public static UInt128 Add(UInt128 a, UInt128 b)
    {
        UInt128 c;
        Add(out c, ref a, ref b);
        return c;
    }

    public static UInt128 Subtract(UInt128 a, UInt128 b)
    {
        UInt128 c;
        Subtract(out c, ref a, ref b);
        return c;
    }

    private static void RightShift64(ref UInt128 c, int d)
    {
        if (d == 0)
            return;
        c.s1 = c.s0 << (64 - d) | c.s1 >> d;
        c.s0 >>= d;
    }

    public static void RightShift(ref UInt128 c, int d)
    {
        if (d < 64)
            RightShift64(ref c, d);
        else
        {
            c.s1 = c.s0 >> (d - 64);
            c.s0 = 0;
        }
    }

    public static void Shift(ref UInt128 c, int d)
    {
        if (d < 0)
            RightShift(ref c, -d);
        else
            LeftShift(ref c, d);
    }

    public static void RightShift(ref UInt128 c)
    {
        c.s1 = c.s0 << 63 | c.s1 >> 1;
        c.s0 >>= 1;
    }

    private static ulong LeftShift64(ref UInt128 c, int d)
    {
        if (d == 0)
            return 0;
        var dneg = 64 - d;
        var result = c.s0 >> dneg;
        c.s0 = c.s0 << d | c.s1 >> dneg;
        c.s1 <<= d;
        return result;
    }

    public static void LeftShift(ref UInt128 c, int d)
    {
        if (d < 64)
            LeftShift64(ref c, d);
        else
        {
            c.s0 = c.s1 << (d - 64);
            c.s1 = 0;
        }
    }

    public static void LeftShift(ref UInt128 c)
    {
        c.s0 = c.s0 << 1 | c.s1 >> 63;
        c.s1 <<= 1;
    }
}

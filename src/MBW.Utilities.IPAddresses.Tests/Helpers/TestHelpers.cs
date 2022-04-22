using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace MBW.Utilities.IPAddresses.Tests.SignaturesLib;

internal static class TestHelpers
{
    delegate void ToString(Span<char> span);
    delegate void ToStringOut(Span<char> span, out int written);

    public static void TestToStringMethods(object obj, string expected)
    {
        List<MethodInfo> toStringMethods = obj.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(s => s.Name == nameof(object.ToString)).ToList();

        MethodInfo GetByArgs(params Type[] arguments)
        {
            return toStringMethods.RemoveGet(s =>
            {
                var @params = s.GetParameters();
                return @params.Count() == arguments.Length && @params.Select(x => x.ParameterType).SequenceEqual(arguments);
            });
        }

        // ToString()
        MethodInfo method = GetByArgs();
        ((string)method.Invoke(obj, null)).Should().Be(expected);

        // ToString(StringBuilder)
        method = GetByArgs(typeof(StringBuilder));
        StringBuilder sb = new StringBuilder();
        method.Invoke(obj, new object[] { sb });
        sb.ToString().Should().Be(expected);

        // ToString(TextWriter)
        method = GetByArgs(typeof(TextWriter));
        sb.Clear();
        TextWriter tw = new StringWriter(sb);
        method.Invoke(obj, new object[] { tw });
        sb.ToString().Should().Be(expected);

        // ToString(Span<char>)
        method = GetByArgs(typeof(Span<char>));
        char[] tmp = new char[60];
        method.CreateDelegate<ToString>(obj)(tmp);
        new string(tmp).TrimEnd('\0').Should().Be(expected);

        // ToString(Span<char>, out int)
        method = GetByArgs(typeof(Span<char>), typeof(int).MakeByRefType());
        method.CreateDelegate<ToStringOut>(obj)(tmp, out var written);
        new string(tmp.AsSpan().Slice(0, written)).Should().Be(expected);

        // No more ToString() methods should exist
        toStringMethods.Should().BeEmpty("Type " + obj.GetType().Name + " should have no more ToString() methods");
    }
}

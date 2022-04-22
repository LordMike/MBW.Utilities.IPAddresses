using MBW.Tests.SignaturesTester;

namespace MBW.Utilities.IPAddresses.Tests.SignaturesLib;

internal static class GenericSignaturesExtensions
{
    public static SignaturesBuilder MustHaveParseMethods(this SignaturesBuilder builder)
    {
        return builder
            .MustHave("static Boolean TryParse(System.String, TSelf ByRef)")
            .MustHave("static Boolean TryParse(System.ReadOnlySpan`1[System.Char], TSelf ByRef)")
            .MustHave("static TSelf op_Explicit(System.String)")
            .MustHave("static TSelf op_Explicit(System.ReadOnlySpan`1[System.Char])")
            .MustHave("static TSelf op_Implicit(System.Net.IPAddress)");
    }

    public static SignaturesBuilder MustHaveToStringMethods(this SignaturesBuilder builder)
    {
        return builder
            .MustHave("instance System.String ToString()")
            .MustHave("instance Void ToString(System.Text.StringBuilder)")
            .MustHave("instance Void ToString(System.IO.TextWriter)")
            .MustHave("instance Void ToString(System.Span`1[System.Char])")
            .MustHave("instance Void ToString(System.Span`1[System.Char], Int32 ByRef)");
    }

    public static SignaturesBuilder MustHaveEquality(this SignaturesBuilder builder)
    {
        return builder
            .MustHave("instance Boolean Equals(System.Object)")
            .MustHave("instance Boolean Equals(TSelf)")
            .MustHave("static Boolean op_Equality(TSelf, TSelf)")
            .MustHave("static Boolean op_Inequality(TSelf, TSelf)");
    }

    public static SignaturesBuilder MustHaveComparer(this SignaturesBuilder builder)
    {
        return builder
            .MustHave("instance Int32 CompareTo(System.Object)")
            .MustHave("instance Int32 CompareTo(TSelf)")
            .MustHave("static Boolean op_LessThan(TSelf, TSelf)")
            .MustHave("static Boolean op_LessThanOrEqual(TSelf, TSelf)")
            .MustHave("static Boolean op_GreaterThan(TSelf, TSelf)")
            .MustHave("static Boolean op_GreaterThanOrEqual(TSelf, TSelf)");
    }
}

namespace MBW.Utilities.IPAddresses.Tokenization;

internal enum TokenType : byte
{
    Slash = 10,
    Dot = 11,
    Colon = 12,
    DoubleColon = 13,
    Number = 20,
    Unknown = 254,
    None = 255,
}
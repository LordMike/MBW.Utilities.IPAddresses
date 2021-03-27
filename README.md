# IP address Parser and utilities for .NET [![Generic Build](https://github.com/LordMike/MBW.Utilities.IPAddresses/actions/workflows/dotnet.yml/badge.svg)](https://github.com/LordMike/MBW.Utilities.IPAddresses/actions/workflows/dotnet.yml) [![NuGet](https://img.shields.io/nuget/v/MBW.Utilities.IPAddresses.svg)](https://www.nuget.org/packages/MBW.Utilities.IPAddresses) [![GHPackages](https://img.shields.io/badge/package-alpha-green)](https://github.com/LordMike/MBW.Utilities.IPAddresses/packages/692030)


Fast IPv4 and IPv6 primitives and parsers for .NET.

## Features

* Supports [multiple formats](src/MBW.Utilities.IPAddresses.Tests/FormatTests.cs) of IPv4 and IPv6 writing
* Conversion to and from .NET's `IPAddress` type
* Fast tokenized parsing of addresses
* Support CIDR-style notation of networks
* Supports [calculations based on networks](src/MBW.Utilities.IPAddresses.Tests/UtilityTests.cs) such as contains
* Utilities to make smallest common networks for a set of IP addresses (supernets)


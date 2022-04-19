# IP address Parser and utilities for .NET [![Generic Build](https://github.com/LordMike/MBW.Utilities.IPAddresses/actions/workflows/dotnet.yml/badge.svg)](https://github.com/LordMike/MBW.Utilities.IPAddresses/actions/workflows/dotnet.yml) [![NuGet](https://img.shields.io/nuget/v/MBW.Utilities.IPAddresses.svg)](https://www.nuget.org/packages/MBW.Utilities.IPAddresses) [![GHPackages](https://img.shields.io/badge/package-alpha-green)](https://github.com/LordMike/MBW.Utilities.IPAddresses/packages/692030)

Fast IPv4 and IPv6 primitives and parsers for .NET.

## Features

* Works in `netstandard2.0`
* Supports [multiple formats](src/MBW.Utilities.IPAddresses.Tests/FormatTests.cs) of IPv4 and IPv6 writing
* Conversion to and from .NET's `IPAddress` type
* Tokenized parsing of addresses
* Structures to determine well known IPv4 and IPv6 addresses (`WellKnownIPv4` and `WellKnownIPv6`)
* Support parsing, writing and handling of CIDR-style notation of networks
* Supports [calculations based on networks](src/MBW.Utilities.IPAddresses.Tests/UtilityTests.cs) such as:
	* Contains sub networks and addresses
	* Comparsions of networks and addresses
* Network creation and splitting
  * Make the smallest common network for a set of IP addresses (supernets)
  * Split networks into N new subnetworks


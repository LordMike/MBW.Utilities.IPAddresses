# IP address Parser and utilities for .NET

Fast IPv4 and IPv6 primitives and parsers for .NET.

## Nuget

Find this on [Nuget](https://www.nuget.org/packages/MBW.Utilities.IPAddresses)

> MBW.Utilities.IPAddresses

## Features

* Supports [multiple formats](src/MBW.Utilities.IPAddresses.Tests/FormatTests.cs) of IPv4 and IPv6 writing
* Conversion to and from .NET's `IPAddress` type
* Fast tokenized parsing of addresses
* Support CIDR-style notation of networks
* Supports [calculations based on networks](src/MBW.Utilities.IPAddresses.Tests/UtilityTests.cs) such as contains
* Utilities to make smallest common networks for a set of IP addresses (supernets)


# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Next]

### Added
-

### Changed
- Allow ExchangeRate to have the same currency as both base and quote by @gliljas in #103
- Removed NumberStyle param for Parse and TryParse methods (Breaking Change)

### Removed
-

## [2.1.1]

### Added
- Allow creating Money using 'with' expression
- Add format N, Number format (e.g., "2,765.43")
- Add format F, Fixed point format (e.g., "2765,43")
- Add compatibility with Native AOT and Trimming

### Changed
- More defensive parsing in JSON converters by @gliljas in #102
- Migrate from SLN to SLNX solution file format
- Aligned the expiration date of the Netherlands Antillean guilder (ANG) with the amended timeline (ISO 4217 AMENDMENT NUMBER 176.)

### Removed
- Removed Microsoft.Bcl.HashCode dependency for .NET Standard 2.0

## [2.1]

### Added
- ISpanFormattable and IUtf8SpanFormattable implemented
- Partially implemented INumber<Money> by adding Radix, Zero, One, NegativeOne,
  Abs(), IsNegative(), IsPositive(), IsZero(), MinMagnitude(), MaxMagnitude()

### Changed
- Rename SafeDivide to Split and move in NodaMoney namespace
- NotNullWhenAttribute is now internal #101

### Removed
-

## [2.0]

### Added
- ISO 4217 Amendment Number 170 VED/926
- ISO 4217 Amendment Number 171 SLE/925 SLL/694
- ISO 4217 Amendment Number 172 SLL/694
- ISO 4217 Amendment Number 174 HRK/191
- ISO 4217 Amendment Number 176 ANG/532 XCG/532
- ISO 4217 Amendment Number 178 CUC/931 CUP/192
- System.Text.Json serialization support
- Added CurrencyInfo to provide information about a currency and also acts as an IFormatProvider. It implicitly cast
  to Currency.
- Support for .NET 9.0, .NET 8.0, .NET Standard 2.0 and .NET Standard 2.1
- Support for OLE Automation Currency conversion using ToOACurrency() and FromOACurrency()
- Add support for [Generic Math](https://devblogs.microsoft.com/dotnet/dotnet-7-generic-math/)
- Add Parsing for `ReadOnlySpan<char>`
- Formatting added format L for currency format using full english name (was format F)
- Added InternationalSymbol to CurrencyInfo (Symbol: `$`, InternationalSymbol: `US$`)
- Added AlternativeSymbols to CurrencyInfo
- Added NumericCode to CurrencyInfo. This is a three-digit code number as string (like '034')
- Added MinimalAmount to CurrencyInfo. This is the minimal amount the currency can be increased or decreased.

### Changed
- Big overall performance improvement on:
  - creation of currency (32x faster) and money (20x faster)
  - smaller footprint and memory allocation (30x fewer allocations)
  - faster operations, formatting and parsing
- JSON Serialization format is changed from `{ "Cost": {"Amount":1.23,"Currency":"USD"} }` to `{ "Cost":"USD 1.23" }`. This is a breaking change
  for JSON serialization, but deserialization of the old format is partly supported for migration purposes (only for System.Text.Json).
- XML Serialization format is change from `<Money Amount="765.43" Currency="USD" />` to `<Money Currency="USD">765.43</Money>`.
  This is a breaking change for XML serialization, but deserialization of the old format is supported for migration purposes.
- Currency is now a 2byte struct and only contains basic information. Other info is moved to CurrencyInfo.
- Namespaces are replaced by IsIso4217 (yes/no). Codes needs to be unique overall.
- Formatting format G is now Currency format with currency code instead of currency symbol
- Formatting format C is Currency format with currency symbol, but if there is none, currency code will be used.
- Changed Number on CurrencyInfo from `string` to `short`
- Changed DecimalDigits on CurrencyInfo from `decimal` to `int`
- Changed MinorUnit on CurrencyInfo to `MinorUnit` type

### Removed
- Removed support for JavaScriptSerializer in ASP.NET (NodaMoney.Serialization.AspNet)
- Removed support .NET Core 3.1, .NET 4.0 and .NET 4.5 (implicitly supported by .NET Standard 2.0)
- CurrencyBuilder is removed (use CurrencyInfo to Create, Register and Unregister).
- Formatting format I is removed (replaced by format G)
- Formatting format O is removed (replaced by format R)
- Formatting format F is removed (replaced by format L)
- Removed MajorUnit from CurrencyInfo

## [1.0.5] - 2018-08-29

### Added
- ISO 4217 Amendment Number 167
- ISO 4217 Amendment Number 168
- ISO 4217 Amendment Number 169
- Fix for rounded value in ExchangeRate #60

## [1.0.4] - 2018-04-18

### Added
- ISO 4217 Amendment Number 166

## [1.0.3] - 2017-12-17

### Added
- ISO 4217 Amendment Number 165

### Changed
- Fix Money XmlSerialization

## [1.0.2] - 2017-11-19

### Added
- ISO 4217 Amendment Number 164 #54
- Improved Money deserialization #55

### Changed
- Changed .NET Standard 1.6 to 1.3

## [1.0.1] - 2017-08-21

### Added
- ISO 4217 Amendment Number 163
- Issue #49 Plus Operator
- Convert .NET Core .json to .csproj

### Changed
- Removed PCL, replaced by .NET Standard

## [1.0.0] - 2016-10-10

### Added
- Changed ExchangeRate to accept 6 decimals instead of 4 decimals #43
- Add Chinese Yuan to MostUsedCurrencies (`var yuan = Money.Yuan(23)`)
- ISO 4217 Amendment Number 161 implemented #33
- Added Standard Money Format 'I' to display code instead of symbol #46
- Added the El Salvador Col�n (SVC) to ISO-4217 list
- Added historic ISO-4217 currencies #30
- Added default currency sign � (`Currency.CurrencySign`)
- Fixed NodaMoney.Serialization.AspNet targets problem #42
- NodaMoney now targets .NET 4.0, .NET 4.5, .NET Standard 1.0, .NET Standard 1.6, PCL
- Changed from GitFlow to GitHub Flow strategy
- Restructured the solution and changed all the projects to .NET core.
- Move from MSTest to Xunit

### Changed
- Projects are converted to .NET Core, which shouldn't be a problem, but nevertheless it is a big change which can cause unexpected problems.

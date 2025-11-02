# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Next]

### Added
- Format K ($1.2K) and k (USD 1.2K) to Compact notation, like USD 1.2K, €3.4M (https://github.com/RemyDuijkeren/NodaMoney/issues/111)

### Changed
- ISO 4217 AMENDMENT NUMBER 180, Bulgaria will use EUR from 1 January 2026, and BGN will move to the historic list.
- Formatting format 'C' now has different behavior depending on capitalization. Format 'C' will use the international currency symbol (US$),
  where 'c' will use the local currency code ($).

### Removed
-

## [2.5]

### Added
- Add MoneyContext to configure money behavior like rounding, scale and precision. This can be specified globally,
  per thread or by money instance. This should solve much of the discussion in issue #27 about internal rounding.
- Add extra constructors on Money to create with a given MoneyContext, instead of MidpointRounding.
- Add MoneyContext property on Money.
- Add Scale property on Money.
- Add Precision property on Money (= still internal!).
- Add NodaMoney.DependencyInjection package to register MoneyContext .NET DI.
- Add the FastMoney struct, based on long, for a smaller footprint and faster operations with a default MoneyContext of { Precision = 19, MaxScale = 4, RoundingStrategy = new StandardRounding(MidpointRounding.ToEven) }.

### Changed
- Optimized Money struct size. This was 18 bytes (padded 24 bytes), but is now 16 bytes (padded 16 bytes) (33% less).
  This means Money is the same size as Decimal struct!!!
- Improved Parsing: memory allocation (75% less) and performance (2x)
- Improved Increment(++) and Decrement(--): zero memory allocation (was 32 B) and performance (3x)

### Removed
- IConvertible implementation for Money. Methods that don't make sense to convert from and to Money are removed, like ToBoolean, ToDateTime, etc.
- Removed From-methods and numeric casts on Money where constructors exist or are more appropriate, like FromDecimal, FromInt32 FromInt64, etc.
- Remove Money constructors and factory methods with MidpointRounding param for non-decimal numeric types, like `new Money(double, Currency, MidpointRounding)`.

## [2.3]

### Added
- CurrencyInfo.TryFromCode method to get CurrencyInfo from code without throwing an exception.

## [2.2.1]

### Changed
- Fixed #107 When subtracting from $0, the result is negated

## [2.2]

### Added
- ISO 4217 AMENDMENT NUMBER 179, add new currency XAD for the Finance Department Arab Monetary Fund (AMF)

### Changed
- Updated System.Text.Json dependency to allow versions from 4.7.2 and up for better backwards compatibility.
- No rounding for Currencies where MinorUnit is NotApplicable, like Currency(Info).NoCurrency (breaking-change).
- Allow ExchangeRate to have the same currency as both base and quote by @gliljas in #103

### Removed
- Removed NumberStyle param for Parse- and TryParse-methods (breaking-change). Money expects to parse a Currency number.
  Pre-parse with Decimal.Parse() if more fine-grained control is needed.
- CurrencyInfo.MinorUnitAsExponentOfBase10 changed from public to internal (breaking-change)

## [2.1.1]

### Added
- Allow creating Money using the 'with' expression
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

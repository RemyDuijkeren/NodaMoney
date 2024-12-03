# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0]

### Added
- ISO 4217 Amendment Number 170 VED/926
- ISO 4217 Amendment Number 171 SLE/925 SLL/694
- ISO 4217 Amendment Number 172 SLL/694
- ISO 4217 Amendment Number 174 HRK/191
- ISO 4217 Amendment Number 176 ANG/532 XCG/532
- System.Text.Json serialization support
- Added CurrencyInfo to provide information about a currency
- Support for .NET 6.0, .NET 8.0 and .NET 9.0 (next to already supported .NET Standard 2.0)
-
- Currency.NumericCode is a three-digit code number of the currency
- Currency.MinimalAmount is the minimal amount the currency can be increased or decreased

### Changed
- JSON Serialization format is changed from `{ "Cost": {"Amount":1.23,"Currency":"USD"} }` to `{ "Cost":"USD 1.23" }`. This is a breaking change
  for JSON serialization, but deserialization of the old format is supported for migration purposes.
- XML Serialization format is change from `<Money Amount="765.43" Currency="USD" />` to `<Money Currency="USD">765.43</Money>`.
  This is a breaking change for XML serialization, but deserialization of the old format is supported for migration purposes.
- Currency is now a 2byte struct and only contains basic information. Use CurrencyInfo to retrieve more info for a Currency.
- CurrencyBuilder is obsolete. This class is replaced by CurrencyInfo and will be removed in a future version.
-
- Performance improvement on:
  - init of currency (14x) and money (7x)
  - parsing money (2x)
  - smaller footprint and memory allocation (12x)
- Currency.Number is changed from `string` to `short`
- Currency.DecimalDigits is changed from `decimal` to `int`
- Currency.MinorUnit changed to total number of minor units of one currency major unit

### Removed
- Removed support for JavaScriptSerializer in ASP.NET (NodaMoney.Serialization.AspNet)
- Removed explicit support .NET Core 3.1, .NET Standard 2.1, .NET 4.0 and .NET 4.61 (implicitly supported by .NET Standard 2.0)
-
- Currency.MajorUnit

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
- Added the El Salvador Colón (SVC) to ISO-4217 list
- Added historic ISO-4217 currencies #30
- Added default currency sign ¤ (`Currency.CurrencySign`)
- Fixed NodaMoney.Serialization.AspNet targets problem #42
- NodaMoney now targets .NET 4.0, .NET 4.5, .NET Standard 1.0, .NET Standard 1.6, PCL
- Changed from GitFlow to GitHub Flow strategy
- Restructured the solution and changed all the projects to .NET core.
- Move from MSTest to Xunit

### Changed
- Projects are converted to .NET Core, which shouldn't be a problem, but nevertheless it is a big change which can cause unexpected problems.

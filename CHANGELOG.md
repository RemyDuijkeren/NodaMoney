# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [vNext]

### Added
- Currency.NumericCode is a three-digit code number of the currency
- Currency.MinimalAmount is the minimal amount the currency can be increased or decreased

### Changed
- Performance improvement on:
  - init of currency (14x) and money (7x)
  - parsing money (2x)
  - smaller footprint and memory allocation (12x)
- Currency.Number is changed from `string` to `short`
- Currency.DecimalDigits is changed from `decimal` to `int`
- Currency.MinorUnit changed to total number of minor units of one currency major unit 

### Removed
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
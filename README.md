﻿NodaMoney
=========

[![NuGet](https://img.shields.io/nuget/dt/NodaMoney.svg?logo=nuget)](https://www.nuget.org/packages/NodaMoney)
[![NuGet](https://img.shields.io/nuget/v/NodaMoney.svg?logo=nuget)](https://www.nuget.org/packages/NodaMoney)
[![Pre-release NuGet](https://img.shields.io/github/v/tag/RemyDuijkeren/NodaMoney?label=pre-release%20nuget&logo=github)](https://github.com/users/RemyDuijkeren/packages/nuget/package/NodaMoney)
[![CI](https://github.com/RemyDuijkeren/NodaMoney/actions/workflows/ci.yml/badge.svg)](https://github.com/RemyDuijkeren/NodaMoney/actions/workflows/ci.yml)

About
----
NodaMoney provides a library that treats Money as a first class citizen in .NET and handles all the ugly bits like currencies
and formatting.

We have the [decimal type](http://msdn.microsoft.com/en-us/library/364x0z75.aspx) in .NET to store an amount of money, which can
be used for very basic things. But it's still a numeric value without knowledge about its currency, major and minor units,
formatting, etc. The .NET Framework has the System.Globalization namespace that helps with formatting of money in different cultures and regions,
but it only captures some info about currencies, but not everything.

There is also some business logic surrounding money, like dividing without losing pennies (like in the movie [Office Space](http://www.imdb.com/title/tt0151804/)),
conversion, etc. that motivates to have a Money type that contains all the domain logic, like Martin Fowler already described in
his book Patterns of Enterprise Application Architecture, see pages about [Money](http://martinfowler.com/eaaCatalog/money.html)
and [Quantity](https://martinfowler.com/eaaDev/Quantity.html).

NodaMoney represents the .NET counterpart of java library [JodaMoney](http://www.joda.org/joda-money/), like NodaTime is the .NET
counterpart of JodaTime. NodaMoney does not provide, nor is it intended to provide, monetary algorithms beyond the most basic and
obvious. This is because the requirements for these algorithms vary widely between domains. This library is intended to act as the
base layer, providing classes that should be in the .NET Framework. It complies with the currencies in [ISO 4217](http://en.wikipedia.org/wiki/ISO_4217).

Usage
-----
The main classes are:
- Money: An immutable structure that represents money in a specified currency.
- Currency: A small immutable structure that represents a currency unit.
- CurrencyInfo: An immutable structure that represents a currency with all its information. It can give all ISO 4217
and custom currencies. It auto-converts to Currency.
- ExchangeRate: A structure that represents a [currency pair](http://en.wikipedia.org/wiki/Currency_pair) that can convert money
from one currency to another currency.

**Initializing Currency**

```csharp
// Create Currency Unit
Currency euro = Currency.FromCode("EUR");

// Create CurrencyInfo (for metadata about Currency)
CurrencyInfo info = CurrencyInfo.FromCode("EUR");
CurrencyInfo info = CurrencyInfo.GetInstance(euro); // Currency to CurrencyInfo
Currencyinfo info = CurrencyInfo.GetInstance(CultureInfo.CurrentCulture);
Currencyinfo info = CurrencyInfo.GetInstance(RegionInfo.CurrentRegion);
Currencyinfo info = CurrencyInfo.GetInstance(NumberFormatInfo.InvariantInfo);
CurrencyInfo info = CurrencyInfo.CurrentCurrency;

Currency euro = info; // implicit cast to Currency Unit
```

**Initializing money**

```csharp
// Define money with explicit currency
Money euros = new Money(6.54m, "EUR");
Money euros = new (6.54m, "EUR");
Money euros = new Money(6.54m, Currency.FromCode("EUR"));
Money euros = new Money(6.54m, CurrencyInfo.FromCode("EUR"));

// From existing money
Money dollars = euros with { Currency = CurrencyInfo.FromCode("USD") };
Money myEuros = euros with { Amount = 10.12m };

// Define money explicit using helper method for most used currencies in the world
Money money = Money.Euro(6.54m);
Money money = Money.USDollar(6.54m);
Money money = Money.PoundSterling(6.54m);
Money money = Money.Yen(6);

// Implicit Currency based on current culture/region.
// When culture is 'NL-nl' code below results in Euros.
Money money = new Money(6.54m);
Money money = new (6.54m);
Money money = (Money)6.54m;
Money money = (Money)6;
Money money = (Money)6.54;

// Auto-rounding to the minor unit will take place with MidpointRounding.ToEven
// also known as banker's rounding
Money euro = new Money(765.425m, "EUR"); // EUR 765.42
Money euro = new Money(765.425m, "EUR", MidpointRounding.AwayFromZero); // EUR 765.43

// Deconstruct money
Money money = new Money(10m, "EUR");
var (amount, currency) = money;
```

**Money operations**

```csharp
Money euro10 = Money.Euro(10);
Money euro20 = Money.Euro(20);
Money dollar10 = Money.USDollar(10);
Money zeroDollar = Money.USDollar(0);

// Compare money
euro10 == euro20; // false
euro10 != euro20; // true;
euro10 == dollar10; // false;
euro20 > euro10; // true;
euro10 <= dollar10; // throws InvalidCurrencyException!
zeroEuro == zeroDollar; // true; special zero handling

// Add and Substract
Money euro30 = euro10 + euro20;
Money euro10 = euro20 - euro10;
Money m = euro10 + dollar10; // throws InvalidCurrencyException!
Money euro10 = euro10 + zeroDollar; // doesn't throw when adding zero
euro20 += euro10; // EUR 30
euro20 -= euro10; // EUR 10

// Add and Substract with implied Currency Context
Money euro30 = euro10 + 20m; // decimal value is assumed to have the same currency context
Money euro10 = euro20 - 10m; // decimal value is assumed to have the same currency context

// Decrement and Increment by minor unit
Money yen = new Money(765m, "JPY"); // the smallest unit is 1 yen
Money euro = new Money(765.43m, "EUR"); // the smallest unit is 1 cent (1EUR = 100 cent)
++yen; // JPY 766
--yen; // JPY 765
++euro; // EUR 765.44
--euro; // EUR 765.43

// Multiply
Money m = euro10 * euro20; // doesn't compile!
Money euro20 = euro10 * 2;
Money discount = euro10 * 0.15m;

// Divide
decimal ratio = euro20 / euro10;
Money euro5 = euro10 / 2;

// Divide without losing money
Money total = new Money(101m, "USD");
IEnumerable<Money> inShares = total.Split(4); // [USD 25, USD 25, USD 25, USD 26]
IEnumerable<Money> byRatio = total.Split([2,1,3]); // [USD 33.67, USD 16.83, USD 50.50]

// Modulus / Remainder
Money total = new Money(105.50m, "USD");
Money unitPrice = new Money(20.00m, "USD"); // USD 20 * 5 = USD 100
Money remainder = total % unitPrice; // USD 5.50
```

**Money formatting**

```csharp
Money yen = new Money(2765m, "JPY");
Money euro = new Money(2765.43m, "EUR");
Money dollar = new Money(2765.43m, "USD");
Money dinar = new Money(2765.432m, "BHD");

// Implicit when current culture is 'en-US'
yen.ToString();    // "¥2,765"
euro.ToString();   // "€2,765.43"
dollar.ToString(); // "$2,765.43"
dinar.ToString();  // "BD2,765.432"

// Implicit when current culture is 'nl-BE'
yen.ToString();    // "¥ 2.765"
euro.ToString();   // "€ 2.765,43"
dollar.ToString(); // "$ 2.765,43"
dinar.ToString();  // "BD 2.765,432"

// Implicit when current culture is 'fr-BE'
yen.ToString();    // "2.765 ¥"
euro.ToString();   // "2.765,43 €"
dollar.ToString(); // "2.765,43 $"
dinar.ToString();  // "2.765,432 BD"

// Explicit format for culture 'nl-NL'
var ci = new CultureInfo("nl-NL");

yen.ToString(ci);    // "¥ 2.765"
euro.ToString(ci);   // "€ 2.765,43"
dollar.ToString(ci); // "$ 2.765,43"
dinar.ToString(ci);  // "BD 2.765,432"

// Standard Formats when currenct culture is 'nl-NL'
euro.ToString("C");  // "€ 2.765,43"    Currency format
euro.ToString("C0"); // "€ 2.765"       Currency format with precision specifier
euro.ToString("G");  // "EUR 2.765,43"  General format (= C but with currency code)
euro.ToString("L");  // "2.765,43 Euro" English name format
euro.ToString("R");  // "EUR 2,765.43"  Round-trip format
euro.ToString("N");  // "2,765.43"      Number format
euro.ToString("F");  // "2765,43"       Fixed point format
```

**Money parsing**

```csharp
// Implicit parsing when current culture is 'nl-BE'
Money euro = Money.Parse("€ 765,43");
Money euro = Money.Parse("-€ 765,43");
Money euro = Money.Parse("€-765,43");
Money euro = Money.Parse("765,43 €");
Money yen = Money.Parse("¥ 765"); // throw FormatException, because ¥ symbol is used for Japanese yen and Chinese yuan
Money dollar = Money.Parse("$ 765,43"); // throw FormatException, because $ symbol is used for multiple currencies

// Implicit parsing when current culture is 'ja-JP'
Money yen = Money.Parse("¥ 765");

// Implicit parsing when current culture is 'zh-CN'
Money yuan = Money.Parse("¥ 765");

// Implicit parsing when current culture is 'en-US'
Money dollar = Money.Parse("$765.43");
Money dollar = Money.Parse("($765.43)"); // -$765.43

// Implicit parsing when current culture is 'es-AR'
Money peso = Money.Parse("$765.43");

// Explicit parsing when current culture is 'nl-BE'
Money euro = Money.Parse("€ 765,43", Currency.FromCode("EUR"));
Money euro = Money.Parse("765,43 €", Currency.FromCode("EUR"));
Money yen = Money.Parse("¥ 765", Currency.FromCode("JPY"));
Money yuan = Money.Parse("¥ 765", Currency.FromCode("CNY"));

// Implicit try parsing when current culture is 'nl-BE'
Money euro;
Money.TryParse("€ 765,43", out euro);

// Explicit try parsing when current culture is 'nl-BE'
Money euro;
Money.TryParse("€ 765,43", Currency.FromCode("EUR"), out euro);
```

**Create custom Currency**

```csharp
// Create custom currency
CurrencyInfo myCurrency = CurrencyInfo.Create("BTA") with
{
    Symbol = "$",
    Number = 1023,
    InternationalSymbol = "CC$",
    MinorUnit = MinorUnit.Two,
    EnglishName = "My Custom Currency",
    IsIso4217 = false,
    AlternativeSymbols = ["cc$"],
    IntroducedOn = new DateTime(2022, 1, 1),
    ExpiredOn = new DateTime(2030, 1, 1)
};

// Fails because it's not registred
var notExisting = CurrencyInfo.FromCode("BTA"); // throw exception

// Register it for the life-time of the app domain
CurrencyInfo.Register(myCurrency);
var exists = CurrencyInfo.FromCode("BTA"); // returns myCurrency

// Create custom currency based on existing currency
CurrencyInfo myCurrency = CurrencyInfo.FromCode("EUR") with { Code = "EUA", EnglishName = "New Euro" };
CurrencyInfo.Register(myCurrency);

var myEuro = Currency.FromCode("EUA"); // returns myCurrency

// Replace currency for the life-time of the app domain
CurrencyInfo oldEuro = CurrencyInfo.Unregister("EUR");
CurrencyInfo newEuro = oldEuro with { Symbol = "€U", EnglishName = "New Euro" };
CurrencyInfo.Register(newEuro);

var myEuro = Currency.FromCode("EUR"); // returns newEuro
```

## Releases

This library uses [Semantic Versioning](https://semver.org/) to give meaning to the version numbers.

You can get the latest stable release or prerelease from the [official Nuget.org feed](https://www.nuget.org/packages/NodaMoney) or from our
[GitHub releases page](https://github.com/remyvd/NodaMoney/releases).

If you'd like to work with the bleeding edge, you can use our [GitHub Nuget feed](https://github.com/RemyDuijkeren/NodaMoney/pkgs/nuget/NodaMoney).
Packages on this feed are alpha and beta and, while they've passed all our tests, are not yet ready for production.

## Support

For support, bugs and new ideas use [GitHub issues](https://github.com/remyvd/NodaMoney/issues).

For supporting the project:
* [Sponsor me](https://github.com/sponsors/RemyDuijkeren)
* [Tip Me](https://bunq.me/RemyDuijkeren)

## Contributing

Your contributions are always welcome! Please have a look at the [contribution guidelines](CONTRIBUTING.md) first.

Previous contributors include:

<a href="https://github.com/RemyDuijkeren/NodaMoney/graphs/contributors">
  <img src="https://contrib.rocks/image?max=5&repo=RemyDuijkeren/NodaMoney" />
</a>

Made with [contrib.rocks](https://contrib.rocks).

## Credits

This library wouldn't have been possible without the following tools, packages and companies:

* [xUnit](https://xunit.net/) - Unit testing
* [MinVer](https://github.com/adamralph/minver) - Minimalistic versioning using Git tags by [Adam Ralph](https://github.com/adamralph)
* [Test Reporter](https://github.com/dorny/test-reporter/tree/v1#readme) - Displays test results directly in GitHub by [Michal Dorner](https://github.com/dorny)
* [Roslynator](https://github.com/dotnet/roslynator) - A set of code analysis tools for C# by [Josef Pihrt](https://github.com/josefpihrt)
* [FluentAssertions](https://github.com/dotnet/roslynator) - A set of extension methods to specify the expected outcome of unit tests by [Dennis Doomen](https://github.com/dennisdoomen)
* [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet) - Powerful .NET library for benchmarking

## License
This project is licensed under the Apache License - see the [LICENSE](LICENSE) file for details.

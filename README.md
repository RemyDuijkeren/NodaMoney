NodaMoney
=========
<img align="right" src="https://raw.githubusercontent.com/remyvd/NodaMoney/master/Tools/logo_nodamoney.png">
You can get the latest stable release from the [official Nuget.org feed](https://www.nuget.org/packages/NodaMoney) or from our
[github releases page](https://github.com/remyvd/NodaMoney/releases).

If you'd like to work with the bleeding edge, you can use our [custom Nuget feed](https://ci.appveyor.com/nuget/nodamoney-pmrx3j3p32f2).
Packages on this feed are pre-release and, while they've passed all our tests, are not yet ready for production.

For support, bugs and new ideas use [GitHub issues](https://github.com/remyvd/NodaMoney/issues). Please see our
[guidelines](CONTRIBUTING.md) for contributing to the NodaMoney.

[![Build status](https://ci.appveyor.com/api/projects/status/o656q9bagslgusj9?svg=true)](https://ci.appveyor.com/project/remyvd/nodamoney)
[![Coverage Status](https://coveralls.io/repos/remyvd/NodaMoney/badge.svg?branch=develop)](https://coveralls.io/r/remyvd/NodaMoney?branch=develop)


About
----
NodaMoney provides a library that treats Money as a first class citizen in .NET and handles all the ugly bits like currencies
and formatting.

We have the [decimal type](http://msdn.microsoft.com/en-us/library/364x0z75.aspx) in .NET to store an amount of money, which can
be used for very basic things. But it's still a numeric value without knowledge about its currency, major and minor units, 
formatting, etc. The .NET Framework has the System.Globalization namespace that helps with formatting of money in different cultures and regions,
but it only captures some info about currencies, but not everything.

There is also some business logic surronding money, like dividing without losing pennies (like in the movie [Office Space](http://www.imdb.com/title/tt0151804/)),
conversion, etc. that motivates to have a Money type that contains all the domain logic, like Martin Fowler already descibed in
his book [Patterns of Enterprise Application Architecture](http://martinfowler.com/eaaCatalog/money.html)

NodaMoney represents the .NET counterpart of java library [JodaMoney](http://www.joda.org/joda-money/), like NodaTime is the .NET
counterpart of JodaTime. NodaMoney does not provide, nor is it intended to provide, monetary algorithms beyond the most basic and 
obvious. This is because the requirements for these algorithms vary widely between domains. This library is intended to act as the
base layer, providing classes that should be in the .NET Framework. It complies with the currencies in [ISO 4217](http://en.wikipedia.org/wiki/ISO_4217).

Usage
-----
At the moment there are four classes:
- Currency: An immutable structure that represents a currency. It can give all ISO 4217 and custom currencies.
- Money: An immutable structure that represents money in a specified currency.
- ExchangeRate: A stucture that represents a [currency pair](http://en.wikipedia.org/wiki/Currency_pair) that can convert money
from one currency to another currency.
- CurrencyBuilder: Defines a custom currency that is new or based on another currency.

**Initalizing money**

```C#
// define money with explicit currency
var euros = new Money(6.54m, Currency.FromCode("EUR"));
var euros = new Money(6.54m, "EUR");

// define money explicit using helper method for most used currencies in the world
var money = Money.Euro(6.54m);
var money = Money.USDollar(6.54m);
var money = Money.PoundSterling(6.54m);
var money = Money.Yen(6);

// define money implicit using currency of current culture/region
var money = new Money(6.54m);
Money money = 6.54m;
Money money = 6;
Money money = (Money)6.54; // need explict cast from double data type  

// auto-rounding to the minor unit will take place with MidpointRounding.ToEven
// also known as banker's rounding 
var euro = new Money(765.425m, "EUR"); // EUR 765.42
var euro = new Money(765.425m, "EUR", MidpointRounding.AwayFromZero); // EUR 765.43
```

**Money operations**

```C#
var euro10 = Money.Euro(10);
var euro20 = Money.Euro(20);
var dollar10 = Money.USDollar(10);

// add and substract
var euro30 = euro10 + euro20;
var euro10 = euro20 - euro10;
var m = euro10 + dollar10; // will throw exception!
euro10 += euro20;
euro10 -= euro20;

// compare money
euro10 == euro20; // false
euro10 != euro20; // true;
euro10 == dollar10; // false;
euro20 > euro10; // true;
euro10 <= dollar10; // will throw exception!

// decrement and increment by minor unit
var yen = new Money(765m, "JPY"); // the smallest unit is 1 yen
var euro = new Money(765.43m, "EUR");
++yen; // JPY 766
--yen; // JPY 765
++euro; // EUR 765.44
--euro; // EUR 765.43
```

**Money formatting**

```C#
var yen = new Money(765m, "JPY");
var euro = new Money(765.43m, "EUR");
var dollar = new Money(765.43m, "USD");
var dinar = new Money(765.432m, "BHD");

// Implicit when current culture is 'en-US'
yen.ToString();    // "¥765"
euro.ToString();   // "€765.43"
dollar.ToString(); // "$765.43"
dinar.ToString();  // "BD765.432"

yen.ToString("C2");    // "¥765.00"
euro.ToString("C2");   // "€765.43"
dollar.ToString("C2"); // "$765.43"
dinar.ToString("C2");  // "BD765.43"

// Implicit when current culture is 'nl-BE'
yen.ToString();    // "¥ 765"
euro.ToString();   // "€ 765,43"
dollar.ToString(); // "$ 765,43"
dinar.ToString();  // "BD 765,432"

// Implicit when current culture is 'fr-BE'
yen.ToString();    // "765 ¥"
euro.ToString();   // "765,43 €"
dollar.ToString(); // "765,43 $"
dinar.ToString();  // "765,432 BD"
}

// Explicit format for culture 'nl-NL'
var ci = new CultureInfo("nl-NL");

yen.ToString(ci);    // "¥ 765"
euro.ToString(ci);   // "€ 765,43"
dollar.ToString(ci); // "$ 765,43"
dinar.ToString(ci);  // "BD 765,432"
```

**Money parsing**

```C#
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

**Adding custom currencies**

```C#
// Create custom currency and register it (for the life-time of the app domain)
var builder = new CurrencyBuilder("BTC", "virtual")
				{
					EnglishName = "Bitcoin",
					Symbol = "฿",
					DecimalDigits = 8
				};

var bitcoin = builder.Build(); // build BTC, but will not register it
var bitcoin = builder.Register(); // build and register BTC in namespace 'virtual'

// When the custom is registered, it can be called as any other currency
Money bitcoins = new Money(1.2, Currency.FromCode("BTC", "virtual"));
Money bitcoins = new Money(1.2. "BTC");

// Replace ISO 4217 currency (for the life-time of the app domain)
Currency oldEuro = CurrencyBuilder.Unregister("EUR", "ISO-4217");

var builder = new CurrencyBuilder("EUR", "ISO-4217");
builder.LoadDataFromCurrency(oldEuro);
builder.EnglishName = "New Euro";
builder.DecimalDigits = 1;
builder.Register();

Currency newEuro = Currency.FromCode("EUR");
```
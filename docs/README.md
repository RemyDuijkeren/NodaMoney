---
title: NodaMoney
---

Overview
--------
NodaMoney is a small, focused .NET library that treats money as a first‑class citizen. It gives you type‑safe money and currency types,
correct rounding, parsing/formatting, and currency‑aware arithmetic so you don’t have to reinvent the tricky bits.

Why not just decimal? The built‑in [decimal](http://msdn.microsoft.com/en-us/library/364x0z75.aspx) is great for precision but has no concept of currency, minor units (cents), symbols or
culture‑aware formatting, nor common money behaviors (like splitting amounts without losing a cent). NodaMoney provides these domain concepts
and behaviors so you can write clearer and safer code.

NodaMoney is inspired by the Java library [JodaMoney](http://www.joda.org/joda-money/) (just like NodaTime is inspired by JodaTime). It aims to be a solid base layer
with clear, minimal APIs and ISO‑4217 currency data.

What you get
- Money: immutable, currency‑aware value type with safe operators and culture‑aware formatting/parsing
- FastMoney: high‑throughput alternative using 64‑bit integer arithmetic (fixed 4‑decimal scale)
- Currency and CurrencyInfo: ISO 4217 catalog with metadata; supports custom currencies
- Rounding and MoneyContext: configurable rounding/scale and default currency; DI support and named contexts
- ExchangeRate: represent currency pairs and convert between currencies
- Serialization: XML, System.Text.Json and Newtonsoft.Json converters; type converters
- Dependency Injection package: Microsoft.Extensions.* integration for configuring MoneyContext
- AOT‑friendly builds and multi‑TFM support (see Compatibility below)

Installation
```powershell
dotnet add package NodaMoney
dotnet add package NodaMoney.DependencyInjection # optional DI integration
```

Quick start
----

```csharp
using NodaMoney;

var price = new Money(12.99m, "USD");
var tax = price * 0.21m;      // currency‑safe arithmetic
var total = price + tax;      // auto‑rounded to minor unit

string text = total.ToString("C", new System.Globalization.CultureInfo("en-US"));
// e.g. "$15.72"

// Parse
var parsed = Money.Parse("$15.72", Currency.FromCode("USD"));

// Split without losing cents
var shares = total.Split(3);  // e.g. [$5.24, $5.24, $5.24]
```

Main building blocks
- `Money`: An immutable structure that represents money in a specified currency
- `FastMoney`: A high‑performance immutable structure optimized for arithmetic
- `Currency`: A compact immutable structure that represents a currency unit
- `CurrencyInfo`: Currency metadata (ISO 4217 + custom); implicitly converts to Currency
- `MoneyContext`: Configurable rounding, scale and default currency; supports DI and named contexts
- `ExchangeRate`: Represents a [currency pair](http://en.wikipedia.org/wiki/Currency_pair) to convert between currencies

Usage
-----

`Money` type is based on `decimal` and has the same 28-digit precision (±1.0 × 10^-28 to ±7.9 × 10^28). `Money` also has
the same size as a decimal, even with the extra Currency and MoneyContext information. By default, `Money` is always
rounded to the currency minor unit (use `MoneyContext` to override), which is executed after every arithmetic operation.

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
Money euros = Money.Euro(6.54m);
Money dollars = Money.USDollar(6.54m);
Money pounds = Money.PoundSterling(6.54m);
Money yens = Money.Yen(6);

// Implicit Currency based on current culture/region.
// When culture is 'NL-nl' code below results in Euros.
Money euros = new Money(6.54m);
Money euros = new (6.54m);
Money euros = (Money)6.54m;

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

// Add and Subtract
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

| Format | Meaning                          | Example (USD, en-US, 12 345.67) |
|--------|----------------------------------|---------------------------------|
| `C`    | Local currency symbol (default)  | `$12,345.67`                    |
| `c`    | Compact + local symbol           | `$12.3K`                        |
| `G`    | ISO currency code                | `USD 12,345.67`                 |
| `g`    | Compact + ISO code               | `USD 12.3K`                     |
| `I`    | International currency symbol    | `US$ 12,345.67`                 |
| `i`    | Compact + international symbol   | `US$ 12.3K`                     |
| `L`    | English currency name            | `12,345.67 US dollar`           |
| `l`    | Compact + English currency name  | `12.3K US dollar`               |
| `N`    | Number format (no currency)      | `12,345.67`                     |
| `F`    | Fixed-point number (no currency) | `12345.67`                      |
| `R`    | Round-trip (“CODE amount”)       | `USD 12345.67`                  |

Notes:
- `G`, `L`, `N`, `F`, `R` keep their existing semantics as much as possible.
- Lowercase variants (`c`, `g`, `i`, `l`) apply **compact** formatting to the numeric part but keep the same identifier style as their uppercase counterpart.

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
dollar.ToString("C"); // "$ 2.765,43"      Currency symbol format
dollar.ToString("C0");// "$ 2.765"         Currency symbol format with precision specifier
dollar.ToString("c"); // "$ 2.8K"          Compact Currency symbol format
dollar.ToString("I"); // "US$ 2.765,43"    International Currency symbol format
dollar.ToString("i"); // "US$ 2.8K"        Compact International Currency symbol format
dollar.ToString("G"); // "USD 2.765,43"    ISO currency code format (= C but with currency code)
dollar.ToString("g"); // "USD 2.8K"        Compact ISO currency code format (international)
dollar.ToString("L"); // "2.765,43 dollar" English name format
dollar.ToString("l"); // "2.8K dollar"     Compact English name format
dollar.ToString("R"); // "USD 2,765.43"    Round-trip format
dollar.ToString("N"); // "2,765.43"        Number format (no currency)
dollar.ToString("F"); // "2765,43"         Fixed point format (no currency)
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

**Exchange rates (convert)**

```csharp
using NodaMoney.Exchange;

// EUR/USD at 1.2591
var rate = new ExchangeRate("EUR", "USD", 1.2591);

var eur = Money.Euro(100.99m);
var usd = rate.Convert(eur);   // -> USD 127.16 (rounded to cents)

// Converting back uses the same rate
var eurAgain = rate.Convert(usd); // -> EUR 100.99
```

## FastMoney

Where `Money` type is based on `decimal`, `FastMoney` is based on `long` and has smaller precision (17 instead of 28) and has a
fixed four decimal scale, like [SqlMoney](https://learn.microsoft.com/en-us/dotnet/api/system.data.sqltypes.sqlmoney?view=net-9.0) and OLE Automation Currency value. Because it is based on `long` it is way faster for
arithmetic operations.

FastMoney type is an optimized version of `Money` that:
- has 17 digits precision (±1.0 × 10^-17 to ±9.2 × 10^17)
- has fixed four decimal places
- no internal rounding based on the currency minor unit
- is faster for add/subtract/multiply/divide/increment/decrement/remainder using 64-bit integer arithmetic
- is best for high-throughput calculations where 4-decimal precision is enough (up to 18 times)
- is smaller than Money (12 bytes vs. 16 bytes)
- aligns and converts with SqlMoney and OLE Automation Currency value (OACurrency)

Only use FastMoney when you need the fastest performance, and you know what you're doing regarding currency rounding and
don't mind the loss of precision. For presentation and formatting, convert `FastMoney` type to `Money`.

Usage:

```csharp
using NodaMoney;

var eur = new FastMoney(10.1234m, "EUR");
var fee = new FastMoney(0.1000m, "EUR");
var total = eur + fee;              // currency-safe operations

// Convert to Money for formatting/rounding/presentation
Money display = eur.ToMoney();      // or (Money)eur
var text = display.ToString("C");

// Convert from Money (explicit)
var money = new Money(12.34m, "EUR");
FastMoney fast = (FastMoney)money;  // or new FastMoney(money)

// Convert from SqlMoney
SqlMoney sqlMoney = db.MoneyFromDb();
FastMoney? fast = FastMoney.FromSqlMoney(sqlMoney, Currency.FromCode("EUR")); // or (FastMoney?)sqlMoney

// Convert to SqlMoney
SqlMoney sqlMoney1 = fast.ToSqlMoney(Currency.FromCode("EUR"));

// Convert from OLE Automation Currency
long oaCurrency = db.CurrencyFromDb();
FastMoney fast = FastMoney.FromOACurrency(oaCurrency, Currency.FromCode("EUR"));

// Convert to OLE Automation Currency
long oaCurrency = fast.ToOACurrency();
```

Currency(Info)
-----
`Currency` is a unit of currency that is a small optimized struct that represents the Currency Code. It is used inside
`Money` and as struct in fast lookups.

`CurrencyInfo` is a class that contains information about the currency, such as the ISO 4217 code, the symbol, the name,
and the minor unit. It is used to create a `Currency` instance (implicit cast) or to register a custom currency.

**Initializing Currency**

```csharp
// Create Currency unit
Currency euro = Currency.FromCode("EUR");

// Create CurrencyInfo (for metadata about Currency)
CurrencyInfo ci = CurrencyInfo.FromCode("EUR");
CurrencyInfo ci = CurrencyInfo.GetInstance(euro); // From Currency unit To CurrencyInfo
Currencyinfo ci = CurrencyInfo.GetInstance(CultureInfo.CurrentCulture);
Currencyinfo ci = CurrencyInfo.GetInstance(RegionInfo.CurrentRegion);
Currencyinfo ci = CurrencyInfo.GetInstance(NumberFormatInfo.InvariantInfo);
CurrencyInfo ci = CurrencyInfo.CurrentCurrency;

Currency euro = ci; // Implicit cast to Currency Unit
```

**Retrieving Currencies**

```csharp
// Get all currencies
var currencyList = CurrencyInfo.GetAllCurrencies();

// Fast lookup by code or symbol
var findCurrencies = CurrencyInfo.GetAllCurrencies("$");
```

**Create custom Currency (advanced)**

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

## MoneyContext (advanced)

MoneyContext centralizes rounding, scale, and default currency configuration used by Money operations. You can set a global default, create
scoped contexts, and integrate it with Microsoft.Extensions.DependencyInjection. Every Money object uses the context it was created in
or was given explicitly.

- What it controls: rounding strategy (e.g., MidpointRounding), maximum scale (decimal digits), and the default currency used when one isn’t explicitly provided.
- Where it’s used: by Money arithmetic, parsing/formatting where applicable, and helpers that assume an implicit currency.

Set own global default without DI:

```csharp
using NodaMoney;
using NodaMoney.Context;

// Set global default MoneyContext, will be used by all created Money objects
MoneyContext myDefaultContext = MoneyContext.Create(opt =>
{
    opt.MaxScale = 4;
    opt.DefaultCurrency = CurrencyInfo.FromCode("USD");
    opt.EnforceZeroCurrencyMatching = true;
});
MoneyContext.DefaultThreadContext = myDefaultContext;

// or
MoneyContext.CreateAndSetDefault(opt =>
{
    opt.MaxScale = 4;
    opt.DefaultCurrency = CurrencyInfo.FromCode("USD");
    opt.EnforceZeroCurrencyMatching = true;
});
```

Use MoneyContext by instance or scope:

```csharp
// Explicit MoneyContext when creating a new Money object
MoneyContext myOwnContext = MoneyContext.Create(opt =>
{
    opt.MaxScale = 2;
    opt.RoundingStrategy = new StandardRounding(MidpointRounding.AwayFromZero);
    opt.DefaultCurrency = CurrencyInfo.FromCode("EUR");
});

Money money1 = new Money(6.54m, "EUR", myOwnContext); // explicit MoneyContext
Money money2 = new Money(6.54m, "EUR"); // implicit global MoneyContext

// This will throw an InvalidOperationException because money1 and money2 have different contexts:
var result = money1 + money2;

// Instead, align contexts using the 'with' expression:
var result = money1 + (money2 with { Context = money1.Context });

// Use MoneyContext in a scope (sets MoneyContext.ThreadContext for the duration of the scope):
using (MoneyContext.CreateScope(myOwnContext))
{
    Money money3 = new Money(10.00m, "EUR");
    Money money4 = new Money(5.00m, "EUR");
    var total = money3 + money4;
}
```

Use MoneyContext with Dependency Injection (Nuget: NodaMoney.DependencyInjection):

```csharp
using Microsoft.Extensions.DependencyInjection;
using NodaMoney;
using NodaMoney.Context;
using NodaMoney.DependencyInjection; // NuGet: NodaMoney.DependencyInjection

var builder = WebApplication.CreateBuilder(args);

// Register MoneyContext with custom options as default global context
builder.Services.AddMoneyContext(options =>
{
    options.DefaultCurrency = Currency.FromCode("USD");
    options.RoundingStrategy = new StandardRounding(MidpointRounding.AwayFromZero);
    options.MaxScale = 2;
    opt.EnforceZeroCurrencyMatching = true;
});

var app = builder.Build();

// Consume MoneyContext via DI
app.MapGet("/total", (MoneyContext context) =>
{
    Console.WriteLine(context.DefaultCurrency); // returns "USD"

    var price = new Money(10m, "USD"); // uses registered default context
    return price.Round().ToString("C");
});

app.Run();
```

You can also configure via IConfiguration (appsettings.json) and register multiple named contexts.
See the [NodaMoney.DependencyInjection README](src/NodaMoney.DependencyInjection/README.md) for full examples.

## Compatibility

- Core library (NodaMoney): net10.0, net9.0; net8.0; netstandard2.0; netstandard2.1
- DI package (NodaMoney.DependencyInjection): net10.0, net9.0; net8.0; netstandard2.0; netstandard2.1
- AOT: compatible on .NET 8/9/10
- Packages ship with SourceLink and symbol packages;

[![NuGet](https://img.shields.io/nuget/dt/NodaMoney.svg?logo=nuget)](https://www.nuget.org/packages/NodaMoney)
[![NuGet](https://img.shields.io/nuget/v/NodaMoney.svg?logo=nuget)](https://www.nuget.org/packages/NodaMoney)
[![Pre-release NuGet](https://img.shields.io/github/v/tag/RemyDuijkeren/NodaMoney?label=pre-release%20nuget&logo=github)](https://github.com/users/RemyDuijkeren/packages/nuget/package/NodaMoney)
[![CI](https://github.com/RemyDuijkeren/NodaMoney/actions/workflows/ci.yml/badge.svg)](https://github.com/RemyDuijkeren/NodaMoney/actions/workflows/ci.yml)


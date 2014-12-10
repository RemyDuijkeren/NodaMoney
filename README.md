NodaMoney
=========
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
At the moment there are three classes:
- Currency: An immutable structure that represents a currency. It can give all ISO 4217 currencies.
- Money: An immutable structure that represents money in a specified currency.
- ExchangeRate: A stucture that represents a [currency pair](http://en.wikipedia.org/wiki/Currency_pair) that can convert money
from one currency to another currency.

**Initalizing money**

```C#
var euros = new Money(6.54m, Currency.FromCode("EUR")); // define money explicit
var euros = Money.Euro(6.54m); // define money explicit using helper method for most used currencies in the world
Money money = 6.54m; // define money implicit using currency of current culture/region
Money money = (Money)6.54; // define money with double values works, but you need to explict cast because of ronding issues. 
```

**Money operations**

```C#
var euro10 = Money.Euro(10);
var euro20 = Money.Euro(20);
var euro30 = euro10 + euro20;
```
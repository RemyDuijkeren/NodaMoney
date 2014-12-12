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
// define money explicit
var euros = new Money(6.54m, Currency.FromCode("EUR"));
var euros = new Money(6.54m, "EUR");

// define money explicit using helper method for most used currencies in the world
var euros = Money.Euro(6.54m);
var euros = Money.USDollar(6.54m);
var euros = Money.PoundSterling(6.54m);
var euros = Money.Yen(6);

// define money implicit using currency of current culture/region
var money = new Money(6.54m);
Money money = 6.54m;
Money money = 6;
Money money = (Money)6.54; // need explict cast from double data type  
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
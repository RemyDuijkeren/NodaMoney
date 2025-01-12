using System.Numerics;

namespace NodaMoney;

// I think the difference lies where the money type is being used for:
// (Total) Amount or (Unit) Price.
//
// A Price is money/unit, which if multiplied by the unit result into money (=(Total) Amount). So:
// unit * price = amount
// unit * money/unit = money
//
// (Total) Amounts should I think always be rounded to prevent errors, but I understand that sometimes (Unit) Prices
// needs more precision. Maybe adding a type Price<unit> which allows more precision could help?
//
// The example you give is a special type of price, an hourrate: money/hour or money/TimeSpan. This could be
// new Price<TimeSpan>(0.0034m, "EUR") or a type Hourrate that extends from Price.
//
// Hour rate could also have business rules about rounding.You could do for example:
// TimeSpan * HourRate = (Total) Amount
//
// Do you see other places, besides price, where you need more precision?

// public readonly struct Price<T>(decimal ratio, Currency currency, T unit)
// {
//
//     public Money Multiple(T unit)
//     {
//         return new Money(ratio * unit, Currency);
//     }
// }

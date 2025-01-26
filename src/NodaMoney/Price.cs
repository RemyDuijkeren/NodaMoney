#if NET7_0_OR_GREATER
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
//
// ### **Recommendation:**
// The choice depends on your priorities for **clarity** and **domain alignment**:
// 1. **General financial clarity** → Use **`Rate`**. (or MonetaryRate)
// - Works well for generic cases or if you want flexibility to expand usage to rates beyond price alone (e.g., exchange rates, fee rates).
// - Gives your type room to grow without feeling too application-specific.
//
// 2. **Specificity and common business terminology** → Use **`Price`**. (or UnitPrice)
// - If this type is primarily meant to represent **money/unit transaction costs**, `Price` is simple, intuitive, and aligns with user expectations.
// - "Price" implies rounding in some cases: Many users might associate "price" with rounded monetary values (e.g., price tags or displayed
//   transaction amounts). Since your `Price` type specifically allows **unrounded decimals** to preserve precision, the term might be slightly misleading.
//
// 3. **Precision and clarity** → Use **`UnitPrice`** or **`PricePerUnit`**.
// - These work if you want to explicitly distinguish between total amounts and per-unit prices.
//
//     If you're planning to make strict distinctions between `Price` and `Rate` types (e.g., `Rate` for fees and taxes, `Price` for fixed costs like products or services), I suggest reserving **`Price` for fixed money/unit relationships** and using **`Rate` for more general financial ratios.** This aligns better with domain conventions.

internal readonly struct Price<T>(decimal ratio, Currency currency) where T : IMultiplyOperators<T, decimal, decimal>
{
    public Money Multiple(T units)
    {
        var result = units * ratio;
        return new Money(result, currency);
    }
}
#endif

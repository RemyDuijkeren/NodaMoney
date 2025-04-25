namespace NodaMoney.Rounding;

// Rounding modes, such as Half-Up or Half-Even (Bankers' Rounding), are primarily determined by business context and
// accounting standards, rather than being inherently tied to specific currencies. While certain currencies have unique
// rounding practices due to the availability of denominations, the choice of rounding mode often depends on the
// financial context and regulatory requirements.
//
// Business Context and Accounting Standards:
//  - Financial Reporting: Many accounting standards recommend specific rounding methods to ensure consistency and
//    accuracy in financial statements. For instance, Half-Even (Bankers' Rounding) is commonly used to minimize
//    cumulative rounding errors over large datasets.
//  - Tax Calculations: Tax authorities may prescribe particular rounding rules for calculating tax liabilities, which
//    businesses must adhere to, regardless of the currency in use.
//  - Point of Sale (POS) Systems: In retail environments, prices might be rounded to the nearest convenient
//    denomination to simplify cash transactions and minimize the use of small coins.
//
// Currency-Specific Rounding Practices:
//
// Certain currencies have unique rounding practices due to the availability of denominations:
//  - Swiss Franc (CHF): Prices are typically rounded to the nearest 0.05 CHF because the smallest coin in circulation
//    is 5 centimes.
//  - Swedish Króna (SEK): After the removal of 1 and 2 öre coins, cash transactions are rounded to the nearest 0.05 SEK.
//  - Euro in NL (EUR): Prices are typically rounded to the nearest 0.05 EUR because the smallest coin in circulation is
//    5 cents. 1 and 2 cents are not used.
//
// These practices are implemented to align with the physical denominations available and to facilitate smoother cash
//  transactions.
//
// Conclusion:
// While certain currencies have specific rounding practices due to their denominations, the choice of rounding
// mode—such as Half-Up or Half-Even—is typically guided by the business context and applicable accounting standards.
// Therefore, we should have a context where we define the rounding and then creating money and money calculations uses
// this context to decide on the rounding strategy. Where rounding is currency specific, it's only in the context of
// cash handling like a POS system, where the available denominations are important.

// Proposal of adding rounding behavior to Money types and/or Currency. Add as Static/Global Property to not add to Money struct
// but have it as a global context? Or add it to CurrencyInfo? Or both? JavaMoney uses a MoneyContext for this. What
// if we create money with a specific context, and we change the context: should it use the context in which it was created?

// Allow custom **rounding behaviors** to be defined on a per-currency or context basis. These rounding strategies also
// leverage metadata to align with currency-specific or business-specific rules.
// ### Key Concepts:
// - Rounding is handled using the `MonetaryRounding` interface.
// - Metadata specifies the **type of rounding rules**:
//   - Standard rounding (e.g., "HALF_EVEN", bank rounding).
//   - Cash rounding (e.g., rounding to the nearest coin denomination).
// - Rounding can be configured globally, per-currency, or even per-transaction.

// IMonetaryRounding?
public interface IRoundingStrategy
{
    decimal Round(decimal amount, CurrencyInfo currencyInfo, int? decimals);
}

/// <summary>Represents the default rounding strategy for monetary calculations.</summary>
/// <param name="mode">Specifies the strategy that mathematical rounding methods should use to round a number.</param>
/// <remarks>
/// This class provides a rounding strategy that uses a specified midpoint rounding method,
/// defaulting to <see cref="MidpointRounding.ToEven"/> (Bankers' rounding). It is commonly
/// used in financial and accounting systems to reduce rounding bias over multiple calculations.
/// </remarks>
/// <example>
/// Can be extended or used as a base class for more specific rounding strategies like
/// <c>HalfEvenRounding</c> or <c>HalfUpRounding</c>.
/// </example>
/// <seealso cref="IRoundingStrategy"/>
internal record DefaultRounding(MidpointRounding mode = MidpointRounding.ToEven) : IRoundingStrategy
{
    /// <summary>Represents a specific rounding strategy utilizing the "Half to Even" method, also known as Bankers' Rounding.</summary>
    /// <remarks>
    /// This class extends from <c>DefaultRounding</c> and enforces the <see cref="MidpointRounding.ToEven"/> strategy.
    /// It is particularly suited for reducing rounding bias in repeated financial calculations and is widely applied
    /// in accounting systems and regulatory settings where even distributions are required.
    /// </remarks>
    public static DefaultRounding HalfEvenRounding() => new(MidpointRounding.ToEven);

    /// <summary>Represents a rounding strategy that rounds halves away from zero.</summary>
    /// <remarks>
    /// This class implements a rounding strategy where values are rounded towards the nearest neighbor,
    /// and in case of a tie, they are rounded away from zero. This approach is commonly used in financial
    /// and business calculations, like retail and Point-of-Sale (POS), to avoid underestimating values.
    /// </remarks>
    public static DefaultRounding HalfUpRounding() => new(MidpointRounding.AwayFromZero);

    public decimal Round(decimal amount, CurrencyInfo currencyInfo, int? decimals)
    {
        if (currencyInfo.MinorUnit == MinorUnit.NotApplicable)
        {
            // no rounding
            return amount;
        }

        if (!currencyInfo.MinorUnitIsDecimalBased)
        {
            // If the minor unit system is not Decimal-based (e.g., a currency with irregular subunit divisions such
            // as thirds or other fractions), the logic modifies the `amount` before rounding. Here’s what happens:
            // 1. Divide `amount` by `currencyInfo.MinimalAmount` (to normalize it to whole "units" of the minor division).
            // 2. Round the result to 0 decimal places (i.e., round to the nearest integer).
            // 3. Multiply it back by `currencyInfo.MinimalAmount` to return the rounded value in its proper scale.
            return Math.Round(amount / currencyInfo.MinimalAmount, 0, mode) * currencyInfo.MinimalAmount;
        }

        // If the minor unit of the currency is decimal-based, the rounding is straightforward. The code rounds
        // `amount` to `currencyInfo.DecimalDigits` decimal places using the provided `rounding` mode.
        return Math.Round(amount, currencyInfo.DecimalDigits, mode);
    }
}

/// <summary>Represents a no-rounding strategy for monetary calculations.</summary>
/// <remarks>
/// This class is used when no rounding is required in monetary operations.
/// It returns the amount without applying any rounding logic. This can be
/// useful in calculations where precision must be preserved exactly as provided
/// or where rounding would lead to incorrect results in downstream processes.
/// </remarks>
/// <seealso cref="IRoundingStrategy"/>
internal record NoRounding : IRoundingStrategy
{
    public decimal Round(decimal amount, CurrencyInfo currencyInfo, int? decimals) => amount;
}

internal record CashDenominationRounding : IRoundingStrategy
{
    public CashDenominationRounding(decimal decimals)
    {
        throw new NotImplementedException();
    }

    public decimal Round(decimal amount, CurrencyInfo currencyInfo, int? decimals) => throw new NotImplementedException();
}

internal record WholeNumberRounding : IRoundingStrategy
{
    public decimal Round(decimal amount, CurrencyInfo currencyInfo, int? decimals)
    {
        return Math.Round(amount, 0, MidpointRounding.ToEven);
    }
}

internal record CustomRoundingStrategy : IRoundingStrategy
{
    public decimal Round(decimal amount, CurrencyInfo currencyInfo, int? decimals)
    {
        // Example: Always round up, ignoring decimal-based or fractional minor units
        if (!currencyInfo.MinorUnitIsDecimalBased)
        {
            return Math.Ceiling(amount / currencyInfo.MinimalAmount) * currencyInfo.MinimalAmount;
        }

        return Math.Ceiling(amount * (decimal)Math.Pow(10, currencyInfo.DecimalDigits)) /
               (decimal)Math.Pow(10, currencyInfo.DecimalDigits);
    }
}

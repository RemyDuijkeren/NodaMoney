namespace NodaMoney.Context;

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

/// <summary>Defines a strategy for rounding monetary amounts based on specific rules or contexts.</summary>
/// <remarks>
/// This interface provides a mechanism to implement various rounding strategies that are applicable
/// in financial and accounting systems. The rounding behavior is influenced by business requirements,
/// accounting standards, or currency-specific practices (such as cash rounding for coin denominations).
/// Implementations may include strategies like WholeNumberRounding, CashDenominationRounding, or
/// other custom rounding behaviors tailored to specific scenarios.
/// </remarks>
/// <seealso cref="StandardRounding"/>
/// <seealso cref="CashDenominationRounding"/>
public interface IRoundingStrategy
{
    /// <summary>Rounds the specified monetary amount according to the defined rounding strategy.</summary>
    /// <param name="amount">The monetary amount to be rounded.</param>
    /// <param name="currencyInfo">The associated currency information used to guide rounding rules, such as denomination or rounding increments.</param>
    /// <param name="decimals">An optional parameter specifying the number of decimal places to round to. If null, the default for the currency will be used.</param>
    /// <returns>The rounded monetary value as a decimal.</returns>
    decimal Round(decimal amount, CurrencyInfo currencyInfo, int? decimals);
}

internal record CashDenominationRounding : IRoundingStrategy
{
    public CashDenominationRounding(decimal decimals)
    {
        throw new NotImplementedException();
    }

    public decimal Round(decimal amount, CurrencyInfo currencyInfo, int? decimals) => throw new NotImplementedException();
}

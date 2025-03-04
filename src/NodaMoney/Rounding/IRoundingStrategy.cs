namespace NodaMoney.Rounding;

// TODO: proposal of adding rounding behavior to Money types. Add as Static/Global Property to not add to Money struct?

internal interface IRoundingStrategy
{
    decimal Round(decimal amount, CurrencyInfo currencyInfo);
}

/// <summary>Represents the default rounding strategy for monetary calculations.</summary>
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
internal record DefaultRounding(MidpointRounding Rounding = MidpointRounding.ToEven) : IRoundingStrategy
{
    public DefaultRounding() : this(MidpointRounding.ToEven) { }

    public decimal Round(decimal amount, CurrencyInfo currencyInfo)
    {
        if (!currencyInfo.MinorUnitIsDecimalBased)
        {
            // If the minor unit system is not decimal based (e.g., a currency with irregular subunit divisions such
            // as thirds or other fractions), the logic modifies the `amount` before rounding. Here’s what happens:
            // 1. Divide `amount` by `currencyInfo.MinimalAmount` (to normalize it to whole "units" of the minor division).
            // 2. Round the result to 0 decimal places (i.e., round to the nearest integer).
            // 3. Multiply it back by `currencyInfo.MinimalAmount` to return the rounded value in its proper scale.
            return Math.Round(amount / currencyInfo.MinimalAmount, 0, Rounding) * currencyInfo.MinimalAmount;
        }

        // If the minor unit of the currency is decimal based, the rounding is straightforward. The code rounds
        // `amount` to `currencyInfo.DecimalDigits` decimal places using the provided `rounding` mode.
        return Math.Round(amount, currencyInfo.DecimalDigits, Rounding);
    }
}

/// <summary>Represents a specific rounding strategy utilizing the "Half to Even" method, also known as Bankers' Rounding.</summary>
/// <remarks>
/// This class extends from <c>DefaultRounding</c> and enforces the <see cref="MidpointRounding.ToEven"/> strategy.
/// It is particularly suited for reducing rounding bias in repeated financial calculations and is widely applied
/// in accounting systems and regulatory settings where even distributions are required.
/// </remarks>
/// <seealso cref="DefaultRounding"/>
/// <seealso cref="IRoundingStrategy"/>
internal record HalfEvenRounding : DefaultRounding; // = MidpointRounding.ToEven

/// <summary>Represents a rounding strategy that rounds halves away from zero.</summary>
/// <remarks>
/// This class implements a rounding strategy where values are rounded towards the nearest neighbor,
/// and in case of a tie, they are rounded away from zero. This approach is commonly used in financial
/// and business calculations, like retail and Point-of-Sale (POS), to avoid underestimating values.
/// </remarks>
/// <seealso cref="IRoundingStrategy"/>
/// <seealso cref="DefaultRounding"/>
internal record HalfUpRounding() : DefaultRounding(MidpointRounding.AwayFromZero);

/// <summary>Represents a no-rounding strategy for monetary calculations.</summary>
/// <remarks>
/// This class is used when no rounding is required in monetary operations.
/// It returns the amount without applying any rounding logic. This can be
/// useful in calculations where precision must be preserved exactly as provided
/// or where rounding would lead to incorrect results in downstream processes.
/// </remarks>
/// <seealso cref="IRoundingStrategy"/>
internal record NoRoundingStrategy : IRoundingStrategy
{
    public decimal Round(decimal amount, CurrencyInfo currencyInfo) => amount;
}

internal record CustomRoundingStrategy : IRoundingStrategy
{
    public decimal Round(decimal amount, CurrencyInfo currencyInfo)
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

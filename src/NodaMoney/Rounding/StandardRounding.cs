namespace NodaMoney.Rounding;

/// <summary>Represents the standard rounding strategy for monetary calculations.</summary>
/// <param name="Mode">Specifies the strategy that mathematical rounding methods should use to round a number.</param>
/// <remarks>
/// This class provides a rounding strategy that uses a specified midpoint rounding method,
/// defaulting to <see cref="MidpointRounding.ToEven"/> (Bankers' rounding). It is commonly
/// used in financial and accounting systems to reduce rounding bias over multiple calculations.
/// </remarks>
/// <seealso cref="IRoundingStrategy"/>
public record StandardRounding(MidpointRounding Mode = MidpointRounding.ToEven) : IRoundingStrategy
{
    /// <inheritdoc cref="IRoundingStrategy.Round"/>
    public decimal Round(decimal amount, CurrencyInfo currencyInfo, int? decimals)
    {
        if (currencyInfo.MinorUnit == MinorUnit.NotApplicable)
        {
            // No rounding
            return amount;
        }

        if (!currencyInfo.MinorUnitIsDecimalBased)
        {
            // If the minor unit system is not Decimal-based (e.g., a currency with irregular subunit divisions such
            // as thirds or other fractions), the logic modifies the `amount` before rounding. Here’s what happens:
            // 1. Divide `amount` by `currencyInfo.MinimalAmount` (to normalize it to whole "units" of the minor division).
            // 2. Round the result to 0 decimal places (i.e., round to the nearest integer).
            // 3. Multiply it back by `currencyInfo.MinimalAmount` to return the rounded value in its proper scale.
            return Math.Round(amount / currencyInfo.MinimalAmount, 0, Mode) * currencyInfo.MinimalAmount;
        }

        // If the minor unit of the currency is decimal-based, the rounding is straightforward. The code rounds
        // `amount` to `currencyInfo.DecimalDigits` decimal places using the provided `rounding` mode.
        return Math.Round(amount, currencyInfo.DecimalDigits, Mode);
    }
}

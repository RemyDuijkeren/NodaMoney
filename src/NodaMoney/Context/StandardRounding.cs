using System.Runtime.CompilerServices;

namespace NodaMoney.Context;

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
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="decimals"/> is less than 0
    /// or greater than 28.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public decimal Round(decimal amount, CurrencyInfo currencyInfo, int? decimals)
    {
        if (currencyInfo is null) throw new ArgumentNullException(nameof(currencyInfo));

        if (currencyInfo.MinorUnit == MinorUnit.NotApplicable)
        {
            return amount; // No rounding needed
        }

        if (decimals is < 0 or > 28) // this pattern also checks for null
        {
            throw new ArgumentOutOfRangeException(nameof(decimals), "Number of decimal must be between 0 and 28.");
        }

        if (!currencyInfo.MinorUnitIsDecimalBased)
        {
            // If the minor unit system is not decimal-based, we scale up the amount before rounding. For robustness
            // and accuracy always use the order "scale up, round, scale down" approach! The steps are:

            // 1. Scale up to normalize it to major units.
            decimal scaledUp = amount * currencyInfo.ScaleFactor;

            // 2. Round to the nearest integer (or overriden decimals).
            decimal rounded = decimals.HasValue
                ? decimal.Round(scaledUp, decimals.Value, Mode) : // round to the specified number of decimals
                decimal.Round(scaledUp, Mode); // round to the nearest integer

            // 3. Scale down to return the rounded value in its proper scale.
            return rounded / currencyInfo.ScaleFactor;
        }

        // If the minor unit of the currency is decimal-based, the rounding is straightforward. The code rounds
        // amount to the currency decimal places (or the overriden decimals) using the provided rounding mode.
        return decimal.Round(amount, decimals ?? currencyInfo.DecimalDigits, Mode);
    }
}

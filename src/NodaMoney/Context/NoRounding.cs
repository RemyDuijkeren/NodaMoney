namespace NodaMoney.Context;

/// <summary>Represents a no-rounding strategy for monetary calculations.</summary>
/// <remarks>
/// This class is used when no rounding is required in monetary operations.
/// It returns the amount without applying any rounding logic. This can be
/// useful in calculations where precision must be preserved exactly as provided
/// or where rounding would lead to incorrect results in downstream processes.
/// </remarks>
/// <seealso cref="IRoundingStrategy"/>
public record NoRounding : IRoundingStrategy
{
    /// <inheritdoc cref="IRoundingStrategy.Round"/>
    public decimal Round(decimal amount, CurrencyInfo currencyInfo, int? decimals) => amount;
}

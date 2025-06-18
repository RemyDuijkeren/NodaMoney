namespace NodaMoney.Context;

/// <summary>
/// Represents options for configuring a monetary context, including rounding strategies, precision, scaling, and default currency.
/// </summary>
public sealed class MoneyContextOptions : IEquatable<MoneyContextOptions>
{
    /// <summary>Gets the default MoneyContextOptions with standard NodaMoney settings.</summary>
    /// <remarks>
    /// These default options include:
    /// - RoundingStrategy: StandardRounding with MidpointRounding.ToEven (banker's rounding)
    /// - Precision: 28 (maximum decimal precision)
    /// - MaxScale: null (no specific maximum scale limitation, follow the currency's scale)
    /// - DefaultCurrency: null (uses the current culture's currency)
    /// </remarks>
    public static MoneyContextOptions Default => new();

    /// <summary>Gets or sets the rounding strategy used to round monetary values in the context.</summary>
    /// <remarks>
    /// The rounding strategy defines how rounding is applied to monetary calculations.
    /// By default, it uses StandardRounding with MidpointRounding.ToEven, commonly referred to as banker's rounding.
    /// </remarks>
    public IRoundingStrategy RoundingStrategy { get; set; } = new StandardRounding();

    /// <summary>Gets or sets the level of precision used for monetary calculations.</summary>
    /// <remarks>
    /// Precision defines the number of significant digits that can be used in monetary computations.
    /// By default, it is set to 28, which provides a high level of accuracy for most financial operations.
    /// Reducing precision may improve performance but could lead to rounding errors in complex calculations.
    /// Increasing precision may be necessary for scenarios that require extremely detailed financial accuracy.
    /// </remarks>
    public int Precision { get; set; } = 28;

    /// <summary>Gets or sets the maximum number of decimal places (scale) allowed for monetary values in the context.</summary>
    /// <remarks>
    /// The <c>MaxScale</c> property determines the highest precision of fractional values that can be maintained.
    /// If set to <c>null</c>, there is no explicit maximum scale, and the currency's default scale or precision
    /// governs the allowable decimal places.
    /// Configuring this property ensures consistency in the rounding and formatting of monetary values across the system.
    /// </remarks>
    public int? MaxScale { get; set; }

    /// <summary>Gets or sets the default currency used in the monetary context.</summary>
    /// <remarks>
    /// When specified, this currency will serve as the default for monetary operations requiring a currency,
    /// unless explicitly overridden. If not set, the default may align with the current culture's currency
    /// or other configured settings.
    /// </remarks>
    public CurrencyInfo? DefaultCurrency { get; set; }

    /// <summary>Gets or sets a value indicating whether zero amounts should require matching currency validation.</summary>
    /// <remarks>
    /// When set to <c>true</c>, zero monetary amounts will be subject to currency matching rules, which can enforce stricter validation
    /// in scenarios where currency consistency is critical, even for zero values. When set to <c>false</c>, zero amounts are exempt from
    /// currency matching, allowing more relaxed validation for such cases. By default, it is <c>false</c>.
    /// </remarks>
    public bool EnforceZeroCurrencyMatching { get; set; } = false;

    /// <inheritdoc />
    public bool Equals(MoneyContextOptions? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return RoundingStrategy.Equals(other.RoundingStrategy)
               && Precision == other.Precision
               && MaxScale == other.MaxScale
               && Equals(DefaultCurrency, other.DefaultCurrency);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is MoneyContextOptions other && Equals(other);

    /// <inheritdoc />
#if NETSTANDARD2_0
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = RoundingStrategy.GetHashCode();
            hashCode = (hashCode * 397) ^ Precision;
            hashCode = (hashCode * 397) ^ MaxScale.GetHashCode();
            hashCode = (hashCode * 397) ^ ((DefaultCurrency?.GetHashCode()) ?? 0);
            return hashCode;
        }
    }
#else
    public override int GetHashCode() => HashCode.Combine(RoundingStrategy, Precision, MaxScale, DefaultCurrency);
#endif
}

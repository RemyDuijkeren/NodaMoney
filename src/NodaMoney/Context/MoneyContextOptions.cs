namespace NodaMoney.Context;

public class MoneyContextOptions : IEquatable<MoneyContextOptions>
{
    public IRoundingStrategy RoundingStrategy { get; set; } = new StandardRounding();
    public int Precision { get; set; } = 28;
    public int? MaxScale { get; set; }
    public CurrencyInfo? DefaultCurrency { get; set; }

    public bool Equals(MoneyContextOptions? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return RoundingStrategy.Equals(other.RoundingStrategy)
               && Precision == other.Precision
               && MaxScale == other.MaxScale
               && Equals(DefaultCurrency, other.DefaultCurrency);
    }

    public override bool Equals(object? obj) => obj is MoneyContextOptions other && Equals(other);

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

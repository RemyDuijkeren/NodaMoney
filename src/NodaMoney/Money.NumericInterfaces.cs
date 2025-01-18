using System.Numerics;

namespace NodaMoney;

public partial struct Money
#if NET7_0_OR_GREATER
    : IMinMaxValue<Money>, IMultiplicativeIdentity<Money, decimal>, IAdditiveIdentity<Money, Money>
    // Decimal implements IFloatingPoint, IMinMaxValue
#endif
{
    /// <inheritdoc/>
    public static Money MaxValue { get; } = new(decimal.MaxValue, Currency.NoCurrency);

    /// <inheritdoc/>
    public static Money MinValue { get; } = new(decimal.MinValue, Currency.NoCurrency);

    /// <inheritdoc/>
    public static decimal MultiplicativeIdentity => 1m;

    /// <inheritdoc/>
    public static Money AdditiveIdentity => new(0m, Currency.NoCurrency);
}

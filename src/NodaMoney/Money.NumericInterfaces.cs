using System.Numerics;

namespace NodaMoney;

public partial struct Money
#if NET7_0_OR_GREATER
    : IMinMaxValue<Money>, IMultiplicativeIdentity<Money, decimal>, IAdditiveIdentity<Money, Money>
    // Decimal implements IFloatingPoint, INumber, INumberBase, ISignedNumber, IFloatingPointConstants (E, Pi, Tua)
#endif
{
    /// <inheritdoc/>
    public static Money MaxValue { get; } = new(decimal.MaxValue, Currency.NoCurrency);

    /// <inheritdoc/>
    public static Money MinValue { get; } = new(decimal.MinValue, Currency.NoCurrency);

    /// <inheritdoc/>
    public static decimal MultiplicativeIdentity => decimal.One;

    /// <inheritdoc/>
    public static Money AdditiveIdentity => new(decimal.Zero, Currency.NoCurrency);

    // public static Money Zero => new(0m, Currency.NoCurrency);
    // public static Money One => new(1m, Currency.NoCurrency);
    // public static Money NegativeOne => new(-1m, Currency.NoCurrency);
    //public static Money Zero(Currency currency) => new(1m, currency);
}

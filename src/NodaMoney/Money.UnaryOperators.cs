using System.Numerics;

namespace NodaMoney;

public partial struct Money
#if NET7_0_OR_GREATER
    : IDecrementOperators<Money>,
        IIncrementOperators<Money>,
        IUnaryPlusOperators<Money, Money>,
        IUnaryNegationOperators<Money, Money>
#endif
{
    /// <summary>Implements the operator +.</summary>
    /// <param name="value">The money.</param>
    /// <returns>The result of the operator.</returns>
    public static Money operator +(Money value) => Plus(value);

    /// <summary>Implements the operator -.</summary>
    /// <param name="value">The money.</param>
    /// <returns>The result of the operator.</returns>
    public static Money operator -(Money value) => Negate(value);

    /// <summary>Implements the operator ++.</summary>
    /// <param name="value">The money.</param>
    /// <returns>The result of the operator.</returns>
    public static Money operator ++(Money value) => Increment(value);

    /// <summary>Implements the operator --.</summary>
    /// <param name="value">The money.</param>
    /// <returns>The result of the operator.</returns>
    public static Money operator --(Money value) => Decrement(value);

    /// <summary>Pluses the specified money.</summary>
    /// <param name="money">The money.</param>
    /// <returns>The result.</returns>
    public static Money Plus(in Money money) => new(+money.Amount, money.Currency, money.Context);

    /// <summary>Negates the specified money.</summary>
    /// <param name="money">The money.</param>
    /// <returns>The result.</returns>
    public static Money Negate(in Money money) => new(-money.Amount, money.Currency, money.Context);

    /// <summary>Increments the specified money.</summary>
    /// <param name="money">The money.</param>
    /// <returns>The result.</returns>
    public static Money Increment(in Money money) => new(money.Amount + money.Currency.MinimalAmount, money.Currency, money.Context);

    /// <summary>Decrements the specified money.</summary>
    /// <param name="money">The money.</param>
    /// <returns>The result.</returns>
    public static Money Decrement(in Money money) => new(money.Amount - money.Currency.MinimalAmount, money.Currency, money.Context);
}

using System.Numerics;

namespace NodaMoney;

internal readonly partial record struct FastMoney
#if NET7_0_OR_GREATER
    : IDecrementOperators<FastMoney>,
        IIncrementOperators<FastMoney>,
        IUnaryPlusOperators<FastMoney, FastMoney>,
        IUnaryNegationOperators<FastMoney, FastMoney>
#endif
{
    /// <summary>Implements the operator +.</summary>
    /// <param name="value">The money.</param>
    /// <returns>The result of the operator.</returns>
    public static FastMoney operator +(FastMoney value) => Plus(value);

    /// <summary>Implements the operator -.</summary>
    /// <param name="value">The money.</param>
    /// <returns>The result of the operator.</returns>
    public static FastMoney operator -(FastMoney value) => Negate(value);

    /// <summary>Implements the operator ++.</summary>
    /// <param name="value">The money.</param>
    /// <returns>The result of the operator.</returns>
    public static FastMoney operator ++(FastMoney value) => Increment(value);

    /// <summary>Implements the operator --.</summary>
    /// <param name="value">The money.</param>
    /// <returns>The result of the operator.</returns>
    public static FastMoney operator --(FastMoney value) => Decrement(value);

    /// <summary>Pluses the specified money.</summary>
    /// <param name="money">The money.</param>
    /// <returns>The result.</returns>
    public static FastMoney Plus(in FastMoney money) => money with { OACurrencyAmount = +money.OACurrencyAmount };

    /// <summary>Negates the specified money.</summary>
    /// <param name="money">The money.</param>
    /// <returns>The result.</returns>
    public static FastMoney Negate(in FastMoney money) => money with { OACurrencyAmount = -money.OACurrencyAmount};

    /// <summary>Increments the specified money.</summary>
    /// <param name="money">The money.</param>
    /// <returns>The result.</returns>
    public static FastMoney Increment(in FastMoney money) => Add(money, money.Currency.MinimalAmount);

    /// <summary>Decrements the specified money.</summary>
    /// <param name="money">The money.</param>
    /// <returns>The result.</returns>
    public static FastMoney Decrement(in FastMoney money) => Subtract(money, money.Currency.MinimalAmount);
}

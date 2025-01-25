﻿using System.Numerics;

namespace NodaMoney;

/// <summary>Represents Money, an amount defined in a specific Currency.</summary>
public partial struct Money
#if NET7_0_OR_GREATER
    : IDecrementOperators<Money>, IIncrementOperators<Money>, IUnaryPlusOperators<Money, Money>, IUnaryNegationOperators<Money, Money>
#endif
{
    /// <summary>Implements the operator +.</summary>
    /// <param name="money">The money.</param>
    /// <returns>The result of the operator.</returns>
    public static Money operator +(Money money) => Plus(money);

    /// <summary>Implements the operator -.</summary>
    /// <param name="money">The money.</param>
    /// <returns>The result of the operator.</returns>
    public static Money operator -(Money money) => Negate(money);

    /// <summary>Implements the operator ++.</summary>
    /// <param name="money">The money.</param>
    /// <returns>The result of the operator.</returns>
    public static Money operator ++(Money money) => Increment(money);

    /// <summary>Implements the operator --.</summary>
    /// <param name="money">The money.</param>
    /// <returns>The result of the operator.</returns>
    public static Money operator --(Money money) => Decrement(money);

    /// <summary>Pluses the specified money.</summary>
    /// <param name="money">The money.</param>
    /// <returns>The result.</returns>
    public static Money Plus(in Money money) => new Money(+money.Amount, money.Currency);

    /// <summary>Negates the specified money.</summary>
    /// <param name="money">The money.</param>
    /// <returns>The result.</returns>
    public static Money Negate(in Money money) => new Money(-money.Amount, money.Currency);

    /// <summary>Increments the specified money.</summary>
    /// <param name="money">The money.</param>
    /// <returns>The result.</returns>
    public static Money Increment(in Money money) => Add(money, money.Currency.MinimalAmount);

    /// <summary>Decrements the specified money.</summary>
    /// <param name="money">The money.</param>
    /// <returns>The result.</returns>
    public static Money Decrement(in Money money) => Subtract(money, money.Currency.MinimalAmount);
}

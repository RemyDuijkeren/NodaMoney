using System.Numerics;
using System.Runtime.CompilerServices;

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
    public static Money Plus(in Money money) => money;

    /// <summary>Negates the specified money.</summary>
    /// <param name="money">The money.</param>
    /// <returns>The result.</returns>
    public static Money Negate(in Money money)
    {
        // Fast-path: -0 == +0; avoid reconstructing when magnitude is zero
        if ((money._low | money._mid | money._high) == 0u)
            return money; // -0 == +0

        // Flip sign by reconstructing with the same magnitude and inverted sign bit
        bool isNegative = (money._flags & SignMask) != 0;
        return new Money(
            unchecked((int)money._low),
            unchecked((int)money._mid),
            unchecked((int)money._high),
            !isNegative,
            money.Scale,
            money.Currency,
            money.Context);
    }

    /// <summary>Increments the specified money.</summary>
    /// <param name="money">The money.</param>
    /// <returns>The result.</returns>
    public static Money Increment(in Money money) => AdjustByMinimal(money, true);

    /// <summary>Decrements the specified money.</summary>
    /// <param name="money">The money.</param>
    /// <returns>The result.</returns>
    public static Money Decrement(in Money money) => AdjustByMinimal(money, false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Money AdjustByMinimal(in Money money, bool increment)
    {
        // We can skip rounding because we're adding/subtracting the minimal amount. We can take the fast path
        var minimal = new decimal(1, 0, 0, false, money.Scale);

        // First compute the delta, then add it => this creates a single code path in IL when adding or subtracting.
        var delta = increment ? minimal : decimal.Negate(minimal);
        var amount = money.Amount + delta;

#if NET5_0_OR_GREATER
        Span<int> bits = stackalloc int[4];
        decimal.GetBits(amount, bits);
        int lo = bits[0], mid = bits[1], hi = bits[2];
        int flags = bits[3];
#else
        int[] bits = decimal.GetBits(amount);
        int lo = bits[0], mid = bits[1], hi = bits[2];
        int flags = bits[3];
#endif
        bool isNegative = (flags & unchecked((int)0x80000000)) != 0;
        byte scale = (byte)((flags >> 16) & 0x7F);

        return new Money(lo, mid, hi, isNegative, scale, money.Currency, money.Context);
    }
}

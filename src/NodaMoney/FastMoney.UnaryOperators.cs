using System.Numerics;
using System.Runtime.CompilerServices;

namespace NodaMoney;

public readonly partial record struct FastMoney
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
    public static FastMoney Plus(in FastMoney money) => money;

    /// <summary>Negates the specified money.</summary>
    /// <param name="money">The money.</param>
    /// <returns>The result.</returns>
    public static FastMoney Negate(in FastMoney money) => money with { OACurrencyAmount = -money.OACurrencyAmount};

    /// <summary>Increments the specified money.</summary>
    /// <param name="money">The money.</param>
    /// <returns>The result.</returns>
    public static FastMoney Increment(in FastMoney money) => AdjustByMinorUnit(money, increment: true);

    /// <summary>Decrements the specified money.</summary>
    /// <param name="money">The money.</param>
    /// <returns>The result.</returns>
    public static FastMoney Decrement(in FastMoney money) => AdjustByMinorUnit(money, increment: false);

    /// <summary>Precomputed steps for DecimalDigits 0..4; FastMoney fixed scale is 4.</summary>
    private static readonly long[] s_stepByDecimalDigits = [10_000L, 1_000L, 100L, 10L, 1L];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static FastMoney AdjustByMinorUnit(in FastMoney money, bool increment)
    {
        int d = CurrencyInfo.GetInstance(money.Currency).DecimalDigits;

        // Fast index with range guard; keeps a single bounds check that the JIT can hoist
        if ((uint)d > 4u)
        {
            // This should never happen for FastMoney; keep throw cold
            throw new InvalidCurrencyException($"Unsupported DecimalDigits {d} for FastMoney increment/decrement.");
        }

        // Compute the OA step based on DecimalDigits (FastMoney fixed scale is 4).
        // step = 10^(4 - d), with d in [0..4]. d>4 is not allowed for FastMoney elsewhere.
        long step = s_stepByDecimalDigits[d]; // Fast lookup for hot path
        long newAmount = increment
            ? checked(money.OACurrencyAmount + step)
            : checked(money.OACurrencyAmount - step);

        return money with { OACurrencyAmount = newAmount };
    }

}

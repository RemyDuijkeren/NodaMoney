using System.Runtime.CompilerServices;

namespace NodaMoney;

/// <summary>Rounds a magnitude that was already split into an integer quotient and remainder to a whole number under a
/// <see cref="MidpointRounding"/> mode, then applies the sign. Shared by the integer conversions of <see cref="Money"/>
/// and <see cref="FastMoney"/>, which work on the raw mantissa so they can skip the decimal round trip.</summary>
internal static class IntegerRounding
{
    /// <summary>Powers of ten for scales 0 to 18. 10^18 is the largest power that still fits comfortably under
    /// long.MaxValue, so a 64-bit mantissa divided or multiplied by an entry never needs more than 64 bits of headroom.</summary>
    internal static readonly long[] Pow10 =
    [
        1L, 10L, 100L, 1_000L, 10_000L, 100_000L, 1_000_000L, 10_000_000L, 100_000_000L, 1_000_000_000L,
        10_000_000_000L, 100_000_000_000L, 1_000_000_000_000L, 10_000_000_000_000L, 100_000_000_000_000L,
        1_000_000_000_000_000L, 10_000_000_000_000_000L, 100_000_000_000_000_000L, 1_000_000_000_000_000_000L
    ];

    /// <summary>Rounds <c>quotient + remainder / divisor</c> (all non-negative magnitude parts) to an integer.</summary>
    /// <param name="quotient">The truncated magnitude quotient.</param>
    /// <param name="remainder">The magnitude remainder, in <c>[0, divisor)</c>.</param>
    /// <param name="divisor">The power of ten the magnitude was divided by.</param>
    /// <param name="mode">The midpoint rounding mode.</param>
    /// <param name="isNegative">Whether the original value is negative; decides the direction of the directional modes and the sign of the result.</param>
    /// <param name="rounded">The signed rounded value.</param>
    /// <returns><see langword="false"/> for a mode this build does not know (the netstandard legs lack the directional
    /// modes at compile time, but a newer runtime can still pass them); the caller then uses the decimal path.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryRound(long quotient, long remainder, long divisor, MidpointRounding mode, bool isNegative, out long rounded)
    {
        if (remainder != 0)
        {
            switch (mode)
            {
                case MidpointRounding.ToEven:
                    long twiceRemainder = remainder * 2;
                    if (twiceRemainder > divisor || (twiceRemainder == divisor && (quotient & 1) != 0))
                        quotient++;
                    break;
                case MidpointRounding.AwayFromZero:
                    if (remainder * 2 >= divisor)
                        quotient++;
                    break;
#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
                case MidpointRounding.ToZero:
                    break; // the truncating division already rounded toward zero
                case MidpointRounding.ToNegativeInfinity:
                    if (isNegative)
                        quotient++; // floor: a negative value's magnitude grows
                    break;
                case MidpointRounding.ToPositiveInfinity:
                    if (!isNegative)
                        quotient++; // ceiling: a positive value's magnitude grows
                    break;
#endif
                default:
                    rounded = 0;
                    return false;
            }
        }

        rounded = isNegative ? -quotient : quotient;
        return true;
    }
}

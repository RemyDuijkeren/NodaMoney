namespace NodaMoney;

/// <summary>Converts a <see cref="double"/> to a <see cref="decimal"/> rounded to 15 significant digits, the precision a
/// double reliably holds. Up to .NET 10 the runtime's own conversion did this rounding. From .NET 11 on it returns the
/// decimal closest to the double's exact binary value instead, so <c>(decimal)0.33</c> becomes
/// <c>0.3300000000000000155431223448</c> and a decimal midpoint like <c>10.005</c> stops rounding as a midpoint. Shared by
/// the double constructors of <see cref="Money"/>, <see cref="FastMoney"/> and <see cref="Exchange.ExchangeRate"/>, so
/// they keep their documented behavior on every runtime.</summary>
/// <remarks>This is the algorithm of the .NET 10 runtime (VarDecFromR8 in System.Decimal.DecCalc, MIT licensed), so the
/// result is bit-for-bit what <c>(decimal)value</c> returned up to .NET 10. See
/// https://learn.microsoft.com/dotnet/core/compatibility/core-libraries/11/decimal-biginteger-floating-point-conversions.</remarks>
internal static class DoubleConversion
{
    private const int MaxScale = 28;

    private static readonly double[] s_powersOfTen =
    [
        1e0, 1e1, 1e2, 1e3, 1e4, 1e5, 1e6, 1e7, 1e8, 1e9, 1e10, 1e11, 1e12, 1e13, 1e14, 1e15, 1e16, 1e17, 1e18, 1e19,
        1e20, 1e21, 1e22, 1e23, 1e24, 1e25, 1e26, 1e27, 1e28,
    ];

    /// <summary>Converts <paramref name="value"/> to a <see cref="decimal"/> rounded to 15 significant digits, without
    /// trailing zeros.</summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The converted value.</returns>
    /// <exception cref="OverflowException"><paramref name="value"/> is NaN, infinity, or outside the range of
    /// <see cref="decimal"/>.</exception>
    public static decimal ToDecimal(double value)
    {
        // Binary exponent, so 2^(exponent - 1) <= |value| < 2^exponent.
        int exponent = (int)((BitConverter.DoubleToInt64Bits(value) >> 52) & 0x7FF) - 1022;
        if (exponent < -94)
            return decimal.Zero; // below 0.5 * 10^-28, rounds to zero

        if (exponent > 96)
            return (decimal)value; // NaN, infinity or too large: every runtime throws the OverflowException here

        // Scale to a 15-digit integer. The decimal exponent is estimated as exponent * log10(2), in fixed point.
        double scaled = Math.Abs(value);
        int scale = 14 - ((exponent * 19728) >> 16);
        if (scale >= 0)
        {
            scale = Math.Min(scale, MaxScale);
            scaled *= s_powersOfTen[scale];
        }
        else if (scale != -1 || scaled >= 1E15)
        {
            scaled /= s_powersOfTen[-scale];
        }
        else
        {
            scale = 0;
        }

        if (scaled < 1E14 && scale < MaxScale)
        {
            scaled *= 10;
            scale++;
        }

        ulong mantissa = (ulong)Math.Round(scaled); // midpoints to even
        if (mantissa == 0)
            return decimal.Zero;

        if (scale < 0)
        {
            // More than 15 integer digits, like 1234567890123456789 to 1234567890123460000.
            decimal integer = mantissa * (decimal)(ulong)s_powersOfTen[-scale];
            return value < 0 ? -integer : integer;
        }

        // Strip the trailing zeros, so 0.33 does not come back as 0.330000000000000. At most 14, like the runtime did:
        // a mantissa that rounded up to 10^15 keeps one zero.
        int zeros = Math.Min(scale, 14);
        if (zeros >= 8 && mantissa % 100_000_000 == 0) { mantissa /= 100_000_000; zeros -= 8; scale -= 8; }
        if (zeros >= 4 && mantissa % 10_000 == 0) { mantissa /= 10_000; zeros -= 4; scale -= 4; }
        if (zeros >= 2 && mantissa % 100 == 0) { mantissa /= 100; zeros -= 2; scale -= 2; }
        if (zeros >= 1 && mantissa % 10 == 0) { mantissa /= 10; scale--; }

        return new decimal((int)mantissa, (int)(mantissa >> 32), 0, value < 0, (byte)scale);
    }
}

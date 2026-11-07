namespace NodaMoney;

/// <summary>Converts a <see cref="double"/> to a <see cref="decimal"/> rounded to 15 significant digits, the precision a
/// double reliably holds. Up to .NET 10 the runtime's own conversion did this rounding. From .NET 11 on it returns the
/// decimal closest to the double's exact binary value instead, so <c>(decimal)0.33</c> becomes
/// <c>0.3300000000000000155431223448</c> and a decimal midpoint like <c>10.005</c> stops rounding as a midpoint. Shared by
/// the double constructors of <see cref="Money"/>, <see cref="FastMoney"/> and <see cref="Exchange.ExchangeRate"/>, so
/// they keep their documented behavior on every runtime.</summary>
/// <remarks>See https://learn.microsoft.com/dotnet/core/compatibility/core-libraries/11/decimal-biginteger-floating-point-conversions.</remarks>
internal static class DoubleConversion
{
    private const int SignificantDigits = 15;
    private const int MaxExponent = 28;

    /// <summary>Powers of ten from 10^-28 to 10^28, indexed by exponent + 28.</summary>
    private static readonly decimal[] s_powersOfTen = CreatePowersOfTen();

    /// <summary>Whether the runtime converts to the exact binary value, as .NET 11 and later do. Detected once, so the
    /// older runtimes, which already round to 15 significant digits, skip the extra work.</summary>
    private static readonly bool s_runtimeConvertsExactly = ConvertsExactly(0.1);

    /// <summary>Converts <paramref name="value"/> to a <see cref="decimal"/> rounded to 15 significant digits, without
    /// trailing zeros.</summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The converted value.</returns>
    /// <exception cref="OverflowException"><paramref name="value"/> is NaN, infinity, or outside the range of
    /// <see cref="decimal"/>.</exception>
    public static decimal ToDecimal(double value)
    {
        decimal result = (decimal)value;
        if (!s_runtimeConvertsExactly || result == decimal.Zero)
            return result;

        // Find the exponent of the leading digit, so 10^exponent <= |result| < 10^(exponent + 1). Log10 on the double
        // can be one off next to a power of ten, so settle it against the decimal.
        decimal magnitude = Math.Abs(result);
        int exponent = Math.Max(-MaxExponent, Math.Min(MaxExponent, (int)Math.Floor(Math.Log10(Math.Abs(value)))));
        while (exponent < MaxExponent && magnitude >= PowerOfTen(exponent + 1))
            exponent++;
        while (exponent > -MaxExponent && magnitude < PowerOfTen(exponent))
            exponent--;

        int decimals = SignificantDigits - 1 - exponent;
        if (decimals < 0)
        {
            // More than 15 integer digits: round away the trailing integer digits, like 1234567890123456789 to
            // 1234567890123460000.
            decimal factor = PowerOfTen(-decimals);
            return decimal.Round(result / factor, 0, MidpointRounding.ToEven) * factor;
        }

        return StripTrailingZeros(decimal.Round(result, Math.Min(decimals, MaxExponent), MidpointRounding.ToEven));
    }

    /// <summary>Round keeps the scale, so 0.33 comes back as 0.3300000000000000. Strip the trailing zeros like the old
    /// runtime conversion did. With at most 16 significant digits left the mantissa fits in 64 bits, which is cheaper
    /// than normalizing through a decimal division.</summary>
    private static decimal StripTrailingZeros(decimal value)
    {
#if NET5_0_OR_GREATER
        Span<int> bits = stackalloc int[4];
        decimal.GetBits(value, bits);
#else
        int[] bits = decimal.GetBits(value);
#endif
        ulong mantissa = ((ulong)(uint)bits[1] << 32) | (uint)bits[0];
        int scale = (bits[3] >> 16) & 0xFF;
        while (scale > 0 && mantissa % 10 == 0)
        {
            mantissa /= 10;
            scale--;
        }

        return new decimal((int)mantissa, (int)(mantissa >> 32), 0, bits[3] < 0, (byte)scale);
    }

    private static decimal PowerOfTen(int exponent) => s_powersOfTen[exponent + MaxExponent];

    private static bool ConvertsExactly(double probe) => (decimal)probe != 0.1m;

    private static decimal[] CreatePowersOfTen()
    {
        var powers = new decimal[(2 * MaxExponent) + 1];
        decimal power = 1m;
        for (int exponent = 0; exponent <= MaxExponent; exponent++)
        {
            powers[MaxExponent - exponent] = new decimal(1, 0, 0, false, (byte)exponent);
            powers[MaxExponent + exponent] = power;
            if (exponent < MaxExponent)
                power *= 10m;
        }

        return powers;
    }
}

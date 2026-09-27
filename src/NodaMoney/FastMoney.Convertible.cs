using System.Data.SqlTypes;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using NodaMoney.Context;

namespace NodaMoney;

public readonly partial record struct FastMoney
{
    // FastMoney <-> Money

    public static explicit operator Money(FastMoney money) => money.ToMoney();

    public static explicit operator FastMoney(Money money) => new(money);
    public Money ToMoney()
    {
        // The Money is created in the ambient context (not this instance's context), so it is rounded the same way
        // as a Money constructed from the same amount would be.
        MoneyContext context = MoneyContext.CurrentContext;
        if (TryBuildMoneyFromTicks(context, out Money money))
            return money;

        // Fallback: convert to decimal and delegate to the standard constructor so rounding is applied using the
        // context (strategy and max scale, currency rules).
        decimal amount = decimal.FromOACurrency(OACurrencyAmount);
        return new Money(amount, Currency, context);
    }

    /// <summary>Builds the <see cref="Money"/> straight from the OA currency ticks, reproducing what
    /// <see cref="decimal.FromOACurrency"/> followed by the <see cref="Money"/> constructor would produce: trailing
    /// zeros are stripped from the scale and a standard rounding context rounds the remaining digits to the target
    /// scale on the integer mantissa. Applies only to the standard and no-rounding kinds and to currencies whose
    /// rounding is plain decimal-digit rounding; everything else takes the decimal path.</summary>
    private bool TryBuildMoneyFromTicks(MoneyContext context, out Money money)
    {
        long ticks = OACurrencyAmount;
        Currency currency = Currency;
        if (ticks == 0)
        {
            money = new Money(0, 0, 0, false, 0, currency, context.Index); // same as the constructor's zero fast path
            return true;
        }

        if (context.Kind == RoundingKind.Custom || ticks == long.MinValue)
        {
            money = default;
            return false;
        }

        bool isNegative = ticks < 0;
        long magnitude = isNegative ? -ticks : ticks;

        // decimal.FromOACurrency strips trailing zeros from the 4-decimal scale; do the same to end up with the same scale.
        int scale = 4;
        while (scale > 0 && magnitude % 10 == 0)
        {
            scale--;
            magnitude /= 10;
        }

        if (context.Kind == RoundingKind.Standard)
        {
            // Same target as StandardRounding.Round: 2 for two-decimal currencies unless MaxScale overrides it, no
            // rounding for currencies without a minor unit, and the decimal path for non-decimal minor units (MGA, MRU).
            int targetScale;
            if (currency.IsMinorUnit2 && context.MaxScale is null or 2)
            {
                targetScale = 2;
            }
            else
            {
                CurrencyInfo currencyInfo = CurrencyInfo.GetInstance(currency);
                if (currencyInfo.MinorUnit == MinorUnit.NotApplicable)
                {
                    targetScale = scale;
                }
                else if (!currencyInfo.MinorUnitIsDecimalBased)
                {
                    money = default;
                    return false;
                }
                else
                {
                    targetScale = context.MaxScale ?? currencyInfo.DecimalDigits;
                }
            }

            if (scale > targetScale)
            {
                long divisor = IntegerRounding.Pow10[scale - targetScale];
                if (!IntegerRounding.TryRound(magnitude / divisor, magnitude % divisor, divisor, context.Mode, isNegative, out long rounded))
                {
                    money = default;
                    return false;
                }

                // A value rounded away to zero keeps its sign and the target scale, exactly like decimal.Round does.
                magnitude = isNegative ? -rounded : rounded;
                scale = targetScale;
            }
        }

        money = new Money(unchecked((int)(uint)magnitude), unchecked((int)(uint)((ulong)magnitude >> 32)), 0, isNegative, (byte)scale, currency, context.Index);
        return true;
    }

    // FastMoney <-> minor units

    /// <summary>Converts the value of this instance to minor units.</summary>
    /// <returns>The value of the <see cref="FastMoney"/> instance, converted to minor units (e.g., cents, yen, etc.).</returns>
    /// <exception cref="OverflowException">The value of this instance is outside the range of a <see cref="long"/> value.</exception>
    public long ToMinorUnits()
    {
        var currencyInfo = CurrencyInfo.GetInstance(Currency);
        decimal roundedAmount = Context.RoundingStrategy.Round(Amount, Currency, null);
        return checked((long)(roundedAmount * currencyInfo.ScaleFactor));
    }

    /// <summary>Creates a <see cref="FastMoney"/> instance from minor units.</summary>
    /// <param name="minorUnits">The amount in minor units (e.g., cents, yen, etc.).</param>
    /// <param name="currency">The currency of the money.</param>
    /// <returns>A new <see cref="FastMoney"/> instance.</returns>
    public static FastMoney FromMinorUnits(long minorUnits, Currency currency)
    {
        var currencyInfo = CurrencyInfo.GetInstance(currency);
        decimal amount = (decimal)minorUnits / currencyInfo.ScaleFactor;
        return new FastMoney(amount, currency);
    }

    // FastMoney <-> SqlMoney

    public static explicit operator SqlMoney(FastMoney money) => money.ToSqlMoney();
    public static explicit operator FastMoney?(SqlMoney money) => FromSqlMoney(money);

#if NET8_0_OR_GREATER
    /// <summary>Converts the value of this instance to a <see cref="SqlMoney"/> instance.</summary>
    /// <returns>The resulting <see cref="SqlMoney"/> value.</returns>
    /// <remarks>Both types share the TDS tick scale (scaled by 10,000), so this skips the decimal round trip.</remarks>
    public SqlMoney ToSqlMoney() => SqlMoney.FromTdsValue(OACurrencyAmount);
    public static FastMoney? FromSqlMoney(SqlMoney sqlMoney) => sqlMoney.IsNull ? null : FromOACurrency(sqlMoney.GetTdsValue());
    public static FastMoney? FromSqlMoney(SqlMoney sqlMoney, Currency currency, MoneyContext? context = null) =>
        sqlMoney.IsNull ? null : FromOACurrency(sqlMoney.GetTdsValue(), currency, context);
#else
    public SqlMoney ToSqlMoney() => new(Amount);
    public static FastMoney? FromSqlMoney(SqlMoney sqlMoney) => sqlMoney.IsNull ? null : new FastMoney(sqlMoney.Value);
    public static FastMoney? FromSqlMoney(SqlMoney sqlMoney, Currency currency, MoneyContext? context = null) =>
        sqlMoney.IsNull ? null : new FastMoney(sqlMoney.Value, currency, context);
#endif

    // FastMoney <-> OACurrency

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public long ToOACurrency() => OACurrencyAmount;

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static FastMoney FromOACurrency(long cy) => new(decimal.FromOACurrency(cy));

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static FastMoney FromOACurrency(long cy, Currency currency, MoneyContext? context = null)
    {
        context ??= MoneyContext.FastMoney;

        // The ticks already carry four decimals, so a context that would not round them further can take them as-is
        // and skip the decimal round trip; any other context rounds through the constructor.
        bool ticksAreFinal = context.Kind == RoundingKind.None
                             || (context.Kind == RoundingKind.Standard && context.Mode == MidpointRounding.ToEven && context.MaxScale == 4);
        return ticksAreFinal ? FromTicks(cy, currency, context) : new FastMoney(decimal.FromOACurrency(cy), currency, context);
    }

    // FastMoney <-> decimal, int, long, double

    /// <summary>Performs an explicit conversion from <see cref="FastMoney"/> to <see cref="double"/>.</summary>
    /// <param name="money">The instance of <see cref="FastMoney"/> to convert.</param>
    /// <returns>The resulting <see cref="double"/> value.</returns>
    /// <remarks>This operation may produce round-off errors. Also, the <see cref="Currency"/> information is lost.</remarks>
    public static explicit operator double(FastMoney money) => money.ToDouble();

    /// <summary>Performs an explicit conversion from <see cref="FastMoney"/> to <see cref="decimal"/>.</summary>
    /// <param name="money">The instance of <see cref="FastMoney"/> to convert.</param>
    /// <returns>The resulting <see cref="decimal"/> value.</returns>
    /// <remarks>The <see cref="Currency"/> information is lost.</remarks>
    public static explicit operator decimal(FastMoney money) => money.Amount;

    /// <summary>Performs an explicit conversion from <see cref="FastMoney"/> to <see cref="double"/>.</summary>
    /// <param name="money">The instance of <see cref="FastMoney"/> to convert.</param>
    /// <returns>The converted <see cref="double"/> value.</returns>
    /// <remarks>Amount, rounded to the nearest <see cref="long"/>. If Amount is halfway between two whole numbers, the even number is returned;
    /// that is, 4.5 is converted to 4, and 5.5 is converted to 6. Also, the <see cref="Currency"/> information is lost.</remarks>
    /// <exception cref="OverflowException">The value of this instance is outside the range of a <see cref="long"/> value.</exception>
    public static explicit operator long(FastMoney money) => money.ToInt64();

    /// <summary>Performs an explicit conversion from <see cref="long"/> to <see cref="FastMoney"/>.</summary>
    /// <param name="amount">The money amount.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator FastMoney(long amount) => new(amount);

    /// <summary>Performs an explicit conversion from <see cref="double"/> to <see cref="FastMoney"/>.</summary>
    /// <param name="amount">The money amount.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator FastMoney(double amount) => new(amount);

    /// <summary>Performs an explicit conversion from <see cref="decimal"/> to <see cref="FastMoney"/>.</summary>
    /// <param name="amount">The money amount.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator FastMoney(decimal amount) => new(amount);

    /// <summary>Converts the value of this instance to an <see cref="double"/>.</summary>
    /// <returns>The value of the <see cref="Money"/> instance, converted to a <see cref="double"/>.</returns>
    /// <remarks>This operation may produce round-off errors. Also, the <see cref="Currency"/> information is lost.</remarks>
    public double ToDouble() => Convert.ToDouble(Amount);

    /// <summary>Converts the value of this instance to an <see cref="decimal"/>.</summary>
    /// <returns>The value of the <see cref="Money"/> instance, converted to a <see cref="decimal"/>.</returns>
    /// <remarks>The <see cref="Currency"/> information is lost.</remarks>
    public decimal ToDecimal() => Amount;

    /// <summary>Converts the value of this instance to an <see cref="int"/>.</summary>
    /// <returns>The value of the <see cref="Money"/> instance, converted to a <see cref="int"/>.</returns>
    /// <remarks>Amount, rounded to the nearest <see cref="int"/>. If Amount is halfway between two whole numbers, the even number is returned;
    /// that is, 4.5 is converted to 4, and 5.5 is converted to 6. Also, the <see cref="Currency"/> information is lost.</remarks>
    /// <exception cref="OverflowException">The value of this instance is outside the range of a <see cref="int"/> value.</exception>
    public int ToInt32()
    {
        // Fast path: do integer rounding directly on OACurrencyAmount (scaled by 10,000)
        if (TryOACurrencyAmountToLongWithRounding(out long int64)) return checked((int)int64);

        // Fallback: use general strategy on decimal
        var rounded = Context.RoundingStrategy.Round(Amount, CurrencyInfo.GetInstance(Currency), 0);
        return checked((int)rounded);
    }

    /// <summary>Converts the value of this instance to an <see cref="long"/>.</summary>
    /// <returns>The value of the <see cref="Money"/> instance, converted to a <see cref="long"/>.</returns>
    /// <remarks>Amount, rounded to the nearest <see cref="long"/>. If Amount is halfway between two whole numbers, the even number is returned;
    /// that is, 4.5 is converted to 4, and 5.5 is converted to 6. Also, the <see cref="Currency"/> information is lost.</remarks>
    /// <exception cref="OverflowException">The value of this instance is outside the range of a <see cref="long"/> value.</exception>
    public long ToInt64()
    {
        // Fast path: do integer rounding directly on OACurrencyAmount (scaled by 10,000)
        if (TryOACurrencyAmountToLongWithRounding(out long int64)) return int64;

        // Fallback: use general strategy on decimal
        var rounded = Context.RoundingStrategy.Round(Amount, CurrencyInfo.GetInstance(Currency), 0);
        return checked((long)rounded);
    }

    bool TryOACurrencyAmountToLongWithRounding(out long int64)
    {
        MoneyContext context = Context;
        if (context.Kind != RoundingKind.Standard)
        {
            int64 = 0;
            return false;
        }

        // Work on the magnitude; long.MinValue's magnitude still fits a ulong.
        long ticks = OACurrencyAmount;
        bool isNegative = ticks < 0;
        ulong magnitude = isNegative ? unchecked((ulong)(-ticks)) : (ulong)ticks;
        return IntegerRounding.TryRound((long)(magnitude / (ulong)ScaleFactor), (long)(magnitude % (ulong)ScaleFactor), ScaleFactor, context.Mode, isNegative, out int64);
    }
}

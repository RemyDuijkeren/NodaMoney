using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using NodaMoney.Context;

namespace NodaMoney;

public readonly partial record struct FastMoney
{
    // FastMoney <-> Money

    public static explicit operator Money(FastMoney money) => money.ToMoney();
    public static explicit operator FastMoney(Money money) => FromMoney(money);
    public Money ToMoney() => new(OACurrencyAmount, Currency, ContextIndex);
    public static FastMoney FromMoney(Money money) => new(money.Amount, money.Currency);

    // FastMoney <-> SqlMoney

    public static explicit operator SqlMoney(FastMoney money) => money.ToSqlMoney();
    public static explicit operator FastMoney?(SqlMoney money) => FromSqlMoney(money);
    public SqlMoney ToSqlMoney() => new(Amount);
    public static FastMoney? FromSqlMoney(SqlMoney sqlMoney) => sqlMoney.IsNull ? null : new FastMoney(sqlMoney.Value);
    public static FastMoney? FromSqlMoney(SqlMoney sqlMoney, Currency currency, MoneyContext? context = null) =>
        sqlMoney.IsNull ? null : new FastMoney(sqlMoney.Value, currency, context);


    // FastMoney <-> OACurrency

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public long ToOACurrency() => OACurrencyAmount;

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static FastMoney FromOACurrency(long cy) => new(decimal.FromOACurrency(cy));

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static FastMoney FromOACurrency(long cy, Currency currency, MoneyContext? context = null) =>
        new(decimal.FromOACurrency(cy), currency, context);

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
        if (Context.RoundingStrategy is StandardRounding sr)
        {
            long q = OACurrencyAmount / ScaleFactor; // truncates toward zero in .NET
            long r = OACurrencyAmount % ScaleFactor; // the remainder with sign of dividend

            if (r == 0)
            {
                int64 = q;
                return true;
            }

            long absR = r >= 0 ? r : -r;

            switch (sr.Mode)
            {
                case MidpointRounding.ToEven:
                    if (absR > 5000)
                    {
                        q += r > 0 ? 1 : -1;
                    }
                    else if (absR == 5000)
                    {
                        // Tie -> go to even
                        bool qIsEven = (q & 1) == 0;
                        if (!qIsEven)
                            q += r > 0 ? 1 : -1;
                    }

                    int64 = q;
                    return true;

                case MidpointRounding.AwayFromZero:
                    if (absR >= 5000)
                        q += r > 0 ? 1 : -1;
                    int64 = q;
                    return true;

#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
                case MidpointRounding.ToZero:
                    // Truncation toward zero already achieved by integer division
                    int64 = q;
                    return true;

                case MidpointRounding.ToNegativeInfinity:
                    // Floor: if remainder negative, step down by 1 (since q truncates toward zero)
                    if (r < 0) q--;
                    int64 = q;
                    return true;

                case MidpointRounding.ToPositiveInfinity:
                    // Ceiling: if remainder positive, step up by 1
                    if (r > 0) q++;
                    int64 = q;
                    return true;
#endif
            }
        }

        int64 = 0;
        return false;
    }
}

using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using NodaMoney.Context;

namespace NodaMoney;

public readonly partial record struct FastMoney
{
    public Money ToMoney() => new(Amount, Currency, Context);
    public static FastMoney FromMoney(Money money) => new(money.Amount, money.Currency, money.Context);

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public long ToOACurrency() => OACurrencyAmount;

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static FastMoney FromOACurrency(long cy) => new(decimal.FromOACurrency(cy));

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static FastMoney FromOACurrency(long cy, Currency currency, MoneyContext? context = null) =>
        new(decimal.FromOACurrency(cy), currency, context);

    public static explicit operator SqlMoney(FastMoney money) => money.ToSqlMoney();
    public static explicit operator FastMoney?(SqlMoney money) => FromSqlMoney(money);
    public SqlMoney ToSqlMoney() => new(Amount);
    public static FastMoney? FromSqlMoney(SqlMoney sqlMoney) => sqlMoney.IsNull ? null : new FastMoney(sqlMoney.Value);
    public static FastMoney? FromSqlMoney(SqlMoney sqlMoney, Currency currency, MoneyContext? context = null) =>
        sqlMoney.IsNull ? null : new FastMoney(sqlMoney.Value, currency, context);

    /// <summary>Converts the value of this instance to an <see cref="float"/>.</summary>
    /// <param name="money">A <see cref="Money"/> value.</param>
    /// <returns>The value of the <see cref="Money"/> instance, converted to a <see cref="float"/>.</returns>
    /// <remarks>Because a <see cref="float"/> has fewer significant digits than a <see cref="Money"/> value, this operation may
    /// produce round-off errors. Also, the <see cref="Currency"/> information is lost.</remarks>
    public float ToSingle() => Convert.ToSingle(Amount);

    /// <summary>Converts the value of this instance to an <see cref="double"/>.</summary>
    /// <param name="money">A <see cref="Money"/> value.</param>
    /// <returns>The value of the current instance, converted to a <see cref="double"/>.</returns>
    /// <remarks>Because a Double has fewer significant digits than a <see cref="Money"/> value, this operation may produce round-off
    /// errors.</remarks>
    public double ToDouble() => Convert.ToDouble(Amount);

    /// <summary>Converts the value of this instance to an <see cref="decimal"/>.</summary>
    /// <param name="money">A <see cref="Money"/> value.</param>
    /// <returns>The value of the <see cref="Money"/> instance, converted to a <see cref="decimal"/>.</returns>
    /// <remarks>The <see cref="Currency"/> information is lost.</remarks>
    public decimal ToDecimal() => Amount;

    /// <summary>Converts the value of this instance to an equivalent 16-bit signed integer using the specified
    /// culture-specific formatting information.</summary>
    /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation that supplies
    /// culture-specific formatting information.</param>
    /// <returns>An 16-bit signed integer equivalent to the value of this instance.</returns>
    public short ToInt16() => Convert.ToInt16(Amount);

    /// <summary>Converts the value of this instance to an equivalent 32-bit signed integer using the specified
    /// culture-specific formatting information.</summary>
    /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation that supplies
    /// culture-specific formatting information.</param>
    /// <returns>An 32-bit signed integer equivalent to the value of this instance.</returns>
    public int ToInt32() => Convert.ToInt32(Amount);

    /// <summary>Converts the value of this instance to an equivalent 64-bit signed integer using the specified
    /// culture-specific formatting information.</summary>
    /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation that supplies
    /// culture-specific formatting information.</param>
    /// <returns>An 64-bit signed integer equivalent to the value of this instance.</returns>
    public long ToInt64() => Convert.ToInt64(Amount);

    /// <summary>Converts the value of this instance to an equivalent 8-bit signed integer using the specified culture-specific
    /// formatting information.</summary>
    /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation that supplies
    /// culture-specific formatting information.</param>
    /// <returns>An 8-bit signed integer equivalent to the value of this instance.</returns>
    [CLSCompliant(false)]
    public sbyte ToSByte() => Convert.ToSByte(Amount);

    /// <summary>Converts the value of this instance to an equivalent 16-bit unsigned integer using the specified
    /// culture-specific formatting information.</summary>
    /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation that supplies
    /// culture-specific formatting information.</param>
    /// <returns>An 16-bit unsigned integer equivalent to the value of this instance.</returns>
    [CLSCompliant(false)]
    public ushort ToUInt16() => Convert.ToUInt16(Amount);

    /// <summary>Converts the value of this instance to an equivalent 32-bit unsigned integer using the specified
    /// culture-specific formatting information.</summary>
    /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation that supplies
    /// culture-specific formatting information.</param>
    /// <returns>An 32-bit unsigned integer equivalent to the value of this instance.</returns>
    [CLSCompliant(false)]
    public uint ToUInt32() => Convert.ToUInt32(Amount);

    /// <summary>Converts the value of this instance to an equivalent 64-bit unsigned integer using the specified
    /// culture-specific formatting information.</summary>
    /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation that supplies
    /// culture-specific formatting information.</param>
    /// <returns>An 64-bit unsigned integer equivalent to the value of this instance.</returns>
    [CLSCompliant(false)]
    public ulong ToUInt64() => Convert.ToUInt64(Amount);
}

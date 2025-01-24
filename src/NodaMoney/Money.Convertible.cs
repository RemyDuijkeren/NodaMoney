namespace NodaMoney;

/// <summary>Represents Money, an amount defined in a specific Currency.</summary>
public partial struct Money
#if !NETSTANDARD1_3
    // : IConvertible //Newtonsoft.Json ignores TypeConverter if IConvertible is implemented https://github.com/JamesNK/Newtonsoft.Json/issues/676
#endif
{
    /// <summary>Performs an explicit conversion from <see cref="Money"/> to <see cref="double"/>.</summary>
    /// <param name="money">The money.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator double(Money money) => Convert.ToDouble(money.Amount);

    /// <summary>Performs an explicit conversion from <see cref="Money"/> to <see cref="long"/>.</summary>
    /// <param name="money">The money.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator long(Money money) => Convert.ToInt64(money.Amount);

    /// <summary>Performs an explicit conversion from <see cref="Money"/> to <see cref="decimal"/>.</summary>
    /// <param name="money">The money.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator decimal(Money money) => money.Amount;

    /// <summary>Performs an explicit conversion from <see cref="long"/> to <see cref="Money"/>.</summary>
    /// <param name="money">The money.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator Money(long money) => new Money(money);

    /// <summary>Performs an explicit conversion from <see cref="ulong"/> to <see cref="Money"/>.</summary>
    /// <param name="money">The money.</param>
    /// <returns>The result of the conversion.</returns>
    [CLSCompliant(false)]
    public static explicit operator Money(ulong money) => new Money(money);

    /// <summary>Performs an explicit conversion from <see cref="byte"/> to <see cref="Money"/>.</summary>
    /// <param name="money">The money.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator Money(byte money) => new Money(money);

    /// <summary>Performs an explicit conversion from <see cref="ushort"/> to <see cref="Money"/>.</summary>
    /// <param name="money">The money.</param>
    /// <returns>The result of the conversion.</returns>
    [CLSCompliant(false)]
    public static explicit operator Money(ushort money) => new Money(money);

    /// <summary>Performs an explicit conversion from <see cref="uint"/> to <see cref="Money"/>.</summary>
    /// <param name="money">The money.</param>
    /// <returns>The result of the conversion.</returns>
    [CLSCompliant(false)]
    public static explicit operator Money(uint money) => new Money(money);

    /// <summary>Performs an implicit conversion from <see cref="double"/> to <see cref="Money"/>.</summary>
    /// <param name="money">The money.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator Money(double money) => new Money((decimal)money);

    /// <summary>Performs an explicit conversion from <see cref="decimal"/> to <see cref="Money"/>.</summary>
    /// <param name="money">The money.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator Money(decimal money) => new Money(money);

    /// <summary>Converts the value of this instance to an <see cref="float"/>.</summary>
    /// <param name="money">A <see cref="Money"/> value.</param>
    /// <returns>The value of the <see cref="Money"/> instance, converted to a <see cref="float"/>.</returns>
    /// <remarks>Because a <see cref="float"/> has fewer significant digits than a <see cref="Money"/> value, this operation may
    /// produce round-off errors. Also, the <see cref="Currency"/> information is lost.</remarks>
    public static float ToSingle(Money money) => Convert.ToSingle(money.Amount);

    /// <summary>Converts the value of this instance to an <see cref="double"/>.</summary>
    /// <param name="money">A <see cref="Money"/> value.</param>
    /// <returns>The value of the current instance, converted to a <see cref="double"/>.</returns>
    /// <remarks>Because a Double has fewer significant digits than a <see cref="Money"/> value, this operation may produce round-off
    /// errors.</remarks>
    public static double ToDouble(Money money) => Convert.ToDouble(money.Amount);

    /// <summary>Converts the value of this instance to an <see cref="decimal"/>.</summary>
    /// <param name="money">A <see cref="Money"/> value.</param>
    /// <returns>The value of the <see cref="Money"/> instance, converted to a <see cref="decimal"/>.</returns>
    /// <remarks>The <see cref="Currency"/> information is lost.</remarks>
    public static decimal ToDecimal(Money money) => money.Amount;

    /// <summary>Converts the value to a <see cref="Money"/> struct, based on the current culture.</summary>
    /// <param name="money">The <see cref="long"/> value on which the returned <see cref="Money"/> should be based.</param>
    /// <returns>The value of the <see cref="long"/> instance, converted to a <see cref="Money"/>.</returns>
    public static Money FromInt64(long money) => (Money)money;

    /// <summary>Converts the value to a <see cref="Money"/> struct, based on the current culture.</summary>
    /// <param name="money">The <see cref="ulong"/> value on which the returned <see cref="Money"/> should be based.</param>
    /// <returns>The value of the <see cref="ulong"/> instance, converted to a <see cref="Money"/>.</returns>
    [CLSCompliant(false)]
    public static Money FromUInt64(ulong money) => (Money)money;

    /// <summary>Converts the value to a <see cref="Money"/> struct, based on the current culture.</summary>
    /// <param name="money">The <see cref="byte"/> value on which the returned <see cref="Money"/> should be based.</param>
    /// <returns>The value of the <see cref="byte"/> instance, converted to a <see cref="Money"/>.</returns>
    public static Money FromByte(long money) => (Money)money;

    /// <summary>Converts the value to a <see cref="Money"/> struct, based on the current culture.</summary>
    /// <param name="money">The <see cref="ushort"/> value on which the returned <see cref="Money"/> should be based.</param>
    /// <returns>The value of the <see cref="ushort"/> instance, converted to a <see cref="Money"/>.</returns>
    [CLSCompliant(false)]
    public static Money FromUInt16(ushort money) => (Money)money;

    /// <summary>Converts the value to a <see cref="Money"/> struct, based on the current culture.</summary>
    /// <param name="money">The <see cref="uint"/> value on which the returned <see cref="Money"/> should be based.</param>
    /// <returns>The value of the <see cref="uint"/> instance, converted to a <see cref="Money"/>.</returns>
    [CLSCompliant(false)]
    public static Money FromUInt32(uint money) => (Money)money;

    /// <summary>Converts the value to a <see cref="Money"/> struct, based on the current culture.</summary>
    /// <param name="money">The <see cref="double"/> value on which the returned <see cref="Money"/> should be based.</param>
    /// <returns>The value of the <see cref="double"/> instance, converted to a <see cref="Money"/>.</returns>
    public static Money FromDouble(double money) => (Money)money;

    /// <summary>Converts the value to a <see cref="Money"/> struct, based on the current culture.</summary>
    /// <param name="money">The <see cref="decimal"/> value on which the returned <see cref="Money"/> should be based.</param>
    /// <returns>The value of the <see cref="decimal"/> instance, converted to a <see cref="Money"/>.</returns>
    public static Money FromDecimal(decimal money) => (Money)money;

#if !NETSTANDARD1_3
#pragma warning disable CA1822 // Mark members as static => Needed for implementation of IConvertible
    /// <summary>Returns the <see cref="TypeCode"/> for this instance.</summary>
    /// <returns>The enumerated constant that is the <see cref="TypeCode"/> of the class or value type that implements this interface.</returns>
    public TypeCode GetTypeCode() => TypeCode.Object;
#pragma warning restore CA1822 // Mark members as static
#endif

    /// <summary>Converts the value of this instance to an equivalent Boolean value using the specified culture-specific
    /// formatting information.</summary>
    /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation that supplies
    /// culture-specific formatting information.</param>
    /// <returns>A Boolean value equivalent to the value of this instance.</returns>
    public bool ToBoolean(IFormatProvider? provider) => Convert.ToBoolean(Amount, provider);

    /// <summary>Converts the value of this instance to an equivalent 8-bit unsigned integer using the specified
    /// culture-specific formatting information.</summary>
    /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation that supplies
    /// culture-specific formatting information.</param>
    /// <returns>An 8-bit unsigned integer equivalent to the value of this instance.</returns>
    public byte ToByte(IFormatProvider? provider) => Convert.ToByte(Amount, provider);

    /// <summary>Converts the value of this instance to an equivalent Unicode character using the specified culture-specific
    /// formatting information.</summary>
    /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation that supplies
    /// culture-specific formatting information.</param>
    /// <returns>A Unicode character equivalent to the value of this instance.</returns>
    public char ToChar(IFormatProvider? provider) => Convert.ToChar(Amount, provider);

    /// <summary>Converts the value of this instance to an equivalent <see cref="DateTime"/> using the specified
    /// culture-specific formatting information.</summary>
    /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation that supplies
    /// culture-specific formatting information.</param>
    /// <returns>A <see cref="DateTime"/> instance equivalent to the value of this instance.</returns>
    public DateTime ToDateTime(IFormatProvider? provider) => Convert.ToDateTime(Amount, provider);

    /// <summary>Converts the value of this instance to an equivalent <see cref="decimal"/> number using the specified
    /// culture-specific formatting information.</summary>
    /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation that supplies
    /// culture-specific formatting information.</param>
    /// <returns>A <see cref="decimal"/> number equivalent to the value of this instance.</returns>
    public decimal ToDecimal(IFormatProvider? provider) => Convert.ToDecimal(Amount, provider);

    /// <summary>Converts the value of this instance to an equivalent double-precision floating-point number using the
    /// specified culture-specific formatting information.</summary>
    /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation that supplies
    /// culture-specific formatting information.</param>
    /// <returns>A double-precision floating-point number equivalent to the value of this instance.</returns>
    public double ToDouble(IFormatProvider? provider) => Convert.ToDouble(Amount, provider);

    /// <summary>Converts the value of this instance to an equivalent 16-bit signed integer using the specified
    /// culture-specific formatting information.</summary>
    /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation that supplies
    /// culture-specific formatting information.</param>
    /// <returns>An 16-bit signed integer equivalent to the value of this instance.</returns>
    public short ToInt16(IFormatProvider? provider) => Convert.ToInt16(Amount, provider);

    /// <summary>Converts the value of this instance to an equivalent 32-bit signed integer using the specified
    /// culture-specific formatting information.</summary>
    /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation that supplies
    /// culture-specific formatting information.</param>
    /// <returns>An 32-bit signed integer equivalent to the value of this instance.</returns>
    public int ToInt32(IFormatProvider? provider) => Convert.ToInt32(Amount, provider);

    /// <summary>Converts the value of this instance to an equivalent 64-bit signed integer using the specified
    /// culture-specific formatting information.</summary>
    /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation that supplies
    /// culture-specific formatting information.</param>
    /// <returns>An 64-bit signed integer equivalent to the value of this instance.</returns>
    public long ToInt64(IFormatProvider? provider) => Convert.ToInt64(Amount, provider);

    /// <summary>Converts the value of this instance to an equivalent 8-bit signed integer using the specified culture-specific
    /// formatting information.</summary>
    /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation that supplies
    /// culture-specific formatting information.</param>
    /// <returns>An 8-bit signed integer equivalent to the value of this instance.</returns>
    [CLSCompliant(false)]
    public sbyte ToSByte(IFormatProvider? provider) => Convert.ToSByte(Amount, provider);

    /// <summary>Converts the value of this instance to an equivalent single-precision floating-point number using the
    /// specified culture-specific formatting information.</summary>
    /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation that supplies
    /// culture-specific formatting information.</param>
    /// <returns>A single-precision floating-point number equivalent to the value of this instance.</returns>
    public float ToSingle(IFormatProvider? provider) => Convert.ToSingle(Amount, provider);

    /// <summary>Converts the value of this instance to an equivalent 16-bit unsigned integer using the specified
    /// culture-specific formatting information.</summary>
    /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation that supplies
    /// culture-specific formatting information.</param>
    /// <returns>An 16-bit unsigned integer equivalent to the value of this instance.</returns>
    [CLSCompliant(false)]
    public ushort ToUInt16(IFormatProvider? provider) => Convert.ToUInt16(Amount, provider);

    /// <summary>Converts the value of this instance to an equivalent 32-bit unsigned integer using the specified
    /// culture-specific formatting information.</summary>
    /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation that supplies
    /// culture-specific formatting information.</param>
    /// <returns>An 32-bit unsigned integer equivalent to the value of this instance.</returns>
    [CLSCompliant(false)]
    public uint ToUInt32(IFormatProvider? provider) => Convert.ToUInt32(Amount, provider);

    /// <summary>Converts the value of this instance to an equivalent 64-bit unsigned integer using the specified
    /// culture-specific formatting information.</summary>
    /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation that supplies
    /// culture-specific formatting information.</param>
    /// <returns>An 64-bit unsigned integer equivalent to the value of this instance.</returns>
    [CLSCompliant(false)]
    public ulong ToUInt64(IFormatProvider? provider) => Convert.ToUInt64(Amount, provider);

    /// <summary>Converts the value of this instance to an <see cref="object"/> of the specified<see cref="Type"/> that has an equivalent value, using the specified culture-specific formatting information.</summary>
    /// <param name="conversionType">The <see cref="Type"/> to which the value of this instance is converted.</param>
    /// <param name="provider">An <see cref="IFormatProvider"/> interface implementation that supplies
    /// culture-specific formatting information.</param>
    /// <returns>An <see cref="object"/> instance of type <paramref name="conversionType"/> whose value is equivalent
    /// to the value of this instance.</returns>
    public object ToType(Type conversionType, IFormatProvider? provider) => Convert.ChangeType(this, conversionType, provider);

    public long ToOACurrency()
    {
        // OACurrency, OLE Automation Currency, supports up to 4 decimals max
        CurrencyInfo currencyInfo = CurrencyInfo.FromCurrency(Currency);
        if (currencyInfo.DecimalDigits > 4)
            throw new InvalidCurrencyException($"The currency '{Currency.Code}' requires more than 4 decimal places, which cannot be represented by OLE Automation Currency.");

        return decimal.ToOACurrency(Amount);
    }

    public static Money FromOACurrency(long oaCurrencyValue) => FromOACurrency(oaCurrencyValue, CurrencyInfo.CurrentCurrency);

    public static Money FromOACurrency(long oaCurrencyValue, Currency currency) => FromOACurrency(oaCurrencyValue, CurrencyInfo.FromCurrency(currency));

    public static Money FromOACurrency(long oaCurrencyValue, CurrencyInfo currencyInfo)
    {
        // OACurrency, OLE Automation Currency, supports up to 4 decimals max
        if (currencyInfo.DecimalDigits > 4)
            throw new InvalidCurrencyException($"The currency '{currencyInfo.Code}' requires more than 4 decimal places, which cannot be represented by OLE Automation Currency.");

        decimal amount = decimal.FromOACurrency(oaCurrencyValue);
        return new Money(amount, currencyInfo);
    }
}

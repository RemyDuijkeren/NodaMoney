namespace NodaMoney;

public partial struct Money
{
    /// <summary>Performs an explicit conversion from <see cref="Money"/> to <see cref="double"/>.</summary>
    /// <param name="money">The instance of <see cref="Money"/> to convert.</param>
    /// <returns>The resulting <see cref="double"/> value.</returns>
    /// <remarks>This operation may produce round-off errors. Also, the <see cref="Currency"/> information is lost.</remarks>
    public static explicit operator double(Money money) => money.ToDouble();

    /// <summary>Performs an explicit conversion from <see cref="Money"/> to <see cref="decimal"/>.</summary>
    /// <param name="money">The instance of <see cref="Money"/> to convert.</param>
    /// <returns>The resulting <see cref="decimal"/> value.</returns>
    /// <remarks>The <see cref="Currency"/> information is lost.</remarks>
    public static explicit operator decimal(Money money) => money.Amount;

    /// <summary>Performs an explicit conversion from <see cref="Money"/> to <see cref="double"/>.</summary>
    /// <param name="money">The instance of <see cref="Money"/> to convert.</param>
    /// <returns>The converted <see cref="double"/> value.</returns>
    /// <remarks>Amount, rounded to the nearest <see cref="long"/>. If Amount is halfway between two whole numbers, the even number is returned;
    /// that is, 4.5 is converted to 4, and 5.5 is converted to 6. Also, the <see cref="Currency"/> information is lost.</remarks>
    /// <exception cref="OverflowException">The value of this instance is outside the range of a <see cref="long"/> value.</exception>
    public static explicit operator long(Money money) => money.ToInt64();

    /// <summary>Performs an explicit conversion from <see cref="long"/> to <see cref="Money"/>.</summary>
    /// <param name="amount">The money amount.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator Money(long amount) => new(amount);

    /// <summary>Performs an explicit conversion from <see cref="double"/> to <see cref="Money"/>.</summary>
    /// <param name="amount">The money amount.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator Money(double amount) => new(amount);

    /// <summary>Performs an explicit conversion from <see cref="decimal"/> to <see cref="Money"/>.</summary>
    /// <param name="amount">The money amount.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator Money(decimal amount) => new(amount);

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
        var rounded = Context.RoundingStrategy.Round(Amount, CurrencyInfo.GetInstance(Currency), 0);
        return checked((long)rounded);
    }

    /// <summary>Converts the value of this instance to minor units.</summary>
    /// <returns>The value of the <see cref="Money"/> instance, converted to minor units (e.g., cents, yen, etc.).</returns>
    /// <exception cref="OverflowException">The value of this instance is outside the range of a <see cref="long"/> value.</exception>
    public long ToMinorUnits()
    {
        var currencyInfo = CurrencyInfo.GetInstance(Currency);
        decimal roundedAmount = Context.RoundingStrategy.Round(Amount, Currency, null);
        return checked((long)(roundedAmount * currencyInfo.ScaleFactor));
    }

    /// <summary>Creates a <see cref="Money"/> instance from minor units.</summary>
    /// <param name="minorUnits">The amount in minor units (e.g., cents, yen, etc.).</param>
    /// <param name="currency">The currency of the money.</param>
    /// <returns>A new <see cref="Money"/> instance.</returns>
    public static Money FromMinorUnits(long minorUnits, Currency currency)
    {
        var currencyInfo = CurrencyInfo.GetInstance(currency);
        decimal amount = (decimal)minorUnits / currencyInfo.ScaleFactor;
        return new Money(amount, currency);
    }
}

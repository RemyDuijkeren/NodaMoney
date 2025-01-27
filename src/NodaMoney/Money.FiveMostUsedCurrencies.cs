namespace NodaMoney;

/// <summary>Represents Money, an amount defined in a specific Currency.</summary>
public partial struct Money
{
    /// <summary>Initializes a new instance of the <see cref="Money"/> structure in euro's.</summary>
    /// <param name="amount">The Amount of money in euro.</param>
    /// <returns>A <see cref="Money"/> structure with EUR as <see cref="Currency"/>.</returns>
    public static Money Euro(decimal amount) => new Money(amount, CurrencyInfo.FromCode("EUR"));

    /// <summary>Initializes a new instance of the <see cref="Money"/> structure in euro's.</summary>
    /// <param name="amount">The Amount of money in euro.</param>
    /// <param name="rounding">The rounding.</param>
    /// <returns>A <see cref="Money"/> structure with EUR as <see cref="Currency"/>.</returns>
    public static Money Euro(decimal amount, MidpointRounding rounding) => new Money(amount, CurrencyInfo.FromCode("EUR"), rounding);

    /// <summary>Initializes a new instance of the <see cref="Money"/> structure in euro's.</summary>
    /// <param name="amount">The Amount of money in euro.</param>
    /// <returns>A <see cref="Money"/> structure with EUR as <see cref="Currency"/>.</returns>
    /// <remarks>This method will first convert to decimal by rounding the value to 15 significant digits using rounding
    /// to nearest. This is done even if the number has more than 15 digits and the less significant digits are zero.
    /// <para>The amount will be rounded to the number of decimal digits of the specified currency
    /// (<see cref="CurrencyInfo.DecimalDigits"/>). As rounding mode, MidpointRounding.ToEven is used
    /// (<see cref="System.MidpointRounding"/>). The behavior of this method follows IEEE Standard 754, section 4. This
    /// kind of rounding is sometimes called rounding to nearest, or banker's rounding. It minimizes rounding errors that
    /// result from consistently rounding a midpoint value in a single direction.</para></remarks>
    public static Money Euro(double amount) => new Money(amount, CurrencyInfo.FromCode("EUR"));

    /// <summary>Initializes a new instance of the <see cref="Money"/> structure in euro's.</summary>
    /// <param name="amount">The Amount of money in euro.</param>
    /// <param name="rounding">The rounding.</param>
    /// <returns>A <see cref="Money"/> structure with EUR as <see cref="Currency"/>.</returns>
    /// <remarks>This method will first convert to decimal by rounding the value to 15 significant digits using rounding
    /// to nearest. This is done even if the number has more than 15 digits and the less significant digits are zero.
    /// <para>The amount will be rounded to the number of decimal digits of the specified currency
    /// (<see cref="CurrencyInfo.DecimalDigits"/>).</para></remarks>
    public static Money Euro(double amount, MidpointRounding rounding) => new Money(amount, CurrencyInfo.FromCode("EUR"), rounding);

    /// <summary>Initializes a new instance of the <see cref="Money"/> structure in euro's.</summary>
    /// <param name="amount">The Amount of money in euro.</param>
    /// <returns>A <see cref="Money"/> structure with EUR as <see cref="Currency"/>.</returns>
    public static Money Euro(long amount) => new Money(amount, CurrencyInfo.FromCode("EUR"));

    /// <summary>Initializes a new instance of the <see cref="Money"/> structure in euro's.</summary>
    /// <param name="amount">The Amount of money in euro.</param>
    /// <returns>A <see cref="Money"/> structure with EUR as <see cref="Currency"/>.</returns>
    [CLSCompliant(false)]
    public static Money Euro(ulong amount) => new Money(amount, CurrencyInfo.FromCode("EUR"));

    /// <summary>Initializes a new instance of the <see cref="Money"/> structure in US dollars.</summary>
    /// <param name="amount">The Amount of money in US dollar.</param>
    /// <returns>A <see cref="Money"/> structure with USD as <see cref="Currency"/>.</returns>
    public static Money USDollar(decimal amount) => new Money(amount, CurrencyInfo.FromCode("USD"));

    /// <summary>Initializes a new instance of the <see cref="Money"/> structure in US dollars.</summary>
    /// <param name="amount">The Amount of money in euro.</param>
    /// <param name="rounding">The rounding.</param>
    /// <returns>A <see cref="Money"/> structure with USD as <see cref="Currency"/>.</returns>
    public static Money USDollar(decimal amount, MidpointRounding rounding) => new Money(amount, CurrencyInfo.FromCode("USD"), rounding);

    /// <summary>Initializes a new instance of the <see cref="Money"/> structure in US dollars.</summary>
    /// <param name="amount">The Amount of money in US dollar.</param>
    /// <returns>A <see cref="Money"/> structure with USD as <see cref="Currency"/>.</returns>
    /// <remarks>This method will first convert to decimal by rounding the value to 15 significant digits using rounding
    /// to nearest. This is done even if the number has more than 15 digits and the less significant digits are zero.
    /// <para>The amount will be rounded to the number of decimal digits of the specified currency
    /// (<see cref="CurrencyInfo.DecimalDigits"/>). As rounding mode, MidpointRounding.ToEven is used
    /// (<see cref="System.MidpointRounding"/>). The behavior of this method follows IEEE Standard 754, section 4. This
    /// kind of rounding is sometimes called rounding to nearest, or banker's rounding. It minimizes rounding errors that
    /// result from consistently rounding a midpoint value in a single direction.</para></remarks>
    public static Money USDollar(double amount) => new Money(amount, CurrencyInfo.FromCode("USD"));

    /// <summary>Initializes a new instance of the <see cref="Money"/> structure in US dollars.</summary>
    /// <param name="amount">The Amount of money in US dollar.</param>
    /// <param name="rounding">The rounding.</param>
    /// <returns>A <see cref="Money"/> structure with USD as <see cref="Currency"/>.</returns>
    /// <remarks>This method will first convert to decimal by rounding the value to 15 significant digits using rounding
    /// to nearest. This is done even if the number has more than 15 digits and the less significant digits are zero.
    /// <para>The amount will be rounded to the number of decimal digits of the specified currency
    /// (<see cref="CurrencyInfo.DecimalDigits"/>).</para></remarks>
    public static Money USDollar(double amount, MidpointRounding rounding) => new Money(amount, CurrencyInfo.FromCode("USD"), rounding);

    /// <summary>Initializes a new instance of the <see cref="Money"/> structure in US dollars.</summary>
    /// <param name="amount">The Amount of money in US dollar.</param>
    /// <returns>A <see cref="Money"/> structure with USD as <see cref="Currency"/>.</returns>
    public static Money USDollar(long amount) => new Money(amount, CurrencyInfo.FromCode("USD"));

    /// <summary>Initializes a new instance of the <see cref="Money"/> structure in US dollars.</summary>
    /// <param name="amount">The Amount of money in US dollar.</param>
    /// <returns>A <see cref="Money"/> structure with USD as <see cref="Currency"/>.</returns>
    [CLSCompliant(false)]
    public static Money USDollar(ulong amount) => new Money(amount, CurrencyInfo.FromCode("USD"));

    /// <summary>Initializes a new instance of the <see cref="Money"/> structure in Japanese Yens.</summary>
    /// <param name="amount">The Amount of money in Japanese Yen.</param>
    /// <returns>A <see cref="Money"/> structure with JPY as <see cref="Currency"/>.</returns>
    public static Money Yen(decimal amount) => new Money(amount, CurrencyInfo.FromCode("JPY"));

    /// <summary>Initializes a new instance of the <see cref="Money"/> structure in Japanese Yens.</summary>
    /// <param name="amount">The Amount of money in Japanese Yens.</param>
    /// <param name="rounding">The rounding.</param>
    /// <returns>A <see cref="Money"/> structure with JPY as <see cref="Currency"/>.</returns>
    public static Money Yen(decimal amount, MidpointRounding rounding) => new Money(amount, CurrencyInfo.FromCode("JPY"), rounding);

    /// <summary>Initializes a new instance of the <see cref="Money"/> structure in Japanese Yens.</summary>
    /// <param name="amount">The Amount of money in Japanese Yen.</param>
    /// <returns>A <see cref="Money"/> structure with JPY as <see cref="Currency"/>.</returns>
    /// <remarks>This method will first convert to decimal by rounding the value to 15 significant digits using rounding
    /// to nearest. This is done even if the number has more than 15 digits and the less significant digits are zero.
    /// <para>The amount will be rounded to the number of decimal digits of the specified currency
    /// (<see cref="CurrencyInfo.DecimalDigits"/>). As rounding mode, MidpointRounding.ToEven is used
    /// (<see cref="System.MidpointRounding"/>). The behavior of this method follows IEEE Standard 754, section 4. This
    /// kind of rounding is sometimes called rounding to nearest, or banker's rounding. It minimizes rounding errors that
    /// result from consistently rounding a midpoint value in a single direction.</para></remarks>
    public static Money Yen(double amount) => new Money(amount, CurrencyInfo.FromCode("JPY"));

    /// <summary>Initializes a new instance of the <see cref="Money"/> structure in Japanese Yens.</summary>
    /// <param name="amount">The Amount of money in Japanese Yen.</param>
    /// <param name="rounding">The rounding.</param>
    /// <returns>A <see cref="Money"/> structure with JPY as <see cref="Currency"/>.</returns>
    /// <remarks>This method will first convert to decimal by rounding the value to 15 significant digits using rounding
    /// to nearest. This is done even if the number has more than 15 digits and the less significant digits are zero.
    /// <para>The amount will be rounded to the number of decimal digits of the specified currency
    /// (<see cref="CurrencyInfo.DecimalDigits"/>).</para></remarks>
    public static Money Yen(double amount, MidpointRounding rounding) => new Money(amount, CurrencyInfo.FromCode("JPY"), rounding);

    /// <summary>Initializes a new instance of the <see cref="Money"/> structure in Japanese Yens.</summary>
    /// <param name="amount">The Amount of money in Japanese Yen.</param>
    /// <returns>A <see cref="Money"/> structure with JPY as <see cref="Currency"/>.</returns>
    public static Money Yen(long amount) => new Money(amount, CurrencyInfo.FromCode("JPY"));

    /// <summary>Initializes a new instance of the <see cref="Money"/> structure in Japanese Yens.</summary>
    /// <param name="amount">The Amount of money in Japanese Yen.</param>
    /// <returns>A <see cref="Money"/> structure with JPY as <see cref="Currency"/>.</returns>
    [CLSCompliant(false)]
    public static Money Yen(ulong amount) => new Money(amount, CurrencyInfo.FromCode("JPY"));

    /// <summary>Initializes a new instance of the <see cref="Money"/> structure in British pounds.</summary>
    /// <param name="amount">The Amount of money in Pound Sterling.</param>
    /// <returns>A <see cref="Money"/> structure with GBP as <see cref="Currency"/>.</returns>
    public static Money PoundSterling(decimal amount) => new Money(amount, CurrencyInfo.FromCode("GBP"));

    /// <summary>Initializes a new instance of the <see cref="Money"/> structure in British pounds.</summary>
    /// <param name="amount">The Amount of money in euro.</param>
    /// <param name="rounding">The rounding.</param>
    /// <returns>A <see cref="Money"/> structure with GBP as <see cref="Currency"/>.</returns>
    public static Money PoundSterling(decimal amount, MidpointRounding rounding) => new Money(amount, CurrencyInfo.FromCode("GBP"), rounding);

    /// <summary>Initializes a new instance of the <see cref="Money"/> structure in British pounds.</summary>
    /// <param name="amount">The Amount of money in Pound Sterling.</param>
    /// <returns>A <see cref="Money"/> structure with GBP as <see cref="Currency"/>.</returns>
    /// <remarks>This method will first convert to decimal by rounding the value to 15 significant digits using rounding
    /// to nearest. This is done even if the number has more than 15 digits and the less significant digits are zero.
    /// <para>The amount will be rounded to the number of decimal digits of the specified currency
    /// (<see cref="CurrencyInfo.DecimalDigits"/>). As rounding mode, MidpointRounding.ToEven is used
    /// (<see cref="System.MidpointRounding"/>). The behavior of this method follows IEEE Standard 754, section 4. This
    /// kind of rounding is sometimes called rounding to nearest, or banker's rounding. It minimizes rounding errors that
    /// result from consistently rounding a midpoint value in a single direction.</para></remarks>
    public static Money PoundSterling(double amount) => new Money(amount, CurrencyInfo.FromCode("GBP"));

    /// <summary>Initializes a new instance of the <see cref="Money"/> structure in British pounds.</summary>
    /// <param name="amount">The Amount of money in Pound Sterling.</param>
    /// <param name="rounding">The rounding.</param>
    /// <returns>A <see cref="Money"/> structure with GBP as <see cref="Currency"/>.</returns>
    /// <remarks>This method will first convert to decimal by rounding the value to 15 significant digits using rounding
    /// to nearest. This is done even if the number has more than 15 digits and the less significant digits are zero.
    /// <para>The amount will be rounded to the number of decimal digits of the specified currency
    /// (<see cref="CurrencyInfo.DecimalDigits"/>).</para></remarks>
    public static Money PoundSterling(double amount, MidpointRounding rounding) => new Money(amount, CurrencyInfo.FromCode("GBP"), rounding);

    /// <summary>Initializes a new instance of the <see cref="Money"/> structure in British pounds.</summary>
    /// <param name="amount">The Amount of money in Pound Sterling.</param>
    /// <returns>A <see cref="Money"/> structure with GBP as <see cref="Currency"/>.</returns>
    public static Money PoundSterling(long amount) => new Money(amount, CurrencyInfo.FromCode("GBP"));

    /// <summary>Initializes a new instance of the <see cref="Money"/> structure in British pounds.</summary>
    /// <param name="amount">The Amount of money in Pound Sterling.</param>
    /// <returns>A <see cref="Money"/> structure with GBP as <see cref="Currency"/>.</returns>
    [CLSCompliant(false)]
    public static Money PoundSterling(ulong amount) => new Money(amount, CurrencyInfo.FromCode("GBP"));

    /// <summary>Initializes a new instance of the <see cref="Money"/> structure in Chinese Yuan.</summary>
    /// <param name="amount">The Amount of money in Chinese Yuan.</param>
    /// <returns>A <see cref="Money"/> structure with CNY as <see cref="Currency"/>.</returns>
    public static Money Yuan(decimal amount) => new Money(amount, CurrencyInfo.FromCode("CNY"));

    /// <summary>Initializes a new instance of the <see cref="Money"/> structure in Chinese Yuan.</summary>
    /// <param name="amount">The Amount of money in Chinese Yuan.</param>
    /// <param name="rounding">The rounding.</param>
    /// <returns>A <see cref="Money"/> structure with CNY as <see cref="Currency"/>.</returns>
    public static Money Yuan(decimal amount, MidpointRounding rounding) => new Money(amount, CurrencyInfo.FromCode("CNY"), rounding);

    /// <summary>Initializes a new instance of the <see cref="Money"/> structure in Chinese Yuan.</summary>
    /// <param name="amount">The Amount of money in Chinese Yuan.</param>
    /// <returns>A <see cref="Money"/> structure with CNY as <see cref="Currency"/>.</returns>
    /// <remarks>This method will first convert to decimal by rounding the value to 15 significant digits using rounding
    /// to nearest. This is done even if the number has more than 15 digits and the less significant digits are zero.
    /// <para>The amount will be rounded to the number of decimal digits of the specified currency
    /// (<see cref="CurrencyInfo.DecimalDigits"/>). As rounding mode, MidpointRounding.ToEven is used
    /// (<see cref="System.MidpointRounding"/>). The behavior of this method follows IEEE Standard 754, section 4. This
    /// kind of rounding is sometimes called rounding to nearest, or banker's rounding. It minimizes rounding errors that
    /// result from consistently rounding a midpoint value in a single direction.</para></remarks>
    public static Money Yuan(double amount) => new Money(amount, CurrencyInfo.FromCode("CNY"));

    /// <summary>Initializes a new instance of the <see cref="Money"/> structure in Chinese Yuan.</summary>
    /// <param name="amount">The Amount of money in Chinese Yuan.</param>
    /// <param name="rounding">The rounding.</param>
    /// <returns>A <see cref="Money"/> structure with CNY as <see cref="Currency"/>.</returns>
    /// <remarks>This method will first convert to decimal by rounding the value to 15 significant digits using rounding
    /// to nearest. This is done even if the number has more than 15 digits and the less significant digits are zero.
    /// <para>The amount will be rounded to the number of decimal digits of the specified currency
    /// (<see cref="CurrencyInfo.DecimalDigits"/>).</para></remarks>
    public static Money Yuan(double amount, MidpointRounding rounding) => new Money(amount, CurrencyInfo.FromCode("CNY"), rounding);

    /// <summary>Initializes a new instance of the <see cref="Money"/> structure in Chinese Yuan.</summary>
    /// <param name="amount">The Amount of money in Chinese Yuan.</param>
    /// <returns>A <see cref="Money"/> structure with CNY as <see cref="Currency"/>.</returns>
    public static Money Yuan(long amount) => new Money(amount, CurrencyInfo.FromCode("CNY"));

    /// <summary>Initializes a new instance of the <see cref="Money"/> structure in Chinese Yuan.</summary>
    /// <param name="amount">The Amount of money in Chinese Yuan.</param>
    /// <returns>A <see cref="Money"/> structure with CNY as <see cref="Currency"/>.</returns>
    [CLSCompliant(false)]
    public static Money Yuan(ulong amount) => new Money(amount, CurrencyInfo.FromCode("CNY"));
}

using NodaMoney.Context;

namespace NodaMoney;

public readonly partial struct Money
{
    /// <summary>Initializes a new instance of the <see cref="Money"/> struct, based on the current culture.</summary>
    /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
    /// <remarks>The amount will be rounded to the number of decimals for the specified currency
    /// (<see cref="NodaMoney.CurrencyInfo.DecimalDigits"/>). As rounding mode, MidpointRounding.ToEven is used
    /// (<see cref="System.MidpointRounding"/>). The behavior of this method follows IEEE Standard 754, section 4. This
    /// kind of rounding is sometimes called rounding to nearest, or banker's rounding. It minimizes rounding errors that
    /// result from consistently rounding a midpoint value in a single direction.</remarks>
    public Money(decimal amount) : this(amount, MoneyContext.CurrentContext.DefaultCurrency ?? CurrencyInfo.CurrentCurrency) { }

    /// <summary>Initializes a new instance of the <see cref="Money"/> struct, based on an ISO 4217 Currency code.</summary>
    /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
    /// <param name="code">An ISO 4217 Currency code, like EUR or USD.</param>
    /// <remarks>The amount will be rounded to the number of decimals for the specified currency
    /// (<see cref="NodaMoney.CurrencyInfo.DecimalDigits"/>). As rounding mode, MidpointRounding.ToEven is used
    /// (<see cref="System.MidpointRounding"/>). The behavior of this method follows IEEE Standard 754, section 4. This
    /// kind of rounding is sometimes called rounding to nearest, or banker's rounding. It minimizes rounding errors that
    /// result from consistently rounding a midpoint value in a single direction.</remarks>
    public Money(decimal amount, string code) : this(amount, CurrencyInfo.FromCode(code)) { }

    /// <summary>Initializes a new instance of the <see cref="Money"/> struct, based on the current culture.</summary>
    /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
    /// <param name="mode">One of the enumeration values that specify which rounding strategy to use.</param>
    /// <remarks>The amount will be rounded to the number of decimals for the specified currency
    /// (<see cref="NodaMoney.CurrencyInfo.DecimalDigits"/>).</remarks>
    [Obsolete("Use the Money(decimal amount, MoneyContext context) constructor instead.", false)]
    public Money(decimal amount, MidpointRounding mode) :
        this(amount, MoneyContext.CurrentContext.DefaultCurrency ?? CurrencyInfo.CurrentCurrency, MoneyContext.Create(mode)) { }

    /// <summary>Initializes a new instance of the <see cref="Money"/> struct, based on an ISO 4217 Currency code.</summary>
    /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
    /// <param name="code">An ISO 4217 Currency code, like EUR or USD.</param>
    /// <param name="mode">One of the enumeration values that specify which rounding strategy to use.</param>
    /// <remarks>The amount will be rounded to the number of decimals for the specified currency (<see cref="NodaMoney.CurrencyInfo.DecimalDigits"/>).</remarks>
    [Obsolete("Use the Money(decimal amount, string code, MoneyContext context) constructor instead.", false)]
    public Money(decimal amount, string code, MidpointRounding mode) : this(amount, CurrencyInfo.FromCode(code), MoneyContext.Create(mode)) { }

    /// <summary>Initializes a new instance of the <see cref="Money"/> struct, based on an ISO 4217 Currency code.</summary>
    /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
    /// <param name="code">An ISO 4217 Currency code, like EUR or USD.</param>
    /// <param name="context">The <see cref="Context"/> to apply to this instance.</param>
    public Money(decimal amount, string code, MoneyContext context) : this(amount, CurrencyInfo.FromCode(code), context) { }

    /// <summary>Initializes a new instance of the <see cref="Money"/> struct.</summary>
    /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
    /// <param name="currency">The Currency of the money.</param>
    /// <param name="mode">One of the enumeration values that specify which rounding strategy to use.</param>
    /// <remarks>The amount will be rounded to the number of decimals for the specified currency
    /// (<see cref="NodaMoney.CurrencyInfo.DecimalDigits"/>).</remarks>
    public Money(decimal amount, Currency currency, MidpointRounding mode) : this(amount, currency, MoneyContext.Create(mode)) { }

    // int, uint ([CLSCompliant(false)]) // auto-casting to decimal so no need for explicit constructors

    /// <summary>Initializes a new instance of the <see cref="Money"/> struct, based on the current culture.</summary>
    /// <param name="amount">The Amount of money as <see langword="double"/> or <see langword="float"/> (float is implicitly
    /// cast to double).</param>
    /// <remarks>This constructor will first convert to decimal by rounding the value to 15 significant digits using rounding
    /// to nearest. This is done even if the number has more than 15 digits and the less significant digits are zero.
    /// <para>The amount will be rounded to the number of decimals for the specified currency
    /// (<see cref="NodaMoney.CurrencyInfo.DecimalDigits"/>). As rounding mode, MidpointRounding.ToEven is used
    /// (<see cref="System.MidpointRounding"/>). The behavior of this method follows IEEE Standard 754, section 4. This
    /// kind of rounding is sometimes called rounding to nearest, or banker's rounding. It minimizes rounding errors that
    /// result from consistently rounding a midpoint value in a single direction.</para></remarks>
    public Money(double amount) : this((decimal)amount) { }

    /// <summary>Initializes a new instance of the <see cref="Money"/> struct, based on an ISO 4217 Currency code.</summary>
    /// <param name="amount">The Amount of money as <see langword="double"/> or <see langword="float"/> (float is implicitly
    /// cast to double).</param>
    /// <param name="code">An ISO 4217 Currency code, like EUR or USD.</param>
    /// <remarks>This constructor will first convert to decimal by rounding the value to 15 significant digits using rounding
    /// to nearest. This is done even if the number has more than 15 digits and the less significant digits are zero.
    /// <para>The amount will be rounded to the number of decimals for the specified currency
    /// (<see cref="NodaMoney.CurrencyInfo.DecimalDigits"/>). As rounding mode, MidpointRounding.ToEven is used
    /// (<see cref="System.MidpointRounding"/>). The behavior of this method follows IEEE Standard 754, section 4. This
    /// kind of rounding is sometimes called rounding to nearest, or banker's rounding. It minimizes rounding errors that
    /// result from consistently rounding a midpoint value in a single direction.</para></remarks>
    public Money(double amount, string code) : this((decimal)amount, CurrencyInfo.FromCode(code)) { }

    /// <summary>Initializes a new instance of the <see cref="Money"/> struct.</summary>
    /// <param name="amount">The Amount of money as <see langword="double"/> or <see langword="float"/> (float is implicitly
    /// cast to double).</param>
    /// <param name="currency">The Currency of the money.</param>
    /// <remarks>This constructor will first convert to decimal by rounding the value to 15 significant digits using rounding
    /// to nearest. This is done even if the number has more than 15 digits and the less significant digits are zero.
    /// <para>The amount will be rounded to the number of decimals for the specified currency
    /// (<see cref="NodaMoney.CurrencyInfo.DecimalDigits"/>). As rounding mode, MidpointRounding.ToEven is used
    /// (<see cref="System.MidpointRounding"/>). The behavior of this method follows IEEE Standard 754, section 4. This
    /// kind of rounding is sometimes called rounding to nearest, or banker's rounding. It minimizes rounding errors that
    /// result from consistently rounding a midpoint value in a single direction.</para></remarks>
    public Money(double amount, Currency currency) : this((decimal)amount, currency) { }

    /// <summary>Initializes a new instance of the <see cref="Money"/> struct, based on the current culture.</summary>
    /// <param name="amount">The Amount of money as <see langword="long"/>, <see langword="int"/>, <see langword="short"/> or<see cref="byte"/>.</param>
    /// <remarks>The integral types are implicitly converted to long and the result evaluates to decimal. Therefore, you can
    /// initialize a Money object using an integer literal, without the suffix, as follows:
    /// <c>Money money = new Money(10, "EUR");</c></remarks>
    public Money(long amount) : this((decimal)amount) { }

    /// <summary>Initializes a new instance of the <see cref="Money"/> struct, based on an ISO 4217 Currency code.</summary>
    /// <param name="amount">The Amount of money as <see langword="long"/>, <see langword="int"/>, <see langword="short"/> or<see cref="byte"/>.</param>
    /// <param name="code">An ISO 4217 Currency code, like EUR or USD.</param>
    /// <remarks>The integral types are implicitly converted to long and the result evaluates to decimal. Therefore, you can
    /// initialize a Money object using an integer literal, without the suffix, as follows:
    /// <c>Money money = new Money(10, "EUR");</c></remarks>
    public Money(long amount, string code) : this((decimal)amount, CurrencyInfo.FromCode(code)) { }

    /// <summary>Initializes a new instance of the <see cref="Money"/> struct.</summary>
    /// <param name="amount">The Amount of money as <see langword="long"/>, <see langword="int"/>, <see langword="short"/> or<see cref="byte"/>.</param>
    /// <param name="currency">The Currency of the money.</param>
    /// <remarks>The integral types are implicitly converted to long and the result evaluates to decimal. Therefore, you can
    /// initialize a Money object using an integer literal, without the suffix, as follows:
    /// <c>Money money = new Money(10, "EUR");</c></remarks>
    public Money(long amount, Currency currency) : this((decimal)amount, currency) { }

    /// <summary>Initializes a new instance of the <see cref="Money"/> struct, based on the current culture.</summary>
    /// <param name="amount">The Amount of money as <see langword="ulong"/>, <see langword="uint"/>, <see langword="ushort"/>
    /// or <see cref="byte"/>.</param>
    /// <remarks>The integral types are implicitly converted to long and the result evaluates to decimal. Therefore, you can
    /// initialize a Money object using an integer literal, without the suffix, as follows:
    /// <c>Money money = new Money(10, "EUR");</c></remarks>
    [CLSCompliant(false)]
    public Money(ulong amount) : this((decimal)amount) { }

    /// <summary>Initializes a new instance of the <see cref="Money"/> struct, based on an ISO 4217 Currency code.</summary>
    /// <param name="amount">The Amount of money as <see langword="ulong"/>, <see langword="uint"/>, <see langword="ushort"/>
    /// or <see cref="byte"/>.</param>
    /// <param name="code">An ISO 4217 Currency code, like EUR or USD.</param>
    /// <remarks>The integral types are implicitly converted to long and the result evaluates to decimal. Therefore, you can
    /// initialize a Money object using an integer literal, without the suffix, as follows:
    /// <c>Money money = new Money(10, "EUR");</c></remarks>
    [CLSCompliant(false)]
    public Money(ulong amount, string code) : this((decimal)amount, CurrencyInfo.FromCode(code)) { }

    /// <summary>Initializes a new instance of the <see cref="Money"/> struct.</summary>
    /// <param name="amount">The Amount of money as <see langword="ulong"/>, <see langword="uint"/>, <see langword="ushort"/>
    /// or <see cref="byte"/>.</param>
    /// <param name="currency">The Currency of the money.</param>
    /// <remarks>The integral types are implicitly converted to long and the result evaluates to decimal. Therefore, you can
    /// initialize a Money object using an integer literal, without the suffix, as follows:
    /// <c>Money money = new Money(10, "EUR");</c></remarks>
    [CLSCompliant(false)]
    public Money(ulong amount, Currency currency) : this((decimal)amount, currency) { }
}

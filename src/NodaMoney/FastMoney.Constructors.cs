using NodaMoney.Context;

namespace NodaMoney;

public readonly partial record struct FastMoney
{
    /// <summary>Initializes a new instance of the <see cref="FastMoney"/> struct based on the provided <see cref="Money"/> instance.</summary>
    /// <param name="money">An instance of <see cref="Money"/> containing the amount and currency to initialize the <see cref="FastMoney"/> struct.</param>
    /// <remarks>The <see cref="FastMoney"/> struct is optimized for performance and memory usage by using 64 bits (8 bytes) for representation,
    /// in contrast to the 128 bits (16 bytes) used by the <see cref="decimal"/> type. This struct maintains compatibility with the <see cref="Money"/> type.</remarks>
    public FastMoney(Money money) : this(money.Amount, money.Currency) { }

    /// <summary>Initializes a new instance of the <see cref="FastMoney"/> struct, based on the current culture.</summary>
    /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
    /// <remarks>The amount will be rounded to the number of decimals for the specified currency
    /// (<see cref="NodaMoney.CurrencyInfo.DecimalDigits"/>). As rounding mode, MidpointRounding.ToEven is used
    /// (<see cref="System.MidpointRounding"/>). The behavior of this method follows IEEE Standard 754, section 4. This
    /// kind of rounding is sometimes called rounding to nearest, or banker's rounding. It minimizes rounding errors that
    /// result from consistently rounding a midpoint value in a single direction.</remarks>
    public FastMoney(decimal amount) : this(amount, MoneyContext.CurrentContext.DefaultCurrency ?? CurrencyInfo.CurrentCurrency) { }

    /// <summary>Initializes a new instance of the <see cref="FastMoney"/> struct, based on an ISO 4217 Currency code.</summary>
    /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
    /// <param name="code">An ISO 4217 Currency code, like EUR or USD.</param>
    /// <remarks>The amount will be rounded to the number of decimals for the specified currency
    /// (<see cref="NodaMoney.CurrencyInfo.DecimalDigits"/>). As rounding mode, MidpointRounding.ToEven is used
    /// (<see cref="System.MidpointRounding"/>). The behavior of this method follows IEEE Standard 754, section 4. This
    /// kind of rounding is sometimes called rounding to nearest, or banker's rounding. It minimizes rounding errors that
    /// result from consistently rounding a midpoint value in a single direction.</remarks>
    public FastMoney(decimal amount, string code) : this(amount, CurrencyInfo.FromCode(code)) { }

    /// <summary>Initializes a new instance of the <see cref="FastMoney"/> struct, based on an ISO 4217 Currency code.</summary>
    /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
    /// <param name="code">An ISO 4217 Currency code, like EUR or USD.</param>
    /// <param name="context">The <see cref="MoneyContext"/> to apply to this instance.</param>
    public FastMoney(decimal amount, string code, MoneyContext context) : this(amount, CurrencyInfo.FromCode(code), context) { }

    /// <summary>Initializes a new instance of the <see cref="FastMoney"/> struct.</summary>
    /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
    /// <param name="currency">The Currency of the money.</param>
    /// <param name="context">The <see cref="MoneyContext"/> to apply to this instance. If <value>null</value> the
    /// current <see cref="MoneyContext"/> will be used.</param>
    public FastMoney(decimal amount, Currency currency, MoneyContext? context = null) : this(amount, CurrencyInfo.GetInstance(currency), context) { }
    public FastMoney(double amount) : this((decimal)amount) { }
    public FastMoney(double amount, Currency currency) : this((decimal)amount, currency) { }
    public FastMoney(double amount, string code) : this((decimal)amount, CurrencyInfo.FromCode(code)) { }
    public FastMoney(long amount) : this((decimal)amount) { }
    public FastMoney(long amount, Currency currency) : this((decimal)amount, currency) { }
    public FastMoney(long amount, string code) : this((decimal)amount, CurrencyInfo.FromCode(code)) { }

    [CLSCompliant(false)]
    public FastMoney(ulong amount) : this((decimal)amount) { }

    [CLSCompliant(false)]
    public FastMoney(ulong amount, Currency currency) : this((decimal)amount, currency) { }

    [CLSCompliant(false)]
    public FastMoney(ulong amount, string code) : this((decimal)amount, CurrencyInfo.FromCode(code)) { }
}

using NodaMoney.Context;

namespace NodaMoney;

public readonly partial record struct FastMoney
{
    /// <summary>Initializes a new instance of the <see cref="FastMoney"/> struct based on the provided <see cref="Money"/> instance.</summary>
    /// <param name="money">An instance of <see cref="Money"/> containing the amount and currency to initialize the <see cref="FastMoney"/> struct.</param>
    /// <remarks>The <see cref="FastMoney"/> struct is optimized for performance and memory usage by using 64 bits (8 bytes) for representation,
    /// in contrast to the 128 bits (16 bytes) used by the <see cref="decimal"/> type. This struct maintains compatibility with the <see cref="Money"/> type.</remarks>
    public FastMoney(Money money) : this()
    {
        // Rescale the Money mantissa to ticks directly when it fits and store the fields without the copy a factory
        // would cost; the decimal path handles the rest, including the range error for amounts outside the FastMoney range.
        Currency currency = money.Currency;
        if (money.TryGetOACurrencyTicks(out long ticks))
        {
            ValidateCurrency(currency);
            OACurrencyAmount = ticks;
            ContextIndex = MoneyContext.FastMoney.Index; // the library's own context, no validation needed
            _currency = currency;
        }
        else
        {
            this = new FastMoney(money.Amount, currency);
        }
    }

    /// <summary>Initializes a new instance of the <see cref="FastMoney"/> struct, based on the current culture.</summary>
    /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
    /// <remarks>The amount will be rounded to the number of decimals for the specified currency
    /// (<see cref="NodaMoney.CurrencyInfo.DecimalDigits"/>). As rounding mode, MidpointRounding.ToEven is used
    /// (<see cref="System.MidpointRounding"/>). The behavior of this method follows IEEE Standard 754, section 4. This
    /// kind of rounding is sometimes called rounding to nearest, or banker's rounding. It minimizes rounding errors that
    /// result from consistently rounding a midpoint value in a single direction.</remarks>
    public FastMoney(decimal amount) : this(amount, ResolveDefaultCurrency()) { }

    /// <summary>Reads <see cref="MoneyContext.CurrentContext"/> once and returns its default currency. Unlike
    /// <see cref="Money"/>, <see cref="FastMoney"/> does not use this context as its own context (it falls back to
    /// <see cref="MoneyContext.FastMoney"/> when none is supplied), so only the currency is passed through.</summary>
    private static Currency ResolveDefaultCurrency()
    {
        MoneyContext context = MoneyContext.CurrentContext;
        return context.DefaultCurrency ?? CurrencyInfo.CurrentCurrency;
    }

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

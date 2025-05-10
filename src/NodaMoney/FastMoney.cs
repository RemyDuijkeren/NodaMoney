using System.Diagnostics.CodeAnalysis;
using NodaMoney.Context;

namespace NodaMoney;

// Internal Currency type https://referencesource.microsoft.com/#mscorlib/system/currency.cs

/// <summary>Represents money, an amount defined in a specific <see cref="Currency"/>.</summary>
/// <remarks>
/// <para>The <see cref="FastMoney"/> struct is optimized for performance and memory usage by using 64 bits (8 bytes) for representation,
/// in contrast to the 128 bits (16 bytes) used by the <see cref="Money"/> type. This struct maintains compatibility with the <see cref="Money"/> type.</para>
/// <para>Instead of storing the amount as <see cref="decimal"/> is stored as 64-bit (8-byte) number in an integer format, scaled by 10,000
/// to give a fixed-point number with 15 digits to the left of the decimal point and 4 digits to the right.</para>
/// <para>This representation provides a range of -922,337,203,685,477.5808 to 922,337,203,685,477.5807.</para>
/// <para>The <see cref="FastMoney"/> struct is useful for calculations involving money and for fixed-point calculations in which accuracy is particularly important.
/// See also OLE Automation Currency, SQL Currency type and https://learn.microsoft.com/en-us/office/vba/language/reference/user-interface-help/currency-data-type.</para>
/// </remarks>
internal readonly record struct FastMoney // or CompactMoney? TODO add interface IMoney or IMonetary or IMonetaryAmount? Using the interface will cause boxing!
{
    /// <summary>Stored as an integer scaled by 10,000</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private long OACurrencyAmount { get; init; } // 8 bytes (64 bits) vs decimal 16 bytes (128 bits)

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
    public FastMoney(decimal amount) : this(amount, CurrencyInfo.CurrentCurrency) { }

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
    /// <param name="mode">One of the enumeration values that specify which rounding strategy to use.</param>
    /// <remarks>The amount will be rounded to the number of decimals for the specified currency (<see cref="NodaMoney.CurrencyInfo.DecimalDigits"/>).</remarks>
    public FastMoney(decimal amount, string code, MidpointRounding mode) : this(amount, CurrencyInfo.FromCode(code), mode) { }

    /// <summary>Initializes a new instance of the <see cref="FastMoney"/> struct, based on an ISO 4217 Currency code.</summary>
    /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
    /// <param name="code">An ISO 4217 Currency code, like EUR or USD.</param>
    /// <param name="context">The <see cref="MoneyContext"/> to apply to this instance.</param>
    public FastMoney(decimal amount, string code, MoneyContext context) : this(amount, CurrencyInfo.FromCode(code), context) { }

    /// <summary>Initializes a new instance of the <see cref="FastMoney"/> struct.</summary>
    /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
    /// <param name="currency">The Currency of the money.</param>
    /// <param name="mode">One of the enumeration values that specify which rounding strategy to use.</param>
    /// <remarks>The amount will be rounded to the number of decimals for the specified currency (<see cref="NodaMoney.CurrencyInfo.DecimalDigits"/>).</remarks>
    public FastMoney(decimal amount, Currency currency, MidpointRounding mode)
        : this(amount, currency, MoneyContext.Create(new StandardRounding(mode))) { }

    /// <summary>Initializes a new instance of the <see cref="FastMoney"/> struct.</summary>
    /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
    /// <param name="currency">The Currency of the money.</param>
    /// <param name="context">The <see cref="MoneyContext"/> to apply to this instance. If <value>null</value> the
    /// current <see cref="MoneyContext"/> will be used.</param>
    public FastMoney(decimal amount, Currency currency, MoneyContext? context = null) : this()
    {
        if (amount < MinValue || amount > MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount is outside the allowable range for FastMoney.");
        }

        CurrencyInfo ci = CurrencyInfo.GetInstance(currency);
        if (ci.DecimalDigits > 4)
        {
            throw new ArgumentOutOfRangeException(nameof(currency), "Currency decimal digits is more then 4, which is outside the allowable range for FastMoney.");
        }

        // Use either provided context or the current global/thread-local context.
        var currentContext = context ?? MoneyContext.CurrentContext;

        int index = currentContext.Index;
        amount = currentContext.RoundingStrategy.Round(amount, CurrencyInfo.GetInstance(currency), null);

        // TODO: How to store index?
        OACurrencyAmount = decimal.ToOACurrency(amount);
        Currency = currency;
    }

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

    /// <summary>Gets the amount of money.</summary>
    public decimal Amount => decimal.FromOACurrency(OACurrencyAmount);

    /// <summary>Gets the <see cref="Currency"/> of the money.</summary>
    public Currency Currency { get; }

    public void Deconstruct(out decimal amount, out Currency currency)
    {
        amount = Amount;
        currency = Currency;
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static long ToOACurrency(FastMoney money) => money.OACurrencyAmount;

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static FastMoney FromOACurrency(long cy, Currency currency, MidpointRounding rounding = MidpointRounding.ToEven) =>
        new(decimal.FromOACurrency(cy), currency, rounding);

    public FastMoney Add(FastMoney other)
    {
        EnsureSameCurrency(this, other); // Ensure currencies match
        long totalAmount = checked(OACurrencyAmount + other.OACurrencyAmount); // Use checked for overflow

        return this with { OACurrencyAmount = totalAmount };
    }

    /// <summary>Constants for range</summary>
    public static readonly decimal MinValue = -922_337_203_685_477.5808m;

    public static readonly decimal MaxValue = 922_337_203_685_477.5807m;

    private static void EnsureSameCurrency(in FastMoney left, in FastMoney right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidCurrencyException(left.Currency, right.Currency);
    }
}

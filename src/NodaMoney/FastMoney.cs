using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace NodaMoney;

// Internal Currency type https://referencesource.microsoft.com/#mscorlib/system/currency.cs

/// <summary>Represents a fast money value with a currency unit. Scaled integer (default scale of 4)</summary>
/// <remarks>Size from -9,223,372,036,854,775,808 to 9,223,372,036,854,775,807 of the minor unit (like cents)</remarks>
/// <remarks>https://learn.microsoft.com/en-us/office/vba/language/reference/user-interface-help/currency-data-type
/// Currency variables are stored as 64-bit (8-byte) numbers in an integer format, scaled by 10,000 to give a fixed-point number with 15 digits to the left of the decimal point and 4 digits to the right.
/// This representation provides a range of -922,337,203,685,477.5808 to 922,337,203,685,477.5807.
/// The Currency data type is useful for calculations involving money and for fixed-point calculations in which accuracy is particularly important.
/// See also OLE Automation Currency and SQL Currency type.
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
internal readonly record struct FastMoney // or CompactMoney? TODO add interface IMoney or IMonetary or IMonetaryAmount?
{
    /// <summary>Stored as an integer scaled by 10,000</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "OACurrency is also used by Microsoft and is well know")]
    public long OACurrencyAmount { get; } // 8 bytes (64 bits) vs decimal 16 bytes (128 bits)

    public FastMoney(Money money) : this(money.Amount, money.Currency) { }

    public FastMoney(decimal amount) : this(amount, CurrencyInfo.CurrentCurrency) { }

    /// <summary>Initializes a new instance of the <see cref="Money"/> struct, based on a ISO 4217 Currency code.</summary>
    /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
    /// <param name="code">A ISO 4217 Currency code, like EUR or USD.</param>
    /// <remarks>The amount will be rounded to the number of decimal digits of the specified currency
    /// (<see cref="NodaMoney.CurrencyInfo.DecimalDigits"/>). As rounding mode, MidpointRounding.ToEven is used
    /// (<see cref="System.MidpointRounding"/>). The behavior of this method follows IEEE Standard 754, section 4. This
    /// kind of rounding is sometimes called rounding to nearest, or banker's rounding. It minimizes rounding errors that
    /// result from consistently rounding a midpoint value in a single direction.</remarks>
    public FastMoney(decimal amount, string code) : this(amount, new Currency(code)) { }

    /// <summary>Initializes a new instance of the <see cref="Money"/> struct, based on a ISO 4217 Currency code.</summary>
    /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
    /// <param name="code">A ISO 4217 Currency code, like EUR or USD.</param>
    /// <param name="rounding">The rounding mode.</param>
    /// <remarks>The amount will be rounded to the number of decimal digits of the specified currency
    /// (<see cref="NodaMoney.CurrencyInfo.DecimalDigits"/>).</remarks>
    public FastMoney(decimal amount, string code, MidpointRounding rounding) : this(amount, new Currency(code), rounding) { }

    public FastMoney(decimal amount, Currency currency, MidpointRounding rounding = MidpointRounding.ToEven) : this()
    {
        CheckedEnsureValidRange(amount); // Ensure the decimal is in the valid range.

        // Ensure Currency is withing 4 decimal digits
        CurrencyInfo ci = CurrencyInfo.GetInstance(currency);
        if (ci.DecimalDigits > 4)
        {
            throw new ArgumentException("Currency decimal digits must be lower or equal to 4!", nameof(currency));
        }

        OACurrencyAmount = decimal.ToOACurrency(Money.Round(amount, currency, rounding));
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

    // internal FastMoney(long oaCurrencyAmount, Currency currency)
    // {
    //     // Determine the scaling factor based on the currency's allowed decimal places
    //     CurrencyInfo ci = CurrencyInfo.GetInstance(currency);
    //     int scaleDifference = 4 - ci.DecimalDigits; // Example: For USD (2 decimals), scaleDifference = 2
    //     long scalingFactor = (long)Math.Pow(10, scaleDifference); // 10^scaleDifference
    //
    //     // Round `oaCurrencyAmount` to the number of decimals allowed by the currency
    //     OACurrencyAmount = (oaCurrencyAmount + (scalingFactor / 2)) / scalingFactor * scalingFactor;
    //     Currency = currency;
    // }

    /// <summary>Gets the amount of money.</summary>
    public decimal Amount => decimal.FromOACurrency(OACurrencyAmount);

    /// <summary>Gets the <see cref="Currency"/> of the money.</summary>
    public Currency Currency { get; }

    public void Deconstruct(out decimal amount, out Currency currency)
    {
        amount = Amount;
        currency = Currency;
    }

    // /// <summary>Returns a value indicating whether two instances of <see cref="Money"/> are equal.</summary>
    // /// <param name="left">A <see cref="Money"/> object on the left side.</param>
    // /// <param name="right">A <see cref="Money"/> object on the right side.</param>
    // /// <returns>true if left and right are equal; otherwise, false.</returns>
    // public static bool operator ==(FastMoney left, FastMoney right) => left.Equals(right);
    //
    // /// <summary>Returns a value indicating whether two instances of <see cref="Money"/> are not equal.</summary>
    // /// <param name="left">A <see cref="Money"/> object on the left side.</param>
    // /// <param name="right">A <see cref="Money"/> object on the right side.</param>
    // /// <returns>true if left and right are not equal; otherwise, false.</returns>
    // public static bool operator !=(FastMoney left, FastMoney right) => !(left == right);
    //
    // /// <summary>Returns a value indicating whether this instance and a specified <see cref="Money"/> object represent the same
    // /// value.</summary>
    // /// <param name="other">A <see cref="Money"/> object.</param>
    // /// <returns>true if value is equal to this instance; otherwise, false.</returns>
    // public bool Equals(FastMoney other) => OACurrencyAmount == other.OACurrencyAmount && Currency == other.Currency;
    //
    // /// <summary>Returns a value indicating whether this instance and a specified <see cref="object"/> represent the same type
    // /// and value.</summary>
    // /// <param name="obj">An <see cref="object"/>.</param>
    // /// <returns>true if value is equal to this instance; otherwise, false.</returns>
    // public override bool Equals(object? obj) => obj is FastMoney money && this.Equals(money);
    //
    // /// <summary>Returns the hash code for this instance.</summary>
    // /// <returns>A 32-bit signed integer hash code.</returns>
    // public override int GetHashCode() => HashCode.Combine(OACurrencyAmount, Currency);
    //
    // /// <summary>Deconstructs the current instance into its components.</summary>
    // /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
    // /// <param name="currency">The Currency of the money.</param>
    // public void Deconstruct(out decimal amount, out Currency currency)
    // {
    //     amount = Amount;
    //     currency = Currency;
    // }

    /// <summary>Addition Example</summary>
    /// <param name="other">other</param>
    public FastMoney Add(FastMoney other)
    {
        EnsureSameCurrency(this, other); // Ensure currencies match
        long totalAmount = checked(OACurrencyAmount + other.OACurrencyAmount); // Use checked for overflow

        return new FastMoney(totalAmount, Currency);
    }

    /// <summary>Constants for range</summary>
    public static readonly decimal MinValue = -922_337_203_685_477.5808m;
    public static readonly decimal MaxValue = 922_337_203_685_477.5807m;

    /// <summary>Ensures the numeric range is valid for FastMoney </summary>
    /// <param name="amount"></param>
    private static void CheckedEnsureValidRange(decimal amount)
    {
        if (amount < MinValue || amount > MaxValue)
        {
            throw new OverflowException("The amount is outside the allowable range for FastMoney.");
        }
    }

    private static void EnsureSameCurrency(in FastMoney left, in FastMoney right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidCurrencyException(left.Currency, right.Currency);
    }
}

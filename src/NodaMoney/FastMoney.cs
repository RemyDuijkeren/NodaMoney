using System.Runtime.InteropServices;

namespace NodaMoney;

// TODO: Do we want to implement a Money type based on integer? That has a default scale of 4? Instead of using Decimal type?

/// <summary>Represents a fast money value with a currency unit. Scaled integer</summary>
/// <remarks>Size from -9,223,372,036,854,775,808 to 9,223,372,036,854,775,807 of the minor unit (like cents)</remarks>
/// <remarks>https://learn.microsoft.com/en-us/office/vba/language/reference/user-interface-help/currency-data-type
/// Currency variables are stored as 64-bit (8-byte) numbers in an integer format, scaled by 10,000 to give a fixed-point number with 15 digits to the left of the decimal point and 4 digits to the right.
/// This representation provides a range of -922,337,203,685,477.5808 to 922,337,203,685,477.5807.
/// The type-declaration character for Currency is the at (@) sign.
/// The Currency data type is useful for calculations involving money and for fixed-point calculations in which accuracy is particularly important.
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct FastMoney : IEquatable<FastMoney> // TODO add interface IMoney or IMonetary or IMonetaryAmount?
{
    readonly long _amount;

    /// <summary>Initializes a new instance of the <see cref="Money"/> struct, based on a ISO 4217 Currency code.</summary>
    /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
    /// <param name="code">A ISO 4217 Currency code, like EUR or USD.</param>
    /// <remarks>The amount will be rounded to the number of decimal digits of the specified currency
    /// (<see cref="NodaMoney.Currency.DecimalDigits"/>). As rounding mode, MidpointRounding.ToEven is used
    /// (<see cref="System.MidpointRounding"/>). The behavior of this method follows IEEE Standard 754, section 4. This
    /// kind of rounding is sometimes called rounding to nearest, or banker's rounding. It minimizes rounding errors that
    /// result from consistently rounding a midpoint value in a single direction.</remarks>
    public FastMoney(decimal amount, string code) : this(amount, new Currency(code)) { }

    /// <summary>Initializes a new instance of the <see cref="Money"/> struct, based on a ISO 4217 Currency code.</summary>
    /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
    /// <param name="code">A ISO 4217 Currency code, like EUR or USD.</param>
    /// <param name="rounding">The rounding mode.</param>
    /// <remarks>The amount will be rounded to the number of decimal digits of the specified currency
    /// (<see cref="NodaMoney.Currency.DecimalDigits"/>).</remarks>
    public FastMoney(decimal amount, string code, MidpointRounding rounding) : this(amount, new Currency(code), rounding) { }

    public FastMoney(decimal amount, Currency currency, MidpointRounding rounding = MidpointRounding.ToEven) : this()
    {
        Currency = currency;
        _amount = decimal.ToOACurrency(Money.Round(amount, currency, rounding));
    }

    /// <summary>Gets the amount of money.</summary>
    public decimal Amount => decimal.FromOACurrency(_amount);

    /// <summary>Gets the <see cref="Currency"/> of the money.</summary>
    public Currency Currency { get; }

    /// <summary>Returns a value indicating whether two instances of <see cref="Money"/> are equal.</summary>
    /// <param name="left">A <see cref="Money"/> object on the left side.</param>
    /// <param name="right">A <see cref="Money"/> object on the right side.</param>
    /// <returns>true if left and right are equal; otherwise, false.</returns>
    public static bool operator ==(FastMoney left, FastMoney right) => left.Equals(right);

    /// <summary>Returns a value indicating whether two instances of <see cref="Money"/> are not equal.</summary>
    /// <param name="left">A <see cref="Money"/> object on the left side.</param>
    /// <param name="right">A <see cref="Money"/> object on the right side.</param>
    /// <returns>true if left and right are not equal; otherwise, false.</returns>
    public static bool operator !=(FastMoney left, FastMoney right) => !(left == right);

    /// <summary>Returns a value indicating whether this instance and a specified <see cref="Money"/> object represent the same
    /// value.</summary>
    /// <param name="other">A <see cref="Money"/> object.</param>
    /// <returns>true if value is equal to this instance; otherwise, false.</returns>
    public bool Equals(FastMoney other) => Amount == other.Amount && Currency == other.Currency;

    /// <summary>Returns a value indicating whether this instance and a specified <see cref="object"/> represent the same type
    /// and value.</summary>
    /// <param name="obj">An <see cref="object"/>.</param>
    /// <returns>true if value is equal to this instance; otherwise, false.</returns>
    public override bool Equals(object? obj) => obj is FastMoney money && this.Equals(money);

    /// <summary>Returns the hash code for this instance.</summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = (hash * 23) + Amount.GetHashCode();
            return (hash * 23) + Currency.GetHashCode();
        }
    }

    /// <summary>Deconstructs the current instance into its components.</summary>
    /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
    /// <param name="currency">The Currency of the money.</param>
    public void Deconstruct(out decimal amount, out Currency currency)
    {
        amount = Amount;
        currency = Currency;
    }

    private static void EnsureSameCurrency(in Money left, in Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidCurrencyException(left.Currency, right.Currency);
    }
}

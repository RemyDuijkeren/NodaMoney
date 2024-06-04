using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.InteropServices;

namespace NodaMoney;

/// <summary>Represents a fast money value with a currency unit.</summary>
/// <remarks>Size from -9,223,372,036,854,775,808 to 9,223,372,036,854,775,807 of the minor unit (like cents)</remarks>
[StructLayout(LayoutKind.Sequential)]
public readonly struct FastMoney : IEquatable<FastMoney>
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
    public FastMoney(decimal amount, string code)
        : this(amount, new CurrencyUnit(code,0))
    {
    }

    /// <summary>Initializes a new instance of the <see cref="Money"/> struct.</summary>
    /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
    /// <param name="currency">The Currency of the money.</param>
    /// <remarks>The amount will be rounded to the number of decimal digits of the specified currency
    /// (<see cref="NodaMoney.Currency.DecimalDigits"/>). As rounding mode, MidpointRounding.ToEven is used
    /// (<see cref="System.MidpointRounding"/>). The behavior of this method follows IEEE Standard 754, section 4. This
    /// kind of rounding is sometimes called rounding to nearest, or banker's rounding. It minimizes rounding errors that
    /// result from consistently rounding a midpoint value in a single direction.</remarks>
    public FastMoney(decimal amount, CurrencyUnit currency)
        : this(amount, currency, MidpointRounding.ToEven)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="Money"/> struct, based on a ISO 4217 Currency code.</summary>
    /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
    /// <param name="code">A ISO 4217 Currency code, like EUR or USD.</param>
    /// <param name="rounding">The rounding mode.</param>
    /// <remarks>The amount will be rounded to the number of decimal digits of the specified currency
    /// (<see cref="NodaMoney.Currency.DecimalDigits"/>).</remarks>
    public FastMoney(decimal amount, string code, MidpointRounding rounding)
        : this(amount, new CurrencyUnit(code, 0), rounding)
    {
    }
        
    public FastMoney(decimal amount, CurrencyUnit currency, MidpointRounding rounding)
        : this()
    {
        Currency = currency;
            
        decimal rounded = Round(amount, currency, rounding);
        _amount = (long)(rounded * (decimal)Math.Pow(10, 2));
    }

    /// <summary>Gets the amount of money.</summary>
    public decimal Amount => _amount / (decimal)Math.Pow(10, 2);

    /// <summary>Gets the <see cref="Currency"/> of the money.</summary>
    public CurrencyUnit Currency { get; }

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
    public override bool Equals(object obj) => obj is FastMoney money && this.Equals(money);

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
    public void Deconstruct(out decimal amount, out CurrencyUnit currency)
    {
        amount = Amount;
        currency = Currency;
    }

    private static decimal Round(in decimal amount, in CurrencyUnit currencyUnit, in MidpointRounding rounding)
    {
        // https://stackoverflow.com/questions/43289478/how-can-i-tell-if-a-number-is-a-power-of-10-in-kotlin-or-java
        static bool IsPowerOf10(long n)
        {
            while (n > 9 && n % 10 == 0)
            {
                n /= 10;
            }

            return n == 1;
        }

        // NOT GOOD, IS ONLY FOR INT. EXTEND
        static bool IsPowerOfTen(in double x)
        {
            return x == 1
                   || x == 10
                   || x == 100
                   || x == 1000
                   || x == 10000
                   || x == 100000
                   || x == 1000000
                   || x == 10000000
                   || x == 100000000
                   || x == 1000000000;
        }

        Currency currency = NodaMoney.Currency.FromCode(currencyUnit.Code);
        return IsPowerOfTen(currency.MinorUnit)
            ? Math.Round(amount, currency.DecimalDigits, rounding)
            : Math.Round(amount / currency.MinimalAmount, 0, rounding) * currency.MinimalAmount;
    }

    [SuppressMessage(
        "Microsoft.Globalization",
        "CA1305:SpecifyIFormatProvider",
        MessageId = "System.String.Format(System.String,System.Object[])",
        Justification = "Test fail when Invariant is used. Inline JIT bug? When cloning CultureInfo it works.")]
    private static void AssertIsSameCurrency(in Money left, in Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidCurrencyException(left.Currency, right.Currency);
    }
}
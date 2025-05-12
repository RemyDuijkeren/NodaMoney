using System.Runtime.InteropServices;
using NodaMoney.Context;

namespace NodaMoney;

/// <summary>Represents money, an amount defined in a specific <see cref="Currency"/>.</summary>
/// <remarks>
/// The <see cref="Money"/> structure allows development of applications that handle
/// various types of Currency. Money will hold the <see cref="Currency"/> and Amount of money,
/// and ensure that two different currencies cannot be added or subtracted to each other.
/// </remarks>
public readonly partial struct Money : IEquatable<Money>
{
#pragma warning disable RCS1181
    // Masks for the Flags field
    private const int CurrencyMask = 0b_1111_1111_1111_1111;     // Bits 0–15, for Currency (16 bits)
    private const int ScaleMask = 0x_FF_00_00;                   // Bits 16-23 for the Decimal scale
    private const int IndexMask = 0b_0111_1111 << 24;            // Bits 24–30, for Index (7 bits)
    private const int SignMask = unchecked((int)0x_80_00_00_00); // Bit 31 for the Decimal sign bit (negative)

    // Fields for storing the components of the decimal representation
    private readonly int _low;
    private readonly int _mid;
    private readonly int _high;
    private readonly int _flags;
#pragma warning restore RCS1181

    /// <summary>Initializes a new instance of the <see cref="Money"/> struct, based on the current culture.</summary>
    /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
    /// <remarks>The amount will be rounded to the number of decimals for the specified currency
    /// (<see cref="NodaMoney.CurrencyInfo.DecimalDigits"/>). As rounding mode, MidpointRounding.ToEven is used
    /// (<see cref="System.MidpointRounding"/>). The behavior of this method follows IEEE Standard 754, section 4. This
    /// kind of rounding is sometimes called rounding to nearest, or banker's rounding. It minimizes rounding errors that
    /// result from consistently rounding a midpoint value in a single direction.</remarks>
    public Money(decimal amount) : this(amount, CurrencyInfo.CurrentCurrency) { }

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
    public Money(decimal amount, MidpointRounding mode) : this(amount, CurrencyInfo.CurrentCurrency, mode) { }

    /// <summary>Initializes a new instance of the <see cref="Money"/> struct, based on the current culture.</summary>
    /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
    /// <param name="context">The <see cref="Context"/> to apply to this instance.</param>
    public Money(decimal amount, MoneyContext context) : this(amount, CurrencyInfo.CurrentCurrency, context) { }

    /// <summary>Initializes a new instance of the <see cref="Money"/> struct, based on an ISO 4217 Currency code.</summary>
    /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
    /// <param name="code">An ISO 4217 Currency code, like EUR or USD.</param>
    /// <param name="mode">One of the enumeration values that specify which rounding strategy to use.</param>
    /// <remarks>The amount will be rounded to the number of decimals for the specified currency (<see cref="NodaMoney.CurrencyInfo.DecimalDigits"/>).</remarks>
    public Money(decimal amount, string code, MidpointRounding mode) : this(amount, CurrencyInfo.FromCode(code), mode) { }

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
    public Money(decimal amount, Currency currency, MidpointRounding mode)
        : this(amount, currency, MoneyContext.Create(new StandardRounding(mode))) { }

    /// <summary>Initializes a new instance of the <see cref="Money"/> struct.</summary>
    /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
    /// <param name="currency">The Currency of the money.</param>
    /// <param name="context">The <see cref="Context"/> to apply to this instance. If <value>null</value> the
    /// current <see cref="Context"/> will be used.</param>
    public Money(decimal amount, Currency currency, MoneyContext? context = null)
    {
        // Fast path when the amount is zero (common in financial calculations)
        if (amount == 0m)
        {
            _low = 0;
            _mid = 0;
            _high = 0;

            int index = (context ?? MoneyContext.CurrentContext).Index;
            _flags = (currency.EncodedValue & CurrencyMask)
                     | ((index << 24) & IndexMask);
            return;
        }

        // Use either provided context or the current global/thread-local context.
        MoneyContext currentContext = context ?? MoneyContext.CurrentContext;
        int contextIndex = currentContext.Index;

        // TODO: Inline Common Cases for Rounding?
        amount = currentContext.RoundingStrategy.Round(amount, CurrencyInfo.GetInstance(currency), currentContext.MaxScale);

        // Extract the 4 integers from the decimal amount.
#if NET5_0_OR_GREATER
        Span<int> bits = stackalloc int[4];
        decimal.GetBits(amount, bits);
#else
        int[] bits = decimal.GetBits(amount);
#endif

        _low = bits[0];
        _mid = bits[1];
        _high = bits[2];
        _flags = (currency.EncodedValue & CurrencyMask) // Store Currency in bits 0–15
                 | ((contextIndex << 24) & IndexMask)          // Store Index in bits 24–30
                 | (bits[3] & (ScaleMask | SignMask));  // Preserve Scale Factor (16–23) and Sign (31)
    }

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

    /// <summary>Initializes a new instance of the <see cref="Money"/> struct, based on a ISO 4217 Currency code.</summary>
    /// <param name="amount">The Amount of money as <see langword="double"/> or <see langword="float"/> (float is implicitly
    /// cast to double).</param>
    /// <param name="code">A ISO 4217 Currency code, like EUR or USD.</param>
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

    /// <summary>Initializes a new instance of the <see cref="Money"/> struct.</summary>
    /// <param name="amount">The Amount of money as <see langword="double"/> or <see langword="float"/> (float is implicitly
    /// cast to double).</param>
    /// <param name="currency">The Currency of the money.</param>
    /// <param name="mode">The rounding mode.</param>
    /// <remarks>This constructor will first convert to decimal by rounding the value to 15 significant digits using rounding
    /// to nearest. This is done even if the number has more than 15 digits and the less significant digits are zero.
    /// <para>The amount will be rounded to the number of decimals for the specified currency
    /// (<see cref="NodaMoney.CurrencyInfo.DecimalDigits"/>).</para></remarks>
    public Money(double amount, Currency currency, MidpointRounding mode) : this((decimal)amount, currency, mode) { }

    /// <summary>Initializes a new instance of the <see cref="Money"/> struct.</summary>
    /// <param name="amount">The Amount of money as <see langword="double"/> or <see langword="float"/> (float is implicitly
    /// cast to double).</param>
    /// <param name="currency">The Currency of the money.</param>
    /// <param name="context">The <see cref="Context"/> to apply to this instance. If <value>null</value> the
    /// current <see cref="Context"/> will be used.</param>
    public Money(double amount, Currency currency, MoneyContext context) : this((decimal)amount, currency, context) { }

    /// <summary>Initializes a new instance of the <see cref="Money"/> struct, based on the current culture.</summary>
    /// <param name="amount">The Amount of money as <see langword="long"/>, <see langword="int"/>, <see langword="short"/> or<see cref="byte"/>.</param>
    /// <remarks>The integral types are implicitly converted to long and the result evaluates to decimal. Therefore, you can
    /// initialize a Money object using an integer literal, without the suffix, as follows:
    /// <c>Money money = new Money(10, "EUR");</c></remarks>
    public Money(long amount) : this((decimal)amount) { }

    /// <summary>Initializes a new instance of the <see cref="Money"/> struct, based on a ISO 4217 Currency code.</summary>
    /// <param name="amount">The Amount of money as <see langword="long"/>, <see langword="int"/>, <see langword="short"/> or<see cref="byte"/>.</param>
    /// <param name="code">A ISO 4217 Currency code, like EUR or USD.</param>
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

    /// <summary>Initializes a new instance of the <see cref="Money"/> struct, based on a ISO 4217 Currency code.</summary>
    /// <param name="amount">The Amount of money as <see langword="ulong"/>, <see langword="uint"/>, <see langword="ushort"/>
    /// or <see cref="byte"/>.</param>
    /// <param name="code">A ISO 4217 Currency code, like EUR or USD.</param>
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

    /// <summary>Gets the amount of money.</summary>
    public decimal Amount
    {
        get
        {
            // Extract scale (bits 16-23) and sign (bit 31) from Flags
            byte scale = (byte)((_flags & ScaleMask) >> 16); // Extract scale (shift bits 16-23)
            bool isNegative = (_flags & SignMask) != 0; // Is negative if SignMask bit is set

            // Reconstruct the decimal with the correct `Flags` value (index removed)
            return new decimal(_low, _mid, _high, isNegative, scale);
        }
        init
        {
            // Separate the Decimal bits during initialization
#if NET5_0_OR_GREATER
            Span<int> bits = stackalloc int[4];
            decimal.GetBits(value, bits);
#else
            int[] bits = decimal.GetBits(value);
#endif
            _low = bits[0];
            _mid = bits[1];
            _high = bits[2];

            // Only preserve the Scale and Sign bits during initialization
            _flags = (_flags & ~(ScaleMask | SignMask)) | (bits[3] & (ScaleMask | SignMask));
        }
    }

    /// <summary>Gets the <see cref="Currency"/> of the money.</summary>
    public Currency Currency
    {
        get => new((ushort)(_flags & CurrencyMask)); // Extract Currency (bits 0–15)
        init => _flags = (_flags & ~CurrencyMask) | (value.EncodedValue & CurrencyMask); // Set Currency (bits 0–15)
    }

    /// <summary>Gets the scaling factor of the Money, which is a number from 0 to 28 that represents the number of decimal digits.</summary>
    public byte Scale => (byte)((_flags & ScaleMask) >> 16); // Extract Scale (bits 16-23)

    /// <summary>Gets the context associated with this <see cref="Money"/> instance.</summary>
    public MoneyContext Context => MoneyContext.Get(ContextIndex);

    /// <summary>Gets the index of the <see cref="Context"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the provided index value is outside the allowed range of 0 to 127.</exception>
    internal byte ContextIndex
    {
        get => (byte)((_flags & IndexMask) >> 24); // Extract Index (bits 24–30)
        init
        {
            // Process Index initialization
            if (value > 127) throw new ArgumentOutOfRangeException(nameof(value), "Index must be within 0 to 127.");
            _flags = (_flags & ~IndexMask) | ((value << 24) & IndexMask); // Set Index (bits 24–30)
        }
    }

    /// <summary>Returns a value indicating whether two instances of <see cref="Money"/> are equal.</summary>
    /// <param name="left">A <see cref="Money"/> object on the left side.</param>
    /// <param name="right">A <see cref="Money"/> object on the right side.</param>
    /// <returns>true if left and right are equal; otherwise, false.</returns>
    public static bool operator ==(Money left, Money right) => left.Equals(right);

    /// <summary>Returns a value indicating whether two instances of <see cref="Money"/> are not equal.</summary>
    /// <param name="left">A <see cref="Money"/> object on the left side.</param>
    /// <param name="right">A <see cref="Money"/> object on the right side.</param>
    /// <returns>true if left and right are not equal; otherwise, false.</returns>
    public static bool operator !=(Money left, Money right) => !(left == right);

    /// <summary>Returns a value indicating whether this instance and a specified <see cref="Money"/> object represent the same
    /// value.</summary>
    /// <param name="other">A <see cref="Money"/> object.</param>
    /// <returns>true if value is equal to this instance; otherwise, false.</returns>
    public bool Equals(Money other) => Amount == other.Amount && Currency == other.Currency;

    /// <summary>Returns a value indicating whether this instance and a specified <see cref="object"/> represent the same type
    /// and value.</summary>
    /// <param name="obj">An <see cref="object"/>.</param>
    /// <returns>true if value is equal to this instance; otherwise, false.</returns>
    public override bool Equals(object? obj) => obj is Money money && this.Equals(money);

    /// <summary>Returns the hash code for this instance.</summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
#if NETSTANDARD2_0
    public override int GetHashCode()
    {
        unchecked
        {
            return (Amount.GetHashCode() * 31) + Currency.GetHashCode();
        }
    }
#else
    public override int GetHashCode() => HashCode.Combine(Amount, Currency);
#endif

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

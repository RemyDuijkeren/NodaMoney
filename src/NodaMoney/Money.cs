using System.Diagnostics;
using System.Runtime.CompilerServices;
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
    private const int CurrencyMask = 0b_11111111_11111111; // Bits 0–15, for Currency (16 bits)
    private const int ScaleMask = 0b_11111111_00000000_00000000; // Bits 16-23 for the Decimal scale
    private const int IndexMask = 0b_01111111 << 24; // Bits 24–30, for Index (7 bits)
    private const int SignMask = unchecked((int)0b_10000000_00000000_00000000_00000000); // Bit 31 for the Decimal sign bit (negative)

    // Fields for storing the components of the decimal representation where bit layout in _flags is as follows:
    // bits 00..15: Currency
    // bits 16..23: Scale
    // bits 24..30: ContextIndex
    // bits 31..32: Sign
    private readonly int _flags;
    private readonly uint _high;
    private readonly uint _mid;
    private readonly uint _low;
#pragma warning restore RCS1181

    /// <summary>Initializes a new instance of the <see cref="Money"/> struct.</summary>
    /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
    /// <param name="currency">The Currency of the money.</param>
    /// <param name="context">The <see cref="Context"/> to apply to this instance. If <value>null</value> the
    /// current <see cref="Context"/> will be used.</param>
    public Money(decimal amount, Currency currency, MoneyContext? context = null)
    {
        // Use either provided context or the current global/thread-local context.
        context ??= MoneyContext.CurrentContext;
        Trace.Assert(context is not null, "MoneyContext.CurrentContext should not be null");

        // Fast-path: when the amount is zero
        if (amount == 0m)
        {
            _low = 0;
            _mid = 0;
            _high = 0;
            _flags = (currency.EncodedValue & CurrencyMask)
                     | ((context!.Index << 24) & IndexMask);
            return;
        }

        // Round the amount to the correct scale
        var currencyInfo = CurrencyInfo.GetInstance(currency);
        amount = context!.RoundingStrategy switch
        {
            NoRounding noRounding => noRounding.Round(amount, currencyInfo, context.MaxScale),
            StandardRounding standardRounding => standardRounding.Round(amount, currencyInfo, context.MaxScale),
            _ => context.RoundingStrategy.Round(amount, currencyInfo, context.MaxScale)
        };

        // Extract the 4 integers from the decimal amount.
#if NET5_0_OR_GREATER
        Span<int> bits = stackalloc int[4];
        decimal.GetBits(amount, bits);
#else
        int[] bits = decimal.GetBits(amount);
#endif

        _low = (uint)bits[0];
        _mid = (uint)bits[1];
        _high = (uint)bits[2];
        _flags = (currency.EncodedValue & CurrencyMask) // Store Currency in bits 0–15
                 | ((context.Index << 24) & IndexMask) // Store Index in bits 24–30
                 | (bits[3] & (ScaleMask | SignMask)); // Preserve Scale Factor (16–23) and Sign (31)
    }

    internal Money(long cy, Currency currency, MoneyContextIndex contextIndex)
    {
        ulong absoluteCy; // has to be ulong to accommodate the case where cy == long.MinValue.
        bool isNegative = false;
        if (cy < 0)
        {
            isNegative = true;
            absoluteCy = (ulong)(-cy);
        }
        else
        {
            absoluteCy = (ulong)cy;
        }

        // In most cases, FromOACurrency() produces a Decimal with Scale set to 4. Unless, that is, some of the trailing digits past
        // the decimal point are zero, in which case, for compatibility with .NET, we reduce the Scale by the number of zeros.
        // While the result is still numerically equivalent, the scale does affect the ToString() value. In particular, it prevents
        // a converted currency value of $12.95 from printing uglily as "12.9500".
        int scale = 4;
        if (absoluteCy != 0)  // For compatibility, a currency of 0 emits the Decimal "0.0000" (scale set to 4).
        {
            while (scale != 0 && ((absoluteCy % 10) == 0))
            {
                scale--;
                absoluteCy /= 10;
            }
        }

        // Use either provided context or the current global/thread-local context.
        //contextIndex ??= MoneyContext.CurrentContext.Index;
        //Trace.Assert(contextIndex is not null, "MoneyContextIndex should not be null");

        _low = (uint)absoluteCy;
        _mid = (uint)(absoluteCy >> 32);
        _high = 0;
        _flags = (currency.EncodedValue & CurrencyMask) // Store Currency in bits 0–15
                 | ((scale << 16) & ScaleMask) // Store Scale Factor (16–23)
                 | ((contextIndex << 24) & IndexMask) // Store Index in bits 24–30
                 | ((isNegative ? 1 : 0) & SignMask); // Store Sign (31)
    }


    /// <summary>Gets the amount of money.</summary>
    public decimal Amount
    {
        get
        {
            // Extract scale (bits 16-23) and sign (bit 31) from Flags
            byte scale = (byte)((_flags & ScaleMask) >> 16); // Extract scale (shift bits 16-23)
            bool isNegative = (_flags & SignMask) != 0; // Is negative if SignMask bit is set

            // Reconstruct the decimal with the correct `Flags` value (index removed)
            return new decimal(unchecked((int)_low), unchecked((int)_mid), unchecked((int)_high), isNegative, scale);
        }
        init
        {
            // Round the amount to the correct scale
            var amount = Context.RoundingStrategy switch
            {
                NoRounding noRounding => noRounding.Round(value, CurrencyInfo.GetInstance(Currency), Context.MaxScale),
                StandardRounding standardRounding => standardRounding.Round(value, CurrencyInfo.GetInstance(Currency), Context.MaxScale),
                _ => Context.RoundingStrategy.Round(value, CurrencyInfo.GetInstance(Currency), Context.MaxScale)
            };

            // Separate the Decimal bits during initialization
#if NET5_0_OR_GREATER
            Span<int> bits = stackalloc int[4];
            decimal.GetBits(amount, bits);
#else
            int[] bits = decimal.GetBits(amount);
#endif
            _low = (uint)bits[0];
            _mid = (uint)bits[1];
            _high = (uint)bits[2];

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

    /// <summary>Gets the scaling factor of the <see cref="Money"/> amount, which is a number from 0 to 28 that represents the number of decimal digits.</summary>
    public byte Scale => (byte)((_flags & ScaleMask) >> 16); // Extract Scale (bits 16-23)

    /// <summary>Gets the maximum number of digits used to represent the <see cref="Money"/> amount.</summary>
    internal byte Precision
    {
        get
        {
            // Use SqlDecimal implementation to get the precision.
            System.Data.SqlTypes.SqlDecimal sqlDecimal = new(Amount);
            return sqlDecimal.Precision;
        }
    }

    /// <summary>Gets the context associated with this <see cref="Money"/> instance.</summary>
    public MoneyContext Context => MoneyContext.Get(ContextIndex);

    /// <summary>Gets the index of the <see cref="Context"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the provided index value is outside the allowed range of 0 to 127.</exception>
    internal MoneyContextIndex ContextIndex
    {
        get => (MoneyContextIndex)((_flags & IndexMask) >> 24); // Extract Index (bits 24–30)
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
    public bool Equals(Money other)
    {
        // Equality for Money should be based on Currency and numeric Amount value, independent of scale, context index, or internal decimal representation.

        // Fast-path: if currency differs, not equal.
        if (!EqualCurrency(other)) return false;

        // Fast-path: both zero magnitudes -> equal regardless of scale/sign.
        uint thisMag = _low | _mid | _high;
        uint otherMag = other._low | other._mid | other._high;
        if ((thisMag | otherMag) == 0u)
            return true;

        // Fast path: different signs (with nonzero magnitudes) => not equal
        if (((_flags ^ other._flags) & SignMask) != 0)
            return false;

        // Fast path: if scales are equal, equality reduces too low/mid/high equality
        if (((_flags ^ other._flags) & ScaleMask) == 0)
        {
            return _low == other._low
                   && _mid == other._mid
                   && _high == other._high;
        }

        // Fallback: compare using reconstructed decimals to be scale-insensitive (e.g., 1.0 == 1.00).
        // Different low/mid/high with a different scale can represent the same numeric amount!
        return EqualityComparer<decimal>.Default.Equals(Amount, other.Amount);
    }

    /// <summary>Returns a value indicating whether this instance and a specified <see cref="object"/> represent the same type
    /// and value.</summary>
    /// <param name="obj">An <see cref="object"/>.</param>
    /// <returns>true if value is equal to this instance; otherwise, false.</returns>
    public override bool Equals(object? obj) => obj is Money money && this.Equals(money);

    /// <summary>Determines whether the specified <see cref="Money"/> instances are equal.</summary>
    /// <param name="left">The first <see cref="Money"/> instance to compare.</param>
    /// <param name="right">The second <see cref="Money"/> instance to compare.</param>
    /// <returns><see langword="true"/> if the specified <see cref="Money"/> instances are equal; otherwise, <see langword="false"/>.</returns>
    public static bool Equals(Money left, Money right) => left.Equals(right);

    /// <summary>Returns the hash code for this instance.</summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode()
    {
        // Fast-path: if zero, compute hash without reconstructing decimal
        if (_low == 0 && _mid == 0 && _high == 0)
        {
            // Canonical zero: sign/scale don't matter for zero; ensure the same hash for any zero representation.
            return (EqualityComparer<decimal>.Default.GetHashCode(0) * 31) +
                   EqualityComparer<Currency>.Default.GetHashCode(this.Currency);
        }

        // use Amount (scale-insensitive) to keep the contract with Equals.
        return (EqualityComparer<decimal>.Default.GetHashCode(this.Amount) * 31) +
               EqualityComparer<Currency>.Default.GetHashCode(this.Currency);
    }

    /// <summary>Deconstructs the current instance into its components.</summary>
    /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
    /// <param name="currency">The Currency of the money.</param>
    public void Deconstruct(out decimal amount, out Currency currency)
    {
        amount = Amount;
        currency = Currency;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool EqualCurrency(in Money other) => ((_flags ^ other._flags) & CurrencyMask) == 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ThrowIfCurrencyMismatch(in Money other)
    {
        if (EqualCurrency(other))
            return;

        throw new InvalidCurrencyException(this.Currency, other.Currency);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ThrowIfCurrencyIncompatible(in Money other)
    {
        // Fast path: if currencies equal, nothing to do (no Context read needed).
        if (EqualCurrency(other))
            return;

        // Fast path: if both amounts are non-zero, enforce the currency match and throw (no Context read needed).
        uint thisMag = _low | _mid | _high;
        uint otherMag = other._low | other._mid | other._high;
        if (thisMag != 0u && otherMag != 0u)
        {
            throw new InvalidCurrencyException(this.Currency, other.Currency);
        }

        // At least one side is zero; only now consult the (expensive) Context flag to decide we should throw.
        if (Context.EnforceZeroCurrencyMatching || other.Context.EnforceZeroCurrencyMatching)
        {
            // Enforced mode: always require matching currencies, regardless of the amount is zero.
            throw new InvalidCurrencyException(this.Currency, other.Currency);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool EqualContext(in Money other) => ((_flags ^ other._flags) & IndexMask) == 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ThrowIfContextMismatch(in Money other)
    {
        if (EqualContext(other))
            return;

        throw new MoneyContextMismatchException(this.Context, other.Context);
    }
}

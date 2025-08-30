using System.Data.SqlTypes;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NodaMoney.Context;

namespace NodaMoney;

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
[StructLayout(LayoutKind.Explicit, Size = 12)]
public readonly partial record struct FastMoney // or CompactMoney? TODO add interface IMoney or IMonetary or IMonetaryAmount? Using the interface will cause boxing!
{
    private const byte Scale = 4;
    private const long ScaleFactor = 10_000;
    private const long MinValueLong = unchecked((long)0x8000000000000000L) / ScaleFactor;
    private const long MaxValueLong = 0x7FFFFFFFFFFFFFFFL / ScaleFactor;

    /// <summary>Stored as an integer scaled by 10,000</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [field: FieldOffset(0)]
    private long OACurrencyAmount { get; init; } // 8 bytes (64 bits) vs decimal 16 bytes (128 bits)

    /// <summary>Gets the amount of money.</summary>
    public decimal Amount => decimal.FromOACurrency(OACurrencyAmount);

    /// <summary>Gets the <see cref="Currency"/> of the money.</summary>
    [field: FieldOffset(8)]
    public Currency Currency { get; init; }

    [field: FieldOffset(10)]
    private MoneyContextIndex ContextIndex { get; init; }

    /// <summary>Gets the context associated with this <see cref="Money"/> instance.</summary>
    public MoneyContext Context => MoneyContext.Get(ContextIndex);

    // [field: FieldOffset(11)]
    // private byte UnusedByte { get; init; }

    /// <summary>Initializes a new instance of the <see cref="FastMoney"/> struct.</summary>
    /// <param name="amount">The Amount of money as <see langword="decimal"/>.</param>
    /// <param name="currencyInfo">The Currency of the money.</param>
    /// <param name="context">The <see cref="MoneyContext"/> to apply to this instance. If <value>null</value> the
    /// current <see cref="MoneyContext"/> will be used.</param>
    public FastMoney(decimal amount, CurrencyInfo currencyInfo, MoneyContext? context = null) : this()
    {
        if (amount is < MinValueLong or > MaxValueLong)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount is outside the allowable range for FastMoney.");
        }

        if (currencyInfo.DecimalDigits > 4)
        {
            throw new InvalidCurrencyException($"The currency '{currencyInfo.Code}' requires more than 4 decimal places, which cannot be represented by {nameof(FastMoney)}.");
        }

        // Use either provided context OR a dedicated FastMoney default context, NOT the global MoneyContext.CurrentContext!
        if (context is not null)
        {
            if (context.MaxScale > 4)
            {
                throw new ArgumentOutOfRangeException(nameof(context),
                    "Context max scale is more then 4, which is outside the allowable range for FastMoney.");
            }

            if (context.Precision > 19)
            {
                throw new ArgumentOutOfRangeException(nameof(context),
                    "Context max precision is more then 19, which is outside the allowable range for FastMoney.");
            }
        }
        else
        {
            context = MoneyContext.FastMoney;
            Trace.Assert(context is not null, "MoneyContext.FastMoney should not be null");
        }

        // Round the amount to the correct scale
        amount = context!.RoundingStrategy switch
        {
            NoRounding => amount,
            // Fast path: ToOACurrency() rounds to 4 decimals using MidpointRounding.ToEven! So we can skip the rounding here.
            StandardRounding { Mode: MidpointRounding.ToEven } when context.MaxScale == 4 => amount,
            StandardRounding standardRounding => standardRounding.Round(amount, currencyInfo, context.MaxScale),
            _ => context.RoundingStrategy.Round(amount, currencyInfo, context.MaxScale)
        };

        ContextIndex = context.Index;
        OACurrencyAmount = decimal.ToOACurrency(amount); // Rounds to 4 decimals using MidpointRounding.ToEven!
        Currency = currencyInfo;
    }

    public bool Equals(FastMoney other) => EqualityComparer<long>.Default.Equals(this.OACurrencyAmount, other.OACurrencyAmount) &&
                                           EqualityComparer<Currency>.Default.Equals(this.Currency, other.Currency);

    public override int GetHashCode() => (EqualityComparer<long>.Default.GetHashCode(this.OACurrencyAmount) * 31) +
                                         EqualityComparer<Currency>.Default.GetHashCode(this.Currency);

    public void Deconstruct(out decimal amount, out Currency currency)
    {
        amount = Amount;
        currency = Currency;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ThrowIfCurrencyMismatch(in FastMoney other)
    {
        if (this.Currency == other.Currency)
            return;

        throw new InvalidCurrencyException(this.Currency, other.Currency);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ThrowIfCurrencyIncompatible(in FastMoney other)
    {
        // Fast path: if currencies equal, nothing to do (no Context read needed).
        if (this.Currency == other.Currency)
            return;

        // Fast path: if both amounts are non-zero, enforce the currency match and throw (no Context read needed).
        if (OACurrencyAmount != 0 && other.OACurrencyAmount != 0)
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
    private void ThrowIfContextMismatch(in FastMoney other)
    {
        if (this.ContextIndex == other.ContextIndex)
            return;

        throw new MoneyContextMismatchException(this.Context, other.Context);
    }
}

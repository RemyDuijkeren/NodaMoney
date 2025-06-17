using System.Data.SqlTypes;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NodaMoney.Context;

namespace NodaMoney;

// ## Performance Benefits of Using Long vs Decimal
// The more significant advantage of `FastMoney` is using `long` for calculations instead of `decimal`:
// 1. **Faster Arithmetic Operations**: Integer operations on `long` are much faster than `decimal` operations:
// - Addition/subtraction with `long` is typically 3-10x faster than with `decimal`
// - Multiplication/division with `long` is typically 5-20x faster than with `decimal`
//
// 2. **Simpler CPU Instructions**: `long` operations use native CPU instructions, while `decimal` requires complex emulation as it's not directly supported by CPUs.
//
// 3. **SIMD Potential**: Operations on `long` values can potentially be vectorized with SIMD instructions, which isn't possible with `decimal`.
//
// 4. **Reduced Method Call Overhead**: Your `Add` method in `FastMoney` directly manipulates the `long` value without converting to `decimal` and back:
//     ``` csharp
// public FastMoney Add(FastMoney other)
// {
//     EnsureSameCurrency(this, other);
//     long totalAmount = checked(OACurrencyAmount + other.OACurrencyAmount);
//     return this with { OACurrencyAmount = totalAmount };
// }
// ```
// This is much more efficient than the equivalent operation with `decimal`.
//
// ## Trade-offs and Limitations
// 1. **Precision**: `FastMoney` is limited to 4 decimal places (scaled by 10,000), while `Money` can support up to 28 decimal places.
// 2. **Range**: `FastMoney` has a smaller range (-922,337,203,685,477.5808 to 922,337,203,685,477.5807) compared to `decimal`.
// 3. **Memory Alignment**: A 12-byte structure isn't optimally aligned for 64-bit systems, but the calculation benefits likely outweigh this.
// 4. **No Rounding**: FastMoney is not doing any internal rounding. For Display convert it to Money type.
//
// ## Recommendation
// 1. **Keep the 12-byte Size**: The performance benefits of using `long` for calculations are significant enough to justify using `FastMoney`, even if it's only 4 bytes smaller than `Money`.
//
// TODO: Benchmark if this all is really true! => Yes, Add/Subtract is 16x faster!
// FastMoney is very similar as SqlMoney but with the benefit of storing the currency.
// SqlMoney (8bytes long + 1byte bool = 9 bytes but 16bytes with padding) https://learn.microsoft.com/en-us/dotnet/api/system.data.sqltypes.sqlmoney?view=net-9.0
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
[StructLayout(LayoutKind.Explicit, Size = 12)]
internal readonly partial record struct FastMoney // or CompactMoney? TODO add interface IMoney or IMonetary or IMonetaryAmount? Using the interface will cause boxing!
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

    /// <summary>Initializes a new instance of the <see cref="FastMoney"/> struct based on the provided <see cref="Money"/> instance.</summary>
    /// <param name="money">An instance of <see cref="Money"/> containing the amount and currency to initialize the <see cref="FastMoney"/> struct.</param>
    /// <remarks>The <see cref="FastMoney"/> struct is optimized for performance and memory usage by using 64 bits (8 bytes) for representation,
    /// in contrast to the 128 bits (16 bytes) used by the <see cref="decimal"/> type. This struct maintains compatibility with the <see cref="Money"/> type.</remarks>
    public FastMoney(Money money) : this(money.Amount, money.Currency, money.Context) { }

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
            throw new ArgumentOutOfRangeException(nameof(currencyInfo), "Currency decimal digits is more then 4, which is outside the allowable range for FastMoney.");
        }

        // Use either provided context or the current global/thread-local context.
        MoneyContext currentContext = context ?? MoneyContext.CurrentContext;
        Trace.Assert(currentContext is not null, "MoneyContext.CurrentContext should not be null");
        if (currentContext!.MaxScale is > 4) // also checks for null
        {
            throw new ArgumentOutOfRangeException(nameof(context), "Context max scale is more then 4, which is outside the allowable range for FastMoney.");
        }

        // Round the amount to the correct scale TODO: do we want to allow this override or just allow no rounding options?
        amount = currentContext.RoundingStrategy switch
        {
            NoRounding => amount,
            StandardRounding { Mode: MidpointRounding.ToEven } => amount,
            StandardRounding standardRounding => standardRounding.Round(amount, currencyInfo, Scale),
            _ => currentContext.RoundingStrategy.Round(amount, currencyInfo, Scale)
        };

        ContextIndex = currentContext.Index;
        OACurrencyAmount = decimal.ToOACurrency(amount); // Rounds to 4 decimals using MidpointRounding.ToEven!
        Currency = currencyInfo;
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

    public void Deconstruct(out decimal amount, out Currency currency)
    {
        amount = Amount;
        currency = Currency;
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static long ToOACurrency(FastMoney money) => money.OACurrencyAmount;

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static FastMoney FromOACurrency(long cy, Currency currency, MoneyContext? context = null) =>
        new(decimal.FromOACurrency(cy), currency, context);

    public static SqlMoney ToSqlMoney(FastMoney money) => new(money.Amount);

    public static FastMoney FromSqlMoney(SqlMoney sqlMoney) => new(sqlMoney.Value); // TODO: what if null?

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void EnsureSameCurrency(in FastMoney left, in FastMoney right)
    {
        if (left.Currency == right.Currency) return;
        throw new InvalidCurrencyException(left.Currency, right.Currency);
    }
}

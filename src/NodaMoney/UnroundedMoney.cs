using System.Runtime.InteropServices;

namespace NodaMoney;

/// <summary>Represents a monetary value along with its associated currency without performing any rounding operations.</summary>
/// <remarks>
/// This class encapsulates an unrounded monetary amount and its currency, ensuring precise financial calculations
/// without applying rounding logic. It is useful in scenarios where exact decimal precision is required.
/// Formatting and Parsing is not possible with this type (convert to Money to do this)!
/// </remarks>
/// <example>
/// Instances of this class are typically used to represent intermediate monetary calculations
/// before applying any rounding to a specific number of decimal places.
/// </example>
[StructLayout(LayoutKind.Sequential)]
internal readonly record struct UnroundedMoney(decimal Amount, Currency Currency) // or UnroundedMoney, IntermediateMoney, RawMoney, ExactMoney, PreciseMoney, MoneyValue? TODO add interface IMoney or IMonetary or IMonetaryAmount?
{
    public UnroundedMoney(Money money) : this(money.Amount, money.Currency) { }

    public UnroundedMoney(decimal amount) : this(amount, CurrencyInfo.CurrentCurrency) { }

    public UnroundedMoney(decimal amount, string code) : this(amount, CurrencyInfo.FromCode(code)) { }

    public UnroundedMoney(double amount) : this((decimal)amount) { }

    public UnroundedMoney(double amount, Currency currency) : this((decimal)amount, currency) { }

    public UnroundedMoney(double amount, string code) : this((decimal)amount, CurrencyInfo.FromCode(code)) { }

    public UnroundedMoney(long amount) : this((decimal)amount) { }

    public UnroundedMoney(long amount, Currency currency) : this((decimal)amount, currency) { }

    public UnroundedMoney(long amount, string code) : this((decimal)amount, CurrencyInfo.FromCode(code)) { }

    [CLSCompliant(false)]
    public UnroundedMoney(ulong amount) : this((decimal)amount) { }

    [CLSCompliant(false)]
    public UnroundedMoney(ulong amount, Currency currency) : this((decimal)amount, currency) { }

    [CLSCompliant(false)]
    public UnroundedMoney(ulong amount, string code) : this((decimal)amount, CurrencyInfo.FromCode(code)) { }
}

using System.Runtime.InteropServices;

namespace NodaMoney;

// name of type: UnroundedMoney, IntermediateMoney, RawMoney, ExactMoney, PreciseMoney?
// TODO add interface IMoney or IMonetary or IMonetaryAmount?

/// <summary>Represents a monetary value along with its associated currency without performing any rounding operations.</summary>
/// <param name="Amount">The Amount of money as <see langword="decimal"/></param>
/// <param name="Currency">The Currency of the money.</param>
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
internal readonly record struct ExactMoney(decimal Amount, Currency Currency)
{
    public ExactMoney(Money money) : this(money.Amount, money.Currency) { }

    public ExactMoney(decimal amount) : this(amount, CurrencyInfo.CurrentCurrency) { }

    public ExactMoney(decimal amount, string code) : this(amount, CurrencyInfo.FromCode(code)) { }

    public ExactMoney(double amount) : this((decimal)amount) { }

    public ExactMoney(double amount, Currency currency) : this((decimal)amount, currency) { }

    public ExactMoney(double amount, string code) : this((decimal)amount, CurrencyInfo.FromCode(code)) { }

    public ExactMoney(long amount) : this((decimal)amount) { }

    public ExactMoney(long amount, Currency currency) : this((decimal)amount, currency) { }

    public ExactMoney(long amount, string code) : this((decimal)amount, CurrencyInfo.FromCode(code)) { }

    [CLSCompliant(false)]
    public ExactMoney(ulong amount) : this((decimal)amount) { }

    [CLSCompliant(false)]
    public ExactMoney(ulong amount, Currency currency) : this((decimal)amount, currency) { }

    [CLSCompliant(false)]
    public ExactMoney(ulong amount, string code) : this((decimal)amount, CurrencyInfo.FromCode(code)) { }
}

using NodaMoney.Exchange;

namespace NodaMoney;

/// <summary>Represents an extended monetary value with an associated exchange rate and transaction date.</summary>
/// <remarks>
/// https://hillside.net/plop/2015/papers/penguins/8.pdf ,
/// Dynamics 365 https://learn.microsoft.com/en-us/power-apps/developer/data-platform/transaction-currency-currency-entity
/// https://community.dynamics.com/blogs/post/?postid=d09fbf59-9f0e-44dc-b902-23c9ae85e0d1
/// </remarks>
/// <param name="Amount">The amount</param>
/// <param name="ExchangeRate">the exchange rate</param>
/// <param name="TransactionDate">the transaction date</param>
internal readonly record struct ExtendedMoney(decimal Amount, ExchangeRate ExchangeRate, DateTimeOffset TransactionDate)
{
    public Currency BaseCurrency => ExchangeRate.BaseCurrency;
    public decimal BaseAmount => ExchangeRate.Convert(new Money(Amount, ExchangeRate.QuoteCurrency)).Amount;
    public Currency Currency => ExchangeRate.QuoteCurrency;

    public ExtendedMoney(decimal amount, Currency currency) :
        this(amount, new ExchangeRate(currency, currency, 1), DateTimeOffset.Now)
    {
    }
}

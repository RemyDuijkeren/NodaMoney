# Exchange Rate Model – Detailed Example

This proposal provides a concrete, .NET‑idiomatic exchange‑rate provider model and a conversion helper that integrates with MoneyContext. It mirrors JavaMoney’s provider/ConversionQuery approach while staying lightweight.

Core abstractions
```csharp
namespace NodaMoney.Exchange;

using NodaMoney.Context;

public interface IExchangeRateProvider
{
    string Id { get; }
    bool TryGetRate(in ConversionQuery query, out ExchangeRate rate);
}

public interface ICompositeExchangeRateProvider : IExchangeRateProvider
{
    IReadOnlyList<IExchangeRateProvider> Providers { get; }
}

public sealed record RateContext(
    string ProviderId,
    DateTimeOffset? AsOf = null,
    string? Source = null,
    IReadOnlyDictionary<string, object?>? Attributes = null
);

public sealed record ExchangeRate(
    CurrencyInfo BaseCurrency,
    CurrencyInfo CounterCurrency,
    decimal Factor,
    RateContext Context
)
{
    public bool IsInverseOf(ExchangeRate other)
        => BaseCurrency == other.CounterCurrency
           && CounterCurrency == other.BaseCurrency
           && Factor != 0m && other.Factor != 0m
           && Math.Round(Factor * other.Factor, 12) == 1m;
}

public readonly record struct ConversionQuery(
    CurrencyInfo BaseCurrency,
    CurrencyInfo CounterCurrency,
    DateTimeOffset? At = null,
    MoneyContext? MoneyContext = null,
    IReadOnlyDictionary<string, object?>? Attributes = null
)
{
    public bool Matches(in ExchangeRate rate)
        => rate.BaseCurrency == BaseCurrency && rate.CounterCurrency == CounterCurrency;
}
```

Conversion helper
```csharp
public static class MoneyConversionExtensions
{
    public static Money ConvertTo(
        this Money amount,
        CurrencyInfo targetCurrency,
        IExchangeRateProvider provider,
        ConversionQuery? query = null)
    {
        if (amount is null) throw new ArgumentNullException(nameof(amount));
        if (targetCurrency is null) throw new ArgumentNullException(nameof(targetCurrency));
        if (provider is null) throw new ArgumentNullException(nameof(provider));
        if (amount.Currency == targetCurrency) return amount;

        MoneyContext ctx = (query?.MoneyContext) ?? MoneyContext.CurrentContext;

        var q = query ?? new ConversionQuery(
            BaseCurrency: amount.Currency,
            CounterCurrency: targetCurrency,
            At: DateTimeOffset.UtcNow,
            MoneyContext: ctx);

        if (!provider.TryGetRate(q, out var rate))
            throw new InvalidOperationException($"No exchange rate {q.BaseCurrency.Code}->{q.CounterCurrency.Code} available");

        decimal raw = amount.Amount * rate.Factor;
        decimal rounded = ctx.RoundingStrategy.Round(raw, targetCurrency, ctx);
        return new Money(rounded, targetCurrency);
    }
}
```

Example providers
- InMemoryExchangeRateProvider (fixtures/tests)
- CompositeExchangeRateProvider (priority/fallback)
- CachedExchangeRateProvider (time-bucketed cache)
- TriangulatingProvider (via hub currency)

See features/PossibleFeatures.md section “Exchange Rate Model example” for full code sketches.

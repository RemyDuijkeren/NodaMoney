# Exchange-Rate Provider Model and Conversion Context

Why it matters
- Chained, metadata-rich exchange rate providers (like JavaMoney) enable deterministic selection, fallbacks, caching, and auditability.

Proposal
- Introduce IExchangeRateProvider, ConversionQuery, ExchangeRate, and RateContext with optional composite and cached providers. Provide a Money.ConvertTo(...) helper that respects MoneyContext rounding.

Possible implementation
- Core abstractions:
  - interface IExchangeRateProvider { string Id { get; } bool TryGetRate(in ConversionQuery query, out ExchangeRate rate); }
  - sealed record RateContext(string ProviderId, DateTimeOffset? AsOf = null, string? Source = null, IReadOnlyDictionary<string, object?>? Attributes = null)
  - sealed record ExchangeRate(CurrencyInfo BaseCurrency, CurrencyInfo CounterCurrency, decimal Factor, RateContext Context)
  - readonly record struct ConversionQuery(CurrencyInfo BaseCurrency, CurrencyInfo CounterCurrency, DateTimeOffset? At = null, MoneyContext? MoneyContext = null, IReadOnlyDictionary<string, object?>? Attributes = null)
- Composition and caching:
  - CompositeExchangeRateProvider(params IExchangeRateProvider[]) prioritizes in order.
  - CachedExchangeRateProvider(IExchangeRateProvider inner, TimeSpan ttl) with bucketed cache key.
  - Optional TriangulatingProvider to route via a hub (e.g., EUR).
- Conversion helper:
  - Money.ConvertTo(target, provider, query?) rounds using MoneyContext.RoundingStrategy for the target currency.
- DI integration:
  - Register default provider chain in NodaMoney.DependencyInjection; support named chains per MoneyContext.

API sketches
- See features/proposals/exchange-rate-model-example.md for detailed code.

Risks / considerations
- Ensure thread-safe provider composition and caching.
- Avoid double rounding; round only at final amount per context.

Open questions
- Should we expose provenance (ExchangeRate) alongside converted Money in APIs?
- Policy for inverse rates vs explicit reverse entries?

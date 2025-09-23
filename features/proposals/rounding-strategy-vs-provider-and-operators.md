# Rounding Strategy vs Provider, and Money Operators/Queries

Summary
- IRoundingStrategy is the concrete algorithm that performs rounding. IRoundingProvider is a selector/factory that returns an IRoundingStrategy for a given RoundingQuery (currency, cash, scale, date, attributes). Support both delegate- and interface-based operator/query APIs.

Proposed abstractions
- IRoundingProvider
  - IRoundingStrategy GetRounding(in RoundingQuery query)
- RoundingQuery (readonly record struct)
  - CurrencyInfo? Currency, bool IsCash = false, int? Scale = null, DateTimeOffset? When = null, IReadOnlyDictionary<string, object?>? Attributes = null
- MoneyContext integration
  - RoundingStrategy => Options.RoundingStrategy ?? Options.RoundingProvider?.GetRounding(BuildQuery()) ?? StandardRounding.ToEven

Operators/Queries
- IMoneyOperator and IMoneyQuery<T> interfaces with With/Query extension methods. Also support delegates for ergonomic scenarios.
- Provide readonly struct implementations like PercentageOperator; register via DI when desired.

Code sketch
```csharp
public interface IRoundingProvider
{
    IRoundingStrategy GetRounding(in RoundingQuery query);
}

public readonly record struct RoundingQuery(
    CurrencyInfo? Currency,
    bool IsCash = false,
    int? Scale = null,
    DateTimeOffset? When = null,
    IReadOnlyDictionary<string, object?>? Attributes = null
);

public interface IMoneyOperator { Money Apply(in Money value, MoneyContext? context = null); }
public interface IMoneyQuery<T> { T Evaluate(in Money value, MoneyContext? context = null); }
```

Design notes
- Keep fast paths by caching resolved strategy when explicit in MoneyContextOptions.
- Prefer readonly structs for operators to avoid heap allocations and be AOT friendly.

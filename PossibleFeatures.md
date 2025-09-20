### Summary
You’ve introduced `MoneyContext` to centralize precision, scale, rounding, and defaults, with thread-local scoping and named lookups. That’s a strong step toward what JavaMoney (JSR 354, Moneta) calls `MonetaryContext`/`Rounding` and what Joda-Money addresses more lightly with immutable types and explicit rounding APIs. Below is a focused comparison and concrete opportunities to close gaps or add differentiating features.

---

### Where your `MoneyContext` shines today
- Thread-local and scoped context
  - `AsyncLocal`-backed `ThreadContext` plus `CreateScope(...)` is ergonomic and testable, mirroring “contextual” rounding in JavaMoney via `RoundingQuery` and per-operation configuration.
- Fast lookup and low overhead
  - Byte-sized `MoneyContextIndex`, pre-warmed contexts for all `MidpointRounding` modes, and `FrozenDictionary` on .NET 8+ are great for hot paths.
- Configurability knobs
  - `RoundingStrategy`, `Precision`, `MaxScale`, `DefaultCurrency`, `EnforceZeroCurrencyMatching`. This maps well to Java’s `MonetaryContext`/`RoundingContext` knobs.
- Out-of-the-box contexts
  - `NoRounding` and `FastMoney` presets reflect typical needs (e.g., 19/4 precision/scale).

This establishes a solid foundation comparable to JavaMoney’s contextual configuration model and beyond Joda-Money’s simpler stance.

---

### Quick primer: Java libraries’ relevant concepts
- JavaMoney (JSR 354 / Moneta):
  - Types: `MonetaryAmount` (with implementations like `Money`, `FastMoney`), `CurrencyUnit`.
  - Context: `MonetaryContext`, `RoundingContext`; provider SPI for `MonetaryRounding` via `RoundingProvider` and lookup via `RoundingQuery` (e.g., by currency, cash vs non-cash, scale, custom attributes).
  - Conversion: `ExchangeRate`, `ExchangeRateProvider`, `CurrencyConversion`, `ConversionQuery`, `ConversionContext` with chains and provider selection.
  - Formatting & parsing: `MonetaryAmountFormat` with `AmountFormatQuery`, locale-aware patterns, symbol/ISO policy.
  - Functional toolkit: `MonetaryFunctions` (summing/grouping by currency, min/max), `MonetaryQuery`/`MonetaryOperator` for fluency.
- Joda-Money:
  - Types: `CurrencyUnit`, `Money`, `BigMoney` (unscaled, arbitrary precision), `MoneyFormatter`.
  - Strong on allocation helpers (`allocate`, `allocateByRatios`), formatting, arithmetic, and immutability, minimal provider infrastructure.

---

### Gaps and opportunities for NodaMoney
Below, each item includes “why it matters” and a concrete suggestion.

#### 1) Rounding provider ecosystem (JSR 354 parity)
- Why: JavaMoney supports pluggable `RoundingProvider`s discovered via SPI and selected via `RoundingQuery` attributes (currency, cash usage, scale, custom flags).
- Opportunity:
  - Introduce `IRoundingProvider` and `RoundingQuery` equivalents:
    - `IRoundingProvider.GetRounding(RoundingQuery query): IRoundingStrategy`
    - `RoundingQuery` could capture `CurrencyInfo`, `isCash`, `scale`, `region`, `date`, and arbitrary key-value attributes.
  - Add a composite registry and selection rules in `MoneyContextOptions` (e.g., `DefaultRoundingProvider`, `Providers` list, and `RoundingQueryPolicy`).
  - Keep `MoneyContext.RoundingStrategy` as an override, but allow “computed strategy” via query when not explicitly set.

#### 2) Cash rounding and denomination policies per currency/region
- Why: Many jurisdictions (e.g., CHF 0.05) need cash vs electronic rounding differences.
- Status: You have `CashDenominationRounding` in the repo; extend it.
- Opportunity:
  - Provide data-backed defaults per currency/region, e.g., registry of cash denomination increments and rules (date-effective).
  - Add `MoneyContextOptions.IsCashTransaction` or an attribute in `RoundingQuery`.

#### 3) Ambiguous currency symbol policies and parsing/formatting
- Why: Java `MonetaryAmountFormat` supports locale-sensitive patterns. You have a TODO in `MoneyContext.cs` lines 9–12 about ambiguous symbols and strict vs relaxed zero-currency checks.
- Opportunity:
  - Implement a `MoneyFormatOptions` plus `IMoneyFormatter`/`IMoneyParser` abstractions.
  - Policy settings:
    - Symbol resolution priority list (e.g., prefer `USD` over other `$` currencies by region, context, or explicit list).
    - Strict vs relaxed parsing for zero amounts and missing currency (tie-in with `EnforceZeroCurrencyMatching`).
    - Locale-aware templates, significant digits, grouping, and sign styles.
  - Provide built-in formatters similar to Java’s `AmountFormatQuery` presets and enable culture-based defaults via `IFormatProvider`/`CultureInfo`.

#### 4) Exchange-rate provider model and conversion context
- Why: JavaMoney’s `ExchangeRateProvider`/`CurrencyConversion` stack allows chaining providers, rate contexts (timestamp, validity, provider name), and queries (`ConversionQuery`).
- Status: You have `NodaMoney.Exchange/*` types; likely basic.
- Opportunity:
  - Define `IExchangeRateProvider`, `IChainableExchangeRateProvider`, `ConversionQuery`, `ConversionContext`.
  - Support provider composition with priority and fallbacks, with deterministic selection and metadata (`RateContext` containing provider id, valuation time, scale policy).
  - Add `Money.ConvertTo(targetCurrency, query?)` that consults the current `MoneyContext` and the active provider registry.

#### 5) Big/unscaled amount type (Joda’s `BigMoney` analog)
- Why: Joda-Money separates `Money` (currency scale rules) from `BigMoney` (arbitrary scale). JavaMoney has `MonetaryAmount` variants (`Money`, `FastMoney`) that differ in precision/scale and performance trade-offs.
- Status: You have `Money`, `FastMoney`, and `ExtendedMoney`. If none act as arbitrary-scale, consider:
  - Introduce `BigMoney`-like type: no currency-scale enforcement, high precision for intermediate computations; apply rounding only when converting to `Money` or when formatting.
  - Integrate with `MoneyContext` for finalization rules.

#### 6) Allocation/proration APIs (Joda-Money feature parity)
- Why: Joda-Money provides `allocate` and `allocateByRatios` for splitting amounts deterministically while preserving total.
- Status: You have `MoneyExtensions.Split`—great start.
- Opportunity:
  - Add first-class instance methods: `Allocate(int parts)`, `Allocate(params int[] ratios)`, and `AllocateExact(...)` with tie-breaking policy hooks in `MoneyContext`.
  - Provide rounding policy hooks per allocation (e.g., largest remainder, round-robin, banker’s) and make them deterministic.

#### 7) Monetary operators and queries (JavaMoney idioms)
- Why: JavaMoney has `MonetaryOperator` and `MonetaryQuery` to compose operations (e.g., tax adders, net/gross conversions, custom rules) and extract facets (e.g., precision).
- Opportunity:
  - Add `IMoneyOperator : Func<Money, Money>` and `IMoneyQuery<T> : Func<Money, T>` abstractions.
  - Provide a small standard library: `PercentageOperator`, `TaxOperator`, `DiscountOperator`, `CapFloorOperator`.
  - Make `Money.With(IMoneyOperator op)` ergonomic; allow `MoneyContext`-aware operators.

#### 8) Provider-based discovery and DI integration
- Why: JavaMoney uses SPI; in .NET, DI is the idiomatic alternative. You already have `NodaMoney.DependencyInjection`.
- Opportunity:
  - Register rounding providers, exchange-rate providers, formatters, and money operators through `IServiceCollection` with named instances (you already support named contexts — extend the pattern).
  - Add options binding for context, providers, and default provider chains per named context.

#### 9) Context equality, identity, and pooling
- Why: You already dedupe by `MoneyContextOptions.Equals(...)` and index contexts up to 128 entries.
- Opportunity:
  - Document guarantees: value equality => same `Index`. Expose a lightweight diagnostics view (e.g., `MoneyContext.Diagnostics` with `Index`, `Name?`, `Options` snapshot) for observability.
  - Consider a weak-reference cache if dynamic contexts are created frequently in large apps to avoid index exhaustion; or raise the 128 cap safely.

#### 10) Culture/locale-sensitive defaults and policies
- Why: Java ecosystems lean heavily on locale; .NET has `CultureInfo`.
- Opportunity:
  - Allow `MoneyContextOptions.Culture` or `IFormatProvider` to be part of the context, influencing formatting/parsing and default currency resolution.
  - Add a “regional currency priority list” per context to disambiguate symbols (aligns with your TODO comment).

#### 11) Validation policies for zero amounts and currency checks
- Why: Your TODO mentions strict vs relaxed zero-currency validation.
- Opportunity:
  - Make `EnforceZeroCurrencyMatching` interact with parsing, comparisons, and arithmetic, and expose an enum policy:
    - `ZeroCurrencyPolicy: Ignore | RequireMatch | InferDefault`
  - Provide analyzer hints or guard helpers to encourage consistent usage.

#### 12) Serialization ecosystem completeness
- Why: Interop is critical. Java stacks often ship JSON/JPA helpers.
- Status: You have `System.Text.Json` converters and type converters.
- Opportunity:
  - Offer Newtonsoft.Json converters parity and BSON (Mongo) serializer.
  - “Safe” wire format option: always include `currencyCode`, `amount` as string (lossless), `contextName?`.
  - AOT-friendly converters with `JsonSerializerContext` source-gen examples.

#### 13) Precision/scale governance and trailing zero policy
- Why: Business rules often care about preserving displayed scale vs calculation scale.
- Opportunity:
  - Add `ScaleDisplayPolicy` and `PreserveTrailingZeros` options separate from `MaxScale`.
  - Allow per-currency overrides and “quantize to currency minor units” toggles, distinct from rounding.

#### 14) Deterministic arithmetic across types
- Why: You have `Money` and `FastMoney`. JavaMoney clearly delineates semantics of each amount type.
- Opportunity:
  - Document and enforce cross-type operations: `Money + FastMoney` behavior, promotion/demotion rules, and context-driven rounding when combining.
  - Provide compile-time guardrails via analyzers where unsafe implicit conversions occur.

#### 15) Temporal aspects for rates and rounding
- Why: Real-world rates/rounding can be time-bound (VAT changes, cash rounding policy changes).
- Opportunity:
  - Add `effectiveFrom`/`effectiveTo` semantics in rate/rounding providers and allow `RoundingQuery`/`ConversionQuery` with an `Instant`/`DateTime` to retrieve historical policies.

#### 16) Test kits and property-based tests
- Why: Java libraries are mature and battle-tested.
- Opportunity:
  - Provide a small test kit for integrators to validate their custom providers (rounding, exchange) against invariants.
  - Add property-based tests for arithmetic invariants (associativity with rounding constraints, non-lossy allocations, etc.).

---

### Mapping your constructs to Java equivalents
- `MoneyContextOptions` ~ `MonetaryContext`/`RoundingContext` + bits of `AmountFormatQuery`.
- `IRoundingStrategy` ~ `MonetaryRounding`.
- `MoneyContext.CreateScope(...)` ~ Using a `RoundingQuery` per-op or thread-scoped defaults.
- Proposed `IRoundingProvider`/`RoundingQuery` ~ Java `RoundingProvider`/`RoundingQuery`.
- Proposed `IExchangeRateProvider`/`ConversionQuery` ~ JavaMoney’s provider model.
- Proposed `BigMoney` type ~ Joda-Money’s `BigMoney`.

---

### Concrete backlog suggestions (actionable)
1) Introduce `IRoundingProvider`, `RoundingQuery`, `RoundingContext` and a composite registry integrated into `MoneyContext`.
2) Add `IExchangeRateProvider`, `ConversionQuery`, `ConversionContext`, and provider chaining with priorities and metadata.
3) Implement `IMoneyFormatter`/`IMoneyParser` with `MoneyFormatOptions`, culture-based defaults, and symbol disambiguation policy.
4) Provide `Allocate(int parts)` and `AllocateByRatios(int[] ratios, AllocationPolicy policy)` on `Money` and `FastMoney` with deterministic remainder handling.
5) Consider a `BigMoney`-like type for arbitrary scale intermediates with explicit finalization to `Money` under `MoneyContext`.
6) Add `IMoneyOperator`/`IMoneyQuery<T>` with a small standard operators/query library and `Money.With(op)`.
7) Extend DI package to register named contexts and provider chains, with options binding and AOT-friendly configuration.
8) Enhance serialization converters (System.Text.Json source-gen, Newtonsoft, BSON) and add a stable, lossless canonical JSON schema.
9) Add `ScaleDisplayPolicy`, `PreserveTrailingZeros`, and per-currency override tables.
10) Formalize cross-type arithmetic rules, document in `docs/README.md`, and add analyzers for unsafe patterns.

---

### Final thoughts
Your `MoneyContext` positions NodaMoney much closer to JavaMoney’s flexibility while keeping the performance edge demonstrated in your benchmarks. By layering provider-based rounding and conversion, richer formatting/parsing policies, allocation helpers, and an optional `BigMoney` type, you can match or surpass the feature breadth of JavaMoney and Joda-Money while remaining idiomatic to .NET and AOT-friendly.

Notably, a few TODOs in `MoneyContext.cs` (symbol ambiguity policy, zero-currency enforcement) align perfectly with the formatting/parsing and validation feature work suggested above—great next steps to deliver visible value to users.

## Rounding Strategy vs Provider, and Operators

### Short answer
- `IRoundingStrategy` and an `IRoundingProvider` are related but not the same. Your `IRoundingStrategy` is the algorithm that performs rounding; an `IRoundingProvider` would be a factory/selector that returns the right `IRoundingStrategy` for a given situation (currency, cash vs electronic, date, scale, etc.). If you want JavaMoney-like flexibility, keep `IRoundingStrategy` and add an `IRoundingProvider` abstraction on top.
- `IMoneyOperator<T>` and `IMoneyQuery<T>` can make sense in .NET if you want a pluggable, discoverable pipeline (DI-friendly, analyzable, cacheable). If you don’t need that, idiomatic .NET often prefers `Func<Money, Money>` and `Func<Money, T>` delegates. You can support both: provide delegate-based entry points and optional interfaces for richer scenarios.

---

### 1) Strategy vs Provider: what problem each solves
- `IRoundingStrategy` (you already have): encapsulates the concrete rounding rule and applies it to a numeric input given constraints (e.g., midpoint mode, scale, precision). It’s the “how to round” piece.
- `IRoundingProvider` (proposed): chooses or constructs the appropriate `IRoundingStrategy` from a query. It’s the “which rounding should we use for this operation?” piece. Selection may depend on:
  - currency (`CurrencyInfo`), cash vs electronic payments
  - `MaxScale`/precision override, region or tenant, effective date/time
  - arbitrary attributes (e.g., business unit, product category)

This layering mirrors JavaMoney’s `RoundingProvider` and `RoundingQuery`. Your `MoneyContext` already exposes a single `RoundingStrategy`; adding a provider allows dynamic selection when the context doesn’t specify an explicit strategy.

#### Minimal .NET-friendly sketch
```csharp
public interface IRoundingProvider
{
    IRoundingStrategy GetRounding(in RoundingQuery query);
}

public readonly record struct RoundingQuery
(
    CurrencyInfo? Currency,
    bool IsCash = false,
    int? Scale = null,
    DateTimeOffset? When = null,
    IReadOnlyDictionary<string, object?>? Attributes = null
);
```
Integration options with `MoneyContext`:
- Keep `MoneyContextOptions.RoundingStrategy` as an explicit override (fast path).
- Add `MoneyContextOptions.RoundingProvider` for computed strategies.
- Resolution order could be: explicit `RoundingStrategy` > `RoundingProvider.GetRounding(query)` > built-in default.

```csharp
public sealed record MoneyContext
{
    public IRoundingStrategy RoundingStrategy => Options.RoundingStrategy
        ?? Options.RoundingProvider?.GetRounding(BuildQuery())
        ?? StandardRounding.ToEven; // sensible default

    private RoundingQuery BuildQuery() => new(
        Currency: DefaultCurrency,
        IsCash: Options.IsCashTransaction,
        Scale: MaxScale,
        When: DateTimeOffset.UtcNow,
        Attributes: Options.Attributes
    );
}
```
This gives you JavaMoney-like flexibility without losing your optimized fast paths.

---

### 2) Do `IMoneyOperator<T>` and `IMoneyQuery<T>` make sense in .NET?
They can, but it’s a trade-off. In JavaMoney, `MonetaryOperator`/`MonetaryQuery` are widely used to compose behaviors. In .NET, the idiomatic baseline is to use delegates and extension methods. Interfaces become compelling when you want:
- discoverability/registration via DI (e.g., register discount/tax operators by name)
- metadata (name, id, version, policy flags) for logging and auditing
- composition and caching (e.g., precompiled operators with captured constants)
- analyzers and policies (e.g., forbid certain operators in specific contexts)

If you aim to provide a small standard library of reusable monetary transformations, having interfaces is reasonable. If you just want lightweight composition for callers, delegates suffice.

#### Pragmatic API design
Support both interfaces and delegates to stay idiomatic yet extensible:

```csharp
public interface IMoneyOperator
{
    Money Apply(in Money value, MoneyContext? context = null);
}

public interface IMoneyQuery<T>
{
    T Evaluate(in Money value, MoneyContext? context = null);
}

public static class MoneyExtensions
{
    public static Money With(this Money value, Func<Money, Money> op) => op(value);
    public static Money With(this Money value, IMoneyOperator op, MoneyContext? ctx = null) => op.Apply(value, ctx);

    public static T Query<T>(this Money value, Func<Money, T> query) => query(value);
    public static T Query<T>(this Money value, IMoneyQuery<T> query, MoneyContext? ctx = null) => query.Evaluate(value, ctx);
}
```

- For library-provided building blocks, ship them as structs (to avoid allocations) that implement `IMoneyOperator`. Example:
```csharp
public readonly struct PercentageOperator : IMoneyOperator
{
    public decimal Percent { get; }
    public PercentageOperator(decimal percent) => Percent = percent;
    public Money Apply(in Money value, MoneyContext? context = null)
        => value * (1 + Percent / 100m); // uses Money operators and context rounding
}
```
- For consumers, the simplest usage remains delegates: `money.With(m => m * 1.21m)`.
- For DI, you can add named registrations in `NodaMoney.DependencyInjection` (e.g., `AddMoneyOperator("vat-standard", new PercentageOperator(21m))`).

#### When to choose one over the other
- Choose only delegates if: you want minimum surface area and no plugin model.
- Add interfaces if: you want to ship reusable operators, enable DI discovery, or attach metadata and policies.

You can start with delegates and later add interfaces without breaking changes by layering them behind extension methods.

---

### 3) Suggested minimal additions to your current model
- Keep `IRoundingStrategy` as-is for the actual algorithm.
- Add optional `IRoundingProvider` and `RoundingQuery` to let `MoneyContext` compute a strategy dynamically when an explicit strategy isn’t set.
- Provide dual operator/query APIs:
  - delegate-based: `Money With(Func<Money, Money>)` and `T Query<T>(Func<Money, T>)`
  - interface-based: `IMoneyOperator` and `IMoneyQuery<T>` for DI and provider ecosystems

This preserves your current performance wins while unlocking JavaMoney-style extensibility.

---

### 4) Compatibility and AOT/alloc considerations
- Prefer `readonly struct` implementations for built-in operators/queries to avoid heap allocations and enable inlining.
- Use `in Money` parameters to avoid copying if `Money` is a struct; if `Money` is a class, regular by-ref semantics are fine.
- If you add provider registries, keep lookups O(1) with precomputed dictionaries, similar to how you manage `MoneyContext` indices.
- Keep `MoneyContext.RoundingStrategy` as a cached property so that provider selection doesn’t happen on every arithmetic primitive unless needed.

---

### TL;DR
- `IRoundingStrategy` is the “rounding algorithm.” An `IRoundingProvider` is a selector/factory that returns the right strategy based on a query. They’re complementary layers; add the provider if you want JavaMoney-like dynamic selection.
- `IMoneyOperator<T>`/`IMoneyQuery<T>` can make sense in .NET for DI, metadata, reuse, and analyzers. Offer both simple delegate-based APIs for everyday use and optional interfaces for advanced scenarios. This gives you the best of both worlds without forcing extra ceremony on users who don’t need it.


## Exchange Rate Model example

### Goal
Provide a concrete, .NET‑idiomatic example of an exchange‑rate provider model and a conversion context that integrates nicely with your existing `Money`, `CurrencyInfo`, and `MoneyContext` (rounding, precision, scale). The design mirrors JavaMoney’s `ExchangeRateProvider`/`ConversionQuery` but keeps NodaMoney’s style and performance focus.

---

### Core abstractions
Below are small, focused interfaces and records you can add to `NodaMoney.Exchange` to model exchange rates and conversion queries. They are intentionally minimal and AOT‑friendly.

```csharp
namespace NodaMoney.Exchange;

using NodaMoney.Context;

/// <summary>Represents a provider of FX rates (e.g., ECB, in-memory, HTTP, DB, cached).</summary>
public interface IExchangeRateProvider
{
    /// <summary>Stable identifier for diagnostics/auditing.</summary>
    string Id { get; }

    /// <summary>Try to fetch a direct exchange rate for the query.</summary>
    bool TryGetRate(in ConversionQuery query, out ExchangeRate rate);
}

/// <summary>Optional composite to chain providers with priority/fallback.</summary>
public interface ICompositeExchangeRateProvider : IExchangeRateProvider
{
    IReadOnlyList<IExchangeRateProvider> Providers { get; }
}

/// <summary>Metadata about how an FX rate was obtained.</summary>
public sealed record RateContext(
    string ProviderId,
    DateTimeOffset? AsOf = null,
    string? Source = null,
    IReadOnlyDictionary<string, object?>? Attributes = null
);

/// <summary>Represents a unidirectional exchange rate base→counter with a factor.</summary>
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
           && Math.Round(Factor * other.Factor, 12) == 1m; // defensive
}

/// <summary>Describes what rate is requested.</summary>
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

---

### A money conversion helper
This is a simple, allocation‑leaning extension method that uses the exchange‑rate provider and the current `MoneyContext` to do the rounding. Adapt the `Money` construction to your actual constructor/creation API.

```csharp
using NodaMoney.Context;
using NodaMoney.Exchange;

public static class MoneyConversionExtensions
{
    /// <summary>Converts a money amount to a target currency using the given provider and query.</summary>
    public static Money ConvertTo(
        this Money amount,
        CurrencyInfo targetCurrency,
        IExchangeRateProvider provider,
        ConversionQuery? query = null)
    {
        if (amount is null) throw new ArgumentNullException(nameof(amount));
        if (targetCurrency is null) throw new ArgumentNullException(nameof(targetCurrency));
        if (provider is null) throw new ArgumentNullException(nameof(provider));

        if (amount.Currency == targetCurrency) return amount; // no-op

        MoneyContext ctx = (query?.MoneyContext) ?? MoneyContext.CurrentContext;

        var q = query ?? new ConversionQuery(
            BaseCurrency: amount.Currency,
            CounterCurrency: targetCurrency,
            At: DateTimeOffset.UtcNow,
            MoneyContext: ctx);

        if (!provider.TryGetRate(q, out var rate))
            throw new InvalidOperationException($"No exchange rate {q.BaseCurrency.Code}->{q.CounterCurrency.Code} available from provider(s)");

        decimal raw = amount.Amount * rate.Factor;

        // Round using the current context and the target currency's scale policy
        decimal rounded = ctx.RoundingStrategy.Round(raw, targetCurrency, ctx);

        // Assuming Money has ctor (decimal amount, CurrencyInfo currency)
        return new Money(rounded, targetCurrency);
    }
}
```

Notes:
- The extension delegates rounding to `MoneyContext.RoundingStrategy` so your `Precision`/`MaxScale` rules are honored.
- If `Money`/`FastMoney` have different constructors or factory methods, adapt the last line accordingly.

---

### Example providers
#### 1) In-memory provider (fixtures, tests, or bootstrapping)
```csharp
using System.Collections.Concurrent;
using NodaMoney.Exchange;

public sealed class InMemoryExchangeRateProvider : IExchangeRateProvider
{
    private readonly ConcurrentDictionary<(string Base, string Ccy), ExchangeRate> _rates;

    public InMemoryExchangeRateProvider(IEnumerable<ExchangeRate> rates, string? id = null)
    {
        Id = id ?? "in-memory";
        _rates = new ConcurrentDictionary<(string, string), ExchangeRate>(
            rates.ToDictionary(r => (r.BaseCurrency.Code, r.CounterCurrency.Code), r => r));
    }

    public string Id { get; }

    public bool TryGetRate(in ConversionQuery query, out ExchangeRate rate)
    {
        if (_rates.TryGetValue((query.BaseCurrency.Code, query.CounterCurrency.Code), out rate))
            return true;

        // Simple inverse fallback if present
        if (_rates.TryGetValue((query.CounterCurrency.Code, query.BaseCurrency.Code), out var inverse))
        {
            rate = new ExchangeRate(
                BaseCurrency: query.BaseCurrency,
                CounterCurrency: query.CounterCurrency,
                Factor: inverse.Factor == 0m ? 0m : 1m / inverse.Factor,
                Context: inverse.Context with { ProviderId = Id, Source = "inverse" }
            );
            return true;
        }

        rate = null!; // out param
        return false;
    }
}
```

#### 2) Composite provider with priority/fallback
```csharp
using NodaMoney.Exchange;

public sealed class CompositeExchangeRateProvider : ICompositeExchangeRateProvider
{
    public CompositeExchangeRateProvider(params IExchangeRateProvider[] providers)
    {
        Providers = providers?.ToList() ?? throw new ArgumentNullException(nameof(providers));
        Id = string.Join("+", Providers.Select(p => p.Id));
    }

    public string Id { get; }
    public IReadOnlyList<IExchangeRateProvider> Providers { get; }

    public bool TryGetRate(in ConversionQuery query, out ExchangeRate rate)
    {
        foreach (var p in Providers)
        {
            if (p.TryGetRate(query, out rate))
            {
                // Re-stamp the provider id in the context for transparency
                rate = rate with { Context = rate.Context with { ProviderId = p.Id } };
                return true;
            }
        }
        rate = null!;
        return false;
    }
}
```

#### 3) Cached decorator
```csharp
using System.Collections.Concurrent;
using NodaMoney.Exchange;

public sealed class CachedExchangeRateProvider : IExchangeRateProvider
{
    private readonly IExchangeRateProvider _inner;
    private readonly TimeSpan _ttl;
    private readonly ConcurrentDictionary<(string Base, string Ccy, long Bucket), ExchangeRate> _cache = new();

    public CachedExchangeRateProvider(IExchangeRateProvider inner, TimeSpan ttl)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        _ttl = ttl;
        Id = $"cache({_inner.Id})";
    }

    public string Id { get; }

    public bool TryGetRate(in ConversionQuery query, out ExchangeRate rate)
    {
        var at = query.At ?? DateTimeOffset.UtcNow;
        long bucket = at.ToUnixTimeSeconds() / (long)_ttl.TotalSeconds;
        var key = (query.BaseCurrency.Code, query.CounterCurrency.Code, bucket);

        if (_cache.TryGetValue(key, out rate)) return true;

        if (_inner.TryGetRate(query, out rate))
        {
            _cache[key] = rate;
            return true;
        }

        return false;
    }
}
```

---

### Putting it all together (usage)
```csharp
using NodaMoney;
using NodaMoney.Context;
using NodaMoney.Exchange;

// 1) Configure a MoneyContext (precision/scale/rounding)
var ctx = MoneyContext.CreateAndSetDefault(opt =>
{
    opt.RoundingStrategy = new StandardRounding(MidpointRounding.ToEven);
    opt.Precision = 19;
    opt.MaxScale = 4; // e.g., FastMoney-like
}, name: "default");

// 2) Build an exchange provider chain
var eur = CurrencyInfo.FromCode("EUR");
var usd = CurrencyInfo.FromCode("USD");
var gbp = CurrencyInfo.FromCode("GBP");

var seedRates = new[]
{
    new ExchangeRate(eur, usd, 1.0850m, new RateContext("seed", AsOf: DateTimeOffset.UtcNow, Source: "fixture")),
    new ExchangeRate(eur, gbp, 0.8450m, new RateContext("seed", AsOf: DateTimeOffset.UtcNow, Source: "fixture")),
};

IExchangeRateProvider provider = new CachedExchangeRateProvider(
    new CompositeExchangeRateProvider(
        new InMemoryExchangeRateProvider(seedRates, id: "in-memory-primary")
        // , new EcbHttpProvider(httpClient) // example of a real provider
    ),
    ttl: TimeSpan.FromMinutes(5));

// 3) Convert money with context-aware rounding
var price = new Money(100m, eur);

// a) Direct EUR → USD
var usdPrice = price.ConvertTo(usd, provider);

// b) With explicit query (e.g., historical date)
var yesterday = DateTimeOffset.UtcNow.AddDays(-1);
var usdYesterday = price.ConvertTo(usd, provider, new ConversionQuery(
    BaseCurrency: price.Currency,
    CounterCurrency: usd,
    At: yesterday,
    MoneyContext: MoneyContext.CurrentContext));

// c) Chained path (e.g., USD → GBP when only EUR legs exist) via custom provider that triangulates through EUR
// See the Triangulating provider sketch below for that scenario.
```

The result of each conversion is rounded per `MoneyContext` and carries the target `CurrencyInfo`. If you want to surface provenance, you can extend `Money` with a domain event, or return a tuple `(Money Money, ExchangeRate Rate)` from a separate `ICurrencyConverter` service, but the simple extension above returns just `Money` to keep the core type lean.

---

### Optional: Triangulating provider (EUR as a hub)
If you don’t always have direct pairs, a triangulating provider can compute cross rates.

```csharp
using NodaMoney.Exchange;

public sealed class TriangulatingProvider : IExchangeRateProvider
{
    private readonly IExchangeRateProvider _inner;
    private readonly CurrencyInfo _hub; // e.g., EUR
    public TriangulatingProvider(IExchangeRateProvider inner, CurrencyInfo hub, string? id = null)
    { _inner = inner; _hub = hub; Id = id ?? $"tri({_inner.Id},{hub.Code})"; }

    public string Id { get; }

    public bool TryGetRate(in ConversionQuery query, out ExchangeRate rate)
    {
        // First try direct
        if (_inner.TryGetRate(query, out rate)) return true;

        // Try base→hub and hub→counter
        if (_inner.TryGetRate(query with { CounterCurrency = _hub }, out var baseToHub)
            && _inner.TryGetRate(query with { BaseCurrency = _hub, CounterCurrency = query.CounterCurrency }, out var hubToCounter))
        {
            decimal factor = baseToHub.Factor * hubToCounter.Factor;
            rate = new ExchangeRate(
                BaseCurrency: query.BaseCurrency,
                CounterCurrency: query.CounterCurrency,
                Factor: factor,
                Context: new RateContext(Id, AsOf: query.At, Source: $"tri({baseToHub.Context.ProviderId},{hubToCounter.Context.ProviderId})")
            );
            return true;
        }

        rate = null!;
        return false;
    }
}
```

---

### DI integration sketch (NodaMoney.DependencyInjection)
If you want to register providers in DI alongside named `MoneyContext`s, a typical setup might look like this:

```csharp
using Microsoft.Extensions.DependencyInjection;
using NodaMoney.Context;
using NodaMoney.Exchange;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFx(this IServiceCollection services)
    {
        services.AddSingleton<IExchangeRateProvider>(sp =>
        {
            var eur = CurrencyInfo.FromCode("EUR");
            var inMem = new InMemoryExchangeRateProvider(new[]
            {
                new ExchangeRate(eur, CurrencyInfo.FromCode("USD"), 1.0850m, new RateContext("seed")),
            }, id: "inmem");

            return new CachedExchangeRateProvider(new CompositeExchangeRateProvider(inMem), TimeSpan.FromMinutes(5));
        });

        return services;
    }
}
```

If your DI package already exposes `MoneyContextExtensions` for named contexts, you can add similar helpers for registering providers per context name (e.g., tenants/regions).

---

### Conversion context and policies
- Time: `ConversionQuery.At` allows historical or forward‑dated lookups. Providers can ignore it or select the appropriate rate snapshot.
- Rounding: Always round using `MoneyContext.RoundingStrategy` for the target currency. This ensures consistent rounding regardless of provider differences.
- Attributes: `ConversionQuery.Attributes` can carry arbitrary flags like `"pricingLayer": "mid" | "bid" | "ask"`, `"isCash": true`, or tenant id. Providers can apply these to select different rate books.
- Diagnostics: `RateContext` captures provider id and provenance for logging/audit.

---

### Testing the model
- Unit tests can use `InMemoryExchangeRateProvider` to assert deterministic conversions and rounding.
- Property tests can validate invariants like conversion consistency around 1.0 via inverse pairs and that `Allocate` (if used) preserves totals post‑conversion when summed.

---

### Summary
- `IExchangeRateProvider` provides the rate; `ConversionQuery` describes the request; `RateContext`/`ExchangeRate` capture metadata.
- `Money.ConvertTo(...)` extension performs conversion and defers rounding to the active `MoneyContext`.
- Composite, cached, and triangulating providers give you production‑ready composition without complicating your core `Money` type.
- This mirrors JavaMoney’s conversion stack while staying idiomatic to .NET and aligned with your repository’s conventions (immutability, guard clauses, context‑driven rounding).

## Split Design

### Goal
Provide Joda-Money–style allocation/proration with NodaMoney ergonomics, named `Split(...)` (not `Allocate`). Keep it:
- precise (sum of parts equals original)
- deterministic (stable tie-breaking)
- context-aware (uses `MoneyContext` rounding/scale)
- fast (zero allocations beyond the result array)

Below is a drop-in design with signatures, policies, algorithms, and test examples. It assumes `Money` is immutable and has `Amount` and `Currency` accessors, and you already have `MoneyContext` and `IRoundingStrategy`.

---

### API shape (proposed)
Expose as instance and extension methods on `Money`/`FastMoney` and a static helper for raw decimals.

```csharp
namespace NodaMoney;

public enum SplitRemainderPolicy
{
    LargestRemainder,   // Joda default: distribute leftover smallest units to parts with largest remainders
    RoundRobin,         // Distribute 1-by-1 from first to last (stable order)
    TowardZero,         // Leave remainder on the caller; parts are truncated toward zero
    AwayFromZero        // Round parts away from zero where possible, compensating at the end
}

public static class MoneyExtensions
{
    // Equal parts
    public static Money[] Split(this Money amount, int parts,
        SplitRemainderPolicy policy = SplitRemainderPolicy.LargestRemainder,
        MoneyContext? context = null);

    // Ratio-based parts (e.g., 2, 3, 5) — any non-negative integers (at least one > 0)
    public static Money[] Split(this Money amount, params int[] ratios);
    public static Money[] Split(this Money amount, SplitRemainderPolicy policy, params int[] ratios);

    // Weighted split by decimal weights (normalized internally)
    public static Money[] SplitByWeights(this Money amount, ReadOnlySpan<decimal> weights,
        SplitRemainderPolicy policy = SplitRemainderPolicy.LargestRemainder,
        MoneyContext? context = null);

    // Exact split into target counts (e.g., denominations), or exact-amount split (throws on mismatch)
    public static Money[] SplitExact(this Money amount, ReadOnlySpan<Money> partsTemplate);
}
```

Notes:
- Return `Money[]` for performance and familiarity; it’s easy to convert to `IReadOnlyList<Money>` if desired.
- Negative amounts should be supported (distribute the sign consistently).
- Default policy is `LargestRemainder` to match Joda’s behavior and most financial expectations.

---

### Rounding/scale strategy
- Determine target scale using `MoneyContext` first, then currency minor units:
  - `scale = context?.MaxScale ?? amount.Currency.MinorUnits`
- Quantize to `scale` using `context.RoundingStrategy`. Allocation works in the smallest unit, then distributes remainder.
- Preserve total exactly: sum(parts) == original amount after rounding, for any policy except `TowardZero` where the leftover is intentionally not redistributed (documented behavior).

---

### Algorithms
#### Common helpers
```csharp
private static int ResolveScale(CurrencyInfo currency, MoneyContext? context)
    => context?.MaxScale ?? currency.MinorUnit;

private static decimal SmallestUnit(int scale) => scale <= 0 ? 1m : 1m / decimal.Pow(10, scale);

// Fast power-of-10 decimal; avoids Math.Pow(double)
private static class decimal
{
    public static decimal Pow(int exponent)
    {
        // exponent in [0..28] for decimal scale; clamp defensively
        if (exponent <= 0) return 1m;
        if (exponent > 28) exponent = 28;
        decimal result = 1m;
        for (int i = 0; i < exponent; i++) result *= 10m;
        return result;
    }
}
```

#### Equal parts
```csharp
public static Money[] Split(this Money amount, int parts,
    SplitRemainderPolicy policy = SplitRemainderPolicy.LargestRemainder,
    MoneyContext? context = null)
{
    if (parts <= 0) throw new ArgumentOutOfRangeException(nameof(parts));

    context ??= MoneyContext.CurrentContext;
    var currency = amount.Currency;
    int scale = ResolveScale(currency, context);
    decimal q = SmallestUnit(scale); // e.g., 0.01 for scale=2

    // Work in smallest units (integers) to preserve exactness
    decimal roundedTotal = context.RoundingStrategy.Round(amount.Amount, currency, context);
    long unitsTotal = (long)System.Math.Round(roundedTotal / q, 0, MidpointRounding.AwayFromZero);

    long baseUnits = unitsTotal / parts;
    long remainder = unitsTotal - baseUnits * parts; // 0..(parts-1) or negative if amount negative

    var result = new Money[parts];

    // Initialize all with base share
    for (int i = 0; i < parts; i++)
    {
        result[i] = new Money(baseUnits * q, currency);
    }

    // Distribute remainder according to policy
    DistributeRemainder(result, remainder, q, policy, context);

    return result;
}
```

#### Ratio-based
```csharp
public static Money[] Split(this Money amount, SplitRemainderPolicy policy, params int[] ratios)
{
    if (ratios is null || ratios.Length == 0) throw new ArgumentException("Ratios required", nameof(ratios));
    if (ratios.Any(r => r < 0)) throw new ArgumentException("Ratios must be non-negative", nameof(ratios));
    if (ratios.All(r => r == 0)) throw new ArgumentException("At least one ratio must be > 0", nameof(ratios));

    var context = MoneyContext.CurrentContext;
    var currency = amount.Currency;
    int scale = ResolveScale(currency, context);
    decimal q = SmallestUnit(scale);

    decimal roundedTotal = context.RoundingStrategy.Round(amount.Amount, currency, context);
    long unitsTotal = (long)System.Math.Round(roundedTotal / q, 0, MidpointRounding.AwayFromZero);

    long sumRatios = ratios.Select(r => (long)r).Sum();

    var result = new Money[ratios.Length];
    var remainders = new (int index, decimal frac)[ratios.Length];
    long distributed = 0;

    for (int i = 0; i < ratios.Length; i++)
    {
        if (ratios[i] == 0)
        {
            result[i] = new Money(0m, currency);
            remainders[i] = (i, 0m);
            continue;
        }

        // Raw proportional units with fractional part
        decimal rawUnits = (unitsTotal * (decimal)ratios[i]) / sumRatios;
        long integerUnits = (long)System.Math.Floor(System.Math.Abs(rawUnits));
        if (rawUnits < 0) integerUnits = -integerUnits;

        result[i] = new Money(integerUnits * q, currency);
        distributed += integerUnits;

        decimal frac = System.Math.Abs(rawUnits - integerUnits);
        remainders[i] = (i, frac);
    }

    long remainderUnits = unitsTotal - distributed;
    DistributeRemainder(result, remainderUnits, q, policy, context, remainders);

    return result;
}
```

#### Remainder distribution
```csharp
private static void DistributeRemainder(
    Money[] parts,
    long remainderUnits,
    decimal q,
    SplitRemainderPolicy policy,
    MoneyContext context,
    (int index, decimal frac)[]? remainders = null)
{
    if (remainderUnits == 0) return;

    int n = parts.Length;
    var currency = parts[0].Currency;

    // Direction for negative totals: add/subtract unit accordingly
    int step = remainderUnits > 0 ? 1 : -1;
    long left = System.Math.Abs(remainderUnits);

    int[] order;

    switch (policy)
    {
        case SplitRemainderPolicy.RoundRobin:
            order = Enumerable.Range(0, n).ToArray();
            break;
        case SplitRemainderPolicy.LargestRemainder:
            if (remainders is null)
            {
                // For equal split, all fractions are the same; fall back to RoundRobin
                order = Enumerable.Range(0, n).ToArray();
            }
            else
            {
                order = remainders
                    .OrderByDescending(x => x.frac) // stable sorted by fractional remainder
                    .ThenBy(x => x.index)
                    .Select(x => x.index)
                    .ToArray();
            }
            break;
        case SplitRemainderPolicy.TowardZero:
            return; // Leave remainder on caller (document: sum(parts) != original)
        case SplitRemainderPolicy.AwayFromZero:
            // Equivalent to LargestRemainder but bias toward increasing magnitude
            order = (remainders ?? Enumerable.Range(0, n).Select(i => (i, 0m)).ToArray())
                .OrderByDescending(x => x.Item2)
                .ThenBy(x => x.Item1)
                .Select(x => x.Item1)
                .ToArray();
            break;
        default:
            order = Enumerable.Range(0, n).ToArray();
            break;
    }

    int idx = 0;
    while (left-- > 0)
    {
        int i = order[idx++];
        if (idx == order.Length) idx = 0;

        var p = parts[i];
        parts[i] = new Money(p.Amount + step * q, currency);
    }
}
```

Implementation remarks:
- We operate in integer smallest units so distribution is exact and fast.
- `LargestRemainder` needs the fractional part of each proportional share to rank receivers of leftover units.
- The result amounts are created once for base units then adjusted by ±`q` for remainders.
- This approach is allocation-friendly and deterministic.

---

### Edge cases and policies
- Zero or near-zero totals: all parts become zero; `LargestRemainder` has no effect.
- Negative amounts: sign propagates; the `step` in remainder distribution handles direction.
- Ratios including zeros: allowed; those slots can still receive 1 smallest unit in `RoundRobin`/`LargestRemainder` if their fractional part ranks (typically zero), but you may choose to exclude zero-ratio indices from receiving remainder to respect “no share” intent. If so, filter `order` accordingly.
- Extremely large `parts` or `ratios`: guard against `unitsTotal` and `sumRatios` overflow; the proposed code uses `long` for units and sums to minimize risk. Add checked contexts if you expect >9e18 smallest units.
- `TowardZero`: explicitly document that the leftover stays undistributed, and the sum of parts will not match the original by up to one smallest unit per part.

---

### Behavior examples
```csharp
// EUR 1.00 split into 3 equal parts at scale=2
// baseUnits=33, remainder=1 → [0.33, 0.33, 0.34] with LargestRemainder

// EUR 10.00 split by ratios 2:3:5 → total units=1000
// shares: 200, 300, 500 → [2.00, 3.00, 5.00]

// Negative amount: -1.00 into 3 → baseUnits=-0.33, remainder=-1
// → [-0.33, -0.33, -0.34] (LargestRemainder)
```

---

### MoneyContext integration
- Always round the original amount to the context target scale before converting to units.
- If you add `ScaleDisplayPolicy` later, you can quantize to a calculation scale (context) and retain a display scale using metadata if needed.
- Optionally expose an overload that accepts a target `CurrencyInfo` and/or `scale` override for advanced scenarios.

---

### FastMoney and ExtendedMoney
- Implement identical APIs for `FastMoney`. If `FastMoney` internally assumes `MaxScale=4`, use that fixed scale.
- For `ExtendedMoney` or a future `BigMoney`, perform splits at a high internal scale (e.g., context `MaxScale` or currency minor unit) and let callers round when converting back to `Money`.

---

### Serialization note
Splits don’t need special serialization, but documenting the policy used can help reproducibility. Consider returning metadata from an alternate API if needed:
```csharp
public readonly record struct SplitResult(IReadOnlyList<Money> Parts, SplitRemainderPolicy Policy);
```

---

### Unit tests (xUnit + FluentAssertions)
```csharp
public class MoneySplitSpec
{
    [Fact]
    public void Split_EqualParts_SumsToOriginal()
    {
        using var _ = MoneyContext.CreateScope(o => { o.RoundingStrategy = new StandardRounding(MidpointRounding.ToEven); o.MaxScale = 2; });
        var eur = CurrencyInfo.FromCode("EUR");
        var m = new Money(1.00m, eur);

        var parts = m.Split(3);

        parts.Sum(p => p.Amount).Should().Be(m.Amount);
        parts.Select(p => p.Amount).Should().ContainInOrder(0.33m, 0.33m, 0.34m);
    }

    [Fact]
    public void Split_ByRatios_Proportional()
    {
        var eur = CurrencyInfo.FromCode("EUR");
        var m = new Money(10.00m, eur);
        var parts = m.Split(SplitRemainderPolicy.LargestRemainder, 2, 3, 5);

        parts.Select(p => p.Amount).Should().ContainInOrder(2.00m, 3.00m, 5.00m);
        parts.Sum(p => p.Amount).Should().Be(10.00m);
    }

    [Fact]
    public void Split_NegativeAmount_DistributesCorrectly()
    {
        using var _ = MoneyContext.CreateScope(o => o.MaxScale = 2);
        var eur = CurrencyInfo.FromCode("EUR");
        var m = new Money(-1.00m, eur);
        var parts = m.Split(3);

        parts.Select(p => p.Amount).Should().ContainInOrder(-0.33m, -0.33m, -0.34m);
        parts.Sum(p => p.Amount).Should().Be(-1.00m);
    }

    [Fact]
    public void Split_TowardZero_LeavesRemainder()
    {
        using var _ = MoneyContext.CreateScope(o => o.MaxScale = 2);
        var eur = CurrencyInfo.FromCode("EUR");
        var m = new Money(1.00m, eur);
        var parts = m.Split(3, SplitRemainderPolicy.TowardZero);

        parts.Sum(p => p.Amount).Should().BeLessThan(m.Amount); // remainder left
    }
}
```

---

### Documentation guidance (docs/README.md)
- Name the feature “Split (Allocation/Proration)” to make discovery easy for ex‑Joda users.
- Document the default policy (`LargestRemainder`) and show equal/ratio examples.
- Clarify rounding: “Splitting uses the current `MoneyContext` scale and rounding strategy; results always sum to the original, except with `TowardZero`.”

---

### Performance considerations
- Use arrays and pre-size them; avoid LINQ in the hot path (you can keep the example clean for readability and then micro-opt with spans/for-loops).
- Compute in smallest units (`long`) to avoid repeated decimal rounding and ensure exactness.
- If `ratios.Length` is large, prefer an in-place partial sorting routine for `LargestRemainder` (e.g., `Array.Sort` with custom comparer); it’s already O(n log n) which is acceptable for typical n.

---

### Summary
- Provide `Split` overloads for equal, ratio, and weight-based scenarios.
- Implement remainder policies with a deterministic and exact smallest-unit algorithm.
- Integrate `MoneyContext` for rounding and scale, and support negative amounts.
- Mirror Joda-Money semantics while staying idiomatic to .NET and your library’s naming and context model.

This design will give your users a clear and powerful `Split(...)` API set that covers most real-world proration/allocation needs and matches or exceeds Joda-Money’s feature parity.

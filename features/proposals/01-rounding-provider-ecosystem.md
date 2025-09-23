# Rounding Provider Ecosystem (JSR 354 parity)

Why it matters
- JavaMoney supports pluggable RoundingProvider discovered via SPI and selected via RoundingQuery attributes (currency, cash vs electronic, scale, custom flags). Unlocks dynamic, policy-driven rounding.

Proposal
- Keep IRoundingStrategy as the algorithm. Add IRoundingProvider as a selector/factory that returns the strategy for a given query. Integrate provider lookup into MoneyContext with fast-path caching when an explicit strategy is set.

Possible implementation
- New abstractions:
  - interface IRoundingProvider { IRoundingStrategy GetRounding(in RoundingQuery query); }
  - readonly record struct RoundingQuery(CurrencyInfo? Currency, bool IsCash = false, int? Scale = null, DateTimeOffset? When = null, IReadOnlyDictionary<string, object?>? Attributes = null)
- MoneyContext integration:
  - MoneyContextOptions: add IRoundingProvider? RoundingProvider; bool IsCashTransaction; IReadOnlyDictionary<string, object?>? Attributes.
  - MoneyContext.RoundingStrategy: resolve as Options.RoundingStrategy ?? Options.RoundingProvider?.GetRounding(BuildQuery()) ?? StandardRounding.ToEven.
  - Cache resolved strategy at context creation for performance when possible.
- Provider composition:
  - CompositeRoundingProvider with deterministic ordering and first-match wins; allow registration in DI with named instances.
- Query building:
  - Default query uses DefaultCurrency, IsCashTransaction, MaxScale, DateTimeOffset.UtcNow, Attributes.

API sketches
- See features/PossibleFeatures.md section “Rounding Strategy vs Provider, and Operators” for concrete code snippets.

Risks / considerations
- Keep lookups O(1) with precomputed dictionaries; avoid per-op allocations.
- Ensure AOT-friendliness (no reflection). Provide sealed structs for built-ins.

Open questions
- Do we allow per-currency overrides directly in MoneyContextOptions in addition to providers?
- Should providers be able to observe the caller’s operation (e.g., allocation vs multiplication)?

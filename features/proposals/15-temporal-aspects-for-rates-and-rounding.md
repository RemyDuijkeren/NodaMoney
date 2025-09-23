# Temporal Aspects for Rates and Rounding

Why it matters
- Rates and cash rounding policies change over time (VAT changes, denomination policies). Consumers often need historical/as-of queries.

Proposal
- Add effectiveFrom/effectiveTo semantics in exchange/rounding providers and include a DateTime/Instant in RoundingQuery and ConversionQuery to select historical policies.

Possible implementation
- Queries:
  - RoundingQuery includes DateTimeOffset? When.
  - ConversionQuery includes DateTimeOffset? At.
- Providers:
  - IRoundingProvider and IExchangeRateProvider implementations select the appropriate snapshot by date.
  - Registries store time-bound entries; optionally, allow open-ended ranges.
- Diagnostics:
  - RateContext carries AsOf timestamp; rounding strategies may expose provenance for audit.

Risks / considerations
- Time-zone semantics: prefer UTC; clearly document expectations.
- Data volume/performance when storing historical snapshots.

Open questions
- Do we allow interpolation between dates or only exact snapshot selection?
- Should MoneyContext expose a default AsOf for operations?

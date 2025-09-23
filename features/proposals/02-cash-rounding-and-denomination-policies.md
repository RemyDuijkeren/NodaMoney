# Cash Rounding and Denomination Policies per Currency/Region

Why it matters
- Cash transactions often require denomination rounding (e.g., CHF 0.05). Policy can differ from electronic rounding and may change over time per region.

Proposal
- Extend existing CashDenominationRounding with data-backed defaults per currency/region and effective dates. Allow toggling via MoneyContext (IsCashTransaction) and/or via RoundingQuery attribute.

Possible implementation
- Data layer:
  - Registry mapping (Currency/Region, EffectiveFrom..To) -> Increment (e.g., CHF: 0.05).
  - Provide JSON or generated source data; keep AOT-friendly.
- Strategy/provider:
  - Implement CashDenominationRounding using the registry; expose as IRoundingStrategy.
  - An IRoundingProvider that chooses cash vs electronic rounding based on query.IsCash.
- MoneyContext options:
  - bool IsCashTransaction; override per scope/operation.
  - Allow custom per-context override table.
- Serialization/formatting tie-in:
  - Formatter option CashDisplay: true uses cash rounding for display without changing stored value.

Risks / considerations
- Keep data current and regionalized; support date-effective changes.
- Ensure conversions and splits use the correct policy when IsCashTransaction is set.

Open questions
- Do we model region at CultureInfo or explicit region code level?
- How to ship/maintain default denomination data (embedded vs external package)?

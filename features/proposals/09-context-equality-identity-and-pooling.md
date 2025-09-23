# Context Equality, Identity, and Pooling

Why it matters
- MoneyContext dedupes by options equality and indexes contexts up to a byte-sized index. Clear guarantees and diagnostics help users reason about identity, and pooling avoids index exhaustion.

Proposal
- Document value equality semantics, expose lightweight diagnostics, and consider weak-reference caching or increased cap for high-churn apps.

Possible implementation
- Guarantees/documentation:
  - Value-equal MoneyContextOptions => same MoneyContextIndex. Stable across process lifetime.
  - Diagnostics view: MoneyContext.Diagnostics returns { Index, Name?, Options snapshot }.
- Pooling/caching:
  - Weak-reference cache for dynamically created contexts; rehydrate or GC when unused.
  - Optionally raise the 128 cap if safe, or make it configurable with safeguards.
- Tooling:
  - Provide a debug-only enumerator of active contexts for observability (no public API commitments).

Risks / considerations
- Thread safety of caches; avoid memory leaks.
- Keeping indices stable while allowing GC of unused contexts.

Open questions
- What compatibility guarantees exist around index assignment across versions?
- Should Names participate in identity or be metadata only?
